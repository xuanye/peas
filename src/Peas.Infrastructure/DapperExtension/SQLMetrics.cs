// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Vulcan.DapperExtensions.Contract;

namespace Peas.Infrastructure
{
    internal class SQLMetrics : ISQLMetrics
    {
        private readonly ILogger<SQLMetrics> _logger;
        private object _param;
        private string _sql;
        private Stopwatch _sw;

        public SQLMetrics(ILogger<SQLMetrics> logger)
        {
            _sw = Stopwatch.StartNew();
            _logger = logger;
        }

        public void AddToMetrics(string sql, object param)
        {
            _sql = sql;
            _param = param;
        }

        public void Dispose()
        {

            _sw?.Stop();
            var es = _sw?.ElapsedMilliseconds;
            _logger.LogDebug($"SQL EXECUTE Finished in {es} ms,SQL={_sql},params = {_param}");
            if (es > 500)
            {
                _logger.LogWarning($"SQL EXECUTE Finished in {es} ms,SQL={_sql},params = {_param}");
            }

            _sw = null;
            _sql = null;
            _param = null;
            //GC.SuppressFinalize(this);
        }
    }
}
