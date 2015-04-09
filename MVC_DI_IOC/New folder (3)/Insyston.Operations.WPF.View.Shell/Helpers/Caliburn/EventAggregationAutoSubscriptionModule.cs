using Autofac;
using Autofac.Core;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.Views.Shell.Helpers.Caliburn
{
    public class EventAggregationAutoSubscriptionModule : Module
    {
        /// <summary>
        /// Attach To ComponentRegistration
        /// </summary>
        /// <param name="registry"> The registry. </param>
        /// <param name="registration"> The registration. </param>
        protected override void AttachToComponentRegistration(IComponentRegistry registry, IComponentRegistration registration)
        {
            registration.Activated += OnComponentActivated;
        }

        /// <summary>
        /// Handler (delegate) for event 'registration activated'.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The e. </param>
        private static void OnComponentActivated(object sender, ActivatedEventArgs<object> e)
        {
            // we never want to fail, so check for null (should never happen), and return if it is
            if (e == null)
            {
                return;
            }

            // try to convert instance to IHandle
            // I originally did e.Instance.GetType().IsAssignableTo<>() and then 'as', 
            // but it seemed redundant
            var handler = e.Instance as IHandle;

            // if it is not null, it implements, so subscribe
            if (handler != null)
            {
                e.Context.Resolve<IEventAggregator>().Subscribe(handler);
            }
        }
    }
}
