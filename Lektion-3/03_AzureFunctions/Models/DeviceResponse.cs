using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _03_AzureFunctions.Models
{
    public class DeviceResponse
    {
        public string ConnectionString { get; set; } = null!;
        public Device Device { get; set; } = null!;
    }
}
