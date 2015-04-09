// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetSettingsViewModel.cs" company="Insyston">
// Insyston
// </copyright>
// <summary>
//   The asset settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Business.Common.Model;
    using Insyston.Operations.Bussiness.Assets;
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModel.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Assets.Validation;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    using Brush = System.Windows.Media.Brush;

    /// <summary>
    /// The asset settings view model.
    /// </summary>
    public class AssetSettingsViewModel : ViewModelUseCaseBase
    {
        #region Private Property

        /// <summary>
        /// The _ current enum step.
        /// </summary>
        private EnumSteps _CurrentEnumStep;

        /// <summary>
        /// The _ category.
        /// </summary>
        private List<DropdownList> _Category;

        /// <summary>
        /// The _ asset settings.
        /// </summary>
        private AssetClassSetting _AssetSettings;

        private DropdownList _selectedCategory;

        private string _currentCategory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetSettingsViewModel"/> class.
        /// </summary>
        public AssetSettingsViewModel()
        {
            this.Validator = new AssetSettingsViewModelValidation();
            this.SelectedCategory = new DropdownList();
            this.PropertyChanged += this.AssetSettingViewModel_PropertyChanged;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// The steps.
        /// </summary>
        public enum EnumSteps
        {
            /// <summary>
            /// The start.
            /// </summary>
            Start,

            /// <summary>
            /// The edit.
            /// </summary>
            Edit,

            /// <summary>
            /// The save.
            /// </summary>
            Save,

            /// <summary>
            /// The cancel.
            /// </summary>
            Cancel,

            /// <summary>
            /// The error.
            /// </summary>
            Error,

            /// <summary>
            /// The none.
            /// </summary>
            None
        }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public List<DropdownList> Category
        {
            get
            {
                return this._Category;
            }

            set
            {
                this.SetField(ref this._Category, value, () => this.Category);
            }
        }

        /// <summary>
        /// Gets or sets the selected category.
        /// </summary>
        public DropdownList SelectedCategory
        {
            get
            {
                return this._selectedCategory;
            }

            set
            {
                this.SetField(ref this._selectedCategory, value, () => this.SelectedCategory);
            }
        }

        /// <summary>
        /// Gets or sets the current category.
        /// </summary>
        public string CurrentCategory
        {
            get
            {
                return this._currentCategory;
            }

            set
            {
                this.SetField(ref this._currentCategory, value, () => this.CurrentCategory);
            }
        }

        /// <summary>
        /// Gets or sets the asset settings.
        /// </summary>
        public AssetClassSetting AssetSettings
        {
            get
            {
                return this._AssetSettings;
            }

            set
            {
                this.SetField(ref this._AssetSettings, value, () => this.AssetSettings);
            }
        }
        #endregion

        #region Public Method 
        /// <summary>
        /// The on step async.
        /// </summary>
        /// <param name="stepName">
        /// The step name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override async Task OnStepAsync(object stepName)
        {
            bool canProcess;
            var step = (EnumSteps)Enum.Parse(typeof(EnumSteps), stepName.ToString());
            switch (step)
            {
                case EnumSteps.Start:
                    this._CurrentEnumStep = EnumSteps.Start;
                    this.AssetSettings = await AssetSettingsFunctions.ReadAssetClassSettings();
                    var allCategoryDefault = new ObservableCollection<EquipCategory>(await AssetSettingsFunctions.ReadAllCategoryDefaultAssetSystemSettingsAsync());
                    allCategoryDefault.Add(new EquipCategory
                    {
                        EquipCatId = -1,
                        Description = "<None>",
                    });
                    this.Category =
                    allCategoryDefault.Select(
                    cate => new DropdownList { Description = cate.Description, ID = cate.EquipCatId })
                    .OrderBy(item => item.Description)
                    .ToList();
                    if (this.AssetSettings != null)
                    {
                        this.SelectedCategory = this.Category.FirstOrDefault(x => x.ID == this.AssetSettings.DefaultCategoryID);
                    }

                    this.SetBackgroundToNotEdit();
                    this.SetActionCommandsAsync();
                    break;
                case EnumSteps.Edit:
                    if (await this.LockAsync())
                    {
                        this.SetBackgroundToEdit();
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetSettings, EnumSteps.Edit);
                        this._CurrentEnumStep = EnumSteps.Edit;
                        this.SetActionCommandsAsync();
                    }

                    break;
                case EnumSteps.Save:
                    this.Validate();
                    if (this.HasErrors == false)
                    {
                        this.ValidateNotError();
                        this.ClearNotifyErrors();
                        await this.UnLockAsync();
                        this.SetBackgroundToNotEdit();
                        if (this.SelectedCategory != null)
                        {
                            this.AssetSettings.DefaultCategoryID = this.SelectedCategory.ID;
                        }
                        else
                        {
                            await this.CheckCategoryIsEnalbe(this.CurrentCategory);
                            this.AssetSettings.DefaultCategoryID = this.SelectedCategory.ID;
                        }

                        if (!this.AssetSettings.IncludeMake)
                        {
                            this.AssetSettings.IncludeModel = false;
                        }

                        await AssetSettingsFunctions.UpdateAssetClassSettings(this.AssetSettings);
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetSettings, EnumSteps.Save);
                        this._CurrentEnumStep = EnumSteps.Save;
                        await this.OnStepAsync(EnumSteps.Start);
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
                        this.ValidateNotError();
                        this.ClearNotifyErrors();
                        await this.UnLockAsync();
                        this.SetBackgroundToNotEdit();
                        this._CurrentEnumStep = EnumSteps.Cancel;
                        await this.OnStepAsync(EnumSteps.Start);
                        this.RaiseActionsWhenChangeStep(EnumScreen.AssetSettings, EnumSteps.Cancel);
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

        /// <summary>
        /// The check if un saved changes.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> CheckIfUnSavedChanges()
        {
            bool canProceed = true;
            if (this.IsChanged)
            {
                ConfirmationWindowView confirm = new ConfirmationWindowView();
                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                confirmViewModel.Title = "Confirm Save - Asset Settings";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
            }

            return canProceed;
        }

        /// <summary>
        /// The unlock item.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task UnlockItem()
        {
            return this.UnLockAsync();
        }

        /// <summary>
        /// The check content is editing.
        /// Calling from DocumentTab
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
        #endregion

        #region Protected Method
        /// <summary>
        /// The set action commands async.
        /// </summary>
        protected override void SetActionCommandsAsync()
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

        /// <summary>
        /// The un lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task UnLockAsync()
        {
            await this.UnLockAsync("AssetClassSettings", this.AssetSettings.ID.ToString());
        }

        /// <summary>
        /// The lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task<bool> LockAsync()
        {
            if (!(await this.LockAsync("AssetClassSettings", this.AssetSettings.ID.ToString())))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Private Method

        /// <summary>
        /// The check category is enalbe.
        /// </summary>
        /// <param name="desc">
        /// The desc.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<DropdownList> CheckCategoryIsEnalbe(string desc)
        {
            var allCategoryDefault = new ObservableCollection<EquipCategory>(await AssetSettingsFunctions.ReadAllCategoryDefaultAssetSystemSettingsAsync());
            allCategoryDefault.Add(new EquipCategory
            {
                EquipCatId = -1,
                Description = "<None>",
            });
            this.Category =
                allCategoryDefault.Select(
                    cate => new DropdownList { Description = cate.Description, ID = cate.EquipCatId })
                    .OrderBy(item => item.Description)
                    .ToList();
            return this.SelectedCategory = this.Category.FirstOrDefault(x => x.Description.Equals(desc));
        }

        /// <summary>
        /// The asset setting view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetSettingViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.ActiveViewModel != null)
            {
                if (this.ActiveViewModel.IsCheckedOut && (e.PropertyName.IndexOf("AssetSettings", StringComparison.Ordinal) != -1) ||
                    this.ActiveViewModel.IsCheckedOut && (e.PropertyName.IndexOf("SelectedCategory", StringComparison.Ordinal) != -1) ||
                    this.ActiveViewModel.IsCheckedOut && (e.PropertyName.IndexOf("AssetSettings.IncludeMake", StringComparison.Ordinal) != -1) ||
                    this.ActiveViewModel.IsCheckedOut && (e.PropertyName.IndexOf("AssetSettings.IncludeMake", StringComparison.Ordinal) != -1) ||
                    this.ActiveViewModel.IsCheckedOut && (e.PropertyName.IndexOf("AssetSettings.DepreciateInInventory", StringComparison.Ordinal) != -1))
                {
                    this.IsChanged = true;
                }
            }
        }

        /// <summary>
        /// The set background to edit.
        /// </summary>
        private void SetBackgroundToEdit()
        {
            this.GridStyle = (Brush)Application.Current.FindResource("GridStyleEdit");
            this.IsCheckedOut = true;
        }

        /// <summary>
        /// The set background to not edit.
        /// </summary>
        private void SetBackgroundToNotEdit()
        {
            this.GridStyle = (Brush)Application.Current.FindResource("GridStyleNotEdit");
            this.IsCheckedOut = false;
            this.IsChanged = false;
        }
        #endregion
    }
}
