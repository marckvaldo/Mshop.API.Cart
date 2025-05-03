using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Infra.Consumer
{
    public class SettingsGRPC
    {
        public string GrpcProduct { get; set; } = string.Empty;
        public string GrpcCustomer { get; set; } = string.Empty;
        public int TimeoutInSeconds { get; set; } = 30;
    }
}
