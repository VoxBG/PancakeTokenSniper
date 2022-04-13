using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BscTokenSniper.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BscTokenSniper.Data
{
    public class SqliteDBContext : BscTokenSniperDBContext
    {
        public string DbPath { get; }
        public SqliteDBContext() : base()
        {
            DbPath = GetDBPath();
        }

        public SqliteDBContext(DbContextOptions<SqliteDBContext> options) : base(options)
        {
            DbPath = GetDBPath();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

        #region Private members
        private string GetDBPath()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            return System.IO.Path.Join(path, "sessions.db");
        }
        #endregion
    }
}
