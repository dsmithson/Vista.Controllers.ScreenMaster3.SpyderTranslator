using System;
using System.Threading;
using System.Threading.Tasks;
using Vista.Controller.ScreenMaster3.SpyderTranslator;

namespace Vista.Controllers.ScreenMaster3.SpyderTranslator.App
{
    class Program
    {
        static async Task Main()
        {
            var config = new SpyderConsoleTranslatorConfig()
            {
                RabbitMQHost = "SM3-3216",
                SpyderServerIP = "192.168.86.133",
                ButtonTranslationMap = await ButtonTranslationMap.LoadScreenMaster3216Map()
            };

            SpyderConsoleTranslator translator = new SpyderConsoleTranslator();
            if(!await translator.StartupAsync(config))
            {
                Console.WriteLine("Failed to start up.  Exiting...");
                return;
            }

            Console.WriteLine("Running Spyder translator");
            await Task.Delay(Timeout.Infinite);
        }
    }
}
