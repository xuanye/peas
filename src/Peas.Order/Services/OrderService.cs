using AutoMapper;
using DotBPE.Rpc;
using DotBPE.Rpc.Server;
using MediatR;
using Peas.Order.Features.Commands.SubmitOrder;
using Peas.Protocol.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Peas.Order.Services
{
    public class OrderService : BaseService<IOrderService>, IOrderService
    {
        private readonly IMapper _mapper;
        private readonly ISender _sender;

        public OrderService(IMapper mapper, ISender sender)
        {
            _mapper = mapper;
            _sender = sender;
        }
        public async Task<RpcResult<SubmitOrderRsp>> SubmitAsync(SubmitOrderReq req)
        {
            var result = new RpcResult<SubmitOrderRsp>() { Data = new SubmitOrderRsp() };
            var submitCommand = _mapper.Map<SubmitOrderCommand>(req.Order);
            submitCommand.UserId = req.Identity;

            var rsp = await _sender.Send(submitCommand);
            if (rsp.Code == 0)
            {
                result.Data.OrderId = rsp.Data;
            }
            else
            {
                result.Data.ReturnMessage = rsp.Message;
                result.Code = rsp.Code;
            }
            return result;
        }
    }
}
