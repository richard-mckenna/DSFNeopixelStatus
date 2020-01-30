using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using DuetAPI.Connection;
using DuetAPI.Machine;
using DuetAPIClient;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;

namespace duetstatuscore {
    class StatusWorker : BackgroundService {

        private SubscribeConnection _connection;
        private DuetData _data;
        public Single heater1Temp = 0.0f;
        public Single heater2Temp = 0.0f;

        public StatusWorker(SubscribeConnection connection, DuetData data)
        {
            _connection = connection;
            _data = data;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while(!stoppingToken.IsCancellationRequested) {
                string socketPath = Defaults.FullSocketPath;
                string filter = "heat/**";

                // Connect to DCS
                await _connection.Connect(SubscriptionMode.Patch, filter, socketPath);
                Console.WriteLine("Connected!");
                _data.connecting = false;

                // In Patch mode the whole object model is sent over after connecting.
                // Dump it (or call connection.GetMachineModel() to deserialize it)
                var machineModel = await _connection.GetMachineModel();
                Console.WriteLine(machineModel.Heat.Heaters.Count);
                
                // Then keep listening for (filtered) patches
                do
                {
                    try
                    {
                        using JsonDocument patch = await _connection.GetMachineModelPatch();
                        //Console.WriteLine(GetIndentedJson(patch));

                        var root = patch.RootElement;
                        if (root.TryGetProperty("heat", out var heat)) {
                            if (heat.TryGetProperty("heaters", out var heaters)) {
                                if (heaters[0].TryGetProperty("current", out var temp1)) {
                                    if (temp1.TryGetSingle(out Single value)) {
                                        _data.heater1Temp = value;
                                    }
                                }
                                if (heaters[1].TryGetProperty("current", out var temp2)) {
                                    if (temp2.TryGetSingle(out Single value)) {
                                        _data.heater2Temp = value;
                                    }
                                }
                                if (heaters[0].TryGetProperty("state", out var state1)) {
                                    if (state1.TryGetInt32(out int value)) {
                                        _data.heater1State = (HeaterState) value;
                                    }
                                }
                                if (heaters[1].TryGetProperty("state", out var state2)) {
                                    if (state2.TryGetInt32(out int value)) {
                                        _data.heater2State = (HeaterState) value;
                                    }
                                }
                            }
                        }

                    }
                    catch (SocketException)
                    {
                        Console.WriteLine("Server has closed the connection");
                        break;
                    }
                }
                while (true);
            }
        }

        private static string GetIndentedJson(JsonDocument jsonDocument)
        {
            using var stream = new MemoryStream();
            using (Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
            {
                jsonDocument.WriteTo(writer);
            }
            stream.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}