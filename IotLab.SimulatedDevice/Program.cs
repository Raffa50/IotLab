using IotLab.Common;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Shared;
using System.Threading;

namespace IotLab.SimulatedDevice
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "AldrigoIot.azure-devices.net",
            deviceId = Config.DeviceId,
            deviceKey = "I+A6Pxh9Tjdq4OkxdU/meC5Wiy+L/ddpMihk9E/OjwQ=";

        static void Main(string[] args)
        {
            var cancellationSource = new CancellationTokenSource();
            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs cancelArgs) => {
                cancellationSource.Cancel();
            };

            Console.WriteLine("Mars orbital station\n");

            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey), TransportType.Mqtt);
            deviceClient.ProductInfo = "HAL-9000";

            #region twin reportedProperties
            try
            {
                var twin = deviceClient.GetTwinAsync().Result;
                var reportedProperties = new TwinCollection
                {
                    ["SoftwareInfo"] = new SoftwareInfo
                    {
                        Name = "HAL",
                        Version = new Version(9, 0, 0, 0),
                        Manufacturer = "Reply"
                    },
                    ["HumanFriendly"] = false,
                    ["Commands"] = "Command:\n\tshutdown\nParams:\n\tForce: bool\n\tUser: string\nDescription:\n\tShuts down the system\n\tShutdown force will force an emergency shutdown if system doesn't respond"
                };
                deviceClient.UpdateReportedPropertiesAsync(reportedProperties).Wait();
            } catch(Exception ex)
            {
                Console.WriteLine("Can't update twin reported properties");
            }
            #endregion

            deviceClient.SetMethodHandlerAsync("shutdown", ShutDownMethod, null).Wait();

            Task.WaitAll( SendDeviceToCloudMessagesAsync(cancellationSource.Token), ReceiveC2dAsync(cancellationSource.Token) );

            Console.WriteLine("Quitting");
            Console.ReadLine();
        }

        private static async Task SendDeviceToCloudMessagesAsync(CancellationToken ct)
        {
            double minTemperature = 20,
                minHumidity = 60;
            int messageId = 1;
            var rand = new Random();

            while (!ct.IsCancellationRequested)
            {
                double currentTemperature = minTemperature + rand.NextDouble() * 15,
                    currentHumidity = minHumidity + rand.NextDouble() * 20;

                var telemetryDataPoint = new
                {
                    messageId = messageId++,
                    deviceId = deviceId,
                    temperature = currentTemperature,
                    humidity = currentHumidity,
                    errors = true
                };

                var message = new Message(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(telemetryDataPoint)));
                message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");

                await deviceClient.SendEventAsync(message);
                //Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(3000);
            }
        }

        private static async Task ReceiveC2dAsync(CancellationToken ct)
        {
            //Console.WriteLine("\nReceiving cloud to device messages from service");
            while (!ct.IsCancellationRequested)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                string content = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                C2DMessage message = null;
                try
                {
                    message = JsonConvert.DeserializeObject<C2DMessage>(content);
                } catch (JsonSerializationException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR: Invalid message received!");
                    Console.ResetColor();

                    await deviceClient.CompleteAsync(receivedMessage);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(message.From))
                {
                    await deviceClient.CompleteAsync(receivedMessage);
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Hello " + message.From);
                Console.ResetColor();

                await deviceClient.CompleteAsync(receivedMessage);
            }
        }

        public static Task<MethodResponse> ShutDownMethod(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine("shutdown invoked!");

            MethodParameters par;
            try
            {
                par = JsonConvert.DeserializeObject<MethodParameters>(methodRequest.DataAsJson);
            }
            catch (JsonException)
            {
                Console.WriteLine("Invalid shutdown call!");
                return Task.FromResult(new MethodResponse(-1));
            }

            if (!par.Force)
            {
                Console.WriteLine("Invalid shutdown call!");
                return Task.FromResult(new MethodResponse(1));
            }

            string resp = par.User + ", my mind is going. I can feel it.";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(resp);
            Console.WriteLine("AND THE WINNER IS: " + par.User);
            Console.ResetColor();

            return Task.FromResult(
                new MethodResponse(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(resp)), 0)
            );
        }
    }
}
