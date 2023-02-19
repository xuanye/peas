using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peas.Order.Features.Models
{
    public class DtoOrder
    {
        public string UserId { get; set; }

        public List<DtoSubmitProduct> Products { get; set; }

        /// <summary>
        /// 订单总价
        /// </summary>
        public long TotalPrice { get; set; }
    }


}
