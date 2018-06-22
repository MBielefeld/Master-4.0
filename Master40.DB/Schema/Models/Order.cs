using System.Collections.Generic;
using Newtonsoft.Json;
using Master40.DB.Schema.Enums;
using Master40.DB.Schema.Repository;

namespace Master40.DB.Models
{
    public class Order : BaseEntity, IOrder
    {
        public string Name { get; set; }
        public int DueTime { get; set; }
        public int CreationTime { get; set; }
        public int FinishingTime { get; set; }
        public int BusinessPartnerId { get; set; }
        [JsonIgnore]
        public BusinessPartner BusinessPartner { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderPart> OrderParts { get; set; }
        public State State { get; set; }
    }
}
