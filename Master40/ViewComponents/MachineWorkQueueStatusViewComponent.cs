using ChartJSCore.Models;
using Master40.DB.Data.Context;
using Master40.DB.Enums;
using Master40.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Master40.ViewComponents
{
    public partial class MachineWorkQueueStatusViewComponent : ViewComponent
    {

        private readonly ProductionDomainContext _context;   

        public MachineWorkQueueStatusViewComponent(ProductionDomainContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(List<string> paramsList)
        {
            Task<List<Chart>> generateChartTask;
            List<string> _paramsList = paramsList;

            if (_paramsList[3] != null) {
                generateChartTask = GenerateChartTaskSingle(paramsList);
                ViewData["chart"] = await generateChartTask;
            }
            else
            {
                generateChartTask = GenerateChartTasksMulti(paramsList);
                ViewData["chart"] = await generateChartTask;
            }

            // create JS to Render Chart.
            ViewData["Type"] = paramsList[1];
            return View($"MachineWorkQueueStatus");
        }

        private Task<List<Chart>> GenerateChartTaskSingle(List<string> paramsList)
        {
            var generateChartTask = Task.Run(() =>
            {
                List<Chart> charts = new List<Chart>();
                List<string> machines = new List<string>();
                string machineName = Convert.ToString(paramsList[3]);

                if (!_context.Kpis.Any())
                {
                    return null;
                }

                SimulationType simType = (paramsList[1].Equals("Decentral"))
                    ? SimulationType.Decentral
                    : SimulationType.Central;

                var simConfig = _context.SimulationConfigurations.Single(x => x.Id == Convert.ToInt32(paramsList[0]));
                
                var statusKpi = _context.Kpis.Where(x => x.SimulationConfigurationId == Convert.ToInt32(paramsList[0])
                                                        && x.SimulationType == simType
                                                        && x.KpiType == KpiType.WorkItemListStatus
                                                        && x.AgentType == "MachineAgent"
                                                        && x.IsKpi
                                                        && x.Name == machineName
                                                        && x.IsFinal && x.SimulationNumber == Convert.ToInt32(paramsList[2]))
                    .ToList();

                DB.Models.Kpi machine = statusKpi.FirstOrDefault();

                //Scaling of Axis
                var maxX = Convert.ToInt32(Math.Floor((decimal)simConfig.SimulationEndTime / 1000) * 1000);
                var maxY = Math.Ceiling(statusKpi.Max(x => x.Value)) + 5;
                var maxStepSize = Math.Ceiling(maxY * 0.1);

                charts.Add(createChartForMachine(statusKpi, machine, maxX, maxY, maxStepSize));
                machines.Add(machineName);
                ViewData["machines"] = machines;
                return charts;
            });
            
            return generateChartTask;
        }

        //Generates a List<Charts> for every Machine in SimulationWorkSchedule
        private Task<List<Chart>> GenerateChartTasksMulti(List<string> paramsList)
        {
            var generateChartTask = Task.Run(() =>
            {
                List <Chart> charts = new List<Chart>();
                List<string> machines = new List<string>();

                if (!_context.Kpis.Any())
                {
                    return null;
                }

                SimulationType simType = (paramsList[1].Equals("Decentral"))
                    ? SimulationType.Decentral
                    : SimulationType.Central;

                //get total simulation time
                var simConfig = _context.SimulationConfigurations.Single(x => x.Id == Convert.ToInt32(paramsList[0]));
                


                var statusKpi = _context.Kpis.Where(x => x.SimulationConfigurationId == Convert.ToInt32(paramsList[0])
                                                        && x.SimulationType == simType
                                                        && x.KpiType == KpiType.WorkItemListStatus
                                                           && x.AgentType == "MachineAgent"
                                                        && x.IsKpi
                                                        && x.IsFinal && x.SimulationNumber == Convert.ToInt32(paramsList[2]))
                    .ToList();
                         
                var eachMachineInStatusKpi = statusKpi.OrderBy(i => i.Name).GroupBy(x => x.Name).Select(k => k.First()).ToList();

                //Scaling of Axis
                var maxX = Convert.ToInt32(Math.Floor((decimal)simConfig.SimulationEndTime / 1000) * 1000);
                var maxY = Math.Ceiling(statusKpi.Max(x => x.Value)) + 5;
                var maxStepSize = Math.Ceiling(maxY * 0.1);

                foreach (var machine in eachMachineInStatusKpi)
                {
                    //new Chart
                    Chart chart = createChartForMachine(statusKpi, machine, maxX, maxY, maxStepSize);

                    machines.Add(machine.Name);
                    charts.Add(chart);
                }

                ViewData["machines"] = machines;
                return charts;
            });

            return generateChartTask;
        }


        private Chart createChartForMachine(List<DB.Models.Kpi> statusKpi, DB.Models.Kpi machine, double maxX, double maxY, double maxStepSize)
        {

            Chart chart = new Chart { Type = "scatter" };
            var cc = new ChartColor();


            //Label nach Status
            var data = new Data { Labels = statusKpi.GroupBy(x => x.Status).Select(k => k.First().Status.ToString()).ToList() };

            // create Dataset for each Lable
            data.Datasets = new List<Dataset>();


            var i = 0;
            foreach (Status status in (Status[])Enum.GetValues(typeof(Status)))
            {
                if (status == Status.Finished || status == Status.Created || status == Status.Processed)
                {
                    continue;
                }

                var kpis = new List<LineScatterData> { };

                kpis.AddRange(statusKpi.Where(x => x.Name == machine.Name && x.Status == status.ToString()).OrderBy(n => n.Time).Select(x => new LineScatterData { x = x.Time.ToString(), y = x.Value.ToString() }).ToList());
                

                var lds = new LineScatterDataset()
                {
                    Data = kpis,
                    BorderWidth = 2,
                    Label = status.ToString(),
                    ShowLine = true,
                    Fill = "true",
                    BackgroundColor = cc.Color[i],
                    BorderColor = cc.Color[i++],
                    LineTension = 0
                };
                data.Datasets.Add(lds);
            }

            chart.Data = data;

            // Specifie xy Axis
            var xAxis = new List<Scale>() { new CartesianScale { Stacked = false, Display = true, Ticks = new CartesianLinearTick { BeginAtZero = true, Min = 0, Max = maxX, StepSize = maxX / 10 } } };
            var yAxis = new List<Scale>()
                {
                    new CartesianScale()
                    {
                        Stacked = true, Ticks = new CartesianLinearTick {BeginAtZero = true, Min = 0, Max = maxY, StepSize = maxStepSize }, Display = true,
                        Id = "first-y-axis", Type = "linear" , ScaleLabel = new ScaleLabel{ LabelString = "Amount", Display = true, FontSize = 12 },
                    }
                };
            //var yAxis = new List<Scale>() { new BarScale{ Ticks = new CategoryTick { Min = "0", Max  = (yMaxScale * 1.1).ToString() } } };
            chart.Options = new Options()
            {
                Scales = new Scales { XAxes = xAxis, YAxes = yAxis },
                Responsive = true,
                MaintainAspectRatio = true,
                Legend = new Legend { Position = "right", Display = true, FullWidth = true },
                Title = new Title { Text = machine.Name + " Work Queue", Position = "top", FontSize = 24, FontStyle = "bold", Display = true }
            };

            return chart;
        }
    }
}
