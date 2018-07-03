using Master40.Agents.Agents.Internal;

namespace Master40.Agents
{
    public class WorkListStatusEntry
    {
        WorkListStatusEntry(Agent agent, Status status, int count, int currentTime)
        {
            Agent = agent;
            Status = status;
            StatusCount = count;
            Time = currentTime;
        }

        Agent Agent { get; set; }
        Status Status { get; set; }
        int StatusCount { get; set; }
        int Time { get; set; }
    }
}