using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Insyston.Operations.Common;
using Insyston.Operations.Locking;
using Insyston.Operations.Security;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.ViewModels.Common.Interfaces
{
    using System.ComponentModel;

    using Insyston.Operations.WPF.ViewModels.Common.WindowManager;

    [Obsolete("We need to merge the baseviewmodel classes together.")]
    public abstract class OldViewModelBase : INotifyPropertyChanged
    {
        public static readonly DependencyProperty ValidateInputProperty = DependencyProperty.RegisterAttached("ValidateInput", typeof(bool), typeof(OldViewModelBase), new PropertyMetadata(OnValidatePropertyChanged));
        protected const string contactSupportMessage = "\nThe error has been logged into the System, Please contact your System Administrator.";

        private static readonly List<FrameworkElement> views = new List<FrameworkElement>();
        private Dictionary<string, string> errorMessages;
        private bool _IsBusy, _HasError, _IsLocked;
        private Image _Icon;
        private string _LockTableName, _LockUniqueIdentifier, _IconFileName;
        private Insyston.Operations.Model.ObservableModelCollection<ActionCommand> actionCommands;

        public OldViewModelBase()
        {
            this.actionCommands = new Insyston.Operations.Model.ObservableModelCollection<ActionCommand>();
            
            this.ActionCommand = new DelegateCommand<string>(this.OnActionCommand);
        }

        public delegate void ClearGridFilterHandler();

        public event EventHandler RequestClose;
        
        public event ClearGridFilterHandler ClearGridFilter;        

        public DelegateCommand<string> ActionCommand { get; private set; }

        public string IconFileName
        {
            get
            {
                return this._IconFileName;
            }
            set
            {
                if (this._IconFileName != value)
                {
                    this._IconFileName = value;
                    this.RaisePropertyChanged("IconFileName");
                    this.RaisePropertyChanged("Icon");
                }
            }
        }

        public Image Icon
        {
            get
            { 
                if (string.IsNullOrEmpty(this._IconFileName))
                {
                    return null;
                }

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();

                if (Utilities.CommonImages.Where(command => this._IconFileName.ToLower().Contains(command)).Count() > 0)
                {
                    bitmap.UriSource = new Uri(string.Format("pack://application:,,,/{0};component/Images/{1}", Utilities._ViewCommonAssemblyName, this._IconFileName));
                }
                else
                {
                    bitmap.UriSource = new Uri(string.Format("pack://application:,,,/{0};component/Images/{1}", ((IView)views.LastOrDefault()).AssemblyName, this._IconFileName));
                }

                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return new Image() { Source = bitmap, Width = 20.0, Height = 20.0, Stretch = System.Windows.Media.Stretch.Uniform, Margin = new Thickness(0,0,2,0) };
            }
            set
            {
                if (this._Icon != value)
                {
                    this._Icon = value;
                    this.RaisePropertyChanged("Icon");
                }
            }
        }


        public List<string> ErrorMessages
        {
            get
            {
                if (this.errorMessages == null)
                {
                    return null;
                }

                return this.errorMessages.Values.ToList();
            }
            set
            {
                this.RaisePropertyChanged("ErrorMessages");
            }
        }

        public Insyston.Operations.Model.ObservableModelCollection<ActionCommand> ActionCommands
        {
            get
            {
                return this.actionCommands;
            }
            set
            {
                if (this.actionCommands != value)
                {
                    this.actionCommands = value;
                    this.RaisePropertyChanged("ActionCommands");
                }
            }
        }

        public bool HasError
        {
            get
            {
                return this._HasError;
            }
            set
            {
                if (this._HasError != value)
                {
                    this._HasError = value;
                    this.RaisePropertyChanged("HasError");
                }
            }
        }

        public bool IsLocked
        {
            get
            {
                return this._IsLocked;
            }
            set
            {
                if (this._IsLocked != value)
                {
                    this._IsLocked = value;
                    this.RaisePropertyChanged("IsLocked");
                    this.OnRecordLockChanged();
                }
            }
        }

        public bool IsBusy
        {
            get
            {
                return this._IsBusy;
            }
            set
            {
                if (this._IsBusy != value)
                {
                    this._IsBusy = value;
                    this.RaisePropertyChanged("IsBusy");
                }
            }
        }

        public static void SetValidateInput(FrameworkElement element, bool value)
        {
            element.SetValue(ValidateInputProperty, value);
        }

        public static bool GetValidateInput(FrameworkElement element)
        {
            return (bool)element.GetValue(ValidateInputProperty);
        }

        public static void OnValidatePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        { 
            views.Add(obj as FrameworkElement);
        }

        public static void RemoveView()
        {
            if (views.Count > 0)
            {
                views.RemoveAt(views.Count - 1);
            }
        }

        public void AddErrorMessage(string key, string errorMessage)
        {
            if (this.errorMessages == null)
            {
                this.errorMessages = new Dictionary<string, string>();
            }

            if (this.errorMessages.ContainsKey(key))
            {
                this.errorMessages[key] = errorMessage;
            }
            else
            {
                this.errorMessages.Add(key, errorMessage);
            }

            this.HasError = true;
            this.RaisePropertyChanged("ErrorMessages");
        }

        public IEnumerable<string> GetErrorMessage(string key)
        {
            if (this.errorMessages == null || string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (this.errorMessages.Where(errkey => errkey.Key == key).Count() > 0)
            {
                return new List<string>() { this.errorMessages[key] };
            }

            return null;
        }

        public void ClearErrorMessages()
        {
            if (this.errorMessages != null)
            {
                this.errorMessages.Clear();
                ((IView)views.LastOrDefault()).ClearErrors();
                this.RaisePropertyChanged("ErrorMessages");
            }

            this.HasError = false;
        }

        public void RemoveErrorMessage(string key, string errorMessage)
        {
            if (this.errorMessages == null)
            {
                return;
            }

            if (this.errorMessages.Keys.Where(item => item == key).Count() > 0 && this.errorMessages[key] == errorMessage)
            {
                this.errorMessages.Remove(key);
                this.RaisePropertyChanged("ErrorMessages");
                this.HasError = this.errorMessages.Count != 0;
            }
        }

        public void RemoveErrorMessage(string key)
        {
            if (this.errorMessages == null)
            {
                return;
            }

            if (this.errorMessages.Keys.Where(item => item == key).Count() > 0)
            {
                this.errorMessages.Remove(key);
                this.RaisePropertyChanged("ErrorMessages");
                this.HasError = this.errorMessages.Count != 0;
            }
        }

        public void Close()
        {
            if (this.RequestClose != null)
            {
                this.RequestClose(this, EventArgs.Empty);
            }
        }
        
        protected virtual void OnActionCommand(string command)
        {
        }

        protected bool Validate()
        {
            this.HasError = false;
            ((IView)views.LastOrDefault()).Validate();
            return !this.HasError;
        }

        protected virtual void OnRecordLockChanged()
        {
        }

        protected async Task UnLockAsync()
        {
            await LockFunctions.UnLockAsync(this._LockTableName, this._LockUniqueIdentifier, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId);
            this.IsLocked = false;
        }

        protected async Task LockAsync()
        {
            try
            {
                await LockFunctions.LockAsync(this._LockTableName, this._LockUniqueIdentifier, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId);
                this.IsLocked = true;
            }
            catch (Exception ex)
            {
                // this.ShowMessage(ex.Message, "Record Lock - Error");
                TelerikWindowManager.Alert("Record Lock - Error", ex.Message);
            }
        }

        protected void AddActionCommand(string command)
        {
            this.AddActionCommand(command, command);
        }

        protected void AddActionCommand(string command, string parameter)
        {
            Assembly assembly;
            UserControl control = null;

            if (views == null || views.Count == 0)
            {
                return;
            }

            if (command != "-")
            {
                if (Utilities.CommonCommands.Where(item => item.ToLower().Contains(command.ToLower())).Count() > 0)
                {
                    assembly = Assembly.Load(Utilities._ViewCommonAssemblyName);
                    control = (UserControl)assembly.CreateInstance(string.Format("{0}.Commands.{1}", Utilities._ViewCommonAssemblyName, command), true, BindingFlags.CreateInstance, null, null, Thread.CurrentThread.CurrentUICulture, null);
                }
                else
                {
                    assembly = Assembly.Load(((IView)views.LastOrDefault()).AssemblyName);
                    control = (UserControl)assembly.CreateInstance(string.Format("{0}.Commands.{1}", ((IView)views.LastOrDefault()).AssemblyName, command), true, BindingFlags.CreateInstance, null, null, Thread.CurrentThread.CurrentUICulture, null);
                }
            }

            if (command == "-" || control != null)
            {
                this.actionCommands.Add(new ActionCommand { Command = control, Parameter = parameter });
            }
        }

        protected void RemoveCommand(string parameter)
        {
            ActionCommand actionCommand;

            actionCommand = this.actionCommands.Where(item => item.Parameter == parameter).FirstOrDefault();

            if (actionCommand != null)
            {
                this.actionCommands.Remove(actionCommand);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
