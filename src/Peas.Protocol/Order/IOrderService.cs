using DotBPE.Gateway;
using DotBPE.Rpc;

namespace Peas.Protocol.Order
{
    [RpcService(1, "order")]
    public interface IOrderService
    {
        /// <summary>
        /// 提交订单 
        /// </summary>    
        [RpcMethod(1)]
        [HttpRoute("/api/order", HttpVerb.Post)]
        Task<RpcResult<SubmitOrderRsp>> SubmitAsync(SubmitOrderReq req);
    }

    /// <summary>
    /// 提交订单请求
    /// </summary>
    public class SubmitOrderReq
    {
        /// <summary>
        /// 当前用户 框架会自动填充
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// 订单信息
        /// </summary>
        public VoOrder Order { get; set; }
    }

    public class SubmitOrderRsp
    {
        /// <summary>
        /// 信息，当发生错误时传递，框架自动提取到外层
        /// </summary>
        public string ReturnMessage { get; set; }

        /// <summary>
        /// 成功后的订单ID
        /// </summary>
        public string OrderId { get; set; }
    }

    /// <summary>
    /// 订单信息
    /// </summary>
    public class VoOrder
    { 
        /// <summary>
        /// 购买的商品
        /// </summary>
        public List<VoSubmitProduct> Products { get; set; }

        /// <summary>
        /// 订单总价
        /// </summary>
        public long TotalPrice { get; set; }

    }

    /// <summary>
    /// 提交的商品信息
    /// </summary>
    public class VoSubmitProduct
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
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 商品总价 单位为分
        /// </summary>
        public long Price { get; set; }

        /// <summary>
        /// 商品单价 单位为分
        /// </summary>
        public long UnitPrice { get; set; }
    }
}
