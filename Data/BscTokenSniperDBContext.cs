using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BscTokenSniper.Data.Entities;
using BscTokenSniper.Data.EntityEvents;
using Microsoft.EntityFrameworkCore;

namespace BscTokenSniper.Data
{
    public class BscTokenSniperDBContext : DbContext
    {
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<TokensOwnedEntity> TokensOwnedEntities { get; set; }

        public BscTokenSniperDBContext() : base() {}

        public BscTokenSniperDBContext(DbContextOptions options) : base(options)
        {
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            OnBeforeSaving();
            return (await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken));
        }

        #region  Private members
        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            var utcNow = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                // for entities that inherit from BaseEntity,
                // set UpdatedOn / CreatedOn appropriately
                if (entry.Entity is BaseEntity trackable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            // set the updated date to "now"
                            trackable.UpdatedOn = utcNow;

                            // mark property as "don't touch"
                            // we don't want to update on a Modify operation
                            entry.Property("CreatedOn").IsModified = false;
                            break;
                        case EntityState.Added:
                            // set both updated and created date to "now"
                            trackable.CreatedOn = utcNow;
                            trackable.UpdatedOn = utcNow;
                            break;
                    }
                }
            }
            
            foreach (var onChangeEvent in onChangeEvents)
            {
                var entityType = onChangeEvent.GetType().GetTypeInfo().GenericTypeArguments.FirstOrDefault();
                var changedOrAddedEntities = entries
                    .Where(x => x.Entity.GetType() == entityType && 
                        x.Entity is BaseEntity &&
                        (x.State == EntityState.Modified || x.State == EntityState.Added))
                    .Select(x => (x, x.Entity as BaseEntity));      

                onChangeEvent.OnChange(this, changedOrAddedEntities);
            }
        }

        private List<IOnChange<BaseEntity>> onChangeEvents = new List<IOnChange<BaseEntity>>
        {
            new SessionOnChange() as IOnChange<BaseEntity>
        };

        #endregion
    }
}
