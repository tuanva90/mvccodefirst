// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EquipSearchViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the EquipSearchViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Insyston.Operations.WPF.ViewModels.Common.Model;

    using WPFDynamic.ViewModels.Controls;

    /// <summary>
    /// The equip search view model.
    /// </summary>
    public class EquipSearchViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The _text searching.
        /// </summary>
        private string _textSearching;

        /// <summary>
        /// The _list condition search.
        /// </summary>
        private ObservableCollection<ItemComboBox> _listConditionSearch;

        /// <summary>
        /// The _item condition search selected.
        /// </summary>
        private ItemComboBox _itemConditionSearchSelected;

        /// <summary>
        /// The _dynamic grid result search.
        /// </summary>
        private DynamicGridViewModel _dynamicGridResultSearch;

        /// <summary>
        /// The _list item result search.
        /// </summary>
        private ObservableCollection<ItemEquipSearch> _listItemResultSearch;

        /// <summary>
        /// Field for IsBusyMessage property.
        /// </summary>
        private string _isBusyMessage;

        /// <summary>
        /// Field for the IsBusy Property.
        /// </summary>
        private bool _isBusy;

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether is busy.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this._isBusy;
            }
            set
            {
                this._isBusy = value;
                this.OnPropertyChanged("IsBusy");
            }
        }

        /// <summary>
        /// Gets or sets the is loading message
        /// </summary>
        public string BusyContent
        {
            get
            {
                return this._isBusyMessage;
            }
            set
            {
                this._isBusyMessage = value;
                this.OnPropertyChanged("BusyContent");
            }
        }

        /// <summary>
        /// The on storyboard change.
        /// </summary>
        public Action<string> OnStoryboardChange;

        /// <summary>
        /// The on searching action.
        /// </summary>
        public Action<string> OnSearchingAction;

        /// <summary>
        /// The on close search action.
        /// </summary>
        public Action<string> OnCloseSearchAction;

        /// <summary>
        /// Gets or sets the previous text searching.
        /// </summary>
        public string PreviousTextSearching { get; set; }

        /// <summary>
        /// Gets or sets the previous item condition search selected.
        /// </summary>
        public ItemComboBox PreviousItemConditionSearchSelected { get; set; }

        /// <summary>
        /// Gets or sets the text searching.
        /// </summary>
        public string TextSearching
        {
            get
            {
                return this._textSearching;
            }

            set
            {
                this._textSearching = value;
                this.OnPropertyChanged("TextSearching");
            }
        }

        /// <summary>
        /// Gets or sets the list condition search.
        /// </summary>
        public ObservableCollection<ItemComboBox> ListConditionSearch
        {
            get
            {
                return this._listConditionSearch;
            }

            set
            {
                this._listConditionSearch = value;
                this.OnPropertyChanged("ListConditionSearch");
            }
        }

        /// <summary>
        /// Gets or sets the item condition search selected.
        /// </summary>
        public ItemComboBox ItemConditionSearchSelected
        {
            get
            {
                return this._itemConditionSearchSelected;
            }

            set
            {
                this._itemConditionSearchSelected = value;
                this.OnPropertyChanged("ItemConditionSearchSelected");
            }
        }

        /// <summary>
        /// Gets or sets the dynamic grid result search.
        /// </summary>
        public DynamicGridViewModel DynamicGridResultSearch
        {
            get
            {
                return this._dynamicGridResultSearch;
            }

            set
            {
                this._dynamicGridResultSearch = value;
                this.OnPropertyChanged("DynamicGridResultSearch");
            }
        }

        /// <summary>
        /// Gets or sets the list item result search.
        /// </summary>
        public ObservableCollection<ItemEquipSearch> ListItemResultSearch
        {
            get
            {
                return this._listItemResultSearch;
            }

            set
            {
                this._listItemResultSearch = value;
                this.OnPropertyChanged("ListItemResultSearch");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipSearchViewModel"/> class.
        /// </summary>
        public EquipSearchViewModel()
        {
            this.ListConditionSearch = new ObservableCollection<ItemComboBox>();
            this.ListItemResultSearch = new ObservableCollection<ItemEquipSearch>();
            this.DynamicGridResultSearch = new DynamicGridViewModel(typeof(object));
            this.BusyContent = "Please Wait Loading";
            this.IsBusy = false;
        }

        /// <summary>
        /// The generate data.
        /// </summary>
        /// <param name="listConditionSearch">
        /// The list condition search.
        /// </param>
        /// <param name="conditionSearchSelected">
        /// The condition search selected.
        /// </param>
        /// <param name="listItemResult">
        /// The list item result.
        /// </param>
        /// <param name="dynamicGridSearchResult">
        /// The dynamic Grid Search Result.
        /// </param>
        public void GenerateData(ObservableCollection<ItemComboBox> listConditionSearch, int conditionSearchSelected, ObservableCollection<ItemEquipSearch> listItemResult, DynamicGridViewModel dynamicGridSearchResult)
        {
            if (this.ListConditionSearch == null)
            {
                this.ListConditionSearch = new ObservableCollection<ItemComboBox>();
            }
            if (this.ListItemResultSearch == null)
            {
                this.ListItemResultSearch = new ObservableCollection<ItemEquipSearch>();
            }

            this.ListConditionSearch = listConditionSearch;
            this.ItemConditionSearchSelected = this.ListConditionSearch.FirstOrDefault(x => x.ItemId == conditionSearchSelected);
            this.ListItemResultSearch = listItemResult;
            this.DynamicGridResultSearch.IsSetSelectedItem = true;
            this.DynamicGridResultSearch = dynamicGridSearchResult;
        }

        /// <summary>
        /// The close button click.
        /// </summary>
        public void CloseButtonClick()
        {
            this.TextSearching = string.Empty;
            if (this.OnCloseSearchAction != null)
            {
                this.OnCloseSearchAction(this.TextSearching);
            }
            this.ChangeToSearchResult();
        }

        /// <summary>
        /// The close button click.
        /// </summary>
        public void ChangeToSearchResult()
        {
            if (this.OnStoryboardChange != null)
            {
                this.OnStoryboardChange("ResultState");
            }
        }

        /// <summary>
        /// The close button click.
        /// </summary>
        public void ChangeToSearchGrid()
        {
            if (this.OnStoryboardChange != null)
            {
                this.OnStoryboardChange("SearchState");
                this.IsBusy = false;
            }
        }

        /// <summary>
        /// The search button click.
        /// </summary>
        public void SearchButtonClick()
        {
            if (!string.IsNullOrEmpty(this.TextSearching))
            {
                IsBusy = true;
                if (this.OnSearchingAction != null)
                {
                    this.OnSearchingAction(this.TextSearching);
                }
            }
            if (this.DynamicGridResultSearch != null)
            {
                this.DynamicGridResultSearch.SelectedItem = null;
                this.DynamicGridResultSearch.SelectedRows = new List<object>();
            }
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
