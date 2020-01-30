using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace duetstatuscore
{
    public class ColorWipe : IAnimation
    {
        public void Execute(AbortRequest request, int leds)
        {
            Console.Clear();

            var ledCount = leds;
            var settings = Settings.CreateDefaultSettings();
            var controller = settings.AddController(ledCount, Pin.Gpio21, StripType.WS2811_STRIP_GRB);

            using (var device = new WS281x(settings))
            {
                var colors = GetAnimationColors();
                while (!request.IsAbortRequested)
                {
                    foreach (var color in colors) {
                        Console.Write(color.ToString());
                        for (int i = 0; i < controller.LEDCount; i++)
                        {
                            controller.SetLED(i, color);
                            device.Render();

                            // wait for a minimum of 5 milliseconds
                            var waitPeriod = (int)Math.Max(500.0 / controller.LEDCount, 5.0); 

                            Thread.Sleep(waitPeriod);
                        }
                    }
                }
                device.Reset();
            }
        }

        public static List<Color> GetAnimationColors()
        {
            var result = new List<Color>();

            result.Add(Color.Red);
            result.Add(Color.DarkOrange);
            result.Add(Color.Yellow);
            result.Add(Color.Green);
            result.Add(Color.Blue);
            result.Add(Color.Purple);
            result.Add(Color.DeepPink);

            return result;
        }
    }
}