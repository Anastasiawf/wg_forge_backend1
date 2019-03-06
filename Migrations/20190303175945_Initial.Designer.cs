﻿// <auto-generated />
using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using wg_backend.Models;

namespace wg_backend.Migrations
{
    [DbContext(typeof(WgContext))]
    [Migration("20190303175945_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:Enum:cat_color", "black,white,black & white,red,red & white,red & black & white")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("wg_backend.Models.Cat", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("name");

                    b.Property<cat_color>("Color")
                        .HasColumnName("color");

                    b.Property<int>("Tail_length")
                        .HasColumnName("tail_length");

                    b.Property<int>("Whiskers_length")
                        .HasColumnName("whiskers_length");

                    b.HasKey("Name");

                    b.ToTable("cats");
                });

            modelBuilder.Entity("wg_backend.Models.Cat_colors_info", b =>
                {
                    b.Property<cat_color>("Color")
                        .HasColumnName("color");

                    b.Property<int>("Count")
                        .HasColumnName("count");

                    b.HasKey("Color");

                    b.ToTable("cat_colors_info");
                });

            modelBuilder.Entity("wg_backend.Models.Cats_stat", b =>
                {
                    b.Property<double>("tail_length_mean");

                    b.Property<double>("tail_length_median");

                    b.Property<int[]>("tail_length_mode");

                    b.Property<double>("whiskers_length_mean");

                    b.Property<double>("whiskers_length_median");

                    b.Property<int[]>("whiskers_length_mode");

                    b.HasKey("tail_length_mean");

                    b.ToTable("cats_stat");
                });

            modelBuilder.Entity("wg_backend.Models.Dog", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("name");

                    b.Property<cat_color>("Color")
                        .HasColumnName("color");

                    b.Property<int>("Tail_length")
                        .HasColumnName("tail_length");

                    b.Property<int>("Whiskers_length")
                        .HasColumnName("whiskers_length");

                    b.HasKey("Name");

                    b.ToTable("dogs");
                });
#pragma warning restore 612, 618
        }
    }
}