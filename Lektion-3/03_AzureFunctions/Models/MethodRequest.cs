using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _03_AzureFunctions.Models
{
    internal class MethodRequest
    {
        public string DeviceId { get; set; } = null!;
        public string MethodName { get; set; } = null!;
        public int Interval { get; set; }
    }
}
