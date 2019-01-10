using System;
using System.Collections.Generic;
using System.Text;
using Master40.Agents.Agents.Internal;
using Master40.Agents.Agents;
using System.Linq;

namespace Master40.Agents.Agents.Model
{

    public class WorkItemList<TWorkItem> : List<WorkItem>
    {
        public WorkItemList()
        {
            
        }
        
        public void CreateOrUpdateWorkItemListStatus(Agent agent, int currentTime)
        {
            foreach (Status status in (Status[])Enum.GetValues(typeof(Status)))
            {
                if(AgentSimulation.ListStatuses.Any(x => x.Time == currentTime && x.Agent.Equals(agent) && x.Status == status)){
                    //Update entries
                    AgentSimulation.ListStatuses.Where(x => x.Time == currentTime && x.Status == status && x.Agent.Equals(agent))
                        .Select(y => y.Count = GetCountOfWorkItemsByStatus(status));
                }
                else
                {
                    //Create new entry
                    ListStatus liststatus = new ListStatus(agent, status, GetCountOfWorkItemsByStatus(status), currentTime);
                    AgentSimulation.ListStatuses.Add(liststatus);
                }
            }

        }

        private int GetCountOfWorkItemsByStatus(Status status)
        {
            var count = 0;
            foreach (WorkItem Item in this)
            {
                if (Item.Status == status)
                {
                    count++;
                    //To be added: UpdateSimulation for each Status with Time(?)
                }
            }
            return count;
        }

    }
}
