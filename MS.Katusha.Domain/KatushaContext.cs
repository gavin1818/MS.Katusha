﻿using System.Data.Entity;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Domain
{
    public class KatushaContext : DbContext, IKatushaContext
    {
        public DbSet<Girl> Girls { get; set; }
        public DbSet<Boy> Boys { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<State> States { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Visit> Visits { get; set; }

        public DbSet<SearchingFor> Searches { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<CountriesToVisit> CountriesToVisit { get; set; }
        public DbSet<LanguagesSpoken> LanguagesSpoken { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>()
                        .HasMany(u => u.SentMessages)
                        .WithRequired(ul => ul.From)
                        .HasForeignKey(ul => ul.FromId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Profile>()
                        .HasMany(u => u.RecievedMessages)
                        .WithRequired(ul => ul.To)
                        .HasForeignKey(ul => ul.ToId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Profile>()
                        .HasMany(u => u.Visited)
                        .WithRequired(ul => ul.Profile)
                        .HasForeignKey(ul => ul.ProfileId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Profile>()
                        .HasMany(u => u.WhoVisited)
                        .WithRequired(ul => ul.VisitorProfile)
                        .HasForeignKey(ul => ul.VisitorProfileId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<State>().HasRequired(x => x.Profile); //.WithOptional(s => s.State).Map( x => x.MapKey("StateId"));
            modelBuilder.Entity<User>().HasRequired(x => x.Profile); //.WithOptional(s => s.Profile).Map(x => x.MapKey("ProfileId"));
            //modelBuilder.Entity<Photo>().HasRequired(p => p.Profile).WithMany(c => c.Photos).Map(x => x.MapKey("ProfileId"));


            base.OnModelCreating(modelBuilder);
        }
    }
}