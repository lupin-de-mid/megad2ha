using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;

namespace ConsoleApp1
{
    public class MegaD2561 : MegaD
    {
        public MegaD2561(string address, string password) : base(address, password)
        {
        }

    
    }

    class Program
    {
        static readonly HttpClient Client = new HttpClient();


        static async Task Main()
        {
            try
            {
                var sensors = new List<Port>()
                {
                    new InPort(0),
                    new InPort(1),
                    new InPort(2),

                    new InPort(3),
                    new InPort(4),
                    new InPort(5),

                    new InPort(6),

                    new OutPort(7),
                    new OutPort(8),
                    new OutPort(9),

                    new OutPwm(10),
                    new OutPort(11),

                    new OutPwm(12),
                    new OutPwm(13),
                    // new OutPort(14),

                    new InPort(15),
                    new InPort(16),
                    new InPort(17),
                    new InPort(18),
                    new InPort(19),
                    new InPort(20),
                    new InPort(21),

                    new OutPort(22),
                    new OutPort(23),
                    new OutPort(24),

                    new OutPwm(25),
                    new OutPort(26),

                    new OutPwm(27),
                    new OutPwm(28),

                    new Ds2413OutPort(30)
                    // new OutPort(29),
                };

                var megad = new MegaD2561("192.168.2.11", "sec");
                foreach (var sensor in sensors)
                    if (!megad.Add(sensor))
                        Console.Out.WriteLine("Unable add " + sensor);

                if (await megad.Update())
                {
                    Console.Out.WriteLine("Unable update");
                }
                
                Console.Out.WriteLine(megad.DumpState());
                
                
                
                var options = new ManagedMqttClientOptionsBuilder()
                    .WithAutoReconnectDelay(TimeSpan.FromSeconds(1))
                    .WithClientOptions(new MqttClientOptionsBuilder()
                        .WithClientId("megabroker")
                        // .WithCredentials("test", "qwsxcderfdsaq")
                        .WithTcpServer("192.168.2.103", 1883)
                        .Build())
                    .Build();

                var mqttClient = new MqttFactory().CreateManagedMqttClient();
                mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(args =>
                {
                    Console.Out.WriteLine(args.AuthenticateResult);
                });
                mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(args =>
                {
                    Console.Out.WriteLine(args.Exception);
                    Environment.Exit(-1);
                });
                mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(args =>
                {
                    Console.Out.WriteLine(args.Exception);
                    Console.Out.WriteLine(args.AuthenticateResult);
                    Console.Out.WriteLine(args.ClientWasConnected);
                });

                await mqttClient.StartAsync(options);

                mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(
                    async args =>
                    {
                        var m = args.ApplicationMessage;
                        Console.Out.WriteLine(m.Topic);
                        var payload = m.ConvertPayloadToString();
                        Console.Out.WriteLine(payload);
                        if (m.Topic.Equals("homeassistant/fan/megad_192_168_2_11/p30a/set"))
                        {
                            bool state;
                            if (payload.IsValidOnOff(out state))
                                await Client.GetAsync($"http://192.168.2.11/sec/?cmd=30A:{(state ? 1 : 0)}");
                            await mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                                .WithTopic("homeassistant/fan/megad_192_168_2_11/p30a/state")
                                .WithPayload(
                                    payload)
                                .WithRetainFlag(false)
                                .Build());
                        }
                    });
                await mqttClient.SubscribeAsync(new TopicFilterBuilder()
                    .WithTopic("homeassistant/fan/megad_192_168_2_11/p30a/set").Build());
                await mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                    .WithTopic("homeassistant/fan/megad_192_168_2_11/p30a/config")
                    .WithPayload(
                        "{\"name\": \"bathroom\", \"state_topic\": \"homeassistant/fan/megad_192_168_2_11/p30a/state\", \"command_topic\":\"homeassistant/fan/megad_192_168_2_11/p30a/set\"}")
                    .WithRetainFlag(false)
                    .Build());

                await mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                    .WithTopic("homeassistant/fan/megad_192_168_2_11/p30a/state")
                    .WithPayload(
                        "OFF")
                    .WithRetainFlag(false)
                    .Build());

                Console.WriteLine(mqttClient.IsConnected);
                // Call asynchronous network methods in a try/catch block to handle exceptions.

                // foreach (var sensor in sensors)
                // {
                //     if (!await sensor.UpdateValue()) Console.Out.WriteLine("Error with " + sensor.Number);
                //     Console.Out.WriteLineAsync(sensor.ToString());
                // }
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        public static async Task GetPortState(int port)
        {
            var url = $"http://192.168.2.11/sec/?pt={port}&cmd=get";
            var responseBody = await GetUrl(url);
            // Above three lines can be replaced with new helper method below
            // string responseBody = await client.GetStringAsync(uri);

            Console.WriteLine($"Port #{port} = {responseBody}");
        }

        public static async Task<string> GetUrl(string url)
        {
            var response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}