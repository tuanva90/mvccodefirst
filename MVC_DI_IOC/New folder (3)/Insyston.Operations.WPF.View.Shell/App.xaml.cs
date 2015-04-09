using System;
using System.Linq;
using System.Windows;
using Insyston.Operations.Security;
using Telerik.Windows.Controls;
using Insyston.Operations.ObjectMap;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.Views.Shell
{
    using System.Globalization;
    using System.Windows.Markup;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Pick up the current culture
            FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            OperationsPrincipal operationsPrincipal = new OperationsPrincipal();
            AppDomain.CurrentDomain.SetThreadPrincipal(operationsPrincipal);
            StyleManager.ApplicationTheme = new Windows8Theme();
            Task.Run(() => Map.InitialiseMap());
            base.OnStartup(e);
        }
    }
}
