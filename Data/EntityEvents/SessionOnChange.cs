using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BscTokenSniper.Data.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BscTokenSniper.Data.EntityEvents
{
    public class SessionOnChange : IOnChange<Session>
    {
        public void OnChange(BscTokenSniperDBContext db, IEnumerable<(EntityEntry entry, Session entity)> entityStatePair)
        {
            var activeEntityPair = entityStatePair.Where(x => x.entity.Active).LastOrDefault();

            foreach (var entityPair in entityStatePair)
            {
                if (entityPair != activeEntityPair) {
                    entityPair.entity.Active = false;
                }
            }

            var sessionsToDeactivate = db.Sessions.Where(x => x.Active).ToList();

            foreach (var session in sessionsToDeactivate)
            {
                session.Active = false;
            }

            db.SaveChanges();
        }
    }
}