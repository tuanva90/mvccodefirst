using System.Web;

using Microsoft.Practices.Unity;
using Unity.WebForms;

[assembly: WebActivator.PostApplicationStartMethod( typeof(UDI.WebASP.App_Start.UnityWebFormsStart), "PostStart" )]
namespace UDI.WebASP.App_Start
{
	/// <summary>
	///		Startup class for the Unity.WebForms NuGet package.
	/// </summary>
	internal static class UnityWebFormsStart
	{
		/// <summary>
		///     Initializes the unity container when the application starts up.
		/// </summary>
		/// <remarks>
		///		Do not edit this method. Perform any modifications in the
		///		<see cref="RegisterDependencies" /> method.
		/// </remarks>
		internal static void PostStart()
		{
			IUnityContainer container = new UnityContainer();
			HttpContext.Current.Application.SetContainer( container );

			RegisterDependencies( container );
		}

		/// <summary>
		///		Registers dependencies in the supplied container.
		/// </summary>
		/// <param name="container">Instance of the container to populate.</param>
		private static void RegisterDependencies( IUnityContainer container )
		{
			// TODO: Add any dependencies needed here
            container.RegisterType<UDI.EF.DAL.IEFContext, UDI.EF.DAL.EFContext>(new PerResolveLifetimeManager());
            //container.Resolve<UDI.EF.DAL.IEFContext>();
            //container.RegisterType(typeof(IRepository<>), typeof(EFGenericRepository<>));
            //Bind the various domain model services and repositories that e.g. our controllers require   

            //Register interfaces in CORE
            container.RegisterType(typeof(UDI.CORE.Repositories.IRepository<>), typeof(UDI.EF.Repositories.EFRepositoryBase<>));
            container.RegisterType<UDI.CORE.Services.ICategoryService, UDI.CORE.Services.Impl.CategoryService>();
            container.RegisterType<UDI.CORE.Services.ICustomerService, UDI.CORE.Services.Impl.CustomerService>();
            container.RegisterType<UDI.CORE.Services.IOrderService, UDI.CORE.Services.Impl.OrderService>();
            container.RegisterType<UDI.CORE.Services.IProductService, UDI.CORE.Services.Impl.ProductService>();
            container.RegisterType<UDI.CORE.Services.IUserService, UDI.CORE.Services.Impl.UserService>();
            container.RegisterType<UDI.CORE.UnitOfWork.IUnitOfWork, UDI.EF.UnitOfWork.EFUnitOfWork>(new PerResolveLifetimeManager());
            //container.Resolve<UDI.CORE.UnitOfWork.IUnitOfWork>();
            container.RegisterType<UDI.CORE.UnitOfWork.IUnitOfWorkManager, UDI.EF.UnitOfWork.EFUnitOfWorkManager>();
            //Register interfaces in EF
            container.RegisterType<UDI.EF.DAL.IEFContext, UDI.EF.DAL.EFContext>();
		}
	}
}