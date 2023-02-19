using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Peas.Infrastructure;
using Peas.Order.Domain.Contract;
using Vulcan.DapperExtensions.Contract;

namespace Peas.Order.Infrastructure.Repository
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        public OrderRepository(IConnectionManagerFactory factory, IOptions<ConnectionStrings> optionsAccessor, ILoggerFactory loggerFactory) 
            : base(factory, optionsAccessor.Value.OrderDatabase, loggerFactory)
        {
        }
    }
}
