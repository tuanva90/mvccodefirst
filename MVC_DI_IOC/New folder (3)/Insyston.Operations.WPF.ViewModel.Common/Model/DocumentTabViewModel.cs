// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentTabViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The document tab view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Model
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    /// <summary>
    /// The document tab view model.
    /// </summary>
    public class DocumentTabViewModel : ViewModelUseCaseBase
    {
        /// <summary>
        /// The tab item add action.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        public delegate void TabItemAddAction(object sender);

        /// <summary>
        /// The tab item add click.
        /// </summary>
        public TabItemAddAction TabItemAddClick;

        /// <summary>
        /// The _list tab items.
        /// </summary>
        private ObservableCollection<CustomTabItem> _listTabItems;

        /// <summary>
        /// The tab selection changed.
        /// </summary>
        public Action<object> TabSelectionChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentTabViewModel"/> class.
        /// </summary>
        public DocumentTabViewModel()
        {
            this._listTabItems = new ObservableCollection<CustomTabItem>();

            CustomTabItem tabItemAdd = new CustomTabItem();

            // Create Tab '+' 
            tabItemAdd.Style = (Style)Application.Current.FindResource("TabItemAddStyle");
            tabItemAdd.Header = "+";
            tabItemAdd.MouseDown += this.TabOnMouseDown;
            this._listTabItems.Add(tabItemAdd);
        }

        /// <summary>
        /// Gets or sets a value indicating whether is content editing.
        /// </summary>
        public bool IsContentEditing { get; set; }

        /// <summary>
        /// Gets or sets the list tab items.
        /// </summary>
        public ObservableCollection<CustomTabItem> ListTabItems
        {
            get
            {
                return this._listTabItems;
            }
            set
            {
                this.SetField(ref this._listTabItems, value, () => ListTabItems);
            }
        }

        /// <summary>
        /// The tab on mouse down event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="mouseButtonEventArgs">
        /// The mouse button event args.
        /// </param>
        private void TabOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            CustomTabItem tabItem = sender as CustomTabItem;
            if (tabItem != null)
            {
                // Change style MainMenu Button
                if (TabSelectionChanged != null)
                {
                    this.TabSelectionChanged(tabItem.Header);
                }

                tabItem.IsSelected = true;

                if (tabItem.Header.ToString().Equals("+"))
                {
                    this.TabItemAddClick("+");
                }
            } 
        }

        /// <summary>
        /// The validate content is editing or not.
        /// </summary>
        /// <param name="tabItem">
        /// The tab item.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ValidateContentClose(CustomTabItem tabItem)
        {
            bool isContentEditing = false;

            bool isChecked = false;

            if (tabItem.ItemvalidateTabContent != null)
            {
                foreach (var tabContentEdit in tabItem.ItemvalidateTabContent)
                {
                    if (!isChecked)
                    {
                        if (tabContentEdit.CheckContentEdit != null)
                        {
                            isContentEditing = tabContentEdit.CheckContentEdit().Result;
                            if (isContentEditing)
                            {
                                isChecked = true;
                            }
                        }
                    }
                }

                // Show ConfirmationWindow when content is editing
                if (isContentEditing)
                {
                    ConfirmationWindowView confirm = new ConfirmationWindowView();
                    ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                    confirmViewModel.Content = "Changes have not been saved. Click OK to proceed without saving changes.";
                    confirmViewModel.Title = "Confirm Save - " + tabItem.Header;
                    confirm.DataContext = confirmViewModel;

                    confirm.ShowDialog();
                    if (confirm.DialogResult == false)
                    {
                        return 0;
                    }

                    return 1;
                }
            }
            
            return 2;
        }

        /// <summary>
        /// The add tab item with content.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="tabItem">
        /// The tab Item.
        /// </param>
        /// <returns>
        /// The <see cref="CustomTabItem"/>.
        /// </returns>
        public CustomTabItem AddTabItemWithContent(string name, CustomTabItem tabItem)
        {
            if (name.Equals("+"))
            {
                name = "Home";
            }
            else
            {
                // Check if tab Configuration exist or not
                foreach (var customTabItem in _listTabItems)
                {
                    if (customTabItem.Header.Equals("Configuration"))
                    {
                        if (customTabItem.IsSelected)
                        {
                            // Change content when Configuration tab is selected.
                            this.ChangeContentSelectedTab(name, tabItem);
                        }
                        else
                        {
                            // Select Configuration tab if don't focus.
                            customTabItem.IsSelected = true;

                            // Change style MainMenu Button
                            if (TabSelectionChanged != null)
                            {
                                this.TabSelectionChanged(tabItem.Header);
                            }
                        }
                        return null;
                    }

                }
            }

            int count = this._listTabItems.Count;

            tabItem.Name = string.Format(name + "{0}", count);
            tabItem.Header = name;
            tabItem.Style = (Style)Application.Current.FindResource("TabItemStyle");
            tabItem.MouseDown += TabOnMouseDown;
            tabItem.IsSelected = true;

            this._listTabItems.Insert(count - 1, tabItem);
            this.OnPropertyChanged(() => ListTabItems);

            // Change style MainMenu Button
            if (TabSelectionChanged != null)
            {
                this.TabSelectionChanged(tabItem.Header);
            }
            return tabItem;
        }

        /// <summary>
        /// The change content selected tab.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="formContent">
        /// The form content.
        /// </param>
        /// <returns>
        /// The <see cref="CustomTabItem"/>.
        /// </returns>
        public CustomTabItem ChangeContentSelectedTab(string name, CustomTabItem formContent)
        {
            if (name.Equals("CollectionSettings") || name.Equals("SecuritySetting") || name.Equals("ColletionQueues") || name.Equals("ConfigurationMenu")
                || name.Equals("AssetClasses") || name.Equals("AssetCollateralClasses") || name.Equals("AssetFeatures") || name.Equals("AssetSetting")||name.Equals("AssetRegister"))
            {
                name = "Configuration";
            }
            foreach (var tabItem in this._listTabItems)
            {
                if (tabItem.IsSelected)
                {
                    int canProceed = this.ValidateContentClose(tabItem);

                    if (canProceed == 1 || canProceed == 2)
                    {
                        foreach (var itemValidate in tabItem.ItemvalidateTabContent)
                        {
                            if (itemValidate.DoUnLockAsync != null)
                            {
                                itemValidate.DoUnLockAsync();
                            }
                        }

                        tabItem.ItemvalidateTabContent.Clear();
                        int count = this._listTabItems.Count;

                        //tabItem.Name = string.Format(name + "{0}", count);
                        tabItem.Header = name;
                        tabItem.Content = formContent.Content;
                    }

                    // Change style MainMenu Button
                    if (TabSelectionChanged != null)
                    {
                        this.TabSelectionChanged(tabItem.Header);
                    }
                    return tabItem;
                }
            }
            return null;
        }

        /// <summary>
        /// The _delete tab item command.
        /// </summary>
        private ICommand _deleteTabItemCommand;

        /// <summary>
        /// Gets or sets the delete tab item command.
        /// </summary>
        public ICommand DeleteTabItemCommand
        {
            get
            {
                if (this._deleteTabItemCommand == null)
                {
                    this._deleteTabItemCommand = new RelayCommand(this.DeleteTabItem);
                }
                return this._deleteTabItemCommand;
            }
            set
            {
                this._deleteTabItemCommand = value;
            }
        }

        /// <summary>
        /// The delete tab item.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        public void DeleteTabItem(object sender)
        {
            CustomTabItem tabItem = sender as CustomTabItem;

            if (tabItem != null)
            {
                if (this._listTabItems.Count > 2)
                {
                    int canProceed = this.ValidateContentClose(tabItem);

                    if (canProceed == 1 || canProceed == 2)
                    {
                        if (tabItem.IsSelected)
                        {
                            var firstOrDefault = this._listTabItems.FirstOrDefault();
                            if (firstOrDefault != null)
                            {
                                firstOrDefault.IsSelected = true;
                            }
                        }

                        this._listTabItems.Remove(tabItem);

                        foreach (var tab in _listTabItems)
                        {
                            if (tab.IsSelected)
                            {
                                // Change style MainMenu Button
                                if (TabSelectionChanged != null)
                                {
                                    this.TabSelectionChanged(tab.Header);
                                }
                                break;
                            }
                        }
                        this.OnPropertyChanged(() => this.ListTabItems);
                    }
                }
            }
        }

        /// <summary>
        /// The un lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override Task UnLockAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
    }
}
