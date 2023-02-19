using AutoMapper;
using MediatR;
using Peas.Order.Domain.DomainService;

namespace Peas.Order.Features.Commands.SubmitOrder
{
    public class SubmitOrderHandler : IRequestHandler<SubmitOrderCommand, BaseResponse<string>>
    {
        private readonly OrderDomainService _service;
        private readonly IMapper _mapper;

        public SubmitOrderHandler(OrderDomainService service,IMapper mapper) 
        {
            _service = service;
            _mapper = mapper;
        }
        public async Task<BaseResponse<string>> Handle(SubmitOrderCommand request, CancellationToken cancellationToken)
        {
            var order = _mapper.Map<PoOrder>(request);
            string orderId = await _service.SubmitOrderAsync(order);

            return new BaseResponse<string> { Data = orderId };
        }
    }
}
