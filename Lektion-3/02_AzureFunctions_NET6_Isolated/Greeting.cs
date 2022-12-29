using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace _02_AzureFunctions_NET6_Isolated
{
    public class Greeting
    {
        private readonly ILogger _logger;

        public Greeting(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Greeting>();
        }

        [Function("Greeting")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }
}
