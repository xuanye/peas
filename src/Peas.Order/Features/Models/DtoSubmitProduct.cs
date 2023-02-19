using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peas.Order.Features.Models
{
    public class DtoSubmitProduct
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

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
