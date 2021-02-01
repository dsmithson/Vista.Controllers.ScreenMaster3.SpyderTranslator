using Spyder.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vista.Controller.ScreenMaster3.SpyderTranslator;

namespace ConsoleVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var config = new SpyderConsoleTranslatorConfig()
            {
                RabbitMQHost = "SM3-3216",
                SpyderServerIP = "192.168.86.133",
                ButtonTranslationMap = await ButtonTranslationMap.LoadScreenMaster3216Map()
            };

            //Start the spyder translator
            SpyderConsoleTranslator translator = new SpyderConsoleTranslator();

            //Set our data context
            this.DataContext = viewModel = new MainWindowViewModel(translator.GetSpyderSimClient());

            //Start the spyder translator.  This order of operations allows us to watch the initialization on our debug UI
            if (!await translator.StartupAsync(config))
            {
                Console.WriteLine("Failed to start up.  Exiting...");
                return;
            }
        }

        private void LargeConsoleControl_ButtonPressed(object sender, Spyder.Console.Controls.SegmentPressEventArgs e)
        {
            viewModel.ButtonAction(e.Port, e.ButtonID, e.Pressed);
        }
    }
}
