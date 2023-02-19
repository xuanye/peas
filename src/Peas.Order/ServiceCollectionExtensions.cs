

using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Peas.Order;
using Peas.Order.Behaviors;
using Peas.Order.Domain.Contract;
using Peas.Order.Domain.DomainService;
using Peas.Order.Features;
using Peas.Order.Infrastructure;
using Peas.Order.Infrastructure.Repository;
using Peas.Order.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddOrderService(this IServiceCollection @this, IConfiguration configuration ) 
        {
            @this.Configure<ConnectionStrings>(o => o.OrderDatabase = configuration.GetConnectionString("order"));
            @this.AddAutoMapperProfiles();
            @this.BindService<OrderService>();

            //MediatR
            @this.AddMediatRService();

            //Domain Service
            @this.AddDomainService();

            //Repository
            @this.AddRepositories();

            return @this;
        }

        private static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
        {
            return services.AddAutoMapper(typeof(OrderMapperProfile).Assembly);
        }

        private static IServiceCollection AddMediatRService(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(BaseResponse).Assembly);
            });
        


            services.AddValidatorsFromAssembly(typeof(BaseResponse).Assembly, ServiceLifetime.Scoped, (result) =>
            {
                return result.ValidatorType.Name.EndsWith("Command") || result.ValidatorType.Name.EndsWith("Query");
            });

       

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }


        private static IServiceCollection AddDomainService(this IServiceCollection services)
        {
            services.AddSingleton<OrderDomainService>();
        
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IOrderRepository, OrderRepository>();   

            return services;
        }

    }
}
