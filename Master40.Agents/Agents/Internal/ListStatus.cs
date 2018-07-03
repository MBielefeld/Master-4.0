using System;
using System.Collections.Generic;
using System.Text;

namespace Master40.Agents.Agents.Internal
{
    public class ListStatus
    {
        public ListStatus(Agent agent, Status status, int count, int currentTime)
        {
            Agent = agent;
            Status = status;
            Count = count;
            Time = currentTime;
        }
        
        public Agent Agent;
        public Status Status;
        public int Count;
        public int Time;
    }
}
