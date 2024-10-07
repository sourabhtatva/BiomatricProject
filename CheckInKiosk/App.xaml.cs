using System.Runtime.InteropServices;
using System.Windows;

namespace CheckInKiosk
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AllocConsole();
            Console.WriteLine("Console is now available for logging.");
        }
    }

}
