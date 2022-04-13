using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BscTokenSniper.Data.Entities;
using BscTokenSniper.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;

namespace BscTokenSniper.Data
{
    public class SessionRepository : IDisposable
    {
        public SessionRepository(BscTokenSniperDBContext dbContext = null)
        {
            this.db = dbContext ?? new SqliteDBContext();
        }

        public void Migrate()
        {
            lock (lockingObject)
            {
                db.Database.Migrate();
            }
        }

        public IEnumerable<TokensOwnedEntity> GetAllActiveSessionsTokensOwned() {
            lock (lockingObject)
            {
                var session = GetActiveSession();
                
                return db.TokensOwnedEntities.Where(x => x.SessionId == session.Id);
            }
        }

        public TokensOwnedEntity AddTokensOwned(TokensOwnedEntity tokensOwned) {
            lock (lockingObject)
            {
                var session = GetActiveSession();
                
                tokensOwned.SessionId = session.Id;
                tokensOwned.Session = session;

                tokensOwned = db.TokensOwnedEntities.Add(tokensOwned).Entity;
                db.SaveChanges();
            }
            return tokensOwned;
        }

        public TokensOwnedEntity RemoveTokensOwned(TokensOwnedEntity tokensOwned) {
            lock (lockingObject)
            {
                tokensOwned = db.TokensOwnedEntities.Remove(tokensOwned).Entity;
                db.SaveChanges();
            }
            return tokensOwned;
        }


        public Session SaveSession(Session session)
        {
            lock (lockingObject)
            {
                if (db.Sessions.Where(x => x.Id == session.Id).Count() == 0)
                {
                    session = db.Sessions.Add(session).Entity;
                }
                db.SaveChanges();
            }

            return session;
        }

        public Session GetActiveSession(SniperConfiguration sniperConfiguration = null)
        {
            lock (lockingObject)
            {
                if (activeSession != null)
                {
                    var sniperConfigurationJson = JsonConvert.SerializeObject(sniperConfiguration);
                    var activeConfiguration = activeSession.Configurations.FirstOrDefault();

                    ValidateSessionWithConfiguration(activeSession, sniperConfigurationJson);
                    if (sniperConfigurationJson != activeSession.Configurations.FirstOrDefault().SniperConfigurationJson) {
                        activeConfiguration.SniperConfigurationJson = sniperConfigurationJson;
                        db.SaveChanges();
                        Log.Logger.Information("Configuration changed to session with Id {sessionId}.", activeSession.Id);
                    }
                }
                else if (sniperConfiguration == null) {
                    Log.Logger.Fatal("Configuration for new session not provided StackTrace: {stackTrace}", Environment.StackTrace);
                    throw new InvalidOperationException("Configuration is required for new session.");
                }
                else
                {
                    activeSession = LoadOrCreateActiveSession(sniperConfiguration);
                }
            }

            return activeSession;
        }

        #region Private members
        private Session LoadOrCreateActiveSession(SniperConfiguration sniperConfiguration)
        {
            var currentConfigJson = JsonConvert.SerializeObject(sniperConfiguration);

            var loadedSession = db.Sessions
                .Include(x => x.Configurations)
                .Include(x => x.TokensOwnedEntities)
                .Where(x => x.Active)
                .FirstOrDefault();

            if (loadedSession != null)
            {
                var configuration = loadedSession.Configurations.FirstOrDefault();

                ValidateSessionWithConfiguration(loadedSession, currentConfigJson);
                configuration.SniperConfigurationJson = currentConfigJson;
                db.SaveChanges();
                Log.Logger.Information("Configuration changed to session with Id {sessionId}.", loadedSession.Id);
            }
            else
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    loadedSession = db.Sessions.Add(new Session()).Entity;
                    db.SaveChanges();

                    var dbConfig = new Configuration { SniperConfiguration = sniperConfiguration };
                    db.Configurations.Add(dbConfig);
                    db.SaveChanges();
                    transaction.Commit();
                }
                Log.Logger.Information("Starting new session with Id {sessionId}.", loadedSession.Id);
            }

            return loadedSession;
        }

        private void ValidateSessionWithConfiguration(Session session, string sniperConfigurationJson)
        {
            if (session.Configurations.FirstOrDefault()?.SniperConfigurationJson != sniperConfigurationJson
                && activeSession.TokensOwnedEntities.Count() > 0)
            {
                Log.Logger.Fatal("There are owned tokens with previous configuration for session with Id {sessionId}.", activeSession.Id);
                throw new InvalidOperationException("There are owned tokens with previous configuration.");
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }

        private BscTokenSniperDBContext db;
        private Session activeSession;

        private object lockingObject = new object();

        #endregion
    }
}