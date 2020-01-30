using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using rpi_ws281x;
using System.Collections.Generic;
using System.Drawing;

namespace duetstatuscore {
    class LEDWorker : BackgroundService {

        private DuetData _data;

        public LEDWorker(DuetData data)
        {
            _data = data;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            // Set the number of LEDs
            var ledCount = 16;

            var settings = Settings.CreateDefaultSettings();
            var controller = settings.AddController(ledCount, Pin.Gpio21, StripType.WS2811_STRIP_GRB);
            var colors = await GetAnimationColors();

            using (var device = new WS281x(settings))
            {
                device.Reset();
                while(!stoppingToken.IsCancellationRequested) {
                    while(_data.connecting) {
                        Console.WriteLine(_data.connecting);
                        foreach (var c in colors) {
                            for (int i = 0; i < controller.LEDCount; i++)
                            {
                                controller.SetLED(i, c);
                                device.Render();
                                // wait for a minimum of 5 milliseconds
                                var waitPeriod = (int)Math.Max(500.0 / controller.LEDCount, 5.0); 
                                Thread.Sleep(waitPeriod);
                            }
                        }
                    }
                    var color = Color.Black;
                    switch(_data.heater1State) 
                    {
                        case DuetAPI.Machine.HeaterState.Off:
                            color = Color.Black;
                            break;
                        case DuetAPI.Machine.HeaterState.Active:
                            color = Color.Green;
                            break;
                        case DuetAPI.Machine.HeaterState.Offline:
                            color = Color.Red;
                            break;
                        case DuetAPI.Machine.HeaterState.Standby:
                            color = Color.Orange;
                            break;
                        case DuetAPI.Machine.HeaterState.Tuning:
                            color = Color.Blue;
                            break;
                        default:
                            color = Color.Black;
                            break;
                    }
                    for (int i = 0; i < controller.LEDCount; i++)
                    {
                        controller.SetLED(i, color);
                    }
                    device.Render();
                }
                device.Reset();
            }
        }

        private static Task<List<Color>> GetAnimationColors()
        {
            var result = new List<Color>();

            result.Add(Color.Red);
            result.Add(Color.DarkOrange);
            result.Add(Color.Yellow);
            result.Add(Color.Green);
            result.Add(Color.Blue);
            result.Add(Color.Purple);
            result.Add(Color.DeepPink);

            return Task.FromResult(result);
        }
    }
}