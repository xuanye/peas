// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using MySql.Data.MySqlClient;
using System.Data;
using Vulcan.DapperExtensions.Contract;

namespace Peas.Infrastructure
{
    public class MySqlConnectionFactory : IConnectionFactory
    {
        public IDbConnection CreateDbConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}
