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
    public class MachineSetupTimeViewComponent : ViewComponent
    {
        private readonly ProductionDomainContext _context;

        public MachineSetupTimeViewComponent(ProductionDomainContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(List<string> paramsList)
        {
            Task<Chart> generateChartTask;

            generateChartTask = GenerateChartTask(paramsList);

            // create JS to Render Chart.
            ViewData["chart"] = await generateChartTask;
            ViewData["Type"] = paramsList[1];
            ViewData["OverTime"] = paramsList[3];
            return View($"MachineSetupTime");
        }

        private Task<Chart> GenerateChartTask(List<string> paramsList)
        {
            var generateChartTask = Task.Run(() =>
            {
                if (!_context.SimulationWorkschedules.Any())
                {
                    return null;
                }

                SimulationType simType = (paramsList[1].Equals("Decentral"))
                    ? SimulationType.Decentral
                    : SimulationType.Central;

                Chart stackedBar = new Chart
                {
                    Type = "bar"
                };

                var machines = _context.Kpis.Where(x => x.SimulationConfigurationId == Convert.ToInt32(paramsList[0])
                                                        && x.SimulationType == simType
                                                        && x.KpiType == KpiType.WorkItemListStatus
                                                        && x.AgentType == "MachineAgent"
                                                        && x.IsKpi
                                                        && x.IsFinal && x.SimulationNumber == Convert.ToInt32(paramsList[2]))
                                           .OrderByDescending(g => g.Name)
                    .ToList();
                var data = new Data { Labels = machines.Select(n => n.Name).ToList() };

                // create Dataset for each Lable    
                data.Datasets = new List<Dataset>();

                var i = 0;
                var cc = new ChartColor();

                //var max = _context.SimulationWorkschedules.Max(x => x.End) - 1440; 
                var barDataSet = new BarDataset { Data = new List<double>(), BackgroundColor = new List<string>(), HoverBackgroundColor = new List<string>(), YAxisID = "y-normal" };
                var barDiversityInvisSet = new BarDataset { Data = new List<double>(), BackgroundColor = new List<string>(), HoverBackgroundColor = new List<string>(), YAxisID = "y-diversity" };
                var barDiversitySet = new BarDataset { Data = new List<double>(), BackgroundColor = new List<string>(), HoverBackgroundColor = new List<string>(), YAxisID = "y-diversity" };
                foreach (var machine in machines)
                {
                    var percent = Math.Round(machine.Value * 100, 2);
                    // var wait = max - work;
                    barDataSet.Data.Add(percent);
                    barDataSet.BackgroundColor.Add(cc.Color[i].Substring(0, cc.Color[i].Length - 4) + "0.4)");
                    barDataSet.HoverBackgroundColor.Add(cc.Color[i].Substring(0, cc.Color[i].Length - 4) + "0.7)");

                    var varianz = machine.Count * 100;

                    barDiversityInvisSet.Data.Add(percent - Math.Round(varianz / 2, 2));
                    barDiversityInvisSet.BackgroundColor.Add(ChartColor.Transparent);
                    barDiversityInvisSet.HoverBackgroundColor.Add(ChartColor.Transparent);

                    barDiversitySet.Data.Add(Math.Round(varianz, 2));
                    barDiversitySet.BackgroundColor.Add(cc.Color[i].Substring(0, cc.Color[i].Length - 4) + "0.8)");
                    barDiversitySet.HoverBackgroundColor.Add(cc.Color[i].Substring(0, cc.Color[i].Length - 4) + "1)");
                    i++;
                }

                data.Datasets.Add(barDataSet);
                data.Datasets.Add(barDiversityInvisSet);
                data.Datasets.Add(barDiversitySet);

                stackedBar.Data = data;

                // Specifie xy Axis
                var xAxis = new List<Scale>() { new CartesianScale { Stacked = true, Id = "x-normal", Display = true } };
                var yAxis = new List<Scale>()
                {
                    new CartesianScale { Stacked = true, Display = true, Ticks = new CartesianLinearTick { BeginAtZero = true, Min = 0, Max = 100}, Id = "y-normal" },
                    new CartesianScale {
                        Stacked = true, Ticks = new CartesianLinearTick {BeginAtZero = true, Min = 0, Max = 100}, Display = false,
                        Id = "y-diversity", ScaleLabel = new ScaleLabel{ LabelString = "Value in %", Display = false, FontSize = 12 },
                    },
                };
                //var yAxis = new List<Scale>() { new BarScale{ Ticks = new CategoryTick { Min = "0", Max  = (yMaxScale * 1.1).ToString() } } };
                stackedBar.Options = new Options()
                {
                    Scales = new Scales { XAxes = xAxis, YAxes = yAxis },
                    MaintainAspectRatio = false,
                    Responsive = true,
                    Title = new Title { Text = "Machine WorkLoad with SetupTime", Position = "top", FontSize = 24, FontStyle = "bold", Display = true },
                    Legend = new Legend { Position = "bottom", Display = false }
                };

                return stackedBar;
            });
            return generateChartTask;
        }
    }
}