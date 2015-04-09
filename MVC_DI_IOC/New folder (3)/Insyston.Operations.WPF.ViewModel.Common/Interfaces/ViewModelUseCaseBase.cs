using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Insyston.Operations.Business.Common.Enums;
using Insyston.Operations.Common.Reflection;
using Insyston.Operations.Locking;
using Insyston.Operations.Locking.Model;
using Insyston.Operations.Logging;
using Insyston.Operations.Model;
using Insyston.Operations.Security;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using Insyston.Operations.WPF.ViewModels.Common.Validation;
using Telerik.Windows.Controls;
using System.Windows.Input;
using Insyston.Operations.Model.ComplexTypes;
using System.Windows.Threading;
using System.Windows;
using System.Collections.ObjectModel;

namespace Insyston.Operations.WPF.ViewModels.Common.Interfaces
{
    using System.Windows.Media;

    using Insyston.Operations.Locking.Helpers;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.WindowManager;

    using Color = System.Drawing.Color;

    public interface RowItemBase
    {
    }

    public abstract class ViewModelUseCaseBase : ObservableModel
    {
        /// <summary>
        /// Create a cancellation token source, this will be used to cancel a task.
        /// </summary>
        protected readonly CancellationTokenSource CancellationToken = new CancellationTokenSource();
        protected static ViewModelUseCaseBase _NavigationModule;
        private ObservableCollection<ValidationFailure> _ValidationSummary;
        private string _AssemblyVersion;
        private bool _CanEdit;
        private bool _IsCheckedOut;
        private bool _IsChanged;
        private ViewModelUseCaseBase _ActiveViewModel;
        private ObservableCollection<ActionCommand> _ActionCommands;
        private Brush _gridStyle;
        private bool _isError;
        public bool IsEditing { get; set; }

        /// <summary>
        /// Gets or sets the instance guid.
        /// </summary>
        public Guid InstanceGUID { get; set; }

        public ViewModelUseCaseBase() : this(true)
        { 
        }

        protected ViewModelUseCaseBase(bool checkVersion)
        {
            this._ValidationSummary = new ObservableCollection<ValidationFailure>();
            this.OnUseCaseStepChanged = new AwaitableDelegateCommand<object>(this.OnStepAsync);
            this.ListErrorHyperlink = new List<CustomHyperlink>();
            this.ActiveViewModel = this;
            if (checkVersion == true)
            {
                Application.Current.Dispatcher.InvokeAsync(new Action(() => this.InitialiseVersion()));
            }
            this._gridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
        }

        public enum StoryBoardState
        {
            SummaryState,
            ResultState,
        }

        private StoryBoardState _currentStoryBoardState;
        public StoryBoardState _CurrentStoryBoardState
        {
            get
            {
                return this._currentStoryBoardState;
            }
            set
            {
                this.SetField(ref _currentStoryBoardState, value, () => _CurrentStoryBoardState);
            }
        }

        private List<CustomHyperlink> _listErrorHyperlink;
        public List<CustomHyperlink> ListErrorHyperlink
        {
            get
            {
                return this._listErrorHyperlink;
            }
            set
            {
                this.SetField(ref this._listErrorHyperlink, value, () => this.ListErrorHyperlink);
            }
        }

        public bool IsError
        {
            get
            {
                return _isError;
            }
            set
            {
                this.SetField(ref this._isError, value, () => this.IsError);
            }
        }
        public event Action<string> StepChanged;

        public Action<EnumScreen, object, object> RaiseStepChanged;

        public Action<EnumScreen> CancelNewItem;

        public Action ErrorHyperlinkSelected;

        public Action NotErrorHyperlink;

        /// <summary>
        /// The on demand disposed.
        /// </summary>
        protected Action OnDemandDisposed { get; set; }

        public Brush GridStyle
        {
            get
            {
                return this._gridStyle;
            }
            set
            {
                this.SetField(ref _gridStyle, value, () => GridStyle);
            }
        }
        /// <summary>
        /// Field for the IsBusy Property.
        /// </summary>
        private bool _isBusy;

        /// <summary>
        /// Gets or sets a value indicating whether is busy.
        /// </summary>
        public virtual bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                this.SetField(ref _isBusy, value, () => IsBusy); 
            }
        }

        /// <summary>
        /// Field for IsBusyMessage property.
        /// </summary>
        private string _isBusyMessage;

        /// <summary>
        /// Gets or sets the is loading message
        /// </summary>
        public string BusyContent
        {
            get
           
            {
                return _isBusyMessage;
            }
            set
            {
                this.SetField(ref _isBusyMessage, value, () => BusyContent); 
            }
        }

        public bool CanEdit
        {
            get
            {
                return this._CanEdit;
            }
            //TODO: Need to look at making the setter private/protected, as the value should be set within the viewmodel and the value should be queried from the Security layer.
            set
            {
                this.SetField(ref _CanEdit, value, () => CanEdit);
            }
        }

        public bool IsCheckedOut
        {
            get
            {
                return this._IsCheckedOut;
            }
            set
            {
                this.SetField(ref _IsCheckedOut, value, () => IsCheckedOut);
            }
        }

        public bool IsChanged
        {
            get
            {
                return this._IsChanged;
            }
            set
            {
                this.SetField(ref _IsChanged, value, () => IsChanged);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return this._AssemblyVersion;
            }
            set
            {
                this.SetField(ref _AssemblyVersion, value, () => AssemblyVersion);
            }
        }

        public virtual ViewModelUseCaseBase ActiveViewModel
        {
            get
            {
                return this._ActiveViewModel;
            }
            set
            {
                this.SetField(ref _ActiveViewModel, value, () => ActiveViewModel);
            }
        }

        public AwaitableDelegateCommand<object> OnUseCaseStepChanged { get; private set; }

        public ObservableCollection<ActionCommand> ActionCommands
        {
            get
            {
                return this._ActionCommands;
            }
            protected set
            {
                this.SetField(ref _ActionCommands, value, () => ActionCommands);
            }
        }

        public ObservableCollection<ValidationFailure> ValidationSummary
        {
            get
            {
                return this._ValidationSummary;
            }
            private set
            {
                //SetField(ref _ValidationSummary, value, () => ValidationSummary);
                if (this._ValidationSummary != value)
                {
                    this._ValidationSummary = value;
                    this.OnPropertyChanged(() => ValidationSummary);
                }
            }
        }

        public IValidator Validator { get; set; }

        //public void OnStep(object stepName)
        //{
        //    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () => await this.OnStepAsync(stepName)));
        //}

        //public virtual async Task OnStepAsync(object stepName, CancellationToken cancellationToken = CancellationToken.None)

        /// <summary>
        /// Sets the IsBusy guard property to true to indicate we are busy and the message to display.
        /// </summary>
        /// <param name="message"> The message to display. </param>
        // ReSharper disable MemberCanBeProtected.Global
        public void SetBusyAction(string message = "Processing")
        // ReSharper restore MemberCanBeProtected.Global
        {
            IsBusy = true;
            BusyContent = message;
        }

        /// <summary>
        /// Sets the IsBusy guard property to false to indicate we are no longer busy
        /// </summary>
        public void ResetBusyAction()
        {
            IsBusy = false;
            BusyContent = string.Empty;
        }

        public virtual async Task OnStepAsync(object stepName)
        { 
            throw new NotImplementedException();
        }

        public async void ShowMessageAsync (string message, string title)
        {
            //this.ActiveViewModel.NotificationWindow.Raise(new UIInformation(null) { Content = message, Title = title, Icon = "Information.ico" });
            NotificationValidate notificationValidate = new NotificationValidate();
            NotificationValidateViewModel notificationViewModel = new NotificationValidateViewModel();
            notificationViewModel.Content = message;
            notificationViewModel.Title = title;

            notificationValidate.DataContext = notificationViewModel;
            notificationValidate.Show();
        }

        public override async void Dispose()
        {
            try
            {
                await this.UnLockAsync();
            }
            catch
            {
            }

            if (this.StepChanged != null)
            {
                //Parallel.ForEach(this.StepChanged.GetInvocationList(), d => this.StepChanged -= d as Action<string>);
                this.StepChanged.GetInvocationList().ForEach(d => this.StepChanged -= d as Action<string>);
            }

            if (this.ActiveViewModel != null)
            {
                if (this.ActiveViewModel != this)
                {
                    this.ActiveViewModel.Dispose();
                    this.ActiveViewModel = null;
                }
            }
            base.Dispose();
        }

        /// <summary>
        /// Cancel any the tasks if the ElementFramework that associated to the currently ViewModel was disposed.
        /// </summary>
        public void OnAssociatedElementFrameworkDispose()
        {
            // marks any the tasks should be canceled
            CancellationToken.Cancel();
        }

        public async Task ValidateAsync()
        {
            await Task.Run(() => this.Validate());
        }

        public async Task ValidateAsync(params string[] propertyNames)
        {
            await this.ValidateAsync(false, propertyNames);
        }

        public async Task ValidateAsync(bool clearExistingErrors, params string[] propertyNames)
        {
            await Task.Run(() => this.Validate(clearExistingErrors, propertyNames));
        }

        public async Task ValidateAsync<T>(params Expression<Func<T>>[] selectorExpressions)
        {
            await this.ValidateAsync<T>(false, selectorExpressions);
        }

        public async Task ValidateAsync<T>(bool clearExistingErrors, params Expression<Func<T>>[] selectorExpressions)
        {
            await Task.Run(() => this.Validate(clearExistingErrors, selectorExpressions));
        }

        protected virtual async Task NavigateToFormAsync(Forms form, ViewModelUseCaseBase viewModel)
        {
            if (_NavigationModule != null)
            {
                await _NavigationModule.NavigateToFormAsync(form, viewModel);
            }
        }

        protected abstract Task UnLockAsync();

        protected async Task UnLockAsync(string tableName, string id)
        {
            await LockFunctions.UnLockAsync(tableName, id, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId);
            this.IsCheckedOut = false;
            this.IsChanged = false;
        }

        protected async Task UnLockAsync(string tableName, string id, Guid _InstanceGUID)
        {
            await LockFunctions.UnLockAsync(tableName, id, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId, _InstanceGUID);
            this.IsCheckedOut = false;
            this.IsChanged = false;
        }

        protected async Task UnLockListAsync(List<string> tableName, List<string> id, Guid _InstanceGUID)
        {
            await LockFunctions.UnLockListAsync(tableName, id, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId, _InstanceGUID);
            this.IsCheckedOut = false;
            this.IsChanged = false;
        }

        protected async Task UnLockListItemsAsync(string tableName, ItemLock itemLock)
        {
            await LockFunctions.UnLockListItemsAsync(tableName, itemLock);
            this.IsChanged = false;
            this.IsCheckedOut = false;
        }

        protected abstract Task<bool> LockAsync();

        protected async Task<bool> LockAsync(string tableName, string id, Guid _InstanceGUID)
        {
            LockResponse result;

            try
            {
                if (id == "0")
                {
                    result = new LockResponse(true);
                }
                else
                {
                    result = await LockFunctions.LockAsync(tableName, id, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId, _InstanceGUID);
                }
            }
            catch (Exception ex)
            {
                result = new LockResponse(ex);
            }

            this.IsCheckedOut = result.Result;
            if (result.Result == false)
            {
                this.ShowMessageAsync (result.Message, "Record Lock - Error");
            }

            return result.Result;
        }

        protected async Task<bool> CheckLockedAsync(string tableName, ItemLock itemCheck)
        {
            LockResponse result;

            try
            {
                result = await LockFunctions.CheckLockedAsync(tableName, itemCheck);
            }
            catch (Exception ex)
            {
                result = new LockResponse(ex);
            }

            this.IsCheckedOut = result.Result;
            if (result.Result == false)
            {
                this.ShowMessageAsync(result.Message, "Record Lock - Error");
            }
            return result.Result;
        }

        protected async Task<bool> LockListItemsAsync(string tableName, ItemLock itemLock)
        {
            LockResponse result;

            try
            {
                await LockFunctions.LockListItemsAsync(tableName, itemLock);
                result = new LockResponse(true);
            }
            catch (Exception ex)
            {
                result = new LockResponse(ex);
            }

            return result.Result;
        }

        protected async Task<bool> LockListAsync(List<string> listTableName, List<string> listId, Guid _InstanceGUID)
        {
            LockResponse result;

            try
            {
                if (listTableName == null || listId == null || listTableName.Count == 0 || listTableName.Count == 0)
                {
                    result = new LockResponse(true);
                }
                else
                {
                    result = await LockFunctions.LockListAsync(listTableName, listId, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId, _InstanceGUID);
                }
            }
            catch (Exception ex)
            {
                result = new LockResponse(ex);
            }

            this.IsCheckedOut = result.Result;
            if (result.Result == false)
            {
                this.ShowMessageAsync(result.Message, "Record Lock - Error");
            }

            return result.Result;
        }

        protected async Task<LockResponse> LockMultiTableAsync(string tableName, string id, Guid _InstanceGUID)
        {
            LockResponse result;

            try
            {
                if (id == "0")
                {
                    result = new LockResponse(true);
                }
                else
                {
                    result = await LockFunctions.LockAsync(tableName, id, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId, _InstanceGUID);
                }
            }
            catch (Exception ex)
            {
                result = new LockResponse(ex);
            }

            this.IsCheckedOut = result.Result;

            return result;
        }

        protected async Task UnLockBulkUpdateAsync(object value, List<string> tableNames, Guid _InstanceGUID)
        {
            foreach (var tableName in tableNames)
            {
                await this.UnLockAsync(tableName, value.ToString(), this.InstanceGUID);
            }
        }
        protected async Task<LockResponse> LockBulkUpdateAsync(object value, List<string> tableNames, Guid _InstanceGUID)
        {
            LockResponse result = null;
            foreach (var tableName in tableNames)
            {
                result = await this.LockMultiTableAsync(tableName, value.ToString(), this.InstanceGUID);
            }
            return result;
        }

        protected async Task<bool> LockAsync(string tableName, string id)
        {
            LockResponse result;

            try
            {
                if (id == "0")
                {
                    result = new LockResponse(true);
                }
                else
                {
                    result = await LockFunctions.LockAsync(tableName, id, ((OperationsPrincipal)Thread.CurrentPrincipal).Identity.User.UserEntityId);
                }
            }
            catch (Exception ex)
            {
                result = new LockResponse(ex);
            }

            this.IsCheckedOut = result.Result;
            if (result.Result == false)
            {
                this.ShowMessageAsync(result.Message, "Record Lock - Error");
            }

            return result.Result;
        }

        protected virtual async void SetActionCommandsAsync()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnStepChanged(string stepName)
        {
            if (this.StepChanged != null)
            {
                this.StepChanged(stepName);
            }
        }

        public virtual void RaiseActionsWhenChangeStep(EnumScreen e, object stepName, object id = null) 
        {
            if (this.RaiseStepChanged != null)
            {
                this.RaiseStepChanged(e, stepName, id);
            }
        }

        public virtual void OnCancelNewItem(EnumScreen e)
        {
            if (this.CancelNewItem != null)
            {
                this.CancelNewItem(e);
            }
        }

        public virtual void OnErrorHyperlinkSelected()
        {
            if (this.ErrorHyperlinkSelected != null)
            {
                this.ErrorHyperlinkSelected();
            }
        }

        public virtual void ValidateNotError()
        {
            if (this.NotErrorHyperlink != null)
            {
                this.NotErrorHyperlink();
            }
        }

        protected void ShowErrorMessage(string message, string title)
        {
            TelerikWindowManager.Alert(title,message);
        }

        public bool Validate()
        {
            ValidationResult validationResults;
            
            if (this.Validator == null)
            {
                return true;
            }
            //lock (this._Lock)
            //{
            List<string> properties;
            string tabHyperlinkError = string.Empty;
            properties = this._Failures.Select(x => x.PropertyName).Distinct().ToList();

            validationResults = this.Validator.Validate(this);

            if (validationResults.Errors.Count > 0)
            {
                this.ListErrorHyperlink = new List<CustomHyperlink>();

                foreach (var error in validationResults.Errors)
                {
                    var errorHyperlink = new CustomHyperlink();
                    errorHyperlink.HyperlinkHeader = error.ErrorMessage;
                  
                    // gets the action for the error ErrorHyperlink
                    var arrayProperiesError = error.PropertyName.Split('.');
                    if (arrayProperiesError.Length > 2)
                    {
                        tabHyperlinkError = arrayProperiesError[2];
                    }
                    else if (arrayProperiesError.Length > 0)
                    {
                        tabHyperlinkError = arrayProperiesError[0];
                    }

                    switch (tabHyperlinkError)
                    {
                        // Error on User module
                        case "LoginName":
                        case "Firstname":
                        case "PostalStateId":
                        case "StateId":
                            errorHyperlink.Action = HyperLinkAction.PersonalDetailsState;
                            errorHyperlink.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton");
                            break;
                        case "NewPassword":
                        case "ReEnterPassword":
                            errorHyperlink.Action = HyperLinkAction.CredentialsState;
                            errorHyperlink.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton");
                            break;

                        // Error on Group module
                        case "GroupName":
                        case "GroupDescription":
                            errorHyperlink.Action = HyperLinkAction.DetailsState;
                            errorHyperlink.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton");
                            break;

                        // Error on Funding module
                        case "CurrentStep":
                        case "SelectedTrancheProfile":
                            errorHyperlink.Action = HyperLinkAction.SummaryState;
                            errorHyperlink.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton");
                            break;

                        // Error on Collection Queues module
                        case "QueueName":
                        case "AssignmentOrder":
                            errorHyperlink.Action = HyperLinkAction.None;
                            errorHyperlink.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton2");
                            break;

                        // Error on Collection Setting module
                        case "CollectionQueueSetting":
                            errorHyperlink.Action = HyperLinkAction.None;
                            errorHyperlink.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton2");
                            break;

                        // Error on Asset Feature
                        case "FeatureName":
                        case "RequiredLengthString":
                            errorHyperlink.Action = HyperLinkAction.DetailsState;
                            errorHyperlink.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton");
                            break;
                        case "SelectedFeatureType":
                            errorHyperlink.Action = HyperLinkAction.AssignedToState;
                            errorHyperlink.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton");
                            break;

                        // Error on Registered Asset
                        case "NetAssetCost":
                        case "ItemId":
                        case "ID":
                            errorHyperlink.Action = HyperLinkAction.DetailsState;
                            errorHyperlink.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton");
                            break;
                        default:
                            errorHyperlink.Action = HyperLinkAction.None;
                            errorHyperlink.SelectedStyle = (Style)Application.Current.FindResource("HyperlinkLikeButton2");
                            break;
                    }
                    this.ListErrorHyperlink.Add(errorHyperlink);
                }
            }

            this._Failures.Clear();
            this._Failures.AddRange(validationResults.Errors);
            if (Enumerable.SequenceEqual(this._Failures.OrderBy(t => t.PropertyName), this.ValidationSummary.OrderBy(t => t.PropertyName)) == false)
            {
                this.ValidationSummary = new ObservableCollection<ValidationFailure>(this._Failures);
            }
            properties.AddRange(this._Failures.Where(x => properties.Contains(x.PropertyName) == false).Select(x => x.PropertyName).Distinct().ToList());
            //Parallel.ForEach(properties, x => this.NotifyErrorsChanged(x));
            properties.ForEach(x => this.NotifyErrorsChanged(x));
                
            return validationResults.IsValid;
            //}
        }

        protected bool Validate(params string[] propertyNames)
        {
            return this.Validate(false, propertyNames);
        }

        protected bool Validate(bool clearExistingErrors, params string[] propertyNames)
        {
            ValidationResult validationResults;
            if (this.Validator == null)
            {
                return true;
            }

            if (propertyNames == null)
            {
                return this.Validate();
            }
            else
            {
                //lock (this._Lock)
                //{
                List<string> properties;

                properties = propertyNames.ToList();
                
                validationResults = this.Validator.ValidateRecursive(this, propertyNames);
                if (clearExistingErrors == true)
                {
                    this._Failures.Clear();
                }
                else
                {
                    this._Failures.RemoveAll((x) => propertyNames.Any(p => p == x.PropertyName));
                }
                this._Failures.AddRange(validationResults.Errors);
                if (Enumerable.SequenceEqual(this._Failures.OrderBy(t => t.PropertyName), this.ValidationSummary.OrderBy(t => t.PropertyName)) == false)
                {
                    this.ValidationSummary = new ObservableCollection<ValidationFailure>(this._Failures);
                }

                if (clearExistingErrors == true)
                {
                    properties.AddRange(this._Failures.Where(x => properties.Contains(x.PropertyName) == false).Select(x => x.PropertyName).Distinct().ToList());
                }

                //Parallel.ForEach(properties, x => this.NotifyErrorsChanged(x));
                properties.ForEach(x => this.NotifyErrorsChanged(x));
                    
                return validationResults.IsValid;
                //}
            }
        }

        protected bool Validate<T>(params Expression<Func<T>>[] selectorExpressions)
        {
            return this.Validate(false, selectorExpressions);
        }

        protected bool Validate<T>(bool clearExistingErrors, params Expression<Func<T>>[] selectorExpressions)
        {
            string[] propertyNames;

            propertyNames = new string[selectorExpressions.Length];
            for (int i = 0; i < selectorExpressions.Length; i++)
            {
                propertyNames[i] = this.GetFullPropertyName(selectorExpressions[i]);
            }
            return this.Validate(clearExistingErrors, propertyNames);
        }

        protected void AddManualValidationError(string propertyName, string error)
        {
            this.ValidationSummary.Add(new ValidationFailure(propertyName, error));
        }

        private async void InitialiseVersion()
        {
            string title = string.Empty;
            string message = string.Empty;

            this.AssemblyVersion = string.Format("Version {0}", Insyston.Operations.Business.Common.Version.AssemblyVersion);

            try
            {
                await Insyston.Operations.Business.Common.Version.IsProductCompatibleAsync();
            }
            catch (VersionException ex)
            {
                ExceptionLogger.WriteLog(ex);
                title = "VersionException";
                message = ex.Message;
            }
            catch (Exception ex)
            {
                ExceptionLogger.WriteLog(ex);
                title = "Unexpected Error";
                message = "Unexpected Error.\nThe error has been logged into the System, Please contact your System Administrator.";
            }

            if (title != string.Empty)
            {
                this.AddManualValidationError(title, message);
                TelerikWindowManager.Confirm(
                    title,
                    message,
                    () =>
                        {
                            Application.Current.Shutdown();
                        });
            }
        }

        private void NotifyErrorsChanged(string fullPath)
        {
            object instance;
            string propertyName;
            string objectPath;
            PropertyPathDescriptor propertyFetcher;

            if (fullPath.Contains(".") == true)
            {
                propertyName = fullPath.Substring(fullPath.LastIndexOf(".") + 1);
                objectPath = fullPath.Substring(0, fullPath.LastIndexOf("."));
            }
            else
            {
                propertyName = fullPath;
                objectPath = string.Empty;
            }

            propertyFetcher = new PropertyPathDescriptor(this, objectPath);

            instance = propertyFetcher.Evaluate();
            if (instance is ObservableModel)
            {
                ((ObservableModel)instance).UpdateErrors(propertyName, this._Failures.Where(x => x.PropertyName == fullPath).ToList());
            }
        }

        /// <summary>
        /// Raises this object will be cleaned up by the Dispose method. 
        /// </summary>
        protected void OnDisposed()
        {
            // Check to see if Dispose has already been called.
            if (null == this.OnDemandDisposed)
            {
                return;
            }
            this.OnDemandDisposed.Invoke();

            // makes sure the dispose action just only one the time of invokes
            this.OnDemandDisposed = null;
        }

        /// <summary>
        /// The clear notify errors.
        /// </summary>
        public void ClearNotifyErrors()
        {
            List<string> properties = this._Failures.Select(x => x.PropertyName).Distinct().ToList();
            this._Failures.Clear();
            this.ValidationSummary.Clear();
            properties.ForEach(x => this.NotifyErrorsChanged(x));
        }
    }
}
/// <summary>
/// The enum screen.
/// </summary>
public enum EnumScreen
{
    Home,
    Security,
    Users,
    UsersAdd,
    Groups,
    GroupsAdd,
    CollectionSettings,
    SecuritySetting,
    ColletionQueues,
    CollectionAssignment,
    Details,
    Activity,
    Configuration,
    Membership,
    FundingSummary,
    FundingContact,
    Collectors,
    EditUser,
    AssetClassesCategory,
    AssetClassesType,
    AssetClassesMake,
    AssetClassesModel,
    AssetCollateralClasses,
    AssetFeatures,
    AssetSettings,
    AssetRegisters,
    RegisteredAsset,
}

public enum EnumSteps
{
    Add,
    Copy,
    Edit,
    Cancel,
    Save,
    SelectUser,
    SelectGroup,
    SelectQueue,
    PowerOff,
    Configuration,
    SelectTranche,
    CollectionSettings,
    SecuritySetting,
    ColletionQueues,
    Home,
    Delete,
    AssignmentFilter,
    ResultState,
    CreateNew,
    ItemLocked,
    SelectedFeatureType,
    SelectModel,
    SelectedAssetClassesCategoryItem,
    SelectedMake,
    AssetClassesCategoryAssignFeaturesState,
    AssetClassesCategoryAssignTypesState,
    AssignFeature,
    SaveAssignFeature,
    SaveAssignTypes,
    SaveUpdateDepreciation,
    CancelAssignFeature,
    CancelAssignTypes,
    CancelUpdateDepreciation,
    AssetClassesCategoryUpdateDepreciationState,
    AssignModel,
    BulkUpdate,
    CancelBulkUpdate,
    EditBulkUpdate,
    SaveAssignModel,
    SelectedCollateral,
    SelectedAssetClassesTypeItem,
    CancelAssignMake,
    SaveAssignMake,
    AssetClassesTypeAssignFeaturesState,
    AssetClassesTypeUpdateDepreciationState,
    AssetClassesTypeAssignMakeState,
    CancelAssignModel,
    EditAssignModel,
    SelectOldTabHyperlink,
    SelectRegister,
    SaveRegisterSummary,
    SelectRegisteredAsset,
    Transfer
}

/// <summary>
/// The toolbar action enum.
/// </summary>
public enum EnumToolbarAction
{
    Add,
    Delete,
}

/// <summary>
/// The equip search type.
/// </summary>
public enum SystemType
{
    Category,
    Type,
    Make,
    Model,
    AllAssetClass
}
public enum EnumRegisteredAssetState
{
    Active = 749,
    Inactive = 750,
    Terminated = 751,
    Cancelled = 752,
}

public enum EnumRegisteredAssetStatus
{
    AssetLive = 124,
    AssetDisposedToClient = 126,
    InertiaNotification = 136,
    AssetDisposedOther = 130,
    AssetWrittenOff = 229,
    AssetFlatCancelled = 219,
    AssetClosed = 209,
    AssetDisposedToRepurchaser = 463,
    AssetDisposedToSupplier = 464,
    AssetIdle = 758,
    AssetReturn = 125,
    AssetReserve = 757,
}