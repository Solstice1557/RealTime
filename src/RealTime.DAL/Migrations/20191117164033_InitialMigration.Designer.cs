﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RealTime.DAL;

namespace RealTime.DAL.Migrations
{
    [DbContext(typeof(PricesDbContext))]
    [Migration("20191117164033_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0");

            modelBuilder.Entity("RealTime.DAL.Entities.Fund", b =>
                {
                    b.Property<int>("FundId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(300);

                    b.Property<string>("Symbol")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<int>("Volume")
                        .HasColumnType("INTEGER");

                    b.HasKey("FundId");

                    b.HasIndex("Symbol")
                        .IsUnique()
                        .HasName("IX_Funds_Symbol");

                    b.ToTable("Funds");
                });

            modelBuilder.Entity("RealTime.DAL.Entities.Price", b =>
                {
                    b.Property<long>("PriceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Close")
                        .HasColumnType("TEXT");

                    b.Property<int>("FundId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("High")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Low")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Open")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Volume")
                        .HasColumnType("TEXT");

                    b.HasKey("PriceId");

                    b.HasIndex("FundId", "Timestamp")
                        .IsUnique()
                        .HasName("IX_Prices_FundIdTimestamp");

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("RealTime.DAL.Entities.Price", b =>
                {
                    b.HasOne("RealTime.DAL.Entities.Fund", "Fund")
                        .WithMany()
                        .HasForeignKey("FundId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
