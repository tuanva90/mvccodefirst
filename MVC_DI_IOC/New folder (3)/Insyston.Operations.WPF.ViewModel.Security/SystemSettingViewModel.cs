using System;
using System.Linq;
using Insyston.Operations.Business.Security;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModels.Common.Commands;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using Insyston.Operations.WPF.ViewModels.Security.Validation;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Insyston.Operations.WPF.ViewModels.Security
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    using Insyston.Operations.WPF.ViewModel.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common;

    public class SystemSettingViewModel : ViewModelUseCaseBase
    {
        private EnumSteps _CurrentEnumStep;

        private LXMUserSystemSetting _SystemSettings;

        private List<string> _ListPasswordStrength;

        public SystemSettingViewModel()        
        {
            this.Validator = new SystemSettingViewModelValidation();
            this.PropertyChanged += this.SystemSettingViewModel_PropertyChanged;

            this._ListPasswordStrength = new List<string>();
            this._ListPasswordStrength.Add("^.{6,}$");
            this._ListPasswordStrength.Add("^[0-9a-zA-Z]{6,}$");
            this._ListPasswordStrength.Add(@"^(?=.*[0-9])(?=.*[a-zA-Z])\w{8,}$");
            this._ListPasswordStrength.Add(@"(?=.{8,})(?=.*[a-zA-Z])(?=.*[\d])(?=.*[\W])");
        }

        public enum EnumSteps
        {
            Start,
            Edit,
            Save,
            Cancel,
            Error,
            None
        }

        public LXMUserSystemSetting SystemSettings
        {
            get
            {
                return this._SystemSettings;
            }
            set
            {
                this.SetField(ref _SystemSettings, value, () => SystemSettings);
            }
        }

        public List<string> ListPasswordStrength
        {
            get
            {
                return this._ListPasswordStrength;
            }
            set
            {
                this.SetField(ref _ListPasswordStrength, value, () => ListPasswordStrength);
            }
        }

        /// <summary>
        /// The check content editing.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<bool> CheckContentEditing()
        {
            if (this.ActiveViewModel.IsCheckedOut && this.IsChanged)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public override async Task OnStepAsync(object stepName)
        {
            bool canProcess = true;
            EnumSteps step = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
            switch (step)
            {
                case EnumSteps.Start:                    
                    this._CurrentEnumStep = EnumSteps.Start;
                    this.SystemSettings = await SystemSettingsFunctions.ReadSystemSettingsAsync();
                    if (this.SystemSettings == null)
                    {
                        this.SystemSettings = new LXMUserSystemSetting();
                    }
                    this.SetActionCommandsAsync();
                    break;
                case EnumSteps.Edit:
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                    this.RaiseActionsWhenChangeStep(EnumScreen.SecuritySetting, EnumSteps.Edit);
                    this._CurrentEnumStep = EnumSteps.Edit;
                    this.IsCheckedOut = true;
                    this.SetActionCommandsAsync();
                    break;
                case EnumSteps.Save:
                    this.Validate();

                    if (this.HasErrors == false)
                    {
                        this.ValidateNotError();
                        this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                        this._CurrentEnumStep = EnumSteps.Save;
                        SystemSettingsFunctions.UpdateSystemSettings(this.SystemSettings);
                        this.RaiseActionsWhenChangeStep(EnumScreen.SecuritySetting, EnumSteps.Save);
                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                        this.SetActionCommandsAsync();
                    }
                    else
                    {
                        this._CurrentEnumStep = EnumSteps.Error;
                        this.SetActionCommandsAsync();
                        this.OnErrorHyperlinkSelected();
                    }
                    break;
                case EnumSteps.Cancel:
                    canProcess = await this.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                        this._CurrentEnumStep = EnumSteps.Cancel;
                        this.SystemSettings = await SystemSettingsFunctions.ReadSystemSettingsAsync();
                        this.RaiseActionsWhenChangeStep(EnumScreen.SecuritySetting, EnumSteps.Cancel);
                        this.IsCheckedOut = false;
                        this.IsChanged = false;
                        this.SetActionCommandsAsync();
                    }
                    break;
                case EnumSteps.Error:
                    NotificationErrorView errorPopup = new NotificationErrorView();
                    NotificationErrorViewModel errorPopupViewModel = new NotificationErrorViewModel();
                    errorPopupViewModel.listCustomHyperlink = this.ListErrorHyperlink;
                    errorPopup.DataContext = errorPopupViewModel;

                    errorPopup.Style = (Style)Application.Current.FindResource("RadWindowStyleNew");

                    errorPopup.ShowDialog();
                    break;
            }
        }

        protected override async void SetActionCommandsAsync()
        {
            switch (this._CurrentEnumStep)
            {
                case EnumSteps.Start:
                case EnumSteps.Save:
                case EnumSteps.Cancel:
                    if (this.CanEdit)
                    {
                        this.ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = EnumSteps.Edit.ToString(), Command = new Edit() },
                        };
                    }
                    break;
                case EnumSteps.Edit:
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = EnumSteps.Save.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = EnumSteps.Cancel.ToString(), Command = new Cancel() }
                    };
                    break;
                case EnumSteps.Error:
                    this.ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = EnumSteps.Save.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = EnumSteps.Cancel.ToString(), Command = new Cancel() },
                        new ActionCommand { Parameter = EnumSteps.Error.ToString(), Command = new Error() }
                    };
                    break;
            }
        }

        public async Task<bool> CheckIfUnSavedChanges()
        {
            bool canProceed = true;
            if (this.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - System Setting";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
            }
            return canProceed;
        }

        protected override async Task UnLockAsync()
        {
            throw new NotImplementedException();
        }

        protected override async Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }

        private void SystemSettingViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Validate(e.PropertyName);
            if ((this._CurrentEnumStep == EnumSteps.Edit) && (e.PropertyName.IndexOf("SystemSettings") != -1))
            {
                this.IsChanged = true;
            }
        }
    }
}
