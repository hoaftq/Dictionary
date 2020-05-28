﻿// <auto-generated />
using System;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataAccess.Migrations
{
    [DbContext(typeof(DictionaryContext))]
    partial class DictionaryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DataAccess.Models.Definition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DictionaryId")
                        .HasColumnType("int");

                    b.Property<int>("WordClassId")
                        .HasColumnType("int");

                    b.Property<int>("WordId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DictionaryId");

                    b.HasIndex("WordClassId");

                    b.HasIndex("WordId");

                    b.ToTable("Definition");
                });

            modelBuilder.Entity("DataAccess.Models.Dictionary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Dictionary");
                });

            modelBuilder.Entity("DataAccess.Models.Phase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(2000)")
                        .HasMaxLength(2000);

                    b.Property<int>("DictionaryId")
                        .HasColumnType("int");

                    b.Property<int>("WordId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DictionaryId");

                    b.HasIndex("WordId");

                    b.ToTable("Phase");
                });

            modelBuilder.Entity("DataAccess.Models.PhaseDefinition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PhaseId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PhaseId");

                    b.ToTable("PhaseDefinition");
                });

            modelBuilder.Entity("DataAccess.Models.PhaseUsage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("PhaseDefinitionId")
                        .HasColumnType("int");

                    b.Property<string>("Sample")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Translation")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PhaseDefinitionId");

                    b.ToTable("PhaseUsage");
                });

            modelBuilder.Entity("DataAccess.Models.RelativeWord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsSynomym")
                        .HasColumnType("bit");

                    b.Property<string>("RelWord")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WordClass")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WordId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WordId");

                    b.ToTable("WordRelativeWord");
                });

            modelBuilder.Entity("DataAccess.Models.Usage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DefinitionId")
                        .HasColumnType("int");

                    b.Property<string>("Sample")
                        .IsRequired()
                        .HasColumnType("nvarchar(2000)")
                        .HasMaxLength(2000);

                    b.Property<string>("Translation")
                        .HasColumnType("nvarchar(2000)")
                        .HasMaxLength(2000);

                    b.HasKey("Id");

                    b.HasIndex("DefinitionId");

                    b.ToTable("Usage");
                });

            modelBuilder.Entity("DataAccess.Models.Word", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<string>("Spelling")
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<string>("SpellingAudioUrl")
                        .HasColumnType("nvarchar(1000)")
                        .HasMaxLength(1000);

                    b.HasKey("Id");

                    b.HasIndex("Content");

                    b.ToTable("Word");
                });

            modelBuilder.Entity("DataAccess.Models.WordClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("WordClass");
                });

            modelBuilder.Entity("DataAccess.Models.WordForm", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<string>("FormType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("WordId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("WordId");

                    b.ToTable("WordForm");
                });

            modelBuilder.Entity("DataAccess.Models.Definition", b =>
                {
                    b.HasOne("DataAccess.Models.Dictionary", "Dictionary")
                        .WithMany()
                        .HasForeignKey("DictionaryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccess.Models.WordClass", "WordClass")
                        .WithMany()
                        .HasForeignKey("WordClassId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccess.Models.Word", "Word")
                        .WithMany("Definitions")
                        .HasForeignKey("WordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataAccess.Models.Phase", b =>
                {
                    b.HasOne("DataAccess.Models.Dictionary", "Dictionary")
                        .WithMany()
                        .HasForeignKey("DictionaryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccess.Models.Word", "Word")
                        .WithMany("Phases")
                        .HasForeignKey("WordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataAccess.Models.PhaseDefinition", b =>
                {
                    b.HasOne("DataAccess.Models.Phase", null)
                        .WithMany("Definitions")
                        .HasForeignKey("PhaseId");
                });

            modelBuilder.Entity("DataAccess.Models.PhaseUsage", b =>
                {
                    b.HasOne("DataAccess.Models.PhaseDefinition", null)
                        .WithMany("Usages")
                        .HasForeignKey("PhaseDefinitionId");
                });

            modelBuilder.Entity("DataAccess.Models.RelativeWord", b =>
                {
                    b.HasOne("DataAccess.Models.Word", "Word")
                        .WithMany("RelativeWords")
                        .HasForeignKey("WordId")
                        .IsRequired();
                });

            modelBuilder.Entity("DataAccess.Models.Usage", b =>
                {
                    b.HasOne("DataAccess.Models.Definition", "Definition")
                        .WithMany("Usages")
                        .HasForeignKey("DefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataAccess.Models.WordForm", b =>
                {
                    b.HasOne("DataAccess.Models.Word", null)
                        .WithMany("WordForms")
                        .HasForeignKey("WordId");
                });
#pragma warning restore 612, 618
        }
    }
}
