using Master40.DB.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Master40.DB.Models
{
    public class WorkSchedule : BaseEntity, IWorkSchedule
    {
        public int HierarchyNumber { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public int MachineToolId { get; set; }
        public MachineTool MachineTool { get; set; }

        /*
         * MachineGroup obsolete as soon as MachineTool is active
         */
        public int MachineGroupId { get; set; }
        [JsonIgnore]
        public MachineGroup MachineGroup { get; set; }
        public int ArticleId { get; set; }
        [JsonIgnore]
        public Article Article { get; set; }
    }
}