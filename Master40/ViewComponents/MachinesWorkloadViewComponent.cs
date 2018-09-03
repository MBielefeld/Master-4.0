using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChartJSCore.Models;
using Master40.DB.Data.Context;
using Master40.Extensions;
using ChartJSCore.Models.Bar;
using Master40.DB.Enums;

namespace Master40.ViewComponents
{
    public partial class MachinesWorkLoadViewComponent : ViewComponent
    {
        private readonly ProductionDomainContext _context;

        public MachinesWorkLoadViewComponent(ProductionDomainContext context)
        {
            _context = context;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(List<string> paramsList)
        {
            Task<Chart> generateChartTask; 
            
            if (Convert.ToInt32(paramsList[3]) == 1)
            {
                generateChartTask = GenerateChartTaskOverTime(paramsList);
            }
            else  generateChartTask = GenerateChartTask(paramsList);

            // create JS to Render Chart.
            ViewData["chart"] = await generateChartTask;
            ViewData["Type"] = paramsList[1];
            ViewData["OverTime"] = paramsList[3];
            return View($"MachinesWorkLoad");
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

                Chart chart = new Chart
                {
                    Type = "bar"                             
                };

                // charttype

                // use available hight in Chart
                // use available hight in Chart
                var machines = _context.Kpis.Where(x => x.SimulationConfigurationId == Convert.ToInt32(paramsList[0])
                                                        && x.SimulationType == simType
                                                        && x.KpiType == KpiType.MachineUtilization
                                                        && x.IsKpi
                                                        && x.IsFinal && x.SimulationNumber == Convert.ToInt32(paramsList[2]))
                                           .OrderByDescending(g => g.Name)
                    .ToList();

                var data = new Data { Labels = machines.OrderBy(m => m.Name).GroupBy(x => x.Name).Select(k => k.First().Name).ToList() };

                // create Dataset for each Lable
                data.Datasets = new List<Dataset>();

                var i = 0;
                var cc = new ChartColor();
                
                //var max = _context.SimulationWorkschedules.Max(x => x.End) - 1440; 
                var barDataSetWork = new BarDataset {Data = new List<double>(), BackgroundColor = new List<string>(), HoverBackgroundColor = new List<string>(), YAxisID= "y-normal"};
                var barDataSetSetup = new BarDataset { Data = new List<double>(), BackgroundColor = new List<string>(), HoverBackgroundColor = new List<string>(), YAxisID = "y-normal"};
                var barDiversityInvisSet = new BarDataset { Data = new List<double>(), BackgroundColor = new List<string>(), HoverBackgroundColor = new List<string>(), YAxisID= "y-diversity"};
                var barDiversitySet = new BarDataset { Data = new List<double>(), BackgroundColor = new List<string>(), HoverBackgroundColor = new List<string>(), YAxisID ="y-diversity"};

                foreach (var machine in machines.Where(x => x.Status == "Work"))
                {
                    
                    var workPercent = Math.Round(machine.Value * 100, 2);
                    var setupPercent = Math.Round(machines.Where(x => x.Name == machine.Name && x.Status == "Setup").Single().Value * 100, 2);
                    // var wait = max - work;
                    barDataSetWork.Data.Add(workPercent);
                    barDataSetWork.BackgroundColor.Add(cc.Color[4].Substring(0, cc.Color[4].Length - 4) + "0.4)");
                    barDataSetWork.HoverBackgroundColor.Add(cc.Color[4].Substring(0, cc.Color[4].Length - 4) + "0.7)");

                    barDataSetSetup.Data.Add(setupPercent);
                    barDataSetSetup.BackgroundColor.Add(cc.Color[0].Substring(0, cc.Color[0].Length - 4) + "0.4)");
                    barDataSetSetup.HoverBackgroundColor.Add(cc.Color[0].Substring(0, cc.Color[0].Length - 4) + "0.7)");

                    var varianz = machine.Count * 100 /2;

                    barDiversityInvisSet.Data.Add(workPercent + setupPercent - Math.Round(varianz / 2, 2));
                    barDiversityInvisSet.BackgroundColor.Add(ChartColor.Transparent);
                    barDiversityInvisSet.HoverBackgroundColor.Add(ChartColor.Transparent);

                    barDiversitySet.Data.Add(Math.Round(varianz, 2));
                    barDiversitySet.BackgroundColor.Add(cc.Color[1].Substring(0, cc.Color[1].Length - 4) + "0.8)");
                    barDiversitySet.HoverBackgroundColor.Add(cc.Color[1].Substring(0, cc.Color[1].Length - 4) + "1)");
                    i++;
                }

                data.Datasets.Add(barDataSetSetup);
                data.Datasets.Add(barDataSetWork);
                data.Datasets.Add(barDiversityInvisSet);
                data.Datasets.Add(barDiversitySet);
                
                chart.Data = data;

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
                chart.Options = new Options()
                {
                    Scales = new Scales { XAxes = xAxis, YAxes = yAxis },
                    MaintainAspectRatio = false,
                    Responsive = true,
                    Title = new Title { Text = "Machine Workloads", Position = "top", FontSize = 24, FontStyle = "bold", Display = true },
                    Legend = new Legend { Position = "bottom", Display = false}
                };

                return chart;
            });
            return generateChartTask;
        }

        private Task<Chart> GenerateChartTaskOverTime(List<string> paramsList)
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

                Chart chart = new Chart { Type = "scatter" };

                // charttype
                var cc = new ChartColor();
                // use available hight in Chart
                // use available hight in Chart
                var machinesKpi = _context.Kpis.Where(x => x.SimulationConfigurationId == Convert.ToInt32(paramsList[0])
                                                        && x.SimulationType == simType
                                                        && x.KpiType == KpiType.MachineUtilization
                                                        && !x.IsKpi
                                                        && !x.IsFinal && x.SimulationNumber == Convert.ToInt32(paramsList[2]))
                    .ToList();
                var settlingTime = _context.SimulationConfigurations.First(x => x.Id == Convert.ToInt32(paramsList[0])).SettlingStart;
                var machines = machinesKpi.Select(n => n.Name).Distinct().ToList();
                var machineNames = machinesKpi.OrderBy(m => m.Name).GroupBy(x => x.Name).Select(k => k.First().Name).ToList();
                var data = new Data { Labels = machinesKpi.OrderBy(m => m.Name).GroupBy(x => x.Name).Select(k => k.First().Name).ToList() };

                // create Dataset for each Lable
                data.Datasets = new List<Dataset>();

                var i = 0;
                foreach (var machine in machineNames)
                {
                    // add zero to start
                    var kpis = new List<LineScatterData> { new LineScatterData {  x = "0", y = "0" } };
                    
                    kpis.AddRange(machinesKpi.Where(x => x.Name == machine && x.Status == "Work").OrderBy(x => x.Time)
                        .Select(x => new LineScatterData { x = x.Time.ToString(), y = ((x.Value + machinesKpi.Where(y => y.Name == machine && y.Status == "Work" && x.Time == y.Time).Select(y => y.Value).FirstOrDefault()) * 100).ToString() }).ToList());
                   
                    var lds = new LineScatterDataset()
                    {
                        Data = kpis,
                        BorderWidth = 2,
                        Label = machine,
                        ShowLine = true,
                        Fill = "false",
                        BackgroundColor = cc.Color[i],
                        BorderColor = cc.Color[i++],
                        LineTension = 0
                    };
                    data.Datasets.Add(lds);

                    var setupKpis = new List<LineScatterData> { new LineScatterData { x = "0", y = "0" } };
                    setupKpis.AddRange(machinesKpi.Where(x => x.Name == machine && x.Status == "Setup").OrderBy(x => x.Time)
                       .Select(x => new LineScatterData { x = x.Time.ToString(), y = (machinesKpi.Where(y => y.Name == machine && y.Status == "Setup" && x.Time == y.Time).Select(y => y.Value).FirstOrDefault() * 100).ToString() }).ToList());

                    var setupLds = new LineScatterDataset()
                    {
                        Data = setupKpis,
                        BorderWidth = 2,
                        Label = machine,
                        ShowLine = true,
                        Fill = "true",
                        BackgroundColor = cc.Color[i],
                        BorderColor = cc.Color[i++],
                        LineTension = 0
                    };
                    data.Datasets.Add(setupLds);
                }

                data.Datasets.Add(new LineScatterDataset()
                {
                    Data = new List<LineScatterData> { new LineScatterData { x = "0", y = "100" }, new LineScatterData { x = Convert.ToDouble(settlingTime).ToString(), y = "100" } },
                    BorderWidth = 1,
                    Label = "Settling time",
                    BackgroundColor = "rgba(0, 0, 0, 0.1)",
                    BorderColor = "rgba(0, 0, 0, 0.3)",
                    ShowLine = true,
                    //Fill = true,
                    //SteppedLine = false,
                    LineTension = 0,
                    PointRadius = new List<int> { 0, 0}
                });

                chart.Data = data;

                // Specifie xy Axis
                var xAxis = new List<Scale>() { new CartesianScale { Stacked = false, Display = true } };
                var yAxis = new List<Scale>()
                {
                    new CartesianScale()
                    {
                        Stacked = false, Ticks = new CartesianLinearTick {BeginAtZero = true, Min = 0, Max = 100}, Display = true,
                        Id = "first-y-axis", Type = "linear" , ScaleLabel = new ScaleLabel{ LabelString = "Value in %", Display = true, FontSize = 12 },
                    }
                };
                //var yAxis = new List<Scale>() { new BarScale{ Ticks = new CategoryTick { Min = "0", Max  = (yMaxScale * 1.1).ToString() } } };
                chart.Options = new Options()
                {
                    Scales = new Scales { XAxes = xAxis, YAxes = yAxis },
                    Responsive = true,
                    MaintainAspectRatio = true,
                    Legend = new Legend { Position = "bottom", Display = true, FullWidth = true },
                    Title = new Title { Text = "Machine Workload over Time", Position = "top", FontSize = 24, FontStyle = "bold", Display = true }
                };

                return chart;
            });
            return generateChartTask;
        }
    }
}
