using CORE.Connection;
using CORE.Connection.Interfaces;
using CORE.Connection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CORE.Users.Tools;

namespace CORE.Users.Configuration
{
    public class BridgeDBConnection<T>
    {
        public static IConnectionDB<T> Create(string ConnectionString, DbEnum DB)
        {
            return Factorizer<T>.Create("Server=tcp:mtwdmsql.database.windows.net,1433;Initial Catalog=CORE3_CRUD;Persist Security Info=False;User ID=mtwdm;Password=admin123#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;", DB);
        }
    }
}
