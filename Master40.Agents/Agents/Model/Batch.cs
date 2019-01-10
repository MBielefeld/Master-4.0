using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Master40.Agents.Agents.Model
{
    public class Batch
    {
        public Func<long, double> PriorityRule { get; set; }
        public List<WorkItem> WorkItems { get; set; }
        public WorkItem Pop(long time) {

            if (WorkItems.Any()) return null;

            var item = WorkItems.FirstOrDefault(x => x.Priority(time) == WorkItems.Min(w => w.Priority(time)));
            WorkItems.Remove(item);
            return item;
        }
    }
}
