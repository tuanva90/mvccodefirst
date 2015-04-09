using System;
using System.Linq;
using Microsoft.Practices.Prism.Modularity;
using System.ComponentModel.Composition.Hosting;
using System.Windows;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Modularity;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions;

namespace Insyston.Operations.View.OpenItems.Receipts
{
    public class BootStrapper : MefBootstrapper
    {
        public int UserID
        {
            get
            {
                return Common.UserID;
            }
            set
            {
                Common.UserID = value;
            }
        }

        public BootStrapper()
        {
        }

        public BootStrapper(int userID)
        {
            UserID = userID;
            Common.LoadReceiptNavigation();
        }

        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();
            base.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly()));
        }      

        protected override System.Windows.DependencyObject CreateShell()
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
