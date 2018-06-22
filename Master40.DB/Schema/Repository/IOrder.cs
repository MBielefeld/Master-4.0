using Master40.DB.Models;
using Master40.DB.Schema.Enums;
using Master40.DB.Data.Repository;

namespace Master40.DB.Schema.Repository
{
    public interface IOrder
    {
        int BusinessPartnerId { get; set; }
        int CreationTime { get; set; }
        int DueTime { get; set; }
        int FinishingTime { get; set; }
        string Name { get; set; }
        State State { get; set; }
    }

    public interface IOrderRepo : IRepository<Order>
    { }
}
