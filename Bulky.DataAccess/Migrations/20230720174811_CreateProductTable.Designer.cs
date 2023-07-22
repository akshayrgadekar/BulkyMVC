﻿// <auto-generated />
using Bulky.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Bulky.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230720174811_CreateProductTable")]
    partial class CreateProductTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Bulky.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayOrder = 1,
                            Name = "Action"
                        },
                        new
                        {
                            Id = 2,
                            DisplayOrder = 2,
                            Name = "SciFi"
                        },
                        new
                        {
                            Id = 3,
                            DisplayOrder = 3,
                            Name = "History"
                        });
                });

            modelBuilder.Entity("Bulky.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ISBN")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ListPrice")
                        .HasColumnType("float");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<double>("Price50")
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Author = "Joseph Murphy",
                            Description = "",
                            ISBN = "",
                            ListPrice = 100.0,
                            Price = 105.0,
                            Price50 = 90.0,
                            Title = "The Power of Your Subconscious Mind"
                        },
                        new
                        {
                            Id = 2,
                            Author = "Darius Foroux",
                            Description = "",
                            ISBN = "",
                            ListPrice = 140.0,
                            Price = 150.0,
                            Price50 = 125.0,
                            Title = "Do It Today"
                        },
                        new
                        {
                            Id = 3,
                            Author = "thibaut meurisse and Kerry J Donovan",
                            Description = "",
                            ISBN = "",
                            ListPrice = 13000.0,
                            Price = 13105.0,
                            Price50 = 11000.0,
                            Title = "Master Your Emotions"
                        },
                        new
                        {
                            Id = 4,
                            Author = "",
                            Description = "",
                            ISBN = "",
                            ListPrice = 169.0,
                            Price = 179.0,
                            Price50 = 149.0,
                            Title = "Energize Your Mind"
                        },
                        new
                        {
                            Id = 5,
                            Author = "",
                            Description = "",
                            ISBN = "",
                            ListPrice = 89.0,
                            Price = 99.0,
                            Price50 = 70.0,
                            Title = "Brain Activity Book for Kids"
                        },
                        new
                        {
                            Id = 6,
                            Author = "",
                            Description = "",
                            ISBN = "",
                            ListPrice = 125.0,
                            Price = 139.0,
                            Price50 = 105.0,
                            Title = "The Secret Garden"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}