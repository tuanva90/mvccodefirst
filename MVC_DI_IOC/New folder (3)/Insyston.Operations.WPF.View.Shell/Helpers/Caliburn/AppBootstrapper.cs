using Autofac;
using Caliburn.Micro;
using Insyston.Operations.WPF.ViewModels.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace Insyston.Operations.WPF.Views.Shell.Helpers.Caliburn
{
    public class AppBootstrapper : AutofacBootstrapper
    {
        /// <summary>
        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            // DisplayRootViewFor<NavigationViewModel>();
        }
        /// <summary>
        /// Configures the <see cref="AppBootstrapper"/>.
        /// </summary>
        protected override void ConfigureBootstrapper()
        {
            // you must call the base version first!
            base.ConfigureBootstrapper();

            // override namespace naming convention
            EnforceNamespaceConvention = false;

            // change our view model base type
            ViewModelBaseType = typeof(IScreen);

            // We want this
            // TelerikConventions.Install();
        }

        /// <summary>
        /// Configures Caliburn to register all Repository objects 
        /// </summary>
        /// <param name="builder"> The container builder. </param>
        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            /*
            // Get the assemblies thta contain Repositories
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(f => f.FullName.Contains("Repository")).ToArray();

            // Add all of the repositories in our app
            builder.RegisterAssemblyTypes(assemblies)

                // must be a type with a name that ends with Repository
              .Where(type => type.Name.EndsWith("Repository"))

                // registered as the Interfaces it implements
              .AsImplementedInterfaces()

                // always create a new one
              .InstancePerDependency();
             * */
        }

        /// <summary>
        /// handles exceptions that have not been handled
        ///  </summary>
        /// <param name="sender"> The event sender. </param>
        /// <param name="e"> The event arguments. </param>
        protected override void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // base.OnUnhandledException(sender, e);
        }
    }
}
