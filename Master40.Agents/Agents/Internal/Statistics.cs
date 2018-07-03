using System;
using System.Collections.Generic;
using System.Linq;
using Master40.Agents.Agents.Model;
using Master40.DB.Enums;
using Master40.DB.Models;

namespace Master40.Agents.Agents.Internal
{
    public static class Statistics
    {

        public static void CreateSimulationWorkSchedule(WorkItem ws, string orderId, bool isHeadDemand)
        {
            var sws = new SimulationWorkschedule
            {
                WorkScheduleId = ws.Id.ToString(),
                Article = ws.WorkSchedule.Article.Name,
                WorkScheduleName = ws.WorkSchedule.Name,
                DueTime = ws.DueTime,
                EstimatedEnd = ws.EstimatedEnd,
                SimulationConfigurationId = -1,
                OrderId = "[" + orderId + "]",
                HierarchyNumber = ws.WorkSchedule.HierarchyNumber,
                ProductionOrderId = "[" + ws.ProductionAgent.AgentId.ToString() + "]",
                Parent = isHeadDemand.ToString()
            };
            AgentSimulation.SimulationWorkschedules.Add(sws);
        }


        public static void UpdateSimulationWorkSchedule(string workScheduleId, int start, int duration, Machine machine)
        {
            var edit = AgentSimulation.SimulationWorkschedules.FirstOrDefault(x => x.WorkScheduleId.Equals(workScheduleId));
            edit.Start = start;
            edit.End = start + duration + 1; // to have Time Points instead of Time Periods (original +1)
            edit.Machine = machine.Name;
        }
        public static void UpdateSimulationWorkScheduleSetup(string workScheduleId, int start, int setupDuration, Machine machine)
        {
            var edit = AgentSimulation.SimulationWorkschedules.FirstOrDefault(x => x.WorkScheduleId.Equals(workScheduleId));
            edit.setupStart = start;
            edit.setupEnd = start + setupDuration + 1;
            edit.Machine = machine.Name;
        }

        public static void UpdateSimulationId(int simulationId, SimulationType simluationType, int simNumber)
        {
            var simItems = AgentSimulation.SimulationWorkschedules
                .Where(x => x.SimulationConfigurationId == -1).ToList();
            foreach (var item in simItems)
            {
                item.SimulationConfigurationId = simulationId;
                item.SimulationType = simluationType;
                item.SimulationNumber = simNumber;
            }
        }

        public static List<Kpi> CreateKpisFromListStatus(List<ListStatus> listStatus, int simulationId, SimulationType simluationType, int simNumber)
        {
            List<Kpi> kpiList = new List<Kpi>();

            if (listStatus != null)
            {
                foreach (ListStatus ls in listStatus)
                {

                    kpiList.Add(new Kpi()
                    {
                        Name = ls.Agent.Name,
                        Value = ls.Count,
                        ValueMin = 0,
                        ValueMax = 1000,
                        IsKpi = true,
                        Status = ls.Status.ToString(),
                        KpiType = KpiType.WorkItemListStatus,
                        SimulationConfigurationId = simulationId,
                        SimulationType = simluationType,
                        SimulationNumber = simNumber,
                        Time = ls.Time,
                        IsFinal = true
                    });
                }
            }
            return kpiList;
        }

        internal static void UpdateSimulationWorkSchedule(List<Guid> productionAgents, Agent requesterAgent, int orderId)
        {
            foreach (var agentId in productionAgents)
            {
                var items = AgentSimulation.SimulationWorkschedules.Where(x => x.ProductionOrderId.Equals("[" + agentId.ToString() + "]"));
                foreach (var item in items)
                {
                    item.ParentId = item.Parent.Equals(false.ToString()) ? "[" + requesterAgent.Creator.AgentId.ToString() + "]" : "[]";
                    item.Parent = requesterAgent.Creator.Name;
                    item.CreatedForOrderId = item.OrderId;
                    item.OrderId = "[" + orderId + "]";

                    // item.OrderId = orderId;
                }
            }

        }
    }
}