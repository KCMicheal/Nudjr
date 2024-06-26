﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Nudjr_Domain.Entities;

namespace Nudjr_Domain.Context
{
    public class NudjrDatabaseContext : DbContext
    {
        public NudjrDatabaseContext(DbContextOptions<NudjrDatabaseContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            IEnumerable<IMutableForeignKey> foreignKeys = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            foreach (IMutableForeignKey fkRelationship in foreignKeys)
            {
                fkRelationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        public DbSet<USER> Users { get; set; }
        public DbSet<NOVUSUBSCRIBER> NovuSubscribers { get; set; }
        public DbSet<USER_SETTING> UserSettings { get; set; }
        public DbSet<USER_ACTIVITY_LOG> UserActivities { get; set; }
        public DbSet<ALARM> Alarms { get; set; }
        public DbSet<EVENT> Events { get; set; }
        public DbSet<NUDGE> Nudges { get; set; }
    }
}
