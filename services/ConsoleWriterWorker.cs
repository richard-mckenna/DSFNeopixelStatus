using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace duetstatuscore {
    class ConsoleWriterWorker : BackgroundService {

        private DuetData _data;

        public ConsoleWriterWorker(DuetData data)
        {
            _data = data;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while(!stoppingToken.IsCancellationRequested) {
                await Task.Run(() => {
                    Console.WriteLine("Temp1: {0}, Temp2: {1}, State1: {2}, State2: {3}", 
                        _data.heater1Temp.ToString(), 
                        _data.heater2Temp.ToString(), 
                        _data.heater1State.ToString(), 
                        _data.heater2State.ToString());
                    Thread.Sleep(1000);
                });
            }
        }
    }
}