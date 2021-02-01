using System;
using System.Collections.Generic;
using System.Text;

namespace Vista.Controller.ScreenMaster3.SpyderTranslator
{
    public class SpyderConsoleTranslatorConfig
    {
        public string RabbitMQHost { get; set; } = "localhost";

        public string RabbitMQUser { get; set; } = "devtest";

        public string RabbitMQPass { get; set; } = "devtest";

        public string RAbbitMQVirtualHost { get; set; } = "/";

        public string SpyderServerIP { get; set; }

        public ButtonTranslationMap ButtonTranslationMap { get; set; }
    }
}
