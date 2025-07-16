using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.E2ETests.GraphQL.Common
{
    public class CustomResponse<T>
    {
        public T Data { get; set; }
    }

    public class RootResponseByCartId
    {
        public CartPayload CartById { get; set; }
    }


    public class RootResponseByCustomerId
    {
        public CartPayload CartByCustomerId { get; set; }
    }
}
