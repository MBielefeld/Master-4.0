﻿using System;
using System.Collections.Generic;
using System.Linq;
using Master40.DB.Data.Context;
using Master40.DB.Enums;
using Master40.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace Master40.BusinessLogicCentral.MRP
{
    public interface IDemandForecast
    {
        List<ProductionOrder> NetRequirement(IDemandToProvider demand, MrpTask task, int simulationId);
    }


    public class DemandForecast : IDemandForecast
    {
        private readonly ProductionDomainContext _context;
        private readonly IProcessMrp _processMrp;

        public DemandForecast(ProductionDomainContext context, IProcessMrp processMrp)
        {
            _context = context;
            _processMrp = processMrp;
        }
        
        /// <summary>
        /// Creates providers for the demands through stock, productionOrders or purchases
        /// </summary>
        /// <param name="demand"></param>
        /// <param name="task"></param>
        /// <param name="simulationId"></param>
        /// <returns>ProductionOrder to fulfill the demand, ProductionOrder is null if there was enough in stock</returns>
        public List<ProductionOrder> NetRequirement(IDemandToProvider demand, MrpTask task, int simulationId)
        {
            //Todo: search for available POs
            var stock = _context.Stocks.Include(a => a.DemandStocks)
                .Single(a => a.ArticleForeignKey == demand.ArticleId);
            var plannedStock = _context.GetPlannedStock(stock,demand);
            var productionOrders = new List<ProductionOrder>();
            var stockProvider = _context.TryCreateStockReservation(demand);
            var time = _context.SimulationConfigurations.Single(a => a.Id == simulationId).Time;
            //if the article has no children it has to be purchased
            var children = _context.ArticleBoms.Where(a => a.ArticleParentId == demand.ArticleId).ToList();
            if (children.Any())
            {
                //if the plannedStock is below zero, articles have to be produced for its negative amount 
                if (plannedStock < 0)
                {
                    var fittingProductionOrders = _context.CheckForProductionOrders(demand,-plannedStock, _context.SimulationConfigurations.Single(a => a.Id == simulationId).Time);
                    var amount = -plannedStock;
                    if (fittingProductionOrders != null)
                    {
                        foreach (var fittingProductionOrder in fittingProductionOrders)
                        {
                            var availableAmount = _context.GetAvailableAmountFromProductionOrder(fittingProductionOrder);
                            var provider = _context.CreateDemandProviderProductionOrder(demand, fittingProductionOrder,
                                availableAmount < -plannedStock ? availableAmount : -plannedStock);
                            //productionOrders.Add(fittingProductionOrder);
                            _context.AssignProductionOrderToDemandProvider(fittingProductionOrder, provider);
                            amount -= availableAmount < -plannedStock ? availableAmount : -plannedStock;
                            if (amount == 0) return productionOrders;
                        }
                    }
                    if (amount > 0)
                    {
                        var pos = _context.CreateChildProductionOrders(demand, amount, simulationId);
                        productionOrders.AddRange(pos);
                    }
                }
            }
            else if (plannedStock < stock.Min)
                _context.CreatePurchaseDemand(demand, stock.Max-stock.Min, _context.SimulationConfigurations.Single(a => a.Id == simulationId).Time);
            if (stock.Min <= 0 || plannedStock >= stock.Min || demand.GetType() == typeof(DemandStock))
                return productionOrders;
            
            if (_context.Demands.OfType<DemandStock>()
                .Any(a => a.ArticleId == demand.ArticleId
                            && a.State != State.Finished))
                return productionOrders;

            var demandStock = plannedStock < 0 ? _context.CreateStockDemand(demand, stock, stock.Min) : _context.CreateStockDemand(demand, stock, stock.Min - plannedStock);
            //call processMrp to plan and schedule the stockDemand
            _processMrp.RunRequirementsAndTermination(demandStock, task, simulationId);
            return productionOrders;
        }

        

        
        
    }
    
}
