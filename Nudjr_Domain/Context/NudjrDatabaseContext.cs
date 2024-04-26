using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Nudjr_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
