using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BscTokenSniper.Data.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BscTokenSniper.Data.EntityEvents
{
    public interface IOnChange<TEntity>
        where TEntity : BaseEntity
    {
        public void OnChange(BscTokenSniperDBContext db, IEnumerable<(EntityEntry entry, TEntity entity)> entityStatePair);
    }
}