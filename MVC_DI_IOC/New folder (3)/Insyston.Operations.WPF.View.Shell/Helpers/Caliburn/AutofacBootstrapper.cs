using Autofac;
using Caliburn.Micro;
using Insyston.Operations.WPF.ViewModels.Collections;
using Insyston.Operations.WPF.ViewModels.Funding;
using Insyston.Operations.WPF.ViewModels.Security;
using Insyston.Operations.WPF.Views.Collections;
using Insyston.Operations.WPF.Views.Security;
using Insyston.Operations.WPF.Views.Assets;
using Insyston.Operations.WPF.Views.Shell.ViewModel;
////using Insyston.Operations.WPF.Collections.ViewModels;
////using Insyston.Operations.WPF.Collections.Views;
////using Insyston.Operations.WPF.Views.Shell.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.Views.Shell.Helpers.Caliburn
{
    using Insyston.Operations.WPF.ViewModels.Assets.AssetClasses;
    using Insyston.Operations.WPF.ViewModels.RegisteredAsset;
    using Insyston.Operations.WPF.ViewModels.Shell;
    using Insyston.Operations.WPF.Views.Assets.AssetClasses;
    using Insyston.Operations.WPF.Views.RegisteredAsset;

    public class AutofacBootstrapper : BootstrapperBase
    {
        /// <summary>
        /// Gets the Container.
        /// </summary>
        public IContainer Container { get; private set; }
        /// <summary>
        /// Should the namespace convention be enforced for type registration. The default is true.
        /// For views, this would require a views namespace to end with Views
        /// For view-models, this would require a view models namespace to end with ViewModels
        /// <remarks>Case is important as views would not match.</remarks>
        /// </summary>
        public bool EnforceNamespaceConvention { get; set; }

        /// <summary>
        /// Should the IoC automatically subscribe any types found that implement the
        /// IHandle interface at activation
        /// </summary>
        public bool AutoSubscribeEventAggegatorHandlers { get; set; }

        /// <summary>
        /// The base type required for a view model
        /// </summary>
        public Type ViewModelBaseType { get; set; }

        /// <summary>
        /// Method for creating the window manager
        /// </summary>
        public Func<IWindowManager> CreateWindowManager { get; set; }

        /// <summary>
        /// Method for creating the event aggregator
        /// </summary>
        public Func<IEventAggregator> CreateEventAggregator { get; set; }

        protected override void Configure()
        {
            // allow base classes to change bootstrapper settings
            this.ConfigureBootstrapper();


            // validate settings
            if (CreateWindowManager == null)
            {
                throw new ArgumentNullException();//"ErrorResources.CreateWindowManagerIsNull");
            }
            if (CreateEventAggregator == null)
            {
                throw new ArgumentNullException();//"ErrorResources.CreateEventAggregatorIsNull");
            }

            // configure container
            var builder = new ContainerBuilder();
            var viewModelAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(f => f.FullName.Contains("ViewModel")).ToArray();

            builder.RegisterAssemblyTypes(viewModelAssemblies)

                // must be a type with a name that ends with ViewModel
              .Where(type => type.Name.EndsWith("ViewModel"))

                // must be in a namespace ending with ViewModels
              .Where(type => !EnforceNamespaceConvention || (!string.IsNullOrWhiteSpace(type.Namespace) && type.Namespace.Contains("ViewModels")))

                // must implement INotifyPropertyChanged (deriving from PropertyChangedBase will statisfy this)
              .Where(type => type.GetInterface(ViewModelBaseType.Name, false) != null)

                // attach a name to it so that we can search by strings
              .Named<object>(type => type.Name.ToString(CultureInfo.InvariantCulture))

                // registered as self
              .AsSelf()

                // always create a new one
              .InstancePerDependency();

            // register views - it is assumed that all the view will be in the main silverlight application assembly
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())

                // must be a type with a name that ends with View
              .Where(type => type.Name.EndsWith("View"))

                // must be in a namespace that ends in Views
              .Where(type => !EnforceNamespaceConvention || (!string.IsNullOrWhiteSpace(type.Namespace) && type.Namespace.Contains("Views")))

                // attach a name to it so that we can search by strings
              .Named<object>(type => type.Name.ToString(CultureInfo.InvariantCulture))

                // registered as self
              .AsSelf()

                // always create a new one
              .InstancePerDependency();

            // register the single window manager for this container
            builder.Register(c => CreateWindowManager()).InstancePerLifetimeScope();

            // register the single event aggregator for this container
            builder.Register(c => CreateEventAggregator()).InstancePerLifetimeScope();

            // should we install the auto-subscribe event aggregation handler module?
            if (AutoSubscribeEventAggegatorHandlers)
            {
                builder.RegisterModule<EventAggregationAutoSubscriptionModule>();
            }

            // allow derived classes to add to the container
            ConfigureContainer(builder);

            Container = builder.Build();
        }


        /// <summary>
        /// Do not override unless you plan to full replace the logic. This is how the framework
        /// retrieves services from the <see cref="Autofac"/> container.
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <param name="key">The key to locate.</param>
        /// <returns>The located service.</returns>
        protected override object GetInstance(Type service, string key)
        {
            object instance;
            if (string.IsNullOrWhiteSpace(key))
            {
                if (Container.TryResolve(service, out instance))
                {
                    //IDeactivate activatable = instance as IDeactivate;
                    //activatable.Deactivated += activatable_Deactivated;
                    return instance;
                }
            }
            else
            {
                if (Container.TryResolveNamed(key, service, out instance))
                {
                    return instance;
                }
            }


            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name));
        }

        /*void activatable_Deactivated(object sender, DeactivationEventArgs e)
        {
            if (e.WasClosed)
            {
                ((IDeactivate)sender).Deactivated -= activatable_Deactivated;
                Container.Realease(sender);
            }
        }*/

        /// <summary>
        /// Do not override unless you plan to full replace the logic. This is how the framework
        /// retrieves services from the <see cref="Autofac"/> container.
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <returns>The located services.</returns>
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        /// <summary>
        /// Do not override unless you plan to full replace the logic. This is how the framework
        /// retrieves services from the <see cref="Autofac"/> container.
        /// </summary>
        /// <param name="instance">The instance to perform injection on.</param>
        protected override void BuildUp(object instance)
        {
            Container.InjectProperties(instance);
        }

        /// <summary>
        /// Override to provide configuration prior to the <see cref="Autofac"/> configuration. You must call the base version BEFORE any 
        /// other statement or the behavior is undefined.
        /// Current Defaults:
        ///  EnforceNamespaceConvention = true
        ///  ViewModelBaseType = <see cref="System.ComponentModel.INotifyPropertyChanged"/> 
        ///  CreateWindowManager = <see cref="WindowManager"/> 
        ///  CreateEventAggregator = <see cref="EventAggregator"/>
        /// </summary>
        protected virtual void ConfigureBootstrapper()
        {
            // by default, enforce the namespace convention
            EnforceNamespaceConvention = true;

            // default is to auto subscribe known event aggregators
            AutoSubscribeEventAggegatorHandlers = false;

            // the default view model base type
            ViewModelBaseType = typeof(System.ComponentModel.INotifyPropertyChanged);

            // default window manager
            CreateWindowManager = () => new WindowManager();

            // default event aggregator
            CreateEventAggregator = () => new EventAggregator();
        }

        /// <summary>
        /// Override to include your own <see cref="Autofac"/> configuration after the framework has finished its configuration, but 
        /// before the container is created.
        /// </summary>
        /// <param name="builder">The <see cref="Autofac"/> configuration builder.</param>
        protected virtual void ConfigureContainer(ContainerBuilder builder)
        {
        }

        protected override IEnumerable<System.Reflection.Assembly> SelectAssemblies()
        {
            //return base.SelectAssemblies();
            return new[]
               {
                   GetType().Assembly, 
                   // base.SelectAssemblies(),
                   //typeof(MainViewModel).Assembly,
                   typeof(CollectionsAssignmentViewModel).Assembly,
                   typeof(HomePageViewModel).Assembly,
                   typeof(FundingSummaryViewModel).Assembly,
                   typeof(UsersViewModel).Assembly,
                   typeof(CollectionsAssignmentView).Assembly,
                   typeof(UsersView).Assembly,
                   typeof(AssetClassesCategoryView).Assembly,
                   typeof(AssetClassesCategoryViewModel).Assembly,
                   typeof(RegisteredAssetView).Assembly,
                   typeof(RegisteredAssetViewModel).Assembly,
               };
        }
    }
}
