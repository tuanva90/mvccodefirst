// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemChildViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the ItemChildViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.WPF.ViewModels.Common;

    /// <summary>
    /// The item child view model.
    /// </summary>
    public class ItemChildViewModel : UserControlViewModelBase, INotifyPropertyChanged
    {
        #region Private Properties

        /// <summary>
        /// The _effective month.
        /// </summary>
        private int? _effectiveMonth;

        /// <summary>
        /// The _is use category default.
        /// </summary>
        private bool _isUseCategoryDefault;

        /// <summary>
        /// The is con tract term active.
        /// </summary>
        private bool _isConTractTermActive;

        /// <summary>
        /// The _is depreciation rate active.
        /// </summary>
        private bool _isDenpRateActive;

        /// <summary>
        /// The _is effective life active.
        /// </summary>
        private bool _isEffectiveLifeActive;

        /// <summary>
        /// The _is check box all show up.
        /// </summary>
        private bool _isCheckBoxAllShowUp;

        /// <summary>
        /// The _is all use category default.
        /// </summary>
        private bool _isAllUseCategoryDefault;

        /// <summary>
        /// The _is all stop depn checked.
        /// </summary>
        private bool _isAllStopDepnChecked;

        /// <summary>
        /// The _is all depn method checked.
        /// </summary>
        private bool _isAllDepnMethodChecked;

        /// <summary>
        /// The _is all salvage checked.
        /// </summary>
        private bool _isAllSalvageChecked;

        /// <summary>
        /// The _is salvage enable.
        /// </summary>
        private bool _isSalvageEnable;

        /// <summary>
        /// The _is all effective life checked.
        /// </summary>
        private bool _isAllEffectiveLifeChecked;

        /// <summary>
        /// The _is effective life enable.
        /// </summary>
        private bool _isEffectiveLifeEnable;

        /// <summary>
        /// The _is stop depr checked.
        /// </summary>
        private bool _isStopDeprChecked;

        /// <summary>
        /// The _has salvage.
        /// </summary>
        private bool _hasSalvage;

        /// <summary>
        /// The _is depn method enable.
        /// </summary>
        private bool _isDepnMethodEnable;

        /// <summary>
        /// The _list item depn method.
        /// </summary>
        private List<AssetClassesCategoryItemDetail> _listItemDepnMethod;

        /// <summary>
        /// The _selected item combobox.
        /// </summary>
        private AssetClassesCategoryItemDetail _selectedItemCombobox;

        /// <summary>
        /// The _salvage percent.
        /// </summary>
        private double? _salvagePercent;

        /// <summary>
        /// The _depn rate percent.
        /// </summary>
        private double? _depnRatePercent;

        /// <summary>
        /// The _effective year.
        /// </summary>
        private int? _effectiveYear;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemChildViewModel"/> class.
        /// </summary>
        public ItemChildViewModel()
        {
            this.IsConTractTermActive = true;
            this.IsEffectiveLifeActive = false;
            this.IsDenpRateActive = false;
            this.IsStopDeprChecked = false;
            this.IsCheckBoxAllShowUp = false;
            this.IsAllUseCategoryDefault = true;
            this.IsAllStopDepnChecked = true;
            this.IsAllDepnMethodChecked = true;
            this.IsAllSalvageChecked = true;
            this.IsAllEffectiveLifeChecked = true;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is use category default.
        /// </summary>
        public bool IsUseCategoryDefault
        {
            get
            {
                return this._isUseCategoryDefault;
            }

            set
            {
                if (value)
                {
                    this._isEffectiveLifeEnable = false;
                    this._isSalvageEnable = false;
                    this._isDepnMethodEnable = false;

                    this._isAllDepnMethodChecked = false;
                    this._isAllEffectiveLifeChecked = false;
                    this._isAllSalvageChecked = false;
                }
                else
                {
                    this._isAllDepnMethodChecked = true;
                    this._isAllEffectiveLifeChecked = true;
                    this._isAllSalvageChecked = true;

                    this._isDepnMethodEnable = true;
                    if (this._selectedItemCombobox != null)
                    {
                        this.SelectedItemCombobox = this._selectedItemCombobox;
                    }
                }

                this._isUseCategoryDefault = value;

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("IsDepnMethodEnable"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsSalvageEnable"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsEffectiveLifeEnable"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsUseCategoryDefault"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsAllDepnMethodChecked"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsAllEffectiveLifeChecked"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsAllSalvageChecked"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is con tract term active.
        /// </summary>
        public bool IsConTractTermActive
        {
            get
            {
                return this._isConTractTermActive;
            }

            set
            {
                if (value)
                {
                    this._isEffectiveLifeActive = false;
                    this._isDenpRateActive = false;
                    this._depnRatePercent = null;
                    if (this._effectiveMonth != null || this._effectiveYear != null)
                    {
                        this._effectiveMonth = null;
                        this._effectiveYear = null;

                        this.PropertyChanged(this, new PropertyChangedEventArgs("EffectiveMonth"));
                        this.PropertyChanged(this, new PropertyChangedEventArgs("EffectiveYear"));
                    }
                }

                this._isConTractTermActive = value;

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("IsConTractTermActive"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsEffectiveLifeActive"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsDenpRateActive"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("DepnRatePercent"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is depreciation rate active.
        /// </summary>
        public bool IsDenpRateActive
        {
            get
            {
                return this._isDenpRateActive;
            }

            set
            {
                this._isDenpRateActive = value;

                if (value)
                {
                    this._isEffectiveLifeActive = false;
                    this._isConTractTermActive = false;
                    if (this.DepnRatePercent != null && !double.IsInfinity(this.DepnRatePercent.Value))
                    {
                        this.DepnRatePercent = this._depnRatePercent;
                    }
                }

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("IsDenpRateActive"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsEffectiveLifeActive"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsConTractTermActive"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is effective life active.
        /// </summary>
        public bool IsEffectiveLifeActive
        {
            get
            {
                return this._isEffectiveLifeActive;
            }

            set
            {
                if (value)
                {
                    this._isDenpRateActive = false;
                    this._isConTractTermActive = false;
                }

                this._isEffectiveLifeActive = value;

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("IsEffectiveLifeActive"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsDenpRateActive"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsConTractTermActive"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is check box all show up.
        /// </summary>
        public bool IsCheckBoxAllShowUp
        {
            get
            {
                return this._isCheckBoxAllShowUp;
            }

            set
            {
                this._isCheckBoxAllShowUp = value;

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("IsEffectiveLifeActive"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is all use category default.
        /// </summary>
        public bool IsAllUseCategoryDefault
        {
            get
            {
                return this._isAllUseCategoryDefault;
            }

            set
            {
                if (value == false)
                {
                    this._isEffectiveLifeEnable = false;
                    this._isSalvageEnable = false;
                    this._isDepnMethodEnable = false;

                    this._isAllDepnMethodChecked = false;
                    this._isAllEffectiveLifeChecked = false;
                    this._isAllSalvageChecked = false;
                    this._isUseCategoryDefault = false;
                }
                else
                {
                    this._isEffectiveLifeEnable = true;
                    this._isSalvageEnable = true;
                    this._isDepnMethodEnable = true;
                }

                this._isAllUseCategoryDefault = value;

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("IsAllUseCategoryDefault"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsUseCategoryDefault"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsEffectiveLifeEnable"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsSalvageEnable"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsDepnMethodEnable"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsAllDepnMethodChecked"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsAllEffectiveLifeChecked"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsAllSalvageChecked"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is all stop depreciation checked.
        /// </summary>
        public bool IsAllStopDepnChecked
        {
            get
            {
                return this._isAllStopDepnChecked;
            }

            set
            {
                this._isAllStopDepnChecked = value;

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("IsAllStopDepnChecked"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is all depreciation method checked.
        /// </summary>
        public bool IsAllDepnMethodChecked
        {
            get
            {
                return this._isAllDepnMethodChecked;
            }

            set
            {
                this._isAllDepnMethodChecked = value;

                if (value && this.SelectedItemCombobox != null)
                {
                    if (this.SelectedItemCombobox.ItemId == -1)
                    {
                        this._isSalvageEnable = false;
                        this._isEffectiveLifeEnable = false;
                    }
                    else
                    {
                        this._isSalvageEnable = true;
                        this._isEffectiveLifeEnable = true;
                    }
                }
                else
                {
                    this._isSalvageEnable = true;
                    this._isEffectiveLifeEnable = true;
                }

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("IsAllDepnMethodChecked"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsSalvageEnable"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsEffectiveLifeEnable"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is all salvage checked.
        /// </summary>
        public bool IsAllSalvageChecked
        {
            get
            {
                return this._isAllSalvageChecked;
            }

            set
            {
                this._isAllSalvageChecked = value;

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("IsAllSalvageChecked"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is salvage enable.
        /// </summary>
        public bool IsSalvageEnable
        {
            get
            {
                return this._isSalvageEnable;
            }

            set
            {
                this._isSalvageEnable = value;

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("IsSalvageEnable"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is all effective life checked.
        /// </summary>
        public bool IsAllEffectiveLifeChecked
        {
            get
            {
                return this._isAllEffectiveLifeChecked;
            }

            set
            {
                this._isAllEffectiveLifeChecked = value;

                if (!value)
                {
                    this.IsConTractTermActive = true;
                }

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("IsAllEffectiveLifeChecked"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is effective life enable.
        /// </summary>
        public bool IsEffectiveLifeEnable
        {
            get
            {
                return this._isEffectiveLifeEnable;
            }

            set
            {
                this._isEffectiveLifeEnable = value;

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("IsEffectiveLifeEnable"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is stop depreciation checked.
        /// </summary>
        public bool IsStopDeprChecked
        {
            get
            {
                return this._isStopDeprChecked;
            }

            set
            {
                this._isStopDeprChecked = value;

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("IsStopDeprChecked"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether has salvage.
        /// </summary>
        public bool HasSalvage
        {
            get
            {
                return this._hasSalvage;
            }

            set
            {
                this._hasSalvage = value;

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("HasSalvage"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is depreciation method enable.
        /// </summary>
        public bool IsDepnMethodEnable
        {
            get
            {
                return this._isDepnMethodEnable;
            }

            set
            {
                this._isDepnMethodEnable = value;

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("IsDepnMethodEnable"));
            }
        }

        /// <summary>
        /// Gets or sets the list item depreciation method.
        /// </summary>
        public List<AssetClassesCategoryItemDetail> ListItemDepnMethod
        {
            get
            {
                return this._listItemDepnMethod;
            }

            set
            {
                this._listItemDepnMethod = value;

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("ListItemDepnMethod"));
            }
        }

        /// <summary>
        /// Gets or sets the selected item combo box.
        /// </summary>
        public AssetClassesCategoryItemDetail SelectedItemCombobox
        {
            get
            {
                return this._selectedItemCombobox;
            }

            set
            {
                this._selectedItemCombobox = value;
                if (value.ItemId != -1)
                {
                    this._isSalvageEnable = true;
                    this._isEffectiveLifeEnable = true;

                    this.IsConTractTermActive = true;
                }
                else
                {
                    this._isSalvageEnable = false;
                    this._isEffectiveLifeEnable = false;

                    this._effectiveMonth = null;
                    this._effectiveYear = null;
                    this._depnRatePercent = null;
                }

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("SelectedItemCombobox"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsSalvageEnable"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("IsEffectiveLifeEnable"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("EffectiveMonth"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("EffectiveYear"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("DepnRatePercent"));
            }
        }

        /// <summary>
        /// Gets or sets the salvage percent.
        /// </summary>
        public double? SalvagePercent
        {
            get
            {
                return this._salvagePercent;
            }

            set
            {
                if (value != null)
                {
                    this._salvagePercent = Math.Round(value.Value, 4, MidpointRounding.AwayFromZero);
                }

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("SalvagePercent"));
            }
        }

        /// <summary>
        /// Gets or sets the depreciation rate percent.
        /// </summary>
        public double? DepnRatePercent
        {
            get
            {
                return this._depnRatePercent;
            }

            set
            {
                if (this._selectedItemCombobox.ItemId != -1 && this._isAllDepnMethodChecked)
                {
                    if (value == null)
                    {
                        this._depnRatePercent = null;
                        this._effectiveYear = null;
                        this._effectiveMonth = null;
                    }
                    else if (value > -1)
                    {
                        this._depnRatePercent = Math.Round(value.Value, 4, MidpointRounding.AwayFromZero);

                        double result;

                        result = double.Parse((100 / this._depnRatePercent).ToString());

                        this._effectiveYear = (int)result;
                        this._effectiveMonth = (int)Math.Round((result - (int)result) * 12, 0, MidpointRounding.AwayFromZero);

                        if (this._effectiveYear == int.MinValue || this._effectiveYear == int.MaxValue)
                        {
                            this._effectiveYear = 0;
                        }

                        if (this._effectiveMonth == int.MinValue || this._effectiveMonth == int.MaxValue)
                        {
                            this._effectiveMonth = 0;
                        }
                    }
                }

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("DepnRatePercent"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("EffectiveYear"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("EffectiveMonth"));
            }
        }

        /// <summary>
        /// Gets or sets the effective year.
        /// </summary>
        public int? EffectiveYear
        {
            get
            {
                return this._effectiveYear;
            }

            set
            {
                if (this._selectedItemCombobox.ItemId != -1 && this._isAllDepnMethodChecked)
                {
                    if (value == null)
                    {
                        this._effectiveYear = null;
                        if (this._effectiveMonth == null)
                        {
                            this._depnRatePercent = null;
                        }
                        else
                        {
                            this.EffectiveYear = 0;
                        }
                    }
                    else if (value == 0 && (this._effectiveMonth == 0 || this._effectiveMonth == null))
                    {
                        this._effectiveYear = 0;
                        this._depnRatePercent = 0;
                        this._effectiveMonth = 0;
                    }
                    else if (value == int.MinValue || value == int.MaxValue)
                    {
                        this._effectiveYear = 0;
                    }
                    else
                    {
                        if (this.EffectiveMonth == null)
                        {
                            this._effectiveMonth = 0;
                        }

                        if (value > -1)
                        {
                            double years = double.Parse(value.ToString());
                            if (this._effectiveMonth != null)
                            {
                                years += double.Parse(((double)this._effectiveMonth / 12).ToString());
                            }

                            this._depnRatePercent = 100 / years;
                        }

                        this._effectiveYear = value;
                    }                   
                }

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("EffectiveYear"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("EffectiveMonth"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("DepnRatePercent"));
            }
        }

        /// <summary>
        /// Gets or sets the effective month.
        /// </summary>
        public int? EffectiveMonth
        {
            get
            {
                return this._effectiveMonth;
            }

            set
            {
                if (this._selectedItemCombobox.ItemId != -1 && this._isAllDepnMethodChecked)
                {
                    if (value == null)
                    {
                        this._effectiveMonth = null;
                        if (this._effectiveYear == null)
                        {
                            this._depnRatePercent = null;
                        }
                        else
                        {
                            this.EffectiveMonth = 0;
                        }
                    }
                    else if (value == 0 && (this._effectiveYear == 0 || this._effectiveYear == null))
                    {
                        this._effectiveMonth = 0;
                        this._depnRatePercent = 0;
                        this._effectiveYear = 0;
                    }
                    else if (value == int.MinValue || value == int.MaxValue)
                    {
                        this._effectiveMonth = 0;
                    }
                    else
                    {
                        if (this.EffectiveYear == null)
                        {
                            this._effectiveYear = 0;
                        }

                        if (value > -1)
                        {
                            if (value > 12)
                            {
                                if (this._effectiveYear == int.MinValue || this._effectiveYear == int.MaxValue)
                                {
                                    this._effectiveYear = value / 12;
                                }
                                else
                                {
                                    this._effectiveYear += value / 12;
                                }

                                this._effectiveMonth = value - ((int)(value / 12) * 12);
                            }
                            else
                            {
                                this._effectiveMonth = value;
                            }

                            double years = double.Parse(((double)this._effectiveMonth / 12).ToString());

                            if (this._effectiveYear != null)
                            {
                                years += double.Parse(this._effectiveYear.ToString());
                            }

                            this._depnRatePercent = 100 / years;
                        }
                    }
                }

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("EffectiveMonth"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("EffectiveYear"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("DepnRatePercent"));
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// The set value default for all.
        /// </summary>
        /// <param name="depnMethodId">
        /// The depreciation method id.
        /// </param>
        /// <param name="salvagePercent">
        /// The salvage percent.
        /// </param>
        /// <param name="effectiveLifeMonth">
        /// The effective life month.
        /// </param>
        /// <param name="effectiveLifeYear">
        /// The effective life year.
        /// </param>
        /// <param name="ratePercent">
        /// The rate percent.
        /// </param>
        public void SetValueDefaultForAll(int depnMethodId, double salvagePercent, int? effectiveLifeMonth, int? effectiveLifeYear, double? ratePercent)
        {
            this.IsConTractTermActive = true;
            this._selectedItemCombobox =
                    this.ListItemDepnMethod.FirstOrDefault(x => x.ItemId == depnMethodId) != null
                        ? this.ListItemDepnMethod.FirstOrDefault(x => x.ItemId == depnMethodId)
                        : this.ListItemDepnMethod.FirstOrDefault(x => x.ItemId == -1);
            this._salvagePercent = salvagePercent;
            this._effectiveMonth = effectiveLifeMonth;
            this._effectiveYear = effectiveLifeYear;
            this._depnRatePercent = ratePercent;

            if (this.PropertyChanged == null)
            {
                return;
            }

            this.PropertyChanged(this, new PropertyChangedEventArgs("IsConTractTermActive"));
            this.PropertyChanged(this, new PropertyChangedEventArgs("SelectedItemCombobox"));
            this.PropertyChanged(this, new PropertyChangedEventArgs("EffectiveMonth"));
            this.PropertyChanged(this, new PropertyChangedEventArgs("EffectiveYear"));
            this.PropertyChanged(this, new PropertyChangedEventArgs("DepnRatePercent"));
            this.PropertyChanged(this, new PropertyChangedEventArgs("SalvagePercent"));
        }
        #endregion
    }
}
