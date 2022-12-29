using System.Net;
using _03_AzureFunctions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace _03_AzureFunctions.Functions
{
    public class DirectMethod
    {
        private readonly ILogger _logger;
        private ServiceClient _serviceClient;

        public DirectMethod(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DirectMethod>();
            _serviceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));
        }

        [Function("DirectMethod")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var methodRequest = JsonConvert.DeserializeObject<MethodRequest>(new StreamReader(req.Body).ReadToEnd());

            var cloudMethod = new CloudToDeviceMethod(methodRequest!.MethodName);
            if (methodRequest.Interval > 0)
                cloudMethod.SetPayloadJson(JsonConvert.SerializeObject(new { interval = methodRequest.Interval }));

            var result = await _serviceClient.InvokeDeviceMethodAsync(methodRequest.DeviceId, cloudMethod);
            return new OkObjectResult(result);

        }
    }
}
