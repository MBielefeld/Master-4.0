﻿using System;
using Master40.DB.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Master40.DB.Migrations
{
    [DbContext(typeof(MasterDBContext))]
    [Migration("20170511093649_FancyDemandTPH")]
    partial class FancyDemandTPH
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Master40.DB.Models.Article", b =>
                {
                    b.Property<int>("ArticleId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ArticleTypeId");

                    b.Property<DateTime>("CreationDate");

                    b.Property<int>("DeliveryPeriod");

                    b.Property<string>("Name");

                    b.Property<double>("Price");

                    b.Property<int>("UnitId");

                    b.HasKey("ArticleId");

                    b.HasIndex("ArticleTypeId");

                    b.HasIndex("UnitId");

                    b.ToTable("Article");
                });

            modelBuilder.Entity("Master40.DB.Models.ArticleBom", b =>
                {
                    b.Property<int>("ArticleBomId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ArticleChildId");

                    b.Property<int?>("ArticleParentId");

                    b.Property<string>("Name");

                    b.Property<decimal>("Quantity");

                    b.HasKey("ArticleBomId");

                    b.HasIndex("ArticleChildId");

                    b.HasIndex("ArticleParentId");

                    b.ToTable("ArticleBoms");
                });

            modelBuilder.Entity("Master40.DB.Models.ArticleToDemand", b =>
                {
                    b.Property<int>("ArticleId");

                    b.Property<int>("DemandToProviderId");

                    b.Property<decimal>("Quantity");

                    b.HasKey("ArticleId", "DemandToProviderId");

                    b.HasIndex("DemandToProviderId");

                    b.ToTable("ArticleToDemand");
                });

            modelBuilder.Entity("Master40.DB.Models.ArticleType", b =>
                {
                    b.Property<int>("ArticleTypeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("ArticleTypeId");

                    b.ToTable("ArticleTypes");
                });

            modelBuilder.Entity("Master40.DB.Models.BusinessPartner", b =>
                {
                    b.Property<int>("BusinessPartnerId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Debitor");

                    b.Property<bool>("Kreditor");

                    b.Property<string>("Name");

                    b.HasKey("BusinessPartnerId");

                    b.ToTable("BusinessPartners");
                });

            modelBuilder.Entity("Master40.DB.Models.DemandToProvider", b =>
                {
                    b.Property<int>("DemandId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ArticleId");

                    b.Property<int?>("DemandRequesterId");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<bool>("IsProvided");

                    b.Property<int>("Quantity");

                    b.HasKey("DemandId");

                    b.HasIndex("ArticleId");

                    b.HasIndex("DemandRequesterId");

                    b.ToTable("Demands");

                    b.HasDiscriminator<string>("Discriminator").HasValue("DemandToProvider");
                });

            modelBuilder.Entity("Master40.DB.Models.Machine", b =>
                {
                    b.Property<int>("MachineId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Capacity");

                    b.Property<int>("Count");

                    b.Property<int>("MachineGroupId");

                    b.Property<string>("Name");

                    b.HasKey("MachineId");

                    b.HasIndex("MachineGroupId");

                    b.ToTable("Machines");
                });

            modelBuilder.Entity("Master40.DB.Models.MachineGroup", b =>
                {
                    b.Property<int>("MachineGroupId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("MachineGroupId");

                    b.ToTable("MachineGroups");
                });

            modelBuilder.Entity("Master40.DB.Models.MachineTool", b =>
                {
                    b.Property<int>("MachineToolId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("MachineId");

                    b.Property<string>("Name");

                    b.Property<int>("SetupTime");

                    b.HasKey("MachineToolId");

                    b.HasIndex("MachineId");

                    b.ToTable("MachineTools");
                });

            modelBuilder.Entity("Master40.DB.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BusinessPartnerId");

                    b.Property<int>("DueTime");

                    b.Property<string>("Name");

                    b.HasKey("OrderId");

                    b.HasIndex("BusinessPartnerId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Master40.DB.Models.OrderPart", b =>
                {
                    b.Property<int>("OrderPartId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ArticleId");

                    b.Property<int>("OrderId");

                    b.Property<int>("Quantity");

                    b.HasKey("OrderPartId");

                    b.HasIndex("ArticleId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderParts");
                });

            modelBuilder.Entity("Master40.DB.Models.ProductionOrder", b =>
                {
                    b.Property<int>("ProductionOrderId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ArticleId");

                    b.Property<string>("Name");

                    b.Property<decimal>("Quantity");

                    b.HasKey("ProductionOrderId");

                    b.HasIndex("ArticleId");

                    b.ToTable("ProductionOrders");
                });

            modelBuilder.Entity("Master40.DB.Models.ProductionOrderBom", b =>
                {
                    b.Property<int>("ProductionOrderBomId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("End");

                    b.Property<string>("Name");

                    b.Property<int>("ProductionOrderChildId");

                    b.Property<int?>("ProductionOrderParentId");

                    b.Property<decimal>("Quantity");

                    b.Property<int>("Start");

                    b.HasKey("ProductionOrderBomId");

                    b.HasIndex("ProductionOrderChildId");

                    b.HasIndex("ProductionOrderParentId");

                    b.ToTable("ProductionOrderBoms");
                });

            modelBuilder.Entity("Master40.DB.Models.ProductionOrderToProductionOrderWorkSchedule", b =>
                {
                    b.Property<int>("ProductionOrderToProductionOrderWorkScheduleId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ProductionOrderId");

                    b.Property<int?>("ProductionOrderWorkScheduleId");

                    b.HasKey("ProductionOrderToProductionOrderWorkScheduleId");

                    b.HasIndex("ProductionOrderId");

                    b.HasIndex("ProductionOrderWorkScheduleId");

                    b.ToTable("ProductionOrderToProductionOrderWorkSchedules");
                });

            modelBuilder.Entity("Master40.DB.Models.ProductionOrderWorkSchedule", b =>
                {
                    b.Property<int>("ProductionOrderWorkScheduleId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Duration");

                    b.Property<int>("HierarchyNumber");

                    b.Property<int?>("MachineGroupId");

                    b.Property<int?>("MachineToolId");

                    b.Property<string>("Name");

                    b.HasKey("ProductionOrderWorkScheduleId");

                    b.HasIndex("MachineGroupId");

                    b.HasIndex("MachineToolId");

                    b.ToTable("ProductionOrderWorkSchedule");
                });

            modelBuilder.Entity("Master40.DB.Models.Purchase", b =>
                {
                    b.Property<int>("PurchaseId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BusinessPartnerId");

                    b.Property<int>("DueTime");

                    b.Property<string>("Name");

                    b.HasKey("PurchaseId");

                    b.HasIndex("BusinessPartnerId");

                    b.ToTable("Purchases");
                });

            modelBuilder.Entity("Master40.DB.Models.PurchasePart", b =>
                {
                    b.Property<int>("PurchasePartId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ArticleId");

                    b.Property<int>("PurchaseId");

                    b.Property<int>("Quantity");

                    b.HasKey("PurchasePartId");

                    b.HasIndex("ArticleId");

                    b.HasIndex("PurchaseId");

                    b.ToTable("PurchaseParts");
                });

            modelBuilder.Entity("Master40.DB.Models.Stock", b =>
                {
                    b.Property<int>("StockId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ArticleForeignKey");

                    b.Property<decimal>("Current");

                    b.Property<decimal>("Max");

                    b.Property<decimal>("Min");

                    b.Property<string>("Name");

                    b.HasKey("StockId");

                    b.HasIndex("ArticleForeignKey")
                        .IsUnique();

                    b.ToTable("Stock");
                });

            modelBuilder.Entity("Master40.DB.Models.Unit", b =>
                {
                    b.Property<int>("UnitId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("UnitId");

                    b.ToTable("Units");
                });

            modelBuilder.Entity("Master40.DB.Models.WorkSchedule", b =>
                {
                    b.Property<int>("WorkScheduleId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ArticleId");

                    b.Property<int>("Duration");

                    b.Property<int>("HierarchyNumber");

                    b.Property<int>("MachineGroupId");

                    b.Property<int?>("MachineToolId");

                    b.Property<string>("Name");

                    b.HasKey("WorkScheduleId");

                    b.HasIndex("ArticleId");

                    b.HasIndex("MachineGroupId");

                    b.HasIndex("MachineToolId");

                    b.ToTable("WorkSchedule");
                });

            modelBuilder.Entity("Master40.Models.Menu", b =>
                {
                    b.Property<int>("MenuId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("MenuName");

                    b.HasKey("MenuId");

                    b.ToTable("Menus");
                });

            modelBuilder.Entity("Master40.Models.MenuItem", b =>
                {
                    b.Property<int>("MenuItemId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Action");

                    b.Property<string>("LinkUrl")
                        .HasMaxLength(255);

                    b.Property<int>("MenuId");

                    b.Property<int?>("MenuOrder");

                    b.Property<string>("MenuText")
                        .HasMaxLength(50);

                    b.Property<int?>("ParentMenuItemId");

                    b.Property<string>("Symbol");

                    b.HasKey("MenuItemId");

                    b.HasIndex("MenuId");

                    b.HasIndex("ParentMenuItemId");

                    b.ToTable("MenuItems");
                });

            modelBuilder.Entity("Master40.DB.Models.DemandOrder", b =>
                {
                    b.HasBaseType("Master40.DB.Models.DemandToProvider");

                    b.Property<int>("OrderPartId");

                    b.HasIndex("OrderPartId");

                    b.ToTable("DemandOrder");

                    b.HasDiscriminator().HasValue("DemandOrder");
                });

            modelBuilder.Entity("Master40.DB.Models.DemandProductionOrderBom", b =>
                {
                    b.HasBaseType("Master40.DB.Models.DemandToProvider");

                    b.Property<int?>("ProductionOrderBomId");

                    b.Property<int>("ProductionOrderId");

                    b.HasIndex("ProductionOrderBomId");

                    b.ToTable("DemandProductionOrderBom");

                    b.HasDiscriminator().HasValue("DemandProductionOrderBom");
                });

            modelBuilder.Entity("Master40.DB.Models.DemandProviderProductionOrder", b =>
                {
                    b.HasBaseType("Master40.DB.Models.DemandToProvider");

                    b.Property<int>("ProductionOrderId");

                    b.HasIndex("ProductionOrderId");

                    b.ToTable("DemandProviderProductionOrder");

                    b.HasDiscriminator().HasValue("DemandProviderProductionOrder");
                });

            modelBuilder.Entity("Master40.DB.Models.DemandProviderPurchase", b =>
                {
                    b.HasBaseType("Master40.DB.Models.DemandToProvider");

                    b.Property<int>("PurchasePartId");

                    b.HasIndex("PurchasePartId");

                    b.ToTable("DemandProviderPurchase");

                    b.HasDiscriminator().HasValue("DemandProviderPurchase");
                });

            modelBuilder.Entity("Master40.DB.Models.DemandProviderStock", b =>
                {
                    b.HasBaseType("Master40.DB.Models.DemandToProvider");

                    b.Property<int>("StockId");

                    b.HasIndex("StockId");

                    b.ToTable("DemandProviderStock");

                    b.HasDiscriminator().HasValue("DemandProviderStock");
                });

            modelBuilder.Entity("Master40.DB.Models.DemandStock", b =>
                {
                    b.HasBaseType("Master40.DB.Models.DemandToProvider");

                    b.Property<int>("StockId");

                    b.HasIndex("StockId");

                    b.ToTable("DemandStock");

                    b.HasDiscriminator().HasValue("DemandStock");
                });

            modelBuilder.Entity("Master40.DB.Models.Article", b =>
                {
                    b.HasOne("Master40.DB.Models.ArticleType", "ArticleType")
                        .WithMany()
                        .HasForeignKey("ArticleTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Master40.DB.Models.Unit", "Unit")
                        .WithMany()
                        .HasForeignKey("UnitId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Master40.DB.Models.ArticleBom", b =>
                {
                    b.HasOne("Master40.DB.Models.Article", "ArticleChild")
                        .WithMany("ArticleChilds")
                        .HasForeignKey("ArticleChildId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Master40.DB.Models.Article", "ArticleParent")
                        .WithMany("ArticleBoms")
                        .HasForeignKey("ArticleParentId");
                });

            modelBuilder.Entity("Master40.DB.Models.ArticleToDemand", b =>
                {
                    b.HasOne("Master40.DB.Models.Article", "Article")
                        .WithMany("ArtilceToDemand")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Master40.DB.Models.DemandToProvider", "DemandToProvider")
                        .WithMany()
                        .HasForeignKey("DemandToProviderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Master40.DB.Models.DemandToProvider", b =>
                {
                    b.HasOne("Master40.DB.Models.Article", "Article")
                        .WithMany()
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Master40.DB.Models.DemandToProvider", "DemandRequester")
                        .WithMany("DemandProvider")
                        .HasForeignKey("DemandRequesterId");
                });

            modelBuilder.Entity("Master40.DB.Models.Machine", b =>
                {
                    b.HasOne("Master40.DB.Models.MachineGroup", "MachineGroup")
                        .WithMany("Machines")
                        .HasForeignKey("MachineGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Master40.DB.Models.MachineTool", b =>
                {
                    b.HasOne("Master40.DB.Models.Machine", "Machine")
                        .WithMany("MachineTools")
                        .HasForeignKey("MachineId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Master40.DB.Models.Order", b =>
                {
                    b.HasOne("Master40.DB.Models.BusinessPartner", "BusinessPartner")
                        .WithMany("Orders")
                        .HasForeignKey("BusinessPartnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Master40.DB.Models.OrderPart", b =>
                {
                    b.HasOne("Master40.DB.Models.Article", "Article")
                        .WithMany()
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Master40.DB.Models.Order", "Order")
                        .WithMany("OrderParts")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Master40.DB.Models.ProductionOrder", b =>
                {
                    b.HasOne("Master40.DB.Models.Article", "Article")
                        .WithMany("ProductionOrders")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Master40.DB.Models.ProductionOrderBom", b =>
                {
                    b.HasOne("Master40.DB.Models.ProductionOrder", "ProductionOrderChild")
                        .WithMany("ProdProductionOrderBomChilds")
                        .HasForeignKey("ProductionOrderChildId");

                    b.HasOne("Master40.DB.Models.ProductionOrder", "ProductionOrderParent")
                        .WithMany("ProductionOrderBoms")
                        .HasForeignKey("ProductionOrderParentId");
                });

            modelBuilder.Entity("Master40.DB.Models.ProductionOrderToProductionOrderWorkSchedule", b =>
                {
                    b.HasOne("Master40.DB.Models.ProductionOrder", "ProductionOrder")
                        .WithMany("ProductionOrderToProductionOrderWorkSchedule")
                        .HasForeignKey("ProductionOrderId");

                    b.HasOne("Master40.DB.Models.ProductionOrderWorkSchedule", "ProductionOrderWorkSchedule")
                        .WithMany("ProductionOrderToWorkSchedules")
                        .HasForeignKey("ProductionOrderWorkScheduleId");
                });

            modelBuilder.Entity("Master40.DB.Models.ProductionOrderWorkSchedule", b =>
                {
                    b.HasOne("Master40.DB.Models.MachineGroup", "MachineGroup")
                        .WithMany("ProductionOrderWorkSchedules")
                        .HasForeignKey("MachineGroupId");

                    b.HasOne("Master40.DB.Models.MachineTool", "MachineTool")
                        .WithMany()
                        .HasForeignKey("MachineToolId");
                });

            modelBuilder.Entity("Master40.DB.Models.Purchase", b =>
                {
                    b.HasOne("Master40.DB.Models.BusinessPartner", "BusinessPartner")
                        .WithMany("Purchases")
                        .HasForeignKey("BusinessPartnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Master40.DB.Models.PurchasePart", b =>
                {
                    b.HasOne("Master40.DB.Models.Article", "Article")
                        .WithMany()
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Master40.DB.Models.Purchase", "Purchase")
                        .WithMany("PurchaseParts")
                        .HasForeignKey("PurchaseId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Master40.DB.Models.Stock", b =>
                {
                    b.HasOne("Master40.DB.Models.Article", "Article")
                        .WithOne("Stock")
                        .HasForeignKey("Master40.DB.Models.Stock", "ArticleForeignKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Master40.DB.Models.WorkSchedule", b =>
                {
                    b.HasOne("Master40.DB.Models.Article", "Article")
                        .WithMany("WorkSchedules")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Master40.DB.Models.MachineGroup", "MachineGroup")
                        .WithMany("WorkSchedules")
                        .HasForeignKey("MachineGroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Master40.DB.Models.MachineTool", "MachineTool")
                        .WithMany()
                        .HasForeignKey("MachineToolId");
                });

            modelBuilder.Entity("Master40.Models.MenuItem", b =>
                {
                    b.HasOne("Master40.Models.Menu", "Menu")
                        .WithMany("MenuItems")
                        .HasForeignKey("MenuId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Master40.Models.MenuItem", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentMenuItemId");
                });

            modelBuilder.Entity("Master40.DB.Models.DemandOrder", b =>
                {
                    b.HasOne("Master40.DB.Models.OrderPart", "OrderPart")
                        .WithMany("DemandOdrders")
                        .HasForeignKey("OrderPartId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Master40.DB.Models.DemandProductionOrderBom", b =>
                {
                    b.HasOne("Master40.DB.Models.ProductionOrderBom", "ProductionOrderBom")
                        .WithMany("DemandProductionOrder")
                        .HasForeignKey("ProductionOrderBomId");
                });

            modelBuilder.Entity("Master40.DB.Models.DemandProviderProductionOrder", b =>
                {
                    b.HasOne("Master40.DB.Models.ProductionOrder", "ProductionOrder")
                        .WithMany()
                        .HasForeignKey("ProductionOrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Master40.DB.Models.DemandProviderPurchase", b =>
                {
                    b.HasOne("Master40.DB.Models.PurchasePart", "PurchasePart")
                        .WithMany("DemandProviderPurchases")
                        .HasForeignKey("PurchasePartId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Master40.DB.Models.DemandProviderStock", b =>
                {
                    b.HasOne("Master40.DB.Models.Stock", "Stock")
                        .WithMany("DemandProviderStock")
                        .HasForeignKey("StockId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Master40.DB.Models.DemandStock", b =>
                {
                    b.HasOne("Master40.DB.Models.Stock", "Stock")
                        .WithMany("DemandStock")
                        .HasForeignKey("StockId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
