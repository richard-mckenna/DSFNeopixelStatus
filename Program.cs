using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DuetAPI.Connection;
using DuetAPIClient;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using rpi_ws281x;


namespace duetstatuscore
{
    class Program
    {
        static DuetData data = new DuetData();

        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) => 
            Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<SubscribeConnection>();
                services.AddSingleton<DuetData>(data);
                services.AddHostedService<StatusWorker>();
                services.AddHostedService<ConsoleWriterWorker>();
                services.AddHostedService<LEDWorker>();
            });
    }
}