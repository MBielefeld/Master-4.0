﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Master40.DB.Data.Context;
using Master40.DB.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace Master40.Simulation.Simulation
{
    public class PowsSimulationItem : ISimulationItem
    {
        public PowsSimulationItem(int start, int end, MasterDBContext context)
        {
            SimulationState = SimulationState.Waiting;
            Start = start;
            End = end;
            NeedToAddNext = false;
            _context = context;
        }

        private MasterDBContext _context;
        public int Start { get; set; }
        public int End { get; set; }
        public int ProductionOrderWorkScheduleId { get; set; }
        public int ProductionOrderId { get; set; }
        public SimulationState SimulationState { get; set; }
        public bool NeedToAddNext { get; set; }
        
        public Task<bool> DoAtStart()
        {

            //_context.Stocks.Single(a => a.ArticleForeignKey == ArticleId).Current++;
            return null;
        }

        public Task<bool> DoAtEnd()
        {
            NeedToAddNext = true;
            var pows = _context.ProductionOrderWorkSchedule.Include(a => a.ProductionOrder).Where(a => a.ProductionOrderId == ProductionOrderId);
            if (pows.Single(a => a.Id == ProductionOrderWorkScheduleId).HierarchyNumber !=
                pows.Max(a => a.HierarchyNumber)) return null;
            var articleId = _context.ProductionOrders.Single(a => a.Id == ProductionOrderId).ArticleId;
            _context.Stocks.Single(a => a.ArticleForeignKey == articleId).Current++;
            
            return null;
        }

    }
}
