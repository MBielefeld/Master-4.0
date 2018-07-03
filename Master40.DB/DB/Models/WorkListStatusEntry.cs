using Master40.DB.Enums;
using System;

namespace Master40.DB.DB.Models
{
    public class WorkListStatusEntry : BaseEntity
    {
        WorkListStatusEntry()
        {

        }
        WorkListStatusEntry(string agent, Status status, int count, int currentTime)
        {
            Agent = agent;
            Status = status;
            StatusCount = count;
            Time = currentTime;
        }
        string Agent { get; set; }
        Status Status { get; set; }
        int StatusCount { get; set; }
        int Time { get; set; }
    }
}
