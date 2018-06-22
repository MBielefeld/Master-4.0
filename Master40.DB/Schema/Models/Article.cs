﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace Master40.DB.Models
{
    public class Article : BaseEntity
    {
        public Article()
        {
            ArticleBoms = new HashSet<ArticleBom>();
            ArticleChilds = new HashSet<ArticleBom>();
            WorkSchedules = new HashSet<WorkSchedule>();
            ProductionOrders = new HashSet<ProductionOrder>();
            DemandToProviders = new HashSet<DemandToProvider>();
            ArticleToBusinessPartners = new HashSet<ArticleToBusinessPartner>();
        }
        public string Name { get; set; }
        [Display(Name = "Packing Unit")]
        public int UnitId { get; set; }
        public virtual Unit Unit { get; set; }
        [Display(Name = "Article Type")]
        public int ArticleTypeId { get; set; }
        [JsonIgnore]
        public virtual ArticleType ArticleType { get; set; }
        //[DisplayFormat(DataFormatString = "{0:0,0}")]
        // 
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreationDate { get; set; }
        public Stock Stock { get; set; }
        [JsonIgnore]
        public virtual ICollection<ArticleBom> ArticleBoms { get; set; }
        [JsonIgnore]
        public virtual ICollection<ArticleBom> ArticleChilds { get; set; }
        [JsonIgnore]
        // public virtual IEnumerable<ArticleBomPart> ArticleChilds { get; set; } 
        public virtual ICollection<WorkSchedule> WorkSchedules { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductionOrder> ProductionOrders { get; set; }
        [JsonIgnore]
        public virtual ICollection<DemandToProvider> DemandToProviders { get; set; }
        [JsonIgnore]
        public virtual ICollection<ArticleToBusinessPartner> ArticleToBusinessPartners { get; set;}
        public bool ToPurchase { get; set; }
        public bool ToBuild { get; set; }
        public string PictureUrl { get; set; }


    }
}