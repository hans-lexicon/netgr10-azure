using System.Net;
using System.Text.Json.Serialization;
using Azure;
using AzureFunctions.Models.RegisterDevice;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctions.Functions
{
    public class RegisterDevice
    {
        private readonly ILogger _logger;
        private RegistryManager _registryManager;

        public RegisterDevice(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RegisterDevice>();
            _registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));
        }

        [Function("RegisterDevice")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            var data = JsonConvert.DeserializeObject<RegisterDeviceRequest>(await new StreamReader(req.Body).ReadToEndAsync());

            if (!string.IsNullOrEmpty(data?.DeviceId))
            {
                var device = await _registryManager.GetDeviceAsync(data?.DeviceId);
                if(device == null)
                    device = await _registryManager.AddDeviceAsync(new Device(data!.DeviceId));

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.WriteString($"{Environment.GetEnvironmentVariable("IotHub")?.Split(";")[0]};DeviceId={device!.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}");
                return response;
            }

            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
