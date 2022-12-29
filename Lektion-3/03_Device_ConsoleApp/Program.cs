using _03_Device_ConsoleApp.Services;
using Newtonsoft.Json;

await DeviceManager.InitializeAsync("https://netgr10-fa.azurewebsites.net/api/RegisterDevice?code=N5dT3fqdFowwuXxU6_M99_Hn2QZdBMAUiA3CwujQQbuhAzFup51Mkg==", "consoleApp");
await DeviceManager.InitializeDirectMethodsAsync();

while(true)
{
    if (DeviceManager.AllowSending)
        await DeviceManager.SendDataAsync(JsonConvert.SerializeObject(new { temperature = 40, humiditiy = 60 }));

    await Task.Delay(DeviceManager.Interval);
}