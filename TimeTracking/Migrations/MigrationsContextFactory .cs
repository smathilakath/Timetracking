using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using TimeTracking.Models;

namespace TimeTracking.Migrations
{
    public class MigrationsContextFactory : IDbContextFactory<OAuthdataContext>
    {
      

        OAuthdataContext IDbContextFactory<OAuthdataContext>.Create()
        {
            return new OAuthdataContext("DBContext");
        }
    }
}