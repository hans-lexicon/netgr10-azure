using Device.ConsoleApp;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System.Text;

DeviceClient deviceClient;
var data = new RegisterDeviceRequest
{
    DeviceId = "console_1",
    DeviceType = "console app"
};

Console.WriteLine("Initializing...");
await Task.Delay(5000);

using var client = new HttpClient();
var response = await client.PostAsync("http://localhost:7247/api/RegisterDevice", new StringContent(JsonConvert.SerializeObject(data)));
var connectionString = await response.Content.ReadAsStringAsync();
if (!string.IsNullOrEmpty(connectionString))
{
    
    deviceClient = DeviceClient.CreateFromConnectionString(connectionString);

    Console.WriteLine("Updating Device Twin Properties...");
    var twin = new TwinCollection();
    twin["deviceType"] = data.DeviceType;
    await deviceClient.UpdateReportedPropertiesAsync(twin);

    Console.WriteLine("Connected and Running.");

    while(true)
    {
        var message = JsonConvert.SerializeObject(new { time = DateTime.Now.ToString()});
        await deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes(message)));

        Console.WriteLine($"Message sent: {message}");
        await Task.Delay(5000);
    }
}




Console.ReadKey();
