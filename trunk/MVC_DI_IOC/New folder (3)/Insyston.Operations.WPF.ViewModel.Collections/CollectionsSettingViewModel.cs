using Insyston.Operations.Business.Collections;
using Insyston.Operations.Business.Common.Model;
using Insyston.Operations.Model;
using Insyston.Operations.WPF.ViewModels.Collections.Validation;
using Insyston.Operations.WPF.ViewModels.Common.Commands;
using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
using Insyston.Operations.WPF.ViewModels.Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.ViewModels.Collections
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    using Insyston.Operations.WPF.ViewModel.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common;

    public class CollectionsSettingViewModel : ViewModelUseCaseBase
    {
        private EnumSteps _CurrentEnumStep;
        private CollectionSetting _CollectionQueueSetting;
        private CollectionSystemDefault _CollectionSystemDefault;
        private List<DropdownList> _Category;

        public CollectionsSettingViewModel()
        {
            Validator = new CollectionsSettingsViewModelValidation();
            PropertyChanged += CollectionsSettingViewModel_PropertyChanged;
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

        public CollectionSetting CollectionQueueSetting
        {
            get
            {
                return _CollectionQueueSetting;
            }
            set
            {
                SetField(ref _CollectionQueueSetting, value, () => CollectionQueueSetting);
            }
        }

        public bool IsPercentaged { get; set; }

        public CollectionSystemDefault CollectionSystemDefault
        {
            get
            {
                return _CollectionSystemDefault;
            }
            set
            {
                SetField(ref _CollectionSystemDefault, value, () => CollectionSystemDefault);
            }
        }

        public List<DropdownList> Category
        {
            get
            {
                return _Category;
            }
            set
            {
                SetField(ref _Category, value, () => Category);
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
            var step = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
            switch (step)
            {
                case EnumSteps.Start:
                    _CurrentEnumStep = EnumSteps.Start;
                    CollectionQueueSetting = await CollectionsQueueSettingsFunctions.ReadCollectionsSystemSettingsAsync() ??
                                             new CollectionSetting();

                    CollectionSystemDefault = await CollectionsQueueSettingsFunctions.ReadCollectionSystemDefaultAsync() ??
                                              new CollectionSystemDefault();

                    var allSystemParam = new ObservableCollection<SystemParam>(await CollectionsQueueSettingsFunctions.ReadAllSystemParamAsync());
                    Category = allSystemParam
                        .Select(cate => new DropdownList { Description = cate.ParamDesc, ID = cate.ParamId })
                        .OrderBy(item => item.Description)
                        .ToList();

                    SetActionCommandsAsync();
                    break;

                case EnumSteps.Edit:
                    this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
                    this.RaiseActionsWhenChangeStep(EnumScreen.CollectionSettings, EnumSteps.Edit);
                    _CurrentEnumStep = EnumSteps.Edit;
                    IsCheckedOut = true;
                    SetActionCommandsAsync();
                    break;

                case EnumSteps.Save:
                    if (IsPercentaged)
                    {
                        CollectionQueueSetting.MinimumArrearsAmount = 0;
                    }
                    else
                    {
                        CollectionQueueSetting.MinimumArrearsPercent = 0;
                    }
                    Validate();

                    if (HasErrors == false)
                    {
                        this.ValidateNotError();
                        this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                        _CurrentEnumStep = EnumSteps.Save;
                        await CollectionsQueueSettingsFunctions.UpdateCollectionsSystemSettings(CollectionQueueSetting, CollectionSystemDefault);
                        this.RaiseActionsWhenChangeStep(EnumScreen.CollectionSettings, EnumSteps.Save);
                        IsCheckedOut = false;
                        this.IsChanged = false;
                        SetActionCommandsAsync();
                    }
                    else
                    {
                        _CurrentEnumStep = EnumSteps.Error;
                        this.SetActionCommandsAsync();
                        this.ListErrorHyperlink = this.ListErrorHyperlink;
                        this.OnErrorHyperlinkSelected();
                    }
                    break;

                case EnumSteps.Cancel:
                    canProcess = await this.CheckIfUnSavedChanges();
                    if (canProcess)
                    {
                        this.ValidateNotError();
                        this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
                        _CurrentEnumStep = EnumSteps.Cancel;
                        CollectionQueueSetting =
                            await CollectionsQueueSettingsFunctions.ReadCollectionsSystemSettingsAsync();
                        CollectionQueueSetting.tempMinimumArrearsPercent =
                            CollectionQueueSetting.MinimumArrearsPercent.HasValue
                                ? CollectionQueueSetting.MinimumArrearsPercent.Value.ToString(
                                    CultureInfo.InvariantCulture)
                                : "0";
                        CollectionQueueSetting.tempMinimumArrearsAmount =
                            CollectionQueueSetting.MinimumArrearsAmount.HasValue
                                ? CollectionQueueSetting.MinimumArrearsAmount.Value.ToString(
                                    CultureInfo.InvariantCulture)
                                : "0";
                        CollectionQueueSetting.tempMinimumArrearsDays =
                            CollectionQueueSetting.MinimumArrearsDays.HasValue
                                ? CollectionQueueSetting.MinimumArrearsDays.Value.ToString(CultureInfo.InvariantCulture)
                                : "0";
                        CollectionSystemDefault =
                            await CollectionsQueueSettingsFunctions.ReadCollectionSystemDefaultAsync();
                        this.RaiseActionsWhenChangeStep(EnumScreen.CollectionSettings, EnumSteps.Cancel);
                        IsCheckedOut = false;
                        this.IsChanged = false;
                        SetActionCommandsAsync();
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
            switch (_CurrentEnumStep)
            {
                case EnumSteps.Start:
                case EnumSteps.Save:
                case EnumSteps.Cancel:
                    if (CanEdit)
                    {
                        ActionCommands = new ObservableCollection<ActionCommand>
                        {
                            new ActionCommand { Parameter = EnumSteps.Edit.ToString(), Command = new Edit() },
                        };
                    }
                    break;

                case EnumSteps.Edit:
                    ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = EnumSteps.Save.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = EnumSteps.Cancel.ToString(), Command = new Cancel() }
                    };
                    break;
                case EnumSteps.Error:
                    ActionCommands = new ObservableCollection<ActionCommand>
                    {
                        new ActionCommand { Parameter = EnumSteps.Save.ToString(), Command = new Save() },
                        new ActionCommand { Parameter = EnumSteps.Cancel.ToString(), Command = new Cancel() },
                        new ActionCommand { Parameter = EnumSteps.Error.ToString(), Command = new Error() }
                    };
                    break;
            }
        }

        private void CollectionsSettingViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Validate(e.PropertyName);
            if (this.ActiveViewModel != null)
            {
                if ((this.ActiveViewModel.IsCheckedOut) && (e.PropertyName.IndexOf("CollectionQueueSetting") != -1 || e.PropertyName.IndexOf("CollectionSystemDefault") != -1))
                {
                    this.IsChanged = true;
                }
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
                confirmViewModel.Title = "Confirm Save - Collection Setting";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
            }
            return canProceed;
        }

        protected override Task UnLockAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
    }
}