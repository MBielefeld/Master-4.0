﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Master40.DB.Models;
using Master40.Simulation.Simulation.SimulationData;

namespace Master40.Simulation.Simulation
{
    public class OrderSimulationItem : ISimulationItem
    {
        public OrderSimulationItem(int start, int end)
        {
            SimulationState = SimulationState.Waiting;
            Start = start;
            End = end;
        }
        public int ProductionOrderWorkScheduleId { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public SimulationState SimulationState { get; set; }
        public Task<bool> DoAtStart()
        {
            return null;
        }

        public Task<bool> DoAtEnd<T>(List<TimeTable<T>.MachineStatus> listMachineStatus) where T : ISimulationItem
        {
            throw new NotImplementedException();
        }
    }
}