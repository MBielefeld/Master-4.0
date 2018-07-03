using System;
using System.Collections.Generic;
using System.Text;
using Master40.DB.Models;

namespace Master40.Agents.Agents.Model
{
    class ProductionSet
    {
        public Guid ProductionSetId { get; set; }

        public Article Article { get; set; }

        public List<RequestItem> ProductionList { get; set;}

        public int ProductionQuantity { get; set; }
        
        public bool IsInProduction { get; set; }

        public ProductionSet(Article article)
        {
            Article = article;
        }
        
    }
}
