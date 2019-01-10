using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Master40.Agents.Agents.Internal;
using Master40.Agents.Agents.Model;
using Master40.DB.Models;
using Master40.Tools.Simulation;

namespace Master40.Agents.Agents
{
    public class MachineAgent : Agent
    {
        // Agent to register your Services
        private readonly DirectoryAgent _directoryAgent;
        private ComunicationAgent _comunicationAgent;
        private Machine Machine { get; }
        private int ProgressQueueSize { get; }
        public WorkItemList<WorkItem> Queue { get; }
        //private Queue<WorkItem> SchduledQueue;
        private LimitedQueue<WorkItem> ProcessingQueue { get; }
        /// <summary>
        /// planing forecast, to drop requests over this value
        /// </summary>
        private int QueueLength { get; set; }
        private bool ItemsInProgess { get; set; }
        private SimulationConfiguration simulationConfiguration { get; }
        private TimeGenerator WorkTimeGenerator { get; }
        private TimeGenerator SetupTimeGenerator { get; }

        public enum InstuctionsMethods
        {
            SetComunicationAgent,
            RequestProposal,
            AcknowledgeProposal,
            StartWorkWith,
            StartWork,
            SetupMachine,
            DoWork,
            FinishWork
        }

        public MachineAgent(Agent creator
            , string name
            ,bool debug
            , DirectoryAgent directoryAgent
            , Machine machine, int queueLength
            , TimeGenerator workTimeGenerator
            , TimeGenerator setupTimeGenerator
            , SimulationConfiguration simConfiguration) : base(creator, name, debug)
        {
            _directoryAgent = directoryAgent;
            ProgressQueueSize = 1; // TODO COULD MOVE TO MODEL for CONFIGURATION, May not required anymore
            Queue = new WorkItemList<WorkItem>();  // ThenBy( x => x.Status)
            ProcessingQueue = new LimitedQueue<WorkItem>(1);
            Machine = machine;
            WorkTimeGenerator = workTimeGenerator;
            SetupTimeGenerator = setupTimeGenerator;
            simulationConfiguration = simConfiguration;
            ItemsInProgess = false;
            RegisterService();
            QueueLength = queueLength; // plaing forecast
        }


        /// <summary>
        /// Register the Machine in the System on Startup.
        /// </summary>
        public void RegisterService()
        {
            _directoryAgent.InstructionQueue.Enqueue(new InstructionSet
            {
                MethodName = DirectoryAgent.InstuctionsMethods.GetOrCreateComunicationAgentForType.ToString(),
                ObjectToProcess = Machine.MachineGroup.Name,
                ObjectType = typeof(string),
                SourceAgent = this
            });
        }

        /// <summary>
        /// Callback
        /// </summary>
        /// <param name="objects"></param>
        private void SetComunicationAgent(InstructionSet objects)
        {
            // save Cast to expected object
            _comunicationAgent = objects.ObjectToProcess as ComunicationAgent;

            // throw if cast failed.
            if (_comunicationAgent == null)
                throw new ArgumentException("Could not Cast Communication Agent from InstructionSet.ObjectToProcess");

            // Debug Message
            DebugMessage("Successfull Registred Service at : " + _comunicationAgent.Name);
        }

        /// <summary>
        /// Is Called from Comunication Agent to get an Proposal when the item with a given priority can be scheduled.
        /// </summary>
        /// <param name="instructionSet"></param>
        private void RequestProposal(InstructionSet instructionSet)
        {
            var workItem = instructionSet.ObjectToProcess as WorkItem;
            if (workItem == null)
                throw new InvalidCastException("Could not Cast Workitem on InstructionSet.ObjectToProcess");

            // debug
            //DebugMessage("Request for Proposal for: " + workItem.WorkSchedule.Article.Name + " at Machine: " + this.Name);
            // Send
            SendProposalTo(instructionSet.SourceAgent, workItem);
        }
        
        /// <summary>
        /// Send Proposal to Comunication Client
        /// </summary>
        /// <param name="targetAgent"></param>
        /// <param name="workItem"></param>
        private void SendProposalTo(Agent targetAgent, WorkItem workItem)
        {
            var max = 0;

            if (workItem.WorkSchedule.MachineTool.SetupTime + (workItem.WorkSchedule.Duration * 3) > QueueLength)
            {
                QueueLength = workItem.WorkSchedule.MachineTool.SetupTime + (workItem.WorkSchedule.Duration * 3);
                DebugMessage("New maximum QueueLength = " + QueueLength.ToString());
            }

            var forceQueuing = -1;
            if (this.Machine.MachineTool != null)
            {
                if ((this.Machine.MachineTool == workItem.WorkSchedule.MachineTool || this.Machine.PlannedMachineTool == workItem.WorkSchedule.MachineTool)) // && (Queue.Any() ? workItem.Priority(Context.TimePeriod, workItem.WorkSchedule.MachineTool.SetupTime * 2) <= Queue.OrderBy(x => x.Priority(Context.TimePeriod)).Min(x => x.Priority(Context.TimePeriod)) : false))
                {
                    DebugMessage("ForceQueuing");
                    forceQueuing = Queue.Where(x => x.WorkSchedule.MachineTool == this.Machine.MachineTool || x.WorkSchedule.MachineTool == this.Machine.PlannedMachineTool).Count();
                }
            }

            //Add SetuopTime to Proposal if Setup is needed
            int setupTime = 0;

            if (this.Machine.MachineTool == null || workItem.WorkSchedule.MachineTool.Name != this.Machine.MachineTool.Name)
            {
                setupTime = workItem.WorkSchedule.MachineTool.SetupTime;
                workItem.Priority(Context.TimePeriod, setupTime);
            }

            if (Queue.Any(e => e.Priority(Context.TimePeriod, setupTime: (this.Machine.MachineTool == null || e.WorkSchedule.MachineTool != this.Machine.MachineTool ? e.WorkSchedule.MachineTool.SetupTime : 0)) <= workItem.Priority(Context.TimePeriod, setupTime)))
            {
                max = Queue.Where(e => e.Priority(Context.TimePeriod, setupTime: (this.Machine.MachineTool == null || e.WorkSchedule.MachineTool != this.Machine.MachineTool ? e.WorkSchedule.MachineTool.SetupTime : 0)) <= workItem.Priority(Context.TimePeriod, setupTime)).Max(e => e.EstimatedEnd);
            }


            // calculat Proposal.
            var proposal = new Proposal
            {
                AgentId = this.AgentId,
                WorkItemId = workItem.Id,
                Postponed = (max > QueueLength && workItem.Status != Status.Ready), // bool to postpone the item for later, exept it is already Ready
                PostponedFor = QueueLength,
                PossibleSchedule = max,
                HasMachineToolAlreadyEquipped = forceQueuing
            };
            // callback 
            CreateAndEnqueueInstuction(methodName: ComunicationAgent.InstuctionsMethods.ProposalFromMachine.ToString(),
                                    objectToProcess: proposal,
                                    targetAgent: targetAgent);

            //Output Queue Status
            Queue.CreateOrUpdateWorkItemListStatus(this, (int)Context.TimePeriod);
        }

        /// <summary>
        /// is Called if The Proposal is accepted by Comunication Agent
        /// </summary>
        /// <param name="instructionSet"></param>
        private void AcknowledgeProposal(InstructionSet instructionSet)
        {
            var workItem = instructionSet.ObjectToProcess as WorkItem;
            if (workItem == null)
                throw new InvalidCastException("Could not Cast Workitem on InstructionSet.ObjectToProcess");

            int setupTime = 0;

            if (this.Machine.MachineTool == null || workItem.WorkSchedule.MachineTool.Name != this.Machine.MachineTool.Name)
            {
                setupTime = workItem.WorkSchedule.MachineTool.SetupTime;
                workItem.Priority(Context.TimePeriod, setupTime);
            }

            if (Queue.Any(e => e.Priority(Context.TimePeriod, setupTime: (this.Machine.MachineTool == null || e.WorkSchedule.MachineTool != this.Machine.MachineTool ? e.WorkSchedule.MachineTool.SetupTime : 0)) <= workItem.Priority(Context.TimePeriod, setupTime)))
            {
                // Get item Latest End.

                var maxItem = Queue.Where(e => e.Priority(Context.TimePeriod, setupTime: (this.Machine.MachineTool == null || e.WorkSchedule.MachineTool != this.Machine.MachineTool ? e.WorkSchedule.MachineTool.SetupTime : 0)) <= workItem.Priority(Context.TimePeriod, setupTime)).Max(e => e.EstimatedEnd);

                // check if Queuable
                if (maxItem > workItem.EstimatedStart)
                {
                    // reset Agent Status
                    workItem.Status = Status.Created;
                    workItem.EstimatedStart = 0;
                    SendProposalTo(instructionSet.SourceAgent, workItem);
                    return;
                }
            }

            DebugMessage("AcknowledgeProposal and Enqueued Item: " + workItem.WorkSchedule.Name);
            Queue.Add(workItem);
            //Remember planned MachineTool
            this.Machine.PlannedMachineTool = this.Machine.MachineTool != null ? this.Machine.MachineTool : workItem.WorkSchedule.MachineTool;
            //MachinenTool 
            var currentOrPlannedMachineTool = this.Machine.MachineTool != null ? this.Machine.MachineTool : this.Machine.PlannedMachineTool;

            // Enqued before another item?
            var position = Queue.OrderBy(x => x.Priority(Context.TimePeriod)).ToList().IndexOf(workItem);
            DebugMessage("Position: " + position + " Priority:" + workItem.Priority() + " Queue length " + Queue.Count());

            // reorganize Queue if an Element has ben Queued which is More Important.
            if (position + 1 < Queue.Count)
            {
                var toRequeue = Queue.OrderBy(x => x.Priority(Context.TimePeriod, currentOrPlannedMachineTool.SetupTime)).ToList().GetRange(position + 1, Queue.Count() - position - 1);

                CallToReQueue(toRequeue);

                DebugMessage("New Queue length = " + Queue.Count);
            }


            if (workItem.Status == Status.Ready)
            {
                // update Processing queue
                UpdateProcessingQueue(workItem);

                // there is at least Something Ready so Start Work
                StartWork(new InstructionSet());

            }

            //Output Queue Status
            Queue.CreateOrUpdateWorkItemListStatus(this, (int)Context.TimePeriod);
        }

        private void CallToReQueue(IEnumerable<WorkItem> toRequeue)
        {
            foreach (var reqItem in toRequeue)
            {
                DebugMessage("-> ToRequeue " + reqItem.Priority(Context.TimePeriod, reqItem.WorkSchedule.MachineTool.SetupTime) + " Current Possition: " + Queue.OrderBy(x => x.Priority(Context.TimePeriod, x.WorkSchedule.MachineTool.SetupTime)).ToList().IndexOf(reqItem) + " Id " + reqItem.Id);

                // remove item from current Queue
                Queue.Remove(reqItem);
                // reset Agent Status
                reqItem.Status = Status.Created;

                // Call Comunication Agent to Requeue
                CreateAndEnqueueInstuction(methodName: ComunicationAgent.InstuctionsMethods.EnqueueWorkItem.ToString(),
                    objectToProcess: reqItem,
                    targetAgent: reqItem.ComunicationAgent);
            }

            //Output Queue Status
            Queue.CreateOrUpdateWorkItemListStatus(this, (int)Context.TimePeriod);
        }

        private void StartWorkWith(InstructionSet instructionSet)
        {
            var workItemStatus = instructionSet.ObjectToProcess as Model.WorkItemStatus;
            if (workItemStatus == null)
            {
                throw new InvalidCastException("Could not Cast >WorkItemStatus< on InstructionSet.ObjectToProcess");
            }


            // update Status
            var workItem = Queue.FirstOrDefault(x => x.Id == workItemStatus.WorkItemId);

            if (workItem != null && workItem.Status == Status.Ready)
            {
                DebugMessage("Set Item: " + workItem.WorkSchedule.Name + " | Status to: " + workItem.Status);
                // update Processing queue
                UpdateProcessingQueue(workItem);

                // there is at least Something Ready so Start Work
                StartWork(new InstructionSet());
            }

            //Output Queue Status
            Queue.CreateOrUpdateWorkItemListStatus(this, (int)Context.TimePeriod);
        }

        private void UpdateProcessingQueue(WorkItem workItem)
        {
            if (ProcessingQueue.CapacitiesLeft && workItem != null)
            {
                CreateAndEnqueueInstuction(methodName: ProductionAgent.InstuctionsMethods.ProductionStarted.ToString(),
                                      objectToProcess: workItem,
                                          targetAgent: workItem.ProductionAgent);
                ProcessingQueue.Enqueue(workItem);
                Queue.Remove(workItem);
            }

            //Output Queue Status
            Queue.CreateOrUpdateWorkItemListStatus(this, (int)Context.TimePeriod);
        }

        private void StartWork(InstructionSet instructionSet)
        {
            DebugMessage("StartWork");
            if (ItemsInProgess)
            {
                DebugMessage("Im still working....");
                return; // still working
            }

            // Dequeue
            var item = ProcessingQueue.Dequeue();


            // Wait if nothing More todo.
            if (item == null)
            {
                // No more work 
                DebugMessage("Nothing more Ready in Queue!");
                return;
            }

            DebugMessage("Start With " + item.WorkSchedule.Name);
            ItemsInProgess = true;
            item.Status = Status.Processed;

            //Output Queue Status
            Queue.CreateOrUpdateWorkItemListStatus(this, (int)Context.TimePeriod);

            //SetupMachine
            int setupDuration = 0;
            if (this.Machine.MachineTool == null || this.Machine.MachineTool.Name != item.WorkSchedule.MachineTool.Name)
            {

                setupDuration = SetupTimeGenerator.GetRandomWorkTime(item.WorkSchedule.MachineTool.SetupTime);
                this.Machine.MachineTool = item.WorkSchedule.MachineTool;
                DebugMessage("Setup MachineTool " + item.WorkSchedule.MachineTool + " with setup time " + item.WorkSchedule.MachineTool.SetupTime.ToString());
                item.SetupDuration = setupDuration;

            }
            else
            {
                DebugMessage("MachineTool " + this.Machine.MachineTool.Name + " already equippded");
            };

            Statistics.UpdateSimulationWorkScheduleSetup(item.Id.ToString(), (int)Context.TimePeriod, item.SetupDuration - 1, this.Machine);

            CreateAndEnqueueInstuction(methodName: MachineAgent.InstuctionsMethods.SetupMachine.ToString(),
                                      objectToProcess: item,
                                          targetAgent: this,
                                              waitFor: setupDuration);

            //Output Queue Status
            Queue.CreateOrUpdateWorkItemListStatus(this, (int)Context.TimePeriod);
        }

        private void SetupMachine(InstructionSet instructionSet)
        {
            var item = instructionSet.ObjectToProcess as WorkItem;
            if (item == null)
            {
                throw new InvalidCastException("Could not Cast >Machine< on InstructionSet.ObjectToProcess");
            }

            CreateAndEnqueueInstuction(methodName: MachineAgent.InstuctionsMethods.DoWork.ToString(),
                                      objectToProcess: item,
                                          targetAgent: this);

        }

        private void DoWork(InstructionSet instructionSet)
        {
            var item = instructionSet.ObjectToProcess as WorkItem;
            if (item == null)
            {
                throw new InvalidCastException("Could not Cast >WorkItemStatus< on InstructionSet.ObjectToProcess");
            }

            // TODO: Roll delay here
            var duration = WorkTimeGenerator.GetRandomWorkTime(item.WorkSchedule.Duration);

            //add Duration to WorkItem for further analysis 
            item.Duration = duration;
            Statistics.UpdateSimulationWorkSchedule(item.Id.ToString(), (int)Context.TimePeriod, duration - 1, this.Machine);

            // get item = ready and lowest priority
            DebugMessage("CreateAndEnqueue von FinishWork");
            CreateAndEnqueueInstuction(methodName: MachineAgent.InstuctionsMethods.FinishWork.ToString(),
                                  objectToProcess: item,
                                      targetAgent: this,
                                          waitFor: duration);

        }

        private void FinishWork(InstructionSet instructionSet)
        {
            var item = instructionSet.ObjectToProcess as WorkItem;
            if (item == null)
            {
                throw new InvalidCastException("Could not Cast >WorkItemStatus< on InstructionSet.ObjectToProcess");
            }

            DebugMessage("FinishWork");

            // Set next Ready Element from Queue
            var itemFromQueue = Queue.Where(x => x.Status == Status.Ready).OrderBy(x => x.Priority(Context.TimePeriod)).ThenBy(x => x.WorkSchedule.Duration).FirstOrDefault();
            UpdateProcessingQueue(itemFromQueue);

            // Set Machine State to Ready for next
            ItemsInProgess = false;
            DebugMessage("Finished Work with " + item.WorkSchedule.Name + " take next...");

            // Call Comunication Agent that item has ben processed.
            CreateAndEnqueueInstuction(methodName: ComunicationAgent.InstuctionsMethods.FinishWorkItem.ToString(),
                                  objectToProcess: new Model.WorkItemStatus
                                  {
                                      CurrentPriority = 0,
                                      Status = Status.Finished,
                                      WorkItemId = item.Id
                                  },
                                      targetAgent: item.ComunicationAgent);

            // Reorganize List

            //Requeue only if estimated time not real work time for WorkItem
            if (item.WorkSchedule.Duration != item.Duration)
            {
                DebugMessage("Do CallToReQueue for " + item.WorkSchedule.Article.Name + ": EstimatedDuration: " + item.WorkSchedule.Duration + " to Duration: " + item.Duration);
                CallToReQueue(Queue.Where(x => x.Status == Status.Created || x.Status == Status.InQueue).ToList());
            }
            else
            {
                DebugMessage("Skip CallToReQueue for " + item.WorkSchedule.Article.Name + ": EstimatedDuration: " + item.WorkSchedule.Duration + " to Duration: " + item.Duration);
            }
            // do Do Work in next Timestep.
            CreateAndEnqueueInstuction(methodName: InstuctionsMethods.StartWork.ToString(),
                                  objectToProcess: new InstructionSet(),
                                      targetAgent: this);//,
                                                         // waitFor: 1);
                                                         //DoWork(new InstructionSet());

            //Output Queue Status
            Queue.CreateOrUpdateWorkItemListStatus(this, (int)Context.TimePeriod);
        }

    }
}
