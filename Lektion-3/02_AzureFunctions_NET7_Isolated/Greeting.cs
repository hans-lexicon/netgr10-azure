using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace _02_AzureFunctions_NET7_Isolated
{
    public class Greeting
    {
        private readonly ILogger _logger;

        public Greeting(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Greeting>();
        }

        [Function("Greeting")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "greeting/{name}")] HttpRequestData req, string? name = null)
        {

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString($"Welcome {name}");

            return response;
        }
    }
}
