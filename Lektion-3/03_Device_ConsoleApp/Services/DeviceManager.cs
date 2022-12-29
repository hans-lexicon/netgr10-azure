using _03_Device_ConsoleApp.Models;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace _03_Device_ConsoleApp.Services
{
    internal static class DeviceManager
    {
        private static DeviceClient _deviceClient;
        public static bool AllowSending { get; private set; } = false;
        public static int Interval { get; private set; } = 10000;

        public static async Task InitializeAsync(string url, string deviceId)
        {
            using var client = new HttpClient();
            var deviceRequest = new DeviceRequest { DeviceId = deviceId };
            
            var response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(deviceRequest)));
            var data = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            _deviceClient = DeviceClient.CreateFromConnectionString(data?.Value.ConnectionString.ToString());
            Console.WriteLine("Device Initialized...");
        }

        public static async Task InitializeDirectMethodsAsync()
        {
            await _deviceClient.SetMethodHandlerAsync("start", Start, null);
            await _deviceClient.SetMethodHandlerAsync("stop", Stop, null);
            Console.WriteLine("Direct Methods Initialized...");
        }

        public static async Task SendDataAsync(string payload)
        {
            var message = new Message(Encoding.UTF8.GetBytes(payload));
            await _deviceClient.SendEventAsync(message);
            Console.WriteLine($"Message sent to Azure Iot Hub ({DateTime.Now})");
        }


        private static Task<MethodResponse> Start(MethodRequest req, object userContext)
        {
            AllowSending = true;

            if (req.DataAsJson != null)
            {
                var payload = JsonConvert.DeserializeObject<dynamic>(req.DataAsJson);
                if (payload != null)
                    if (payload?.interval != null)
                        Interval = payload?.interval;
            }
            Console.WriteLine($"Start triggered ({DateTime.Now})");
            return Task.FromResult(new MethodResponse(null, 200));
        }

        private static Task<MethodResponse> Stop(MethodRequest req, object userContext)
        {
            AllowSending = false;

            Console.WriteLine($"Stop triggered ({DateTime.Now})");
            return Task.FromResult(new MethodResponse(null, 200));
        }
    }
}
