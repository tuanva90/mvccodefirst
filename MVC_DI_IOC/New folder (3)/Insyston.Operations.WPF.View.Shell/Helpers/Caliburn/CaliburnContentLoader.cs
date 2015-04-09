using Caliburn.Micro;
using FirstFloor.ModernUI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Insyston.Operations.WPF.Views.Shell.Helpers.Caliburn
{
    using Insyston.Operations.Model;

    public class CaliburnContentLoader : DefaultContentLoader
    {

        /// <summary>
        /// Field for the TheConductor property.
        /// </summary>
        private static IConductor _theConductor;

        /// <summary>
        /// Gets the TheConductor.
        /// </summary>
        private static IConductor TheConductor
        {
            get
            {
                if (_theConductor == null)
                {
                    _theConductor = new Conductor<Screen>.Collection.OneActive();
                    var activator = _theConductor as IActivate;
                    activator.Activate();
                }
                return _theConductor;
            }
        }
        
        protected override object LoadContent(Uri uri)
        {
            var content = base.LoadContent(uri);
            if (content == null)
                return content;

            var vm = ViewModelLocator.LocateForView(content);
            if (vm == null)
                return content;

            if (content is DependencyObject)
            {
                ViewModelBinder.Bind(vm, content as DependencyObject, null);
            }

            if (TheConductor != null)
            {
                TheConductor.ActivateItem(vm);
            }

            return content;
        }
    }
}
