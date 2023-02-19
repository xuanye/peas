using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peas.Order.Domain.DomainService
{
    public class PoOrder
    {
        public string UserId { get; set; }

        public List<PoSubmitProduct> Products { get; set; }

        /// <summary>
        /// 订单总价
        /// </summary>
        public long TotalPrice { get; set; }
    }
}
