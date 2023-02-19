using DotBPE.Rpc.Client;
using Peas.Order.Domain.Contract;
using Peas.Protocol.Product;

namespace Peas.Order.Domain.DomainService
{
    public class OrderDomainService
    {
        private readonly IClientProxy _proxy;
        private readonly IOrderRepository _repository;

        public OrderDomainService(IClientProxy proxy ,IOrderRepository repository)
        {
            _proxy = proxy;
            _repository = repository;
        }

        public async Task<string> SubmitOrderAsync(PoOrder order)
        {
            var procductService = await _proxy.CreateAsync<IProductService>();

            var productIds =  order.Products.ConvertAll(p => p.ProductId);

            var productList = await procductService.GetProductsByIdsAsync(new GetProductsReq() { Ids = string.Join(",", productIds), Identity = order.UserId });

            //TODO:对比价格，如果有不同则告知价格已经变化

            //其他保存订单的逻辑

            return "orderId";

        }
    }
}
