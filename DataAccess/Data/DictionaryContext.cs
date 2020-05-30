using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class DictionaryContext : DbContext
    {
        public DbSet<Dictionary> Dictionaries { get; set; }

        public DbSet<WordClass> WordClasses { get; set; }

        public DbSet<Word> Words { get; set; }

        public DbSet<Definition> Definitions { get; set; }

        public DbSet<Usage> Usages { get; set; }

        public DbSet<Phase> Phases { get; set; }

        public DbSet<RelativeWord> WordRelativeWord { get; set; }

        public DbSet<WordForm> WordForms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configBuilder = new ConfigurationBuilder();
            var config = configBuilder.AddJsonFile("appsettings.json", true).Build();

            optionsBuilder.UseSqlServer(config["ConnectionString"]);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dictionary>(b =>
            {
                b.ToTable("Dictionary").Property(e => e.Name).HasMaxLength(100).IsRequired();
            });
            modelBuilder.Entity<WordClass>(b =>
            {
                b.ToTable("WordClass").Property(p => p.Name).HasMaxLength(100).IsRequired();
            });
            modelBuilder.Entity<Word>(b =>
            {
                b.ToTable("Word").HasIndex(p => p.Content);//.IsUnique(); //TODO
                b.Property(p => p.Content).HasMaxLength(200).IsRequired();
                b.Property(p => p.Spelling).HasMaxLength(200);
                b.Property(p => p.SpellingAudioUrl).HasMaxLength(1000);
                b.HasMany(p => p.RelativeWords).WithOne(p => p.Word).OnDelete(DeleteBehavior.ClientSetNull);
            });
            modelBuilder.Entity<Definition>(b =>
            {
                b.ToTable("Definition");
                b.Property(p => p.Content).IsRequired();
            });
            modelBuilder.Entity<Usage>(e =>
            {
                e.ToTable("Usage");
                e.Property(p => p.Sample).HasMaxLength(2000).IsRequired();
                e.Property(p => p.Translation).HasMaxLength(2000);
            });
            modelBuilder.Entity<WordForm>(e =>
            {
                e.ToTable("WordForm");
                e.Property(p => p.Content).HasMaxLength(200).IsRequired();
            });
            modelBuilder.Entity<RelativeWord>(e =>
            {
                //e.HasKey(p => new { p.WordId, p.RelWord }); //TODO
            });
            modelBuilder.Entity<Phase>(e =>
            {
                e.ToTable("Phase");
                e.Property(p => p.Content).HasMaxLength(2000).IsRequired();
            });
            modelBuilder.Entity<PhaseDefinition>(e =>
            {
                e.ToTable("PhaseDefinition");
            });
            modelBuilder.Entity<PhaseUsage>(e =>
            {
                e.ToTable("PhaseUsage");
            });

        }
    }
}
