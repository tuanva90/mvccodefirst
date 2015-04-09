using System;
using System.Linq;
using System.ComponentModel.Composition.Hosting;
using Microsoft.Practices.Prism.MefExtensions;
using System.Windows;
using System.Reflection;

namespace Insyston.Operations.WPF.View.Receipts
{
    public class BootStrapper : MefBootstrapper
    {       
        public BootStrapper()
        {
            Shared.LoadReceiptNavigation();
        }        

        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();
            base.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly()));
            base.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(Assembly.Load("Insyston.Operations.WPF.ViewModel.Receipts")));
        }      

        protected override DependencyObject CreateShell()
        {
            return this.Container.GetExportedValue<ReceiptsShell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose | ShutdownMode.OnMainWindowClose;
        }

        public ReceiptsShell Shell
        {
            get
            {
                return (ReceiptsShell)base.Shell;
            }
        }
    }
}
