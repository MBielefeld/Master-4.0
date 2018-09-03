using Newtonsoft.Json;
using System.Collections.Generic;

namespace Master40.DB.Models
{
    public class MachineTool : BaseEntity
    {
        public string Name { get; set; }
        [JsonIgnore]
        //MARVIN 22.05.2018 MachineTools werden immer auf eine Gruppe von Machinen angewandt
        //public Machine Machine { get; set; }
        //public ICollection<MachineGroup> MachineGroups { get; set; }
        public MachineGroup MachineGroup { get; set; }
        public string Discription { get; set; }
        public int SetupTime { get; set; }
    }
    
}
