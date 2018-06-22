using Master40.DB.Data.Context;
using Master40.DB.Schema;
using Master40.DB.Schema.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Master40.DB.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MasterDBContext _context;

        public UnitOfWork(MasterDBContext context)
        {
            _context = context;
            Articles = new ArticleRepository(_context);
        }

        public IArticleRepo Articles { get; private set; }
        public IArticleToBusinessPartnerRepo ArticleToBusinessPartners { get; private set; }
        public IArticleTypeRepo ArticleTypes { get; private set; }
        public IBusinessPartnerRepo BusinessPartners { get; private set; }
        public IDemandProviderRepo DemandProviders { get; private set; }
        public IKpiRepo Kpis { get; private set; }
        public IOrder Orders { get; private set; }
        public IOrderPartRepo OrderParts { get; private set; }
        public IProductionOrderRepo ProductionOrders { get; private set; }
        public IProductionOrderBomRepo ProductionOrderBoms { get; private set; }
        public IProductionOrderWorkScheduleRepo ProductionOrderWorkSchedules { get; private set; }
        public IPurchaseRepo Purchases { get; private set; }
        public IPurchasePartRepo PurchaseParts { get; private set; }
        public ISimulationRepo Simulations { get; private set; }
        public ISimulationConfigurationRepo SimulationConfigurations { get; private set; }
        public ISimulationWorkscheduleRepo SimulationWorkschedules { get; private set; }
        public IStockRepo Stocks { get; private set; }
        public IStockExchangeRepo StockExchanges { get; private set; }
        public IUnitRepo Units { get; private set; }
        public IWorkScheduleRepo WorkSchedules { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
