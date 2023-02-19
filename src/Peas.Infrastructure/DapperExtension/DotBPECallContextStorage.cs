// Copyright (c) Xuanye Wong. All rights reserved.
// Licensed under MIT license

using DotBPE.Rpc.Server;
using Microsoft.Extensions.Logging;
using Vulcan.DapperExtensions.Contract;

namespace Peas.Infrastructure
{
    public class DotBPECallContextStorage : IRuntimeContextStorage
    {
        private readonly IContextAccessor _contextAccessor;
        private readonly ILogger<DotBPECallContextStorage> _logger;

        public DotBPECallContextStorage(IContextAccessor contextAccessor, ILogger<DotBPECallContextStorage> logger)
        {
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public bool ContainsKey(string key)
        {
            _logger.LogDebug("CallContext.ContainsKey key:{0}", key);
            var has = _contextAccessor.CallContext.ContainsKey(key);
            _logger.LogDebug("CallContext.ContainsKey key:{0},res ={1}", key, has);
            return has;
        }

        public object Get(string key)
        {
            _logger.LogDebug("CallContext.Get key:{0}", key);
            return this._contextAccessor.CallContext.Get(key);
        }

        public void Remove(string key)
        {
            _logger.LogDebug("CallContext.Remove key:{0}", key);
            _contextAccessor.CallContext.Remove(key);
        }

        public void Set(string key, object item)
        {
            _logger.LogDebug("CallContext.Set key:{0}", key);
            _contextAccessor.CallContext.AddOrUpdate(key, item);
        }
    }
}