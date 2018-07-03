using System;
using System.Collections.Generic;
using System.Text;
using Master40.Agents.Agents.Internal;
using Master40.Agents.Agents;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace Master40.Agents.Agents.Model
{

    public class WorkItemList<TWorkItem> : List<WorkItem>
    {
        public WorkItemList()
        {
            
        }
        
        public void reportAllWorkItemsByStatus(Agent agent, int currentTime)
        {
            
            foreach (Status status in (Status[])Enum.GetValues(typeof(Status)))
            {
                ListStatus liststatus = new ListStatus(agent, status, getCountOfWorkItemsByStatus(status), currentTime);
                AgentSimulation.ListStatuses.Add(liststatus);
                //reportMessage += status.ToString() + ": " + getCountOfWorkItemsByStatus(status) + " | ";
            }

        }

        private int getCountOfWorkItemsByStatus(Status status)
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
