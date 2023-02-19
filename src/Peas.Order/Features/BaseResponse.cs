using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peas.Order.Features
{
    public class BaseResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }

    public class BaseResponse<T>:BaseResponse
    {
        public T Data { get; set; }
    }
}
