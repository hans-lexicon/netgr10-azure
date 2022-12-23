using System.Net;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Functions
{
    public class GetDevices
    {
        private readonly ILogger _logger;
        private RegistryManager _registryManager;

        public GetDevices(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetDevices>();
            _registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));
        }

        [Function("GetDevices")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            var devices = _registryManager.CreateQuery("select * from devices");
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(await devices.GetNextAsTwinAsync());

            return response;
        }
    }
}
