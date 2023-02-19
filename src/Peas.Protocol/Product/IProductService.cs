using DotBPE.Gateway;
using DotBPE.Rpc;
using Peas.Protocol.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peas.Protocol.Product
{
    [RpcService(2, "product")]
    public interface IProductService
    {

        /// <summary>
        /// 提交订单 
        /// </summary>    
        [RpcMethod(1)]
        [HttpRoute("/api/products/byIds", HttpVerb.Get)]
        Task<RpcResult<GetProductsRsp>> GetProductsByIdsAsync(GetProductsReq req);
    }

    public class GetProductsReq
    {
        public string Identity { get; set; }

        /// <summary>
        /// 逗号分割ids 1,2,
        /// </summary>
        public string Ids { get; set; }
    }

    public class GetProductsRsp
    {
        public string ReturnMessage { get; set; }
        public List<VoProduct> List { get; set;}
    }

    public class VoProduct
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
       

        /// <summary>
        /// 商品单价 单位为分
        /// </summary>
        public long UnitPrice { get; set; }
    }
}
