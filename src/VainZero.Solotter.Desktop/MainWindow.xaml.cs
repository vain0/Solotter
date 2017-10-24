using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
using VainZero.Solotter;

namespace VainZero.Solotter.Desktop
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var notifier = new MessageBoxNotifier("Solotter");
            try
            {
                var executablePath = Assembly.GetExecutingAssembly().Location;

                var userConfigRepo = UserConfigModule.fileSystemConfigRepo(executablePath);
                var userConfig = userConfigRepo.Find();
                userConfigRepo.Save(userConfig);

                var themeManager = new ThemeManager();
                themeManager.Load(Resources, userConfig.ThemeColorName);

                var accessTokenRepo = AccessTokenModule.fileSystemAccessTokenRepo(executablePath);
                var authFrame = AuthFrame.Create(accessTokenRepo, userConfig, notifier);

                Content = authFrame;
            }
            catch (Exception ex)
            {
                notifier.NotifyInfo("Failed to start. " + ex.ToString());
                Application.Current.Shutdown(Marshal.GetHRForException(ex));
            }
        }
    }
}
