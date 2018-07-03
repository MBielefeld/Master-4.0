using System;
using System.Collections.Generic;
using System.Text;
using Master40.Agents.Agents;
using Master40.Agents.Agents.Internal;
using Master40.DB.Models;

namespace Master40.Agents.Agents
{
    public class DemandAgent : Agent
    {
        public Article article;

        public int QuantityToProduce;

        protected DemandAgent(Agent creator, string name, bool debug) : base(creator, name, debug)
        {

        }


    }
}
