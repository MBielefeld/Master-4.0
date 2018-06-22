using Master40.DB.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Master40.DB.Data.Context
{
    public interface IMasterDBContext
    {
        DbSet<ArticleBom> ArticleBoms { get; set; }
        DbSet<Article> Articles { get; set; }
        DbSet<ArticleToBusinessPartner> ArticleToBusinessPartners { get; set; }
        DbSet<ArticleType> ArticleTypes { get; set; }
        DbSet<BusinessPartner> BusinessPartners { get; set; }
        DbSet<DemandProductionOrderBom> DemandProductionOrderBoms { get; set; }
        DbSet<DemandToProvider> Demands { get; set; }
        DbSet<Kpi> Kpis { get; set; }
        DbSet<MachineGroup> MachineGroups { get; set; }
        DbSet<Machine> Machines { get; set; }
        DbSet<MachineTool> MachineTools { get; set; }
        DbSet<OrderPart> OrderParts { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<ProductionOrderBom> ProductionOrderBoms { get; set; }
        DbSet<ProductionOrder> ProductionOrders { get; set; }
        DbSet<ProductionOrderWorkSchedule> ProductionOrderWorkSchedules { get; set; }
        DbSet<PurchasePart> PurchaseParts { get; set; }
        DbSet<Purchase> Purchases { get; set; }
        DbSet<SimulationConfiguration> SimulationConfigurations { get; set; }
        DbSet<SimulationOrder> SimulationOrders { get; set; }
        DbSet<SimulationWorkschedule> SimulationWorkschedules { get; set; }
        DbSet<StockExchange> StockExchanges { get; set; }
        DbSet<Stock> Stocks { get; set; }
        DbSet<Unit> Units { get; set; }
        DbSet<WorkSchedule> WorkSchedules { get; set; }
    }
}