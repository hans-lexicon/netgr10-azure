using System.Linq.Expressions;
using System.Net;
using _03_AzureFunctions.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace _03_AzureFunctions.Functions
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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            try
            {
                _logger.LogInformation("Registrering new device");

                var body = new StreamReader(req.Body).ReadToEnd();

                var deviceRequest = JsonConvert.DeserializeObject<DeviceRequest>(body);
                if (!string.IsNullOrEmpty(deviceRequest?.DeviceId))
                {
                    var device = await _registryManager.GetDeviceAsync(deviceRequest?.DeviceId);
                    if (device == null)
                    {
                        device = await _registryManager.AddDeviceAsync(new Device(deviceRequest?.DeviceId));
                    }

                    var response = new DeviceResponse
                    {
                        ConnectionString = $"{Environment.GetEnvironmentVariable("IotHub")?.Split(";")[0]};DeviceId={device!.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}",
                        Device = device
                    };

                    return new OkObjectResult(response);
                }

                return new BadRequestObjectResult(new { message = "No deviceId was supplied." });
            }
            catch (Exception ex)
            {
                _logger.LogInformation("ERROR! " + ex.Message);
            }

            return new BadRequestResult();
        }
    }
}
