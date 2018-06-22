using Master40.DB.Models;
using Master40.DB.Data.Repository;

namespace Master40.DB.Schema.Repository
{
    public interface IWorkSchedule
    {
        int HierarchyNumber { get; set; }
        string Name { get; set; }
        int Duration { get; set; }
        int MachineGroupId { get; set; }
    }

    public interface ISimulationProductionOrderWorkSchedule
    {
        int HierarchyNumber { get; set; }
        int Start { get; set; }
        int End { get; set; }
        int ProductionOrderId { get; set; }
        ProductionOrder ProductionOrder { get; set; }
    }

    public interface IWorkScheduleRepo : IRepository<WorkSchedule>
    {

    }
}
