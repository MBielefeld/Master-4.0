using Master40.DB.Data.Context;
using Master40.DB.Enums;
using Master40.DB.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Master40.ViewComponents
{
    public partial class MachineSetupViewComponent : ViewComponent
    {
        private readonly ProductionDomainContext _context;

        public MachineSetupViewComponent(ProductionDomainContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(List<string> paramsList)
        {
            Task calculation;
            calculation = calculateAllKpisFromWorkSchedueles(paramsList);

            await calculation;
            ViewData["simId"] = paramsList[0];
            ViewData["Type"] = paramsList[1];
            ViewData["simNumber"] = paramsList[2];
            return View($"MachineSetup");
        }

        private List<string> getMachines(List<SimulationWorkschedule> simulationWorkSchedules)
        {
            List<string> machines = simulationWorkSchedules.OrderBy(x => x.Machine).Select(n => n.Machine).Distinct().ToList();

            return machines;
        }
        private List<string> getMachineTools(List<SimulationWorkschedule> simulationWorkSchedules)
        {
            List<string> machineTools = simulationWorkSchedules.OrderBy(x => x.MachineTool).Select(n => n.MachineTool).Distinct().ToList();

            return machineTools;
        }

        private Task calculateAllKpisFromWorkSchedueles(List<string> paramsList)
        {
            var generateChartTask = Task.Run(() =>
            {
                if (!_context.SimulationWorkschedules.Any())
                {
                    return;
                }

                SimulationType simType = (paramsList[1].Equals("Decentral"))
                    ? SimulationType.Decentral
                    : SimulationType.Central;

                var simulationWorkSchedules = _context.SimulationWorkschedules.Where(x => x.SimulationConfigurationId == Convert.ToInt32(paramsList[0])
                                                       && x.SimulationType == simType
                                                        && x.SimulationNumber == Convert.ToInt32(paramsList[2]))
                                          .OrderByDescending(g => g.Start)
                   .ToList();

                //machineList
                List<string> machines = getMachines(simulationWorkSchedules);
                ViewData["machines"] = machines;

                List<string> machinetools = getMachineTools(simulationWorkSchedules);
                ViewData["machineTools"] = machinetools;

                //SetupAmount
                List<double> setupList = calculateSetupItems(simulationWorkSchedules, machinetools);
                ViewData["dataSetupList"] = setupList;

                //WorkItemsAmount
                List<double> workList = calculateWorkItems(simulationWorkSchedules, machinetools);
                ViewData["dataWorkList"] = workList;

                //Work to SetupRatio
                List<double> ratioList = calculateRatio(workList, setupList);
                ViewData["dataRatioList"] = ratioList;

                //SetupTime
                List<double> setupTime = calculateSetupTime(simulationWorkSchedules, machinetools);
                ViewData["dataSetupTimeList"] = setupTime;

                //WorkTime
                List<double> workTime = calculateWorkTime(simulationWorkSchedules, machinetools);
                ViewData["dataWorkTimeList"] = workTime;

                //WorkTime to SetupTimeRatio
                List<double> ratioTimeList = calculateRatio(workTime, setupTime);
                ViewData["dataRatioTimeList"] = ratioTimeList;

                return;
            });

            return generateChartTask;
        }

        private List<double> calculateSetupTime(List<SimulationWorkschedule> simulationWorkSchedules, List<string> machineTools)
        {
            List<double> dataSet = new List<double>();
            var machines = simulationWorkSchedules.OrderBy(x => x.Machine).Select(n => n.Machine).Distinct().ToList();

            foreach (var machine in machines)
            {
                foreach (var machineTool in machineTools)
                {
                    double count = simulationWorkSchedules.Where(x => x.Machine == machine && x.MachineTool == machineTool && x.setupEnd - x.setupStart != 0).Select(x => x.setupEnd - x.setupStart).ToList().Sum();
                    dataSet.Add((count > 0 ? count : 0));
                }

                dataSet.Add(
                      (double) simulationWorkSchedules.Where(x => x.Machine == machine && x.setupEnd - x.setupStart != 0).Select(x => x.setupEnd - x.setupStart).ToList().Sum()                    
                );
            }

            return dataSet;
        }

        private List<double> calculateWorkTime(List<SimulationWorkschedule> simulationWorkSchedules, List<string> machineTools)
        {
            List<double> dataSet = new List<double>();
            var machines = simulationWorkSchedules.OrderBy(x => x.Machine).Select(n => n.Machine).Distinct().ToList();

            foreach (var machine in machines)
            {
                foreach (var machineTool in machineTools)
                {
                    double count = simulationWorkSchedules.Where(x => x.Machine == machine && x.MachineTool == machineTool && x.End - x.Start != 0).Select(x => x.End - x.Start).ToList().Sum();
                    dataSet.Add((count > 0 ? count : 0));
                }

                dataSet.Add(
                      (double)simulationWorkSchedules.Where(x => x.Machine == machine && x.End - x.Start != 0).Select(x => x.End - x.Start).ToList().Sum()
                );
            }

            return dataSet;
        }


        private List<double> calculateRatio(List<double> workList, List<double> setupList)
        {
            List<double> dataSet = new List<double>();

            for (int i = 0; i < setupList.Count; i++)
            {
                double count = Math.Round(workList[i] / setupList[i], 2);
                dataSet.Add((count > 0 ? count : 0));
            }

            return dataSet;
        }

        private List<double> calculateSetupItems(List<SimulationWorkschedule> simulationWorkSchedules, List<string> machineTools)
        {
            List<double> dataSet = new List<double>();
            var machines = simulationWorkSchedules.OrderBy(x => x.Machine).Select(n => n.Machine).Distinct().ToList();

            foreach (var machine in machines)
            {
                foreach (var machineTool in machineTools)
                {
                    var count = simulationWorkSchedules.Where(x => x.Machine == machine && x.MachineTool == machineTool && x.setupEnd - x.setupStart != 0).ToList().Count();

                    dataSet.Add((count > 0 ? count : 0));
                }

                dataSet.Add(
                    (double) simulationWorkSchedules.Where(x => x.Machine == machine && x.setupEnd - x.setupStart != 0).ToList().Count()
                );
            }
            
            return dataSet;
        }


        private List<double> calculateWorkItems(List<SimulationWorkschedule> simulationWorkSchedules, List<string> machineTools)
        {

            List<double> dataSet = new List<double>();
            var machines = simulationWorkSchedules.OrderBy(x => x.Machine).Select(n => n.Machine).Distinct().ToList();

            foreach (var machine in machines)
            {
                foreach (var machineTool in machineTools)
                {
                    var count = simulationWorkSchedules.Where(x => x.Machine == machine && x.MachineTool == machineTool && x.End - x.Start != 0).ToList().Count();

                    dataSet.Add((count > 0 ? count : 0));
                }

                dataSet.Add(
                    (double) simulationWorkSchedules.Where(x => x.Machine == machine && x.End - x.Start != 0).ToList().Count()
                );
            }
            
            return dataSet;
        }
    }
}
