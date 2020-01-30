using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace duetstatuscore
{
    public class RainbowColorAnimation : IAnimation
    {
        private static int colorOffset = 0;

        public void Execute(AbortRequest request, int leds)
        {
            var ledCount = leds;
            var settings = Settings.CreateDefaultSettings();
            var controller = settings.AddController(ledCount, Pin.Gpio21, StripType.WS2811_STRIP_GRB);

            using (var device = new WS281x(settings))
            {
                var colors = GetAnimationColors();
                while (!request.IsAbortRequested)
                {
                    for (int i = 0; i < controller.LEDCount; i++)
                    {
                        var colorIndex = (i + colorOffset) % colors.Count;
                        controller.SetLED(i, colors[colorIndex]);
                    }
                    device.Render();
                    colorOffset = (colorOffset + 1) % colors.Count;

                    Thread.Sleep(50);
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