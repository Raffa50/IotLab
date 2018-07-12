using IotLab.Common;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace IotLab.SendCloudToDevice
{
    class Program
    {
        static ServiceClient serviceClient;
        const int ListenPort= 12000;

        static void Main(string[] args)
        {
            Console.WriteLine("Send Cloud-to-Device message\n");
            serviceClient = ServiceClient.CreateFromConnectionString(Config.IotHub.ConnectionStringService);

            Console.WriteLine("Press any key to send a C2D message.");
            Console.ReadLine();
            var msg = new C2DMessage { From = "Aldrigo" };
            SendCloudToDeviceMessageAsync(msg).Wait();
            Console.WriteLine("Message sent!");

            var info = GetInfoAsync().Result;
            Console.WriteLine(info);

            ShutDown().Wait();

            Console.ReadLine();
        }

        private async static Task SendCloudToDeviceMessageAsync(C2DMessage msg)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(msg)));
            await serviceClient.SendAsync(Config.DeviceId, commandMessage);
        }

        private static async Task ShutDown()
        {
            var methodInvocation = new CloudToDeviceMethod("shutdown") { ResponseTimeout = TimeSpan.FromSeconds(30) };
            var parameters = new
            {
                Force = true,
                User = "Raffaele"
            };
            methodInvocation.SetPayloadJson(JsonConvert.SerializeObject(parameters));

            var response = await serviceClient.InvokeDeviceMethodAsync(Config.DeviceId, methodInvocation);
        }

        private async static Task<string> GetInfoAsync()
        {
            var rm = RegistryManager.CreateFromConnectionString(Config.IotHub.ConnectionStringService);
            var twin = await rm.GetTwinAsync(Config.DeviceId);

            return twin.Properties.Reported.ToJson();
        }
    }
}
