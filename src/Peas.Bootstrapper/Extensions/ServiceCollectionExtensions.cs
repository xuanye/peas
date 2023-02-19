using Dapper;
using DotBPE.Extra;
using Peas.Infrastructure;
using DotBPE.Rpc.Server;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Peas.Order.Infrastructure;
using Vulcan.DapperExtensions.Contract;
using Vulcan.DapperExtensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDotBPEWithHttpApi(this IServiceCollection services)
        {

            return services.AddDotBPE()
                .AddDynamicProxy()
                .AddMessagePackSerializer()
                .AddTextJsonParser()
            .AddHttpApi();
        }
        public static IServiceCollection AddDapperExtension(this IServiceCollection services)
        {

            DefaultTypeMap.MatchNamesWithUnderscores = true;

           
           
            //Vulcan数据层需要的对象，如果不使用Vulcan可以不用注册
            services.AddSingleton<IRuntimeContextStorage, DotBPECallContextStorage>();            //使用MySQL
            services.AddSingleton<IConnectionFactory, MySqlConnectionFactory>();            //链接管理器
            services.AddSingleton<IConnectionManagerFactory, ConnectionManagerFactory>();            //repository

            return services;
        }
    }
}
