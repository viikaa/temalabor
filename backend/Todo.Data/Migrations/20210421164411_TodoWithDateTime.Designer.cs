﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Todo.DAL;

namespace Todo.DAL.Migrations
{
    [DbContext(typeof(TodoDbContext))]
    [Migration("20210421164411_TodoWithDateTime")]
    partial class TodoWithDateTime
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Todo.DAL.Column", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Columns");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Title = "Első"
                        },
                        new
                        {
                            Id = 2,
                            Title = "Második"
                        });
                });

            modelBuilder.Entity("Todo.DAL.Todo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ColumnId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ColumnId");

                    b.ToTable("Todos");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ColumnId = 1,
                            Deadline = new DateTime(2021, 4, 21, 18, 44, 10, 125, DateTimeKind.Local).AddTicks(2835),
                            Description = "lol",
                            Priority = 0,
                            Title = "egyeske"
                        },
                        new
                        {
                            Id = 2,
                            ColumnId = 2,
                            Deadline = new DateTime(2021, 4, 21, 18, 44, 10, 130, DateTimeKind.Local).AddTicks(1886),
                            Description = "xd",
                            Priority = 0,
                            Title = "ketteske"
                        });
                });

            modelBuilder.Entity("Todo.DAL.Todo", b =>
                {
                    b.HasOne("Todo.DAL.Column", "Column")
                        .WithMany("Todos")
                        .HasForeignKey("ColumnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Column");
                });

            modelBuilder.Entity("Todo.DAL.Column", b =>
                {
                    b.Navigation("Todos");
                });
#pragma warning restore 612, 618
        }
    }
}