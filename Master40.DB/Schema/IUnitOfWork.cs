using Master40.DB.Schema.Repository;
using System;

namespace Master40.DB.Schema
{
    public interface IUnitOfWork : IDisposable
    {
        IArticleRepo Articles { get; }
        IArticleToBusinessPartnerRepo ArticleToBusinessPartners { get; }
        IArticleTypeRepo ArticleTypes { get; }
        IBusinessPartnerRepo BusinessPartners { get; }
        IDemandProviderRepo DemandProviders { get; }
        IKpiRepo Kpis { get; }
        IOrder Orders { get; }
        IOrderPartRepo OrderParts { get; }
        IProductionOrderRepo ProductionOrders { get; }
        IProductionOrderBomRepo ProductionOrderBoms { get; }
        IProductionOrderWorkScheduleRepo ProductionOrderWorkSchedules { get; }
        IPurchaseRepo Purchases { get; }
        IPurchasePartRepo PurchaseParts { get; }
        ISimulationRepo Simulations { get; }
        ISimulationConfigurationRepo SimulationConfigurations { get; }
        ISimulationWorkscheduleRepo SimulationWorkschedules { get; }
        IStockRepo Stocks { get; }
        IStockExchangeRepo StockExchanges { get; }
        IUnitRepo Units { get; }
        IWorkScheduleRepo WorkSchedules { get; }
        int Complete();
    }
}
