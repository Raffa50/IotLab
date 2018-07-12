using IotLab.Common;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Threading.Tasks;

namespace IotLab.CreateDeviceIdentity
{
    class Program
    {
        static string connectionString = "HostName=AldrigoIot.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=4jj8nDHL8NjJitoP+hXkHko1fbzJHcoEAfGky0usr7U=";

        static void Main(string[] args)
        {
            var registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            AddDeviceAsync(registryManager).Wait();
            Console.ReadLine();
        }

        private static async Task AddDeviceAsync(RegistryManager registryManager)
        {
            if (registryManager == null)
                throw new ArgumentNullException(nameof(registryManager));

            string deviceId = Config.DeviceId;
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }
            Console.WriteLine("Generated device key: " + device.Authentication.SymmetricKey.PrimaryKey);
        }
    }
}
