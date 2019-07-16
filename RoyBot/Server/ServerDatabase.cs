using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoyBot.Server
{
    public class ServerDatabase : DbContext
    {

        public ServerDatabase(DbContextOptions<ServerDatabase> options)
            : base(options)
        {

        }

        public DbSet<ServerModel> Servers
        {
            get;
            set;
        }
    }
}
