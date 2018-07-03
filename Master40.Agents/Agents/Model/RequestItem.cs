﻿using System;
using System.Collections.Generic;
using Master40.Agents.Agents.Internal;
using Master40.DB.Models;

namespace Master40.Agents.Agents.Model
{
    public class RequestItem
    {
        public RequestItem()
        {
            Provided = false;
            IsHeadDemand = false;
            ProviderList = new List<Guid>();
        }
        public Article Article { get; set; }
        public IDemandToProvider IDemandToProvider { get; set; }
        public Guid StockExchangeId { get; set; }
        public int Quantity { get; set; }
        public int QuantityInProduction { get; set; }
        public int DueTime { get; set; }
        public Agent Requester { get; set; }
        public List<Guid> ProviderList { get; set; }
        public int OrderId { get; set; }
        public int Providable { get; set; }
        public bool Provided { get; set; }
        public bool IsHeadDemand { get; set; }
    }
}