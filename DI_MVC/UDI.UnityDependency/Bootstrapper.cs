using System.Web.Mvc;
using Microsoft.Practices.Unity;
using UDI.CORE.Repositories;
using Unity.Mvc4;

namespace UDI.UnityDependency
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();        
    
            // Database context, one per request, ensure it is disposed
            container.RegisterType<UDI.EF.DAL.IEFContext, UDI.EF.DAL.EFContext>(new ContainerControlledLifetimeManager());
            container.Resolve<UDI.EF.DAL.IEFContext>();
            //container.RegisterType(typeof(IRepository<>), typeof(EFGenericRepository<>));
            //Bind the various domain model services and repositories that e.g. our controllers require   

            //Register interfaces in CORE
            container.RegisterType(typeof(UDI.CORE.Repositories.IRepository<>), typeof(UDI.EF.Repositories.EFRepositoryBase<>));
            container.RegisterType<UDI.CORE.Services.ICategoryService, UDI.CORE.Services.Impl.CategoryService>();
            container.RegisterType<UDI.CORE.Services.ICustomerService, UDI.CORE.Services.Impl.CustomerService>();
            container.RegisterType<UDI.CORE.Services.IOrderService, UDI.CORE.Services.Impl.OrderService>();
            container.RegisterType<UDI.CORE.Services.IProductService, UDI.CORE.Services.Impl.ProductService>();
            container.RegisterType<UDI.CORE.Services.IUserService, UDI.CORE.Services.Impl.UserService>();
            container.RegisterType<UDI.CORE.UnitOfWork.IUnitOfWork, UDI.EF.UnitOfWork.EFUnitOfWork>(new ContainerControlledLifetimeManager());
            container.Resolve<UDI.CORE.UnitOfWork.IUnitOfWork>();
            container.RegisterType<UDI.CORE.UnitOfWork.IUnitOfWorkManager, UDI.EF.UnitOfWork.EFUnitOfWorkManager>();
            //Register interfaces in EF
            container.RegisterType<UDI.EF.DAL.IEFContext, UDI.EF.DAL.EFContext>();

            return container;
        }
    }
}