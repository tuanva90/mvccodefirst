// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisteredAssetDetailViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   The registered asset detail view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.RegisteredAsset.RegisteredAssetDetail
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.Business.Common.Enums;
    using Insyston.Operations.Bussiness.Assets;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.Bussiness.RegisteredAsset;
    using Insyston.Operations.Bussiness.RegisteredAsset.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.Security;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView;
    using Insyston.Operations.WPF.ViewModels.RegisteredAsset.Helpers;
    using Insyston.Operations.WPF.ViewModels.RegisteredAsset.Validation;

    using WPFDynamic.ViewModels.Controls;

    /// <summary>
    /// The registered asset detail view model.
    /// </summary>
    public class RegisteredAssetDetailViewModel : ViewModelUseCaseBase
    {
        #region Variables

        /// <summary>
        /// The _asset id.
        /// </summary>
        //private int _assetId;

        /// <summary>
        /// The _asset status.
        /// </summary>
        private string _assetStatus;

        /// <summary>
        /// The _asset state.
        /// </summary>
        private string _assetState;

        /// <summary>
        /// The _asset acquisition date.
        /// </summary>
        private DateTime _assetAcquisitionDate;

        /// <summary>
        /// The _net asset cost.
        /// </summary>
        private decimal _netAssetCost;

        /// <summary>
        /// The _asset po number.
        /// </summary>
        private string _assetPONumber;

        /// <summary>
        /// The _asset gst.
        /// </summary>
        private decimal _assetGST;

        /// <summary>
        /// The _total asset cost.
        /// </summary>
        private decimal _totalAssetCost;

        /// <summary>
        /// The _asset description.
        /// </summary>
        private string _assetDescription;

        /// <summary>
        /// The _asset serial num.
        /// </summary>
        private string _assetSerialNum;

        /// <summary>
        /// The _asset annexure.
        /// </summary>
        private string _assetAnnexure;

        /// <summary>
        /// The _asset contract num.
        /// </summary>
        private Nullable<int> _assetContractNum;

        /// <summary>
        /// The _select repor companyt default.
        /// </summary>
        private AssetRelationRowItem _selectReporCompanytDefault;

        /// <summary>
        /// The _list entity relation.
        /// </summary>
        private ObservableCollection<AssetRelationRowItem> _listEntityRelation;

        /// <summary>
        /// The _list asset registers.
        /// </summary>
        private ObservableCollection<AssetRegisterRowItem> _listAssetRegisters;

        /// <summary>
        /// The _select register default.
        /// </summary>
        private AssetRegisterRowItem _selectRegisterDefault;

        /// <summary>
        /// The _list locations.
        /// </summary>
        private ObservableCollection<AssetRegisterLocationRowItem> _listLocations;

        /// <summary>
        /// The _select location default.
        /// </summary>
        private AssetRegisterLocationRowItem _selectLocationDefault;

        /// <summary>
        /// The _list asset supplier.
        /// </summary>
        private ObservableCollection<AssetRelationRowItem> _listAssetSupplier;

        /// <summary>
        /// The _select supplier default.
        /// </summary>
        private AssetRelationRowItem _selectSupplierDefault;

        /// <summary>
        /// The _list asset sub status.
        /// </summary>
        private ObservableCollection<SystemParam> _listAssetSubStatus;

        /// <summary>
        /// The _select sub default.
        /// </summary>
        private SystemParam _selectSubDefault;

        /// <summary>
        /// The _search tool view model.
        /// </summary>
        private EquipSearchViewModel _searchToolViewModel;

        /// <summary>
        /// The _report company name.
        /// </summary>
        private string _reportCompanyName;

        /// <summary>
        /// The _is transfer.
        /// </summary>
        private bool _isAssetRegisterEnable;

        /// <summary>
        /// The _is reporting company enable.
        /// </summary>
        private bool _isReportingCompanyEnable;

        /// <summary>
        /// The _is effective date enable.
        /// </summary>
        private bool _isEffectiveDateEnable;

        /// <summary>
        /// The _is acquisition date enable.
        /// </summary>
        private bool _isAcquisitionDateEnable;

        /// <summary>
        /// The _is net asset cost enable.
        /// </summary>
        private bool _isNetAssetCostEnable;

        /// <summary>
        /// The _is asset gst enable.
        /// </summary>
        private bool _isAssetGSTEnable;

        /// <summary>
        /// The _is location enable.
        /// </summary>
        private bool _isLocationEnable;

        /// <summary>
        /// The _is control enable.
        /// </summary>
        private bool _isControlEnable;

        /// <summary>
        /// The _is status inactive.
        /// </summary>
        private bool _isStateInactive;

        /// <summary>
        /// The _is transfer mode.
        /// </summary>
        private bool _isTransferMode;

        /// <summary>
        /// The _is status active.
        /// </summary>
        private bool _isStateActive;

        /// <summary>
        /// The _is state terminated.
        /// </summary>
        private bool _isStateTerminated;

        /// <summary>
        /// The _is status return.
        /// </summary>
        private bool _isStatusReturn;

        /// <summary>
        /// The _is status idle.
        /// </summary>
        private bool _isStatusIdle;

        /// <summary>
        /// The _is status live.
        /// </summary>
        private bool _isStatusLive;

        /// <summary>
        /// The _is status reserve.
        /// </summary>
        private bool _isStatusReserve;

        /// <summary>
        /// The _dynamic combo box report company.
        /// </summary>
        private DynamicCheckComboBoxViewModel _dynamicComboBoxReportCompany;

        /// <summary>
        /// The _dynamic combo box location.
        /// </summary>
        private DynamicCheckComboBoxViewModel _dynamicComboBoxLocation;

        /// <summary>
        /// The _is content enable.
        /// </summary>
        public bool _isContentEnable;

        /// <summary>
        /// The _dynamic registered asset feature view model.
        /// </summary>
        private DynamicGridViewModel _dynamicRegisteredAssetFeatureViewModel;

        /// <summary>
        /// The _asset effective date.
        /// </summary>
        private DateTime? _assetEffectiveDate;

        /// <summary>
        /// The all registered feature.
        /// </summary>
        private ObservableCollection<RegisteredAssetFeatureRowItem> _allRegisteredFeature;

        /// <summary>
        /// The _register asset id.
        /// </summary>
        private int _registerAssetId;

        /// <summary>
        /// The _is gl module key.
        /// </summary>
        private bool _isGlModuleKey;

        /// <summary>
        /// The _is in active asset.
        /// </summary>
        private bool _isInActiveAsset;

        #endregion

        #region Contructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredAssetDetailViewModel"/> class.
        /// </summary>
        public RegisteredAssetDetailViewModel()
        {
            this.SearchToolViewModel = new EquipSearchViewModel();
            this.DynamicComboBoxReportCompany = new DynamicCheckComboBoxViewModel();
            this.DynamicComboBoxLocation = new DynamicCheckComboBoxViewModel();
            this.SearchToolViewModel.OnSearchingAction = this.OnSearchingAction;
            this.SearchToolViewModel.OnCloseSearchAction = this.OnCloseSearchAction;
            this.PropertyChanged += this.ReigsteredAssetDetailViewModel_PropertyChanged;
            this.ResetDefaultEnable();
            this.DynamicRegisteredAssetFeatureViewModel = new DynamicGridViewModel(typeof(RegisteredAssetFeatureRowItem));
            this.AllRegisteredFeature = new ObservableCollection<RegisteredAssetFeatureRowItem>();
            this.IsContentEnable = true;
            this.IsAcquisitionDateChange = false;
            this.IsTotalAssetCostChange = false;
            this.Validator = new RegisteredAssetDetailViewModelValidate();
            this.IsInActiveAsset = false;
        }

        #endregion

        #region Property

        /// <summary>
        /// The asset setting make model.
        /// </summary>
        public AssetClassSetting AssetSettingMakeModel;

        /// <summary>
        /// Gets or sets a value indicating whether is acquisition date change.
        /// </summary>
        public bool IsAcquisitionDateChange { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is acquisition date change.
        /// </summary>
        public bool IsTotalAssetCostChange { get; set; }

        /// <summary>
        /// Gets or sets the register asset id.
        /// </summary>
        public int RegisterAssetId
        {
            get
            {
                return this._registerAssetId;
            }

            set
            {
                this.SetField(ref this._registerAssetId, value, () => this.RegisterAssetId);
            }
        }

        /// <summary>
        /// Gets or sets the state id.
        /// </summary>
        public int StateId { get; set; }

        /// <summary>
        /// Gets or sets the status id.
        /// </summary>
        public int StatusId { get; set; }

        /// <summary>
        /// Gets or sets the selected registered asset.
        /// </summary>
        public RegisteredAssetRowItem SelectedRegisteredAsset { get; set; }

        /// <summary>
        /// The on properties changed detail.
        /// </summary>
        public Action<object, PropertyChangedEventArgs> OnPropertiesChangedDetail;

        /// <summary>
        /// Gets or sets the current state status.
        /// </summary>
        public EnumStateAndStatus CurrentStateStatus = EnumStateAndStatus.None;

        /// <summary>
        /// Gets or sets the current search result item.
        /// </summary>
        public EquipSearchRowItem CurrentSearchResultItem { get; set; }

        /// <summary>
        /// The is change category depreciation.
        /// </summary>
        public bool IsChangeBookDepreciation = false;

        /// <summary>
        /// Gets or sets the asset status.
        /// </summary>
        public string AssetStatus
        {
            get
            {
                return this._assetStatus;
            }

            set
            {
                this.SetField(ref this._assetStatus, value, () => this.AssetStatus);
            }
        }

        /// <summary>
        /// Gets or sets the asset state.
        /// </summary>
        public string AssetState
        {
            get
            {
                return this._assetState;
            }

            set
            {
                this.SetField(ref this._assetState, value, () => this.AssetState);
            }
        }

        /// <summary>
        /// Gets or sets the asset acquisition date.
        /// </summary>
        public DateTime AssetAcquisitionDate
        {
            get
            {
                return this._assetAcquisitionDate;
            }

            set
            {
                this.SetField(ref this._assetAcquisitionDate, value, () => this.AssetAcquisitionDate);
            }
        }

        /// <summary>
        /// Gets or sets the net asset cost.
        /// </summary>
        public decimal NetAssetCost
        {
            get
            {
                return this._netAssetCost;
            }

            set
            {
                this.SetField(ref this._netAssetCost, value, () => this.NetAssetCost);
            }
        }

        /// <summary>
        /// Gets or sets the asset po number.
        /// </summary>
        public string AssetPoNumber
        {
            get
            {
                return this._assetPONumber;
            }

            set
            {
                this.SetField(ref this._assetPONumber, value, () => this.AssetPoNumber);
            }
        }

        /// <summary>
        /// Gets or sets the asset.
        /// </summary>
        public decimal AssetGst
        {
            get
            {
                return this._assetGST;
            }

            set
            {
                this.SetField(ref this._assetGST, value, () => this.AssetGst);
            }
        }

        /// <summary>
        /// Gets or sets the total asset cost.
        /// </summary>
        public decimal TotalAssetCost
        {
            get
            {
                return this._totalAssetCost;
            }

            set
            {
                this.SetField(ref this._totalAssetCost, value, () => this.TotalAssetCost);
            }
        }

        /// <summary>
        /// Gets or sets the asset description.
        /// </summary>
        public string AssetDescription
        {
            get
            {
                return this._assetDescription;
            }

            set
            {
                this.SetField(ref this._assetDescription, value, () => this.AssetDescription);
            }
        }

        /// <summary>
        /// Gets or sets the asset serial numerable.
        /// </summary>
        public string AssetSerialNum
        {
            get
            {
                return this._assetSerialNum;
            }

            set
            {
                if (this.IsCheckedOut)
                {
                    int typeId = this.GetSearchResultItems().EquipTypeId;
                    if (typeId != 0)
                    {
                        if (RegisteredAssetFunction.IsCollateralValid(typeId))
                        {
                            this.SerialNumberChanged(value);
                        }
                        else
                        {
                            this.SetField(ref this._assetSerialNum, value, () => this.AssetSerialNum);
                        }
                    }
                    else
                    {
                        this.SetField(ref this._assetSerialNum, value, () => this.AssetSerialNum);
                    }                   
                }
                else
                {
                    this.SetField(ref this._assetSerialNum, value, () => this.AssetSerialNum);
                }
            }
        }

        /// <summary>
        /// Gets or sets the asset annexure.
        /// </summary>
        public string AssetAnnexure
        {
            get
            {
                return this._assetAnnexure;
            }

            set
            {
                this.SetField(ref this._assetAnnexure, value, () => this.AssetAnnexure);
            }
        }

        /// <summary>
        /// Gets or sets the asset contract numerable.
        /// </summary>
        public Nullable<int> AssetContractNum
        {
            get
            {
                return this._assetContractNum;
            }

            set
            {
                this.SetField(ref this._assetContractNum, value, () => this.AssetContractNum);
            }
        }

        /// <summary>
        /// Gets or sets the select default.
        /// </summary>
        public AssetRelationRowItem SelectReporCompanytDefault
        {
            get
            {
                return this._selectReporCompanytDefault;
            }

            set
            {
                this.SetField(ref this._selectReporCompanytDefault, value, () => this.SelectReporCompanytDefault);
            }
        }

        /// <summary>
        /// Gets or sets the list entity relation.
        /// </summary>
        public ObservableCollection<AssetRelationRowItem> ListEntityRelation
        {
            get
            {
                return this._listEntityRelation;
            }

            set
            {
                this.SetField(ref this._listEntityRelation, value, () => this.ListEntityRelation);
            }
        }

        /// <summary>
        /// Gets or sets the selected register.
        /// </summary>
        public AssetRegister SelectedRegister { get; set; }

        /// <summary>
        /// Gets or sets the list entity relation.
        /// </summary>
        public ObservableCollection<AssetRegisterRowItem> ListAssetRegisters
        {
            get
            {
                return this._listAssetRegisters;
            }

            set
            {
                this.SetField(ref this._listAssetRegisters, value, () => this.ListAssetRegisters);
            }
        }

        /// <summary>
        /// Gets or sets the select register default.
        /// </summary>
        public AssetRegisterRowItem SelectRegisterDefault
        {
            get
            {
                return this._selectRegisterDefault;
            }

            set
            {
                this.SetField(ref this._selectRegisterDefault, value, () => this.SelectRegisterDefault);
            }
        }

        /// <summary>
        /// Gets or sets the list locations.
        /// </summary>
        public ObservableCollection<AssetRegisterLocationRowItem> ListLocations
        {
            get
            {
                return this._listLocations;
            }

            set
            {
                this.SetField(ref this._listLocations, value, () => this.ListLocations);
            }
        }

        /// <summary>
        /// Gets or sets the select location default.
        /// </summary>
        public AssetRegisterLocationRowItem SelectLocationDefault
        {
            get
            {
                return this._selectLocationDefault;
            }

            set
            {
                this.SetField(ref this._selectLocationDefault, value, () => this.SelectLocationDefault);
            }
        }

        /// <summary>
        /// Gets or sets the list asset supplier.
        /// </summary>
        public ObservableCollection<AssetRelationRowItem> ListAssetSupplier
        {
            get
            {
                return this._listAssetSupplier;
            }

            set
            {
                this.SetField(ref this._listAssetSupplier, value, () => this.ListAssetSupplier);
            }
        }

        /// <summary>
        /// Gets or sets the select supplier default.
        /// </summary>
        public AssetRelationRowItem SelectSupplierDefault
        {
            get
            {
                return this._selectSupplierDefault;
            }

            set
            {
                this.SetField(ref this._selectSupplierDefault, value, () => this.SelectSupplierDefault);
            }
        }

        /// <summary>
        /// Gets or sets the list asset sub status.
        /// </summary>
        public ObservableCollection<SystemParam> ListAssetSubStatus
        {
            get
            {
                return this._listAssetSubStatus;
            }

            set
            {
                this.SetField(ref this._listAssetSubStatus, value, () => this.ListAssetSubStatus);
            }
        }

        /// <summary>
        /// Gets or sets the select sub default.
        /// </summary>
        public SystemParam SelectSubDefault
        {
            get
            {
                return this._selectSubDefault;
            }

            set
            {
                this.SetField(ref this._selectSubDefault, value, () => this.SelectSubDefault);
            }
        }

        /// <summary>
        /// Gets or sets the report company name.
        /// </summary>
        public string ReportCompanyName
        {
            get
            {
                return this._reportCompanyName;
            }

            set
            {
                this.SetField(ref this._reportCompanyName, value, () => this.ReportCompanyName);
            }
        }

        /// <summary>
        /// Gets or sets the search tool view model.
        /// </summary>
        public EquipSearchViewModel SearchToolViewModel
        {
            get
            {
                return this._searchToolViewModel;
            }

            set
            {
                this.SetField(ref this._searchToolViewModel, value, () => this.SearchToolViewModel);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is transfer.
        /// </summary>
        public bool IsAssetRegisterEnable
        {
            get
            {
                return this._isAssetRegisterEnable;
            }

            set
            {
                this.SetField(ref this._isAssetRegisterEnable, value, () => this.IsAssetRegisterEnable);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is all enable.
        /// </summary>
        public bool IsReportingCompanyEnable
        {
            get
            {
                return this._isReportingCompanyEnable;
            }

            set
            {
                this.SetField(ref this._isReportingCompanyEnable, value, () => this.IsReportingCompanyEnable);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is effective date enable.
        /// </summary>
        public bool IsEffectiveDateEnable
        {
            get
            {
                return this._isEffectiveDateEnable;
            }

            set
            {
                this.SetField(ref this._isEffectiveDateEnable, value, () => this.IsEffectiveDateEnable);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is acquisition date enable.
        /// </summary>
        public bool IsAcquisitionDateEnable
        {
            get
            {
                return this._isAcquisitionDateEnable;
            }

            set
            {
                this.SetField(ref this._isAcquisitionDateEnable, value, () => this.IsAcquisitionDateEnable);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is net asset cost enable.
        /// </summary>
        public bool IsNetAssetCostEnable
        {
            get
            {
                return this._isNetAssetCostEnable;
            }

            set
            {
                this.SetField(ref this._isNetAssetCostEnable, value, () => this.IsNetAssetCostEnable);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is asset gst enable.
        /// </summary>
        public bool IsAssetGSTEnable
        {
            get
            {
                return this._isAssetGSTEnable;
            }

            set
            {
                this.SetField(ref this._isAssetGSTEnable, value, () => this.IsAssetGSTEnable);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is location enable.
        /// </summary>
        public bool IsLocationEnable
        {
            get
            {
                return this._isLocationEnable;
            }

            set
            {
                this.SetField(ref this._isLocationEnable, value, () => this.IsLocationEnable);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is control enable.
        /// </summary>
        public bool IsControlEnable
        {
            get
            {
                return this._isControlEnable;
            }

            set
            {
                this.SetField(ref this._isControlEnable, value, () => this.IsControlEnable);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is status inactive.
        /// </summary>
        public bool IsStateInactive
        {
            get
            {
                return this._isStateInactive;
            }

            set
            {
                this._isStateInactive = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is transfer mode.
        /// </summary>
        public bool IsTransferMode
        {
            get
            {
                return this._isTransferMode;
            }

            set
            {
                if (value)
                {
                    this.IsEffectiveDateEnable = true;

                    this.IsControlEnable = false;
                    this.IsAcquisitionDateEnable = false;
                    this.IsNetAssetCostEnable = false;
                    this.IsAssetGSTEnable = false;
                    this.IsLocationEnable = false;
                }

                this._isTransferMode = value;               
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is status active.
        /// </summary>
        public bool IsStateActive
        {
            get
            {
                return this._isStateActive;
            }

            set
            {
                if (value)
                {
                    this.IsAssetRegisterEnable = false;
                    this.IsReportingCompanyEnable = false;
                }

                this._isStateActive = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is state terminated.
        /// </summary>
        public bool IsStateTerminated
        {
            get
            {
                return this._isStateTerminated;
            }

            set
            {
                if (value)
                {
                    this.IsAssetRegisterEnable = false;
                    this.IsReportingCompanyEnable = false;
                    this.IsAcquisitionDateEnable = false;
                    this.IsAssetGSTEnable = false;
                    this.IsNetAssetCostEnable = false;
                }

                this._isStateTerminated = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is status return.
        /// </summary>
        public bool IsStatusReturn
        {
            get
            {
                return this._isStatusReturn;
            }

            set
            {
                if (value)
                {
                    this.IsAcquisitionDateEnable = false;
                }

                this._isStatusReturn = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is status idle.
        /// </summary>
        public bool IsStatusIdle
        {
            get
            {
                return this._isStatusIdle;
            }

            set
            {
                if (value)
                {
                }

                this._isStatusIdle = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is status live.
        /// </summary>
        public bool IsStatusLive
        {
            get
            {
                return this._isStatusLive;
            }

            set
            {
                if (value)
                {
                    this.IsNetAssetCostEnable = false;
                    this.IsAssetGSTEnable = false;
                    this.IsLocationEnable = false;
                }

                this._isStatusLive = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is status reserve.
        /// </summary>
        public bool IsStatusReserve
        {
            get
            {
                return this._isStatusReserve;
            }

            set
            {
                if (value)
                {
                    this.IsNetAssetCostEnable = false;
                    this.IsAssetGSTEnable = false;
                }

                this._isStatusReserve = value;
            }
        }

        /// <summary>
        /// Gets or sets the dynamic combo box report company.
        /// </summary>
        public DynamicCheckComboBoxViewModel DynamicComboBoxReportCompany
        {
            get
            {
                return this._dynamicComboBoxReportCompany;
            }

            set
            {
                this.SetField(ref this._dynamicComboBoxReportCompany, value, () => this.DynamicComboBoxReportCompany);
            }
        }

        /// <summary>
        /// Gets or sets the dynamic combo box location.
        /// </summary>
        public DynamicCheckComboBoxViewModel DynamicComboBoxLocation
        {
            get
            {
                return this._dynamicComboBoxLocation;
            }

            set
            {
                this.SetField(ref this._dynamicComboBoxLocation, value, () => this.DynamicComboBoxLocation);
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether is content enable.
        /// </summary>
        public bool IsContentEnable
        {
            get
            {
                return this._isContentEnable;
            }

            set
            {
                this.SetField(ref this._isContentEnable, value, () => this.IsContentEnable);
            }
        }

        /// <summary>
        /// Gets or sets the dynamic registered asset feature view model.
        /// </summary>
        public DynamicGridViewModel DynamicRegisteredAssetFeatureViewModel
        {
            get
            {
                return this._dynamicRegisteredAssetFeatureViewModel;
            }

            set
            {
                this.SetField(ref this._dynamicRegisteredAssetFeatureViewModel, value, () => this.DynamicRegisteredAssetFeatureViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the asset effective date.
        /// </summary>
        public DateTime? AssetEffectiveDate
        {
            get
            {
                return this._assetEffectiveDate;
            }

            set
            {
                this.SetField(ref this._assetEffectiveDate, value, () => this.AssetEffectiveDate);
            }
        }

        /// <summary>
        /// Gets or sets the all registered feature.
        /// </summary>
        public ObservableCollection<RegisteredAssetFeatureRowItem> AllRegisteredFeature
        {
            get
            {
                return this._allRegisteredFeature;
            }

            set
            {
                this.SetField(ref this._allRegisteredFeature, value, () => this.AllRegisteredFeature);
            }
        }

        /// <summary>
        /// Gets or sets the internal company id.
        /// </summary>
        public int InternalCompanyId { get; set; }

        /// <summary>
        /// Gets or sets the financer id.
        /// </summary>
        public int FinancerId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is gl module key.
        /// </summary>
        public bool IsGlModuleKey
        {
            get
            {
                return this._isGlModuleKey;
            }

            set
            {
                this.SetField(ref this._isGlModuleKey, value, () => this.IsGlModuleKey);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is in active asset.
        /// </summary>
        public bool IsInActiveAsset
        {
            get
            {
                return this._isInActiveAsset;
            }

            set
            {
                this.SetField(ref this._isInActiveAsset, value, () => this.IsInActiveAsset);
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// The reset default enable.
        /// </summary>
        public async void ResetDefaultEnable()
        {
            this.AssetSettingMakeModel = new AssetClassSetting();
            this.AssetSettingMakeModel = await RegisteredAssetFunction.GetAssetSetting();

            this.IsAssetRegisterEnable = true;
            this.IsReportingCompanyEnable = true;
            this.IsEffectiveDateEnable = false;

            this.IsControlEnable = true;
            this.IsAcquisitionDateEnable = true;
            this.IsNetAssetCostEnable = true;
            this.IsAssetGSTEnable = true;
            this.IsLocationEnable = true;

            this.GetDataForEquipSearch();
            this.GetDataForFeatureGrid();
        }

        /// <summary>
        /// The enable control.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        public void EnableControl(EnumRegisteredAssetState state, EnumRegisteredAssetStatus status)
        {
            switch (state)
            {
                    case EnumRegisteredAssetState.Active:
                        this.IsStateActive = true;
                        break;
                    case EnumRegisteredAssetState.Inactive:
                        this.IsStateInactive = true;
                        break;
                    case EnumRegisteredAssetState.Terminated:
                        this.IsStateTerminated = true;
                        break;
            }

            switch (status)
            {
                    case EnumRegisteredAssetStatus.AssetIdle:
                        this.IsStatusIdle = true;
                        break;
                    case EnumRegisteredAssetStatus.AssetLive:
                        this.IsStatusLive = true;
                        break;
                    case EnumRegisteredAssetStatus.AssetReserve:
                        this.IsStatusReserve = true;
                        break;
                    case EnumRegisteredAssetStatus.AssetReturn:
                        this.IsStatusReturn = true;
                    break;
            }
        }

        /// <summary>
        /// The get data for equip search.
        /// </summary>
        public async void GetDataForEquipSearch()
        {
            // Get list condition to search.
            var listConditionSearch = new ObservableCollection<ItemComboBox>
                                           {
                                               new ItemComboBox
                                                   {
                                                       Name = "All Asset Class", ItemId = 0, TypeItem = SystemType.AllAssetClass,
                                                   },
                                               new ItemComboBox
                                                   {
                                                       Name = "Category", ItemId = 1, TypeItem = SystemType.Category,
                                                   },
                                               new ItemComboBox
                                                   {
                                                       Name = "Type", ItemId = 2, TypeItem = SystemType.Type,
                                                   },
                                           };
            
            // Get list result search.
            ObservableCollection<ItemComboBox> listCategory = new ObservableCollection<ItemComboBox>(await RegisteredAssetFunction.GetAllCategory());
            ObservableCollection<ItemComboBox> listType = new ObservableCollection<ItemComboBox>(new List<ItemComboBox>
                                                                                                     {
                                                                                                         new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Type }
                                                                                                     });

            listCategory.Insert(0, new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Category });

            var listItemResultSearch = new ObservableCollection<ItemEquipSearch>
                                            {
                                                new ItemEquipSearch
                                                    {
                                                        ListSourceItems = new DynamicCheckComboBoxViewModel { ComboBoxItemList = listCategory, SelectedItem = listCategory.FirstOrDefault(), TypeItem = SystemType.Category, IsEnableComboBox = true },
                                                        Name = "Category",
                                                        ItemType = SystemType.Category,
                                                    },
                                                new ItemEquipSearch
                                                    {
                                                        ListSourceItems = new DynamicCheckComboBoxViewModel { ComboBoxItemList = listType, SelectedItem = listType.FirstOrDefault(), TypeItem = SystemType.Type, IsEnableComboBox = true },
                                                        Name = "Type",
                                                        ItemType = SystemType.Type,
                                                    },
                                            };

            // Create data column for grid result search.
            var listAllItems = new List<EquipSearchRowItem>();
            var dynamicGridResultSearch = new DynamicGridViewModel(typeof(EquipSearchRowItem));
            dynamicGridResultSearch.GridColumns = new List<DynamicColumn>

                                                                                         {
                                                                                             new DynamicColumn { ColumnName = "EquipCategoryName",  Header = "Category", Width = 82, HeaderTextAlignment = TextAlignment.Center, TextAlignment = TextAlignment.Center },
                                                                                             new DynamicColumn { ColumnName = "EquipTypeName", Header = "Type", Width = 82, HeaderTextAlignment = TextAlignment.Center, TextAlignment = TextAlignment.Center },
                                                                                         };
            dynamicGridResultSearch.GridDataRows = listAllItems.ToList<object>();
            dynamicGridResultSearch.IsSetSelectedItem = true;
            dynamicGridResultSearch.SelectedItemChanged = this.SetSelectedSearchResultItem;
            dynamicGridResultSearch.LoadRadGridView();

            if (this.AssetSettingMakeModel != null)
            {
                // Check if IncludeMake is true
                if (this.AssetSettingMakeModel.IncludeMake)
                {
                    // Insert Make to list Condition to Search
                    ItemComboBox itemConditionMake = new ItemComboBox
                                                         {
                                                             Name = "Make",
                                                             ItemId = 3,
                                                             TypeItem = SystemType.Make,
                                                         };
                    listConditionSearch.Add(itemConditionMake);

                    // Insert Make item to list Result Search
                    ObservableCollection<ItemComboBox> listMake = new ObservableCollection<ItemComboBox>(new List<ItemComboBox>
                                                                                                     {
                                                                                                         new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Make }
                                                                                                     });
                    ItemEquipSearch itemEquipMake = new ItemEquipSearch
                                                   {
                                                       ListSourceItems = new DynamicCheckComboBoxViewModel { ComboBoxItemList = listMake, SelectedItem = listMake.FirstOrDefault(), TypeItem = SystemType.Make, IsEnableComboBox = true },
                                                       Name = "Make",
                                                       ItemType = SystemType.Make,
                                                   };

                    listItemResultSearch.Add(itemEquipMake);

                    // Insert Make column to Grid Search result
                    DynamicColumn columnMake = new DynamicColumn
                                                   {
                                                       ColumnName = "EquipMakeName",
                                                       Header = "Make",
                                                       Width = 82,
                                                       HeaderTextAlignment = TextAlignment.Left,
                                                       TextAlignment = TextAlignment.Left
                                                   };

                    dynamicGridResultSearch.GridColumns.Add(columnMake);
                }

                // Check if IncludeModel is true
                if (this.AssetSettingMakeModel.IncludeModel)
                {
                    // Insert Model to list Condition to Search
                    ItemComboBox itemConditionModel = new ItemComboBox
                                                          {
                                                              Name = "Model",
                                                              ItemId = 4,
                                                              TypeItem = SystemType.Model,
                                                          };
                    listConditionSearch.Add(itemConditionModel);

                    // Insert Model item to list Result Search
                    ObservableCollection<ItemComboBox> listModel = new ObservableCollection<ItemComboBox>(new List<ItemComboBox>
                                                                                                     {
                                                                                                         new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Model }
                                                                                                     });
                    ItemEquipSearch itemEquipModel = new ItemEquipSearch
                    {
                        ListSourceItems = new DynamicCheckComboBoxViewModel { ComboBoxItemList = listModel, SelectedItem = listModel.FirstOrDefault(), TypeItem = SystemType.Model, IsEnableComboBox = true },
                        Name = "Model",
                        ItemType = SystemType.Model,
                    };

                    listItemResultSearch.Add(itemEquipModel);

                    // Insert Model column to Grid Search result
                    DynamicColumn columnModel = new DynamicColumn
                    {
                        ColumnName = "EquipModelName",
                        Header = "Model",
                        Width = 82,
                        HeaderTextAlignment = TextAlignment.Left,
                        TextAlignment = TextAlignment.Left
                    };

                    dynamicGridResultSearch.GridColumns.Add(columnModel);
                }
            }

            // Set action when user select on Grid Result Search
            foreach (var item in listItemResultSearch)
            {
                item.ListSourceItems.SelectedItemChanged += this.ResultSearch_SelectedItemChanged;
            }

            if (this.SearchToolViewModel != null)
            {
                this.SearchToolViewModel.GenerateData(listConditionSearch, 0, listItemResultSearch, dynamicGridResultSearch);
            }
        }

        /// <summary>
        /// The get detail data source.
        /// </summary>
        /// <param name="itemSelected">
        /// The item selected.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetDetailDataSource(RegisteredAssetRowItem itemSelected)
        {
            this.ListAssetRegisters =
                new ObservableCollection<AssetRegisterRowItem>(await RegisteredAssetFunction.GetListRegister());
            this.ListAssetRegisters.Insert(
                0,
                new AssetRegisterRowItem
                    {
                        ID = -1,
                RegisterName = "<None>",
            });

            this.ListAssetSupplier =
                new ObservableCollection<AssetRelationRowItem>(await RegisteredAssetFunction.GetListSupplier());
            this.ListAssetSupplier.Insert(
                0,
                new AssetRelationRowItem
                    {
                        NodeId = -1,
                NodeName = "<None>",
            });

            this.ListAssetSubStatus =
                new ObservableCollection<SystemParam>(await RegisteredAssetFunction.GetListSubStatus());
            this.ListAssetSubStatus.Insert(
                0,
                new SystemParam
                    {
                        ParamId = -1,
                DisplayName = "<None>",
            });

            this.IsGlModuleKey = await Authorisation.IsModuleInstalledAsync(Modules.GLModule);
            // Load data for equip search control
            this.GetDataForEquipSearch();
            if (itemSelected.Id != 0)
            {
                this.RegisterAssetId = itemSelected.Id;
                this.AssetStatus = itemSelected.AssetStatus;
                this.StateId = itemSelected.StateId;
                this.StatusId = itemSelected.StatusId;
                this.AssetState = itemSelected.AssetState;
                this.AssetAcquisitionDate = itemSelected.AcqDate;
                this.AssetPoNumber = itemSelected.PONumber;
                this.AssetContractNum = itemSelected.ContractNumber;
                this.AssetSerialNum = itemSelected.SerialNumber;
                this.AssetDescription = itemSelected.Description;
                this.AssetGst = itemSelected.GstAsset;
                this.NetAssetCost = itemSelected.Cost;
                this.TotalAssetCost = this.NetAssetCost + this.AssetGst;
                this.AssetPoNumber = itemSelected.PONumber;
                this.InternalCompanyId = itemSelected.InternalCoyNodeId;
                this.FinancerId = itemSelected.FinancierNodeId;
                this.AssetAnnexure = RegisteredAssetFunction.GetAssetAnnexure(itemSelected.Id);
                this.SelectedRegister = await AssetRegisterFunction.GetAssetRegisterDetail(itemSelected.AssetRegisterId);
                this.AssetEffectiveDate = RegisteredAssetFunction.GetEffectiveDay(itemSelected.Id);
                if (this.AssetEffectiveDate == null)
                {
                    this.AssetEffectiveDate = DateTime.Today;
                }

                // Load data for equip search control
                // this.GetDataForEquipSearch();
                this.CurrentSearchResultItem = new EquipSearchRowItem
                                                            {
                                                                EquipCategoryId = itemSelected.CategoryId,
                                                                EquipTypeId = itemSelected.TypeId,
                                                                EquipMakeId = itemSelected.MakeId,
                                                                EquipModelId = itemSelected.ModelId,
                                                            };
                EquipSearchRowItem serachItemResult = new EquipSearchRowItem
                                                          {
                                                              EquipCategoryId = itemSelected.CategoryId,
                                                              EquipTypeId = itemSelected.TypeId,
                                                              EquipMakeId = itemSelected.MakeId,
                                                              EquipModelId = itemSelected.ModelId,
                                                          };
                await this.GenerateDataSearchSelected(serachItemResult);

                // Get data for Report company drop down list
                if (this.DynamicComboBoxReportCompany.SelectedItem != null)
                {
                    await this.GetDataForReportCompanyComboBox(itemSelected.AssetRegisterId, this.DynamicComboBoxReportCompany.SelectedItem.ItemId);
                }
                else
                {
                    await this.GetDataForReportCompanyComboBox(itemSelected.AssetRegisterId, -1);
                }

                // Get data for Location drop down list
                await this.GetDataForLocationComboBox(itemSelected.AssetRegisterId, itemSelected.AssetRegisterLocationId);

                this.SelectRegisterDefault =
                    this.ListAssetRegisters.FirstOrDefault(x => x.ID.Equals(itemSelected.AssetRegisterId));
                
                if (this.ListAssetSupplier.FirstOrDefault(x => x.NodeId.Equals(itemSelected.SupplierNodeId)) != null)
                {
                    this.SelectSupplierDefault = this.ListAssetSupplier.FirstOrDefault(x => x.NodeId.Equals(itemSelected.SupplierNodeId));
                }
                else
                {
                    this.SelectSupplierDefault = this.ListAssetSupplier.FirstOrDefault(x => x.NodeId == -1);
                }

                if (this.ListAssetSubStatus.FirstOrDefault(x => x.ParamId.Equals(itemSelected.SpAssetStatus)) != null)
                {
                    this.SelectSubDefault =
                        this.ListAssetSubStatus.FirstOrDefault(x => x.ParamId.Equals(itemSelected.SpAssetStatus));
                }
                else
                {
                    this.SelectSubDefault = this.ListAssetSubStatus.FirstOrDefault(x => x.ParamId == -1);
                }

                // Get and load data for Grid Feature
                this.AllRegisteredFeature = new ObservableCollection<RegisteredAssetFeatureRowItem>(await RegisteredAssetFunction.GetRegisteredFeatureItems(itemSelected.Id));
                this.UpdateDataForFeatureGrid();
            }
            else
            {
                this.RegisterAssetId = itemSelected.Id;
                this.AssetStatus = itemSelected.AssetStatus;
                this.AssetState = itemSelected.AssetState;
                this.StatusId = itemSelected.StatusId;
                this.StateId = itemSelected.StateId;
                this.AssetAcquisitionDate = itemSelected.AcqDate;
                this.AssetPoNumber = null;
                this.AssetContractNum = null;
                this.AssetSerialNum = null;
                this.AssetDescription = null;
                this.AssetGst = 0;
                this.NetAssetCost = 0;
                this.TotalAssetCost = this.NetAssetCost + this.NetAssetCost;
                this.AssetPoNumber = null;
                this.AssetAnnexure = null;

                this.SelectRegisterDefault =
                    this.ListAssetRegisters.FirstOrDefault(x => x.IsDefault);

                var assetRegisterRowItem = this.SelectRegisterDefault;
                if (assetRegisterRowItem != null)
                {
                    // Get data for Report company drop down list
                    if (this.DynamicComboBoxReportCompany.SelectedItem != null)
                    {
                        await this.GetDataForReportCompanyComboBox(assetRegisterRowItem.ID, this.DynamicComboBoxReportCompany.SelectedItem.ItemId);
                    }
                    else
                    {
                        await this.GetDataForReportCompanyComboBox(assetRegisterRowItem.ID, -1);
                    }

                    this.ListLocations = new ObservableCollection<AssetRegisterLocationRowItem>(
                       await AssetRegisterFunction.GetLocationsForRegister(assetRegisterRowItem.ID));
                }

                this.ListLocations.Insert(
                                           0,
                                           new AssetRegisterLocationRowItem
                                           {
                                               AssetRegisterLocationID = -1,
                                               LocationName = "<None>",
                                           });
                this.SelectLocationDefault = this.ListLocations.FirstOrDefault(
                    x => x.AssetRegisterLocationID.Equals(-1));

                this.SelectSupplierDefault = this.ListAssetSupplier.FirstOrDefault(x => x.NodeId.Equals(-1));
                
                this.SelectSubDefault =
                    this.ListAssetSubStatus.FirstOrDefault(x => x.ParamId.Equals(-1));
            }

            this.SelectedRegisteredAsset = new RegisteredAssetRowItem
                                               {
                                                   Id = this.RegisterAssetId,
                                                   AssetRegisterId = this.RegisterAssetId,
                                                   AssetStatus = this.AssetStatus,
                                                   StateId = this.StateId,
                                                   StatusId = this.StatusId,
                                                   AssetState = this.AssetState,
                                                   AcqDate = this.AssetAcquisitionDate,
                                                   PONumber = this.AssetPoNumber,
                                                   SerialNumber = this.AssetSerialNum,
                                                   ContractNumber = this.AssetContractNum,
                                                   Description = this.AssetDescription,
                                                   GstAsset = this.AssetGst,
                                                   Cost = this.NetAssetCost,
                                                   FinancierNodeId = this.FinancerId,
                                                   InternalCoyNodeId = this.InternalCompanyId,
                                               };
        }

        /// <summary>
        /// The get search result items.
        /// </summary>
        /// <returns>
        /// The <see cref="EquipSearchRowItem"/>.
        /// </returns>
        public EquipSearchRowItem GetSearchResultItems()
        {
            EquipSearchRowItem result = null;
            if (this.SearchToolViewModel != null)
            {
                ItemEquipSearch itemSearchCategory = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Category);
                ItemEquipSearch itemSearchType = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Type);
                ItemEquipSearch itemSearchMake = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Make);
                ItemEquipSearch itemSearchModel = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Model);

                    result = new EquipSearchRowItem
                                 {
                                     EquipCategoryId = itemSearchCategory == null ? 0 : itemSearchCategory.ListSourceItems.SelectedItem.ItemId,
                                     EquipCategoryName = itemSearchCategory == null ? null : itemSearchCategory.ListSourceItems.SelectedItem.Name,
                                     EquipTypeId = itemSearchType == null ? 0 : itemSearchType.ListSourceItems.SelectedItem.ItemId,
                                     EquipTypeName = itemSearchType == null ? null : itemSearchType.ListSourceItems.SelectedItem.Name,
                                     EquipMakeId = itemSearchMake == null ? 0 : itemSearchMake.ListSourceItems.SelectedItem.ItemId,
                                     EquipMakeName = itemSearchMake == null ? null : itemSearchMake.ListSourceItems.SelectedItem.Name,
                                     EquipModelId = itemSearchModel == null ? 0 : itemSearchModel.ListSourceItems.SelectedItem.ItemId,
                                     EquipModelName = itemSearchModel == null ? null : itemSearchModel.ListSourceItems.SelectedItem.Name,
                                 };
            }

            return result;
        }

        /// <summary>
        /// The get list feature to save.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<RegisteredAssetFeatureRowItem> GetListFeatureToSave()
        {
            if (this.AllRegisteredFeature != null && this.DynamicRegisteredAssetFeatureViewModel != null)
            {
                foreach (var rowItem in this.DynamicRegisteredAssetFeatureViewModel.MembersTable.Rows)
                {
                    RegisteredAssetFeatureRowItem item = rowItem.RowObject as RegisteredAssetFeatureRowItem;
                    if (item != null)
                    {
                        var registeredAssetFeatureRowItem = this.AllRegisteredFeature.FirstOrDefault(x => x.FeatureId == item.FeatureId);
                        if (registeredAssetFeatureRowItem != null)
                        {
                            registeredAssetFeatureRowItem.FeatureValue = item.FeatureValue;
                        }
                    }
                }

                return this.AllRegisteredFeature.ToList();
            }

            return null;
        }

        /// <summary>
        /// The get data for save transfer mode.
        /// </summary>
        /// <param name="selectItem">
        /// The select Item.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetDataForSaveTransferMode(RegisteredAssetRowItem selectItem)
        {
            if (selectItem != null && this.SelectRegisterDefault != null
                && this.DynamicComboBoxReportCompany.SelectedItem != null
               && this.AssetEffectiveDate != null)
            {
                await
                    RegisteredAssetFunction.SaveDataForTransferMode(
                        selectItem,
                        this.SelectRegisterDefault,
                        this.DynamicComboBoxReportCompany.SelectedItem,
                        this.AssetEffectiveDate);
                selectItem.AssetRegisterId = this.SelectRegisterDefault.ID;
                if (AssetRegisterFunction.GetCategory() == 5
                    && await Authorisation.IsModuleInstalledAsync(Modules.GLModule))
                {
                    selectItem.FinancierNodeId = this.DynamicComboBoxReportCompany.SelectedItem.ItemId;
                }

                if (AssetRegisterFunction.GetCategory() == 7)
                {
                    selectItem.InternalCoyNodeId = this.DynamicComboBoxReportCompany.SelectedItem.ItemId;
                }

                RegisteredAssetFunction.InsertHistoryRecord(selectItem, this.AssetEffectiveDate);
            }
        }

        /// <summary>
        /// The get data for feature grid.
        /// </summary>
        public async void GetDataForFeatureGrid()
        {
            if (this.DynamicRegisteredAssetFeatureViewModel == null)
            {
                this.DynamicRegisteredAssetFeatureViewModel = new DynamicGridViewModel(typeof(RegisteredAssetFeatureRowItem));
            }

            this.DynamicRegisteredAssetFeatureViewModel.IsEnableHoverRow = false;
            this.DynamicRegisteredAssetFeatureViewModel.IsShowGroupPanel = false;
            this.DynamicRegisteredAssetFeatureViewModel.MaxWidthGrid = 700;
            this.DynamicRegisteredAssetFeatureViewModel.IsEnableRadioButtonRow = false;
            this.DynamicRegisteredAssetFeatureViewModel.IsGridReadOnly = false;
            this.AllRegisteredFeature = new ObservableCollection<RegisteredAssetFeatureRowItem>();
            this.DynamicRegisteredAssetFeatureViewModel.GridColumns = new List<DynamicColumn>
                                                                                         {
                                                                                             new DynamicColumn
                                                                                                 {
                                                                                                     ColumnName = "FeatureName",  Header = "Feature", IsReadOnly = true, HeaderTextAlignment = TextAlignment.Center, TextAlignment = TextAlignment.Left, Width = 150
                                                                                                 },
                                                                                             new DynamicColumn
                                                                                                 {
                                                                                                     ColumnName = "FeatureValue", Header = "Value", IsReadOnly = false, HeaderTextAlignment = TextAlignment.Center, TextAlignment = TextAlignment.Left, Width = 150,
                                                                                                 },
                                                                                         };
            this.DynamicRegisteredAssetFeatureViewModel.FilteringGenerate = true;
            this.DynamicRegisteredAssetFeatureViewModel.GridDataRows = this.AllRegisteredFeature.ToList<object>();
            this.DynamicRegisteredAssetFeatureViewModel.LoadRadGridView();
            this.DynamicRegisteredAssetFeatureViewModel.GroupedItemChanged = this.GroupedFeatureItemChanged;
        }

        /// <summary>
        /// The get data detail to save.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<RegisteredAsset> GetDataDetailToSave()
        {
            RegisteredAsset itemSave = new RegisteredAsset();
            EquipSearchRowItem equip = this.GetSearchResultItems();
            itemSave.ID = this.RegisterAssetId;
            itemSave.StateID = this.StateId;
            itemSave.StatusID = this.StatusId;
            itemSave.AssetRegisterID = this.SelectRegisterDefault.ID;
            if (this.DynamicComboBoxReportCompany.SelectedItem != null)
            {
                if (AssetRegisterFunction.GetCategory() == 5
                    && await Authorisation.IsModuleInstalledAsync(Modules.GLModule))
                {
                    itemSave.FinancierNodeID = this.DynamicComboBoxReportCompany.SelectedItem.ItemId;
                    itemSave.InternalCoyNodeID = this.InternalCompanyId;
                }
                else
                {
                    itemSave.InternalCoyNodeID = this.DynamicComboBoxReportCompany.SelectedItem.ItemId;
                    itemSave.FinancierNodeID = this.FinancerId;
                }
            }

            itemSave.AquisitionDate = this.AssetAcquisitionDate;
            itemSave.SupplierNodeID = this.SelectSupplierDefault.NodeId;
            if (this.SelectLocationDefault != null)
            {
                itemSave.AssetRegisterLocationID = this.SelectLocationDefault.AssetRegisterLocationID;
            }

            itemSave.AssetCost = this.NetAssetCost;
            itemSave.GSTAssetD = this.AssetGst;
            itemSave.PONumber = this.AssetPoNumber;
            if (this.AssetDescription != null)
            {
                itemSave.Description = this.AssetDescription;
            }
            else
            {
                itemSave.Description = string.Empty;
            }

            itemSave.SerialNumber = this.AssetSerialNum;
            itemSave.AquisitionDate = this.AssetAcquisitionDate;
            itemSave.LastModifedDate = DateTime.Today;
            itemSave.EquipCatID = equip.EquipCategoryId;
            itemSave.EquipTypeID = equip.EquipTypeId;
            itemSave.EquipMakeID = equip.EquipMakeId;
            itemSave.EquipModelID = equip.EquipModelId;
            return itemSave;
        }

        /// <summary>
        /// Gets or sets the item equip search.
        /// </summary>
        public ItemEquipSearch ItemEquipSearch { get; set; }

        /// <summary>
        /// The generate data search selected.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GenerateDataSearchSelected(object o)
        {
            this.SetSelectedSearchResultItem(o);
        }

        /// <summary>
        /// The update data for feature grid.
        /// </summary>
        public void UpdateDataForFeatureGrid()
        {
            this.DynamicRegisteredAssetFeatureViewModel.GridDataRows = this.AllRegisteredFeature.ToList<object>();
            this.DynamicRegisteredAssetFeatureViewModel.LoadRadGridView();
        }

        public void ValidateAcquisitionDate(EnumRegisteredAssetStatus status)
        {
            bool depreciateInInventory = RegisteredAssetFunction.GetDepreciateInInventory();
            switch (status)
            {
                    case EnumRegisteredAssetStatus.AssetIdle:
                    if (depreciateInInventory)
                    {
                        RegisteredAssetFunction.UpdateBookDepreciation(this.AssetAcquisitionDate, this.RegisterAssetId);
                    }

                    break;
                    case EnumRegisteredAssetStatus.AssetLive:
                    DateTime contractStartDate = RegisteredAssetFunction.GetContractStartDate(this.AssetContractNum);
                    if (this.AssetAcquisitionDate <= contractStartDate)
                    {
                        ConfirmationWindowView confirm = new ConfirmationWindowView();
                        ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                        confirmViewModel.Content = "Asset is live on Contract " + this.AssetContractNum + " with Start Date of " + contractStartDate + ". Please select OK to continue.";
                        confirmViewModel.Title = "Confirm - Asset Contract";
                        confirm.DataContext = confirmViewModel;

                        confirm.ShowDialog();
                    }
                    else
                    {
                        this.ShowMessageAsync("Asset is live on Contract " + this.AssetContractNum + " with Start Date of " + contractStartDate + ". Acquisition Date cannot be greater than the Contract Start Date. Please remove the Asset from the Contract or change Acquisition Date before trying again.", "Asset Contract - Error");
                    }

                    break;
                    case EnumRegisteredAssetStatus.AssetReserve:
                    DateTime? quoteStartDate = RegisteredAssetFunction.GetQuoteStartDate(this.RegisterAssetId);
                    if (this.AssetAcquisitionDate <= quoteStartDate)
                    {
                        ConfirmationWindowView confirm = new ConfirmationWindowView();
                        ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                        confirmViewModel.Content = "Asset is reserved on Quote " + this.AssetContractNum + " with Start Date of " + quoteStartDate + ". Please select OK to continue.";
                        confirmViewModel.Title = "Confirm - Asset Quote";
                        confirm.DataContext = confirmViewModel;

                        confirm.ShowDialog();
                    }
                    else
                    {
                        this.ShowMessageAsync("Asset is reserved on Quote " + this.AssetContractNum + " with Start Date of " + quoteStartDate + ". Acquisition Date cannot be greater than the Quote Start Date. Please remove Asset from the Quote or change Acquisition Date before trying again.", "Asset Contract - Error");
                    }

                    break;
            }
        }
        #endregion

        #region Protected Method

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
        #endregion

        #region Private Method
        /// <summary>
        /// The grouped item changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GroupedFeatureItemChanged(object sender, object e)
        {
            if ((int)e == -1)
            {
                this.DynamicRegisteredAssetFeatureViewModel.MaxWidthGrid = 10000;
            }
            else
            {
                this.DynamicRegisteredAssetFeatureViewModel.MaxWidthGrid = (int)e + 1;
            }
        }

        /// <summary>
        /// The method to searching for equip search user control.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        private async void OnSearchingAction(string s)
        {
            List<EquipSearchRowItem> result = new List<EquipSearchRowItem>();
            if (string.IsNullOrEmpty(s))
            {
                this.SearchToolViewModel.PreviousTextSearching = s;

                // Clear data of grid search result
                this.SearchToolViewModel.DynamicGridResultSearch.MembersTable.Rows.Clear();
            }
            else if (string.IsNullOrEmpty(this.SearchToolViewModel.PreviousTextSearching) 
                || (!string.IsNullOrEmpty(s) && (!this.SearchToolViewModel.PreviousTextSearching.Equals(s) || this.SearchToolViewModel.PreviousItemConditionSearchSelected.TypeItem != this.SearchToolViewModel.ItemConditionSearchSelected.TypeItem)))
            {
                this.SearchToolViewModel.PreviousTextSearching = s;
                this.SearchToolViewModel.PreviousItemConditionSearchSelected = this.SearchToolViewModel.ItemConditionSearchSelected;
                if (this.SearchToolViewModel.DynamicGridResultSearch != null)
                {
                    // Search all item correspond with condition search is selected
                    if (this.SearchToolViewModel.ItemConditionSearchSelected.TypeItem == SystemType.AllAssetClass)
                    {
                        result = await RegisteredAssetFunction.GetAllAssetClassSearch(s);
                    }
                    else if (this.SearchToolViewModel.ItemConditionSearchSelected.TypeItem == SystemType.Category)
                    {
                        result = await RegisteredAssetFunction.GetCategoryForSearch(s);
                    }
                    else if (this.SearchToolViewModel.ItemConditionSearchSelected.TypeItem == SystemType.Type)
                    {
                        result = await RegisteredAssetFunction.GetTypeForSearch(s);
                    }
                    else if (this.SearchToolViewModel.ItemConditionSearchSelected.TypeItem == SystemType.Make)
                    {
                        result = await RegisteredAssetFunction.GetMakeForSearch(s);
                    }
                    else if (this.SearchToolViewModel.ItemConditionSearchSelected.TypeItem == SystemType.Model)
                    {
                        result = await RegisteredAssetFunction.GetModelForSearch(s);
                    }

                    // Generate data for grid search result
                    this.SearchToolViewModel.DynamicGridResultSearch.GridDataRows = result.GroupBy(x => new { x.EquipCategoryName, x.EquipTypeName, x.EquipMakeName, x.EquipModelName }).Select(x => x.First()).ToList<object>();
                    this.SearchToolViewModel.DynamicGridResultSearch.LoadRadGridView();
                }
            }

            if (this.SearchToolViewModel != null)
            {
                this.SearchToolViewModel.ChangeToSearchGrid();
            }
        }

        /// <summary>
        /// The on close search action.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        private void OnCloseSearchAction(string s)
        {
            this.SearchToolViewModel.PreviousTextSearching = s;
            
            // Clear data of grid search result
            this.SearchToolViewModel.DynamicGridResultSearch.MembersTable.Rows.Clear();
        }

        /// <summary>
        /// The serial number changed.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SerialNumberChanged(string value)
        {
            if (await Authorisation.IsModuleInstalledAsync(Modules.PPSR))
            {
                int? registrationStatusId = RegisteredAssetFunction.GetRegistrationStatusId(this.RegisterAssetId);
                if (registrationStatusId == 635)
                {
                    this.ShowMessageAsync("The Serial Number has been used in a current PPSR registration has been modified. This field cannot be modified in PPSR. You are required to discharge the PPSR registration and create a new registration.", "Serial Number - Error");
                    return;
                }

                int? registrationStatusIdQuote = RegisteredAssetFunction.GetRegistrationStatusIdFromPPSRQuoteAssetView(this.RegisterAssetId);
                if (registrationStatusIdQuote == 635)
                {
                    this.ShowMessageAsync("The Serial Number has been used in a current PPSR registration has been modified. This field cannot be modified in PPSR. You are required to discharge the PPSR registration and create a new registration.", "Serial Number - Error");
                    return;
                }
            }

            this.SetField(ref this._assetSerialNum, value, () => this.AssetSerialNum);
        }

        /// <summary>
        /// The validate pps rregistrationforfor quote asset.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task ValidatePPSRregistrationforforQuoteAsset(string value)
        {
            if (await Authorisation.IsModuleInstalledAsync(Modules.PPSR))
            {
                int? registrationStatusId = RegisteredAssetFunction.GetRegistrationStatusIdFromPPSRQuoteAssetView(this.RegisterAssetId);
                if (registrationStatusId == 635)
                {
                    this.ShowMessageAsync("The Serial Number has been used in a current PPSR registration has been modified. This field cannot be modified in PPSR. You are required to discharge the PPSR registration and create a new registration.", "Serial Number - Error");
                    return;
                }
            }

            this.SetField(ref this._assetSerialNum, value, () => this.AssetSerialNum);
        }

        /// <summary>
        /// The selected grid result item changed.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        private async void SetSelectedSearchResultItem(object o)
        {
            EquipSearchRowItem itemSelect = o as EquipSearchRowItem;

            if (itemSelect != null)
            {
                ItemEquipSearch itemSearchCategory = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Category);
                ItemEquipSearch itemSearchType = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Type);
                ItemEquipSearch itemSearchMake = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Make);
                ItemEquipSearch itemSearchModel = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Model);

                List<RegisteredAssetFeatureRowItem> listFeatureItems = new List<RegisteredAssetFeatureRowItem>();

                if (itemSearchCategory != null && itemSearchType != null)
                {
                    // Set select item result of category
                    itemSearchCategory.ListSourceItems.SetSelectedItem(itemSelect.EquipCategoryId);

                    // Get list type item correspond with item category selected 
                    if (itemSearchCategory.ListSourceItems.SelectedItem != null)
                    {
                        // Get Features items for Grid feature
                        listFeatureItems.AddRange((await RegisteredAssetFunction.GetCategoryFeatureItem(itemSearchCategory.ListSourceItems.SelectedItem.ItemId)).ConvertAll(x => new RegisteredAssetFeatureRowItem
                                                                                                                                                                                     {
                                                                                                                                                                                         FeatureId = x.FeatureTypeId,
                                                                                                                                                                                         FeatureName = x.FeatureName,
                                                                                                                                                                                     }));
                        ObservableCollection<ItemComboBox> listType = new ObservableCollection<ItemComboBox>(await RegisteredAssetFunction.GetTypeAssignByCategory(itemSearchCategory.ListSourceItems.SelectedItem.ItemId));
                        listType.Insert(0, new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Type });
                        itemSearchType.ListSourceItems.SetListSourceItem(listType);
                    }

                    // Set select item result of type
                    itemSearchType.ListSourceItems.SetSelectedItem(itemSelect.EquipTypeId);

                    // Get list make item correspond with item type selected 
                    if (itemSearchType.ListSourceItems.SelectedItem != null)
                    {
                        // Get Features items for Grid feature
                        listFeatureItems.AddRange((await RegisteredAssetFunction.GetTypeFeatureItem(itemSearchType.ListSourceItems.SelectedItem.ItemId)).ConvertAll(x => new RegisteredAssetFeatureRowItem
                                                                                                                                                                                    {
                                                                                                                                                                                        FeatureId = x.FeatureTypeId,
                                                                                                                                                                                        FeatureName
                                                                                                                                                                                            =
                                                                                                                                                                                            x
                                                                                                                                                                                            .FeatureName,
                                                                                                                                                                                    }));
                        if (itemSearchMake != null)
                        {
                            ObservableCollection<ItemComboBox> listMake;
                            if (itemSearchType.ListSourceItems.SelectedItem.ItemId != 0)
                            {
                                listMake = new ObservableCollection<ItemComboBox>(await RegisteredAssetFunction.GetMakeAssignByType(itemSearchType.ListSourceItems.SelectedItem.ItemId));
                                listMake.Insert(0, new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Make });
                            }
                            else
                            {
                                listMake = new ObservableCollection<ItemComboBox>
                                           {
                                               new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Make }
                                           };
                            }

                            itemSearchMake.ListSourceItems.SetListSourceItem(listMake);
                        }
                    }

                    // Set select item result of make
                    if (itemSearchMake != null)
                    {
                        itemSearchMake.ListSourceItems.SetSelectedItem(itemSelect.EquipMakeId);

                        // Get list model item correspond with item make selected 
                        if (itemSearchMake.ListSourceItems.SelectedItem != null)
                        {
                            if (itemSearchModel != null)
                            {
                                ObservableCollection<ItemComboBox> listModel;
                                if (itemSearchMake.ListSourceItems.SelectedItem.ItemId != 0)
                                {
                                    listModel = new ObservableCollection<ItemComboBox>(await RegisteredAssetFunction.GetModelAssignByMake(itemSearchMake.ListSourceItems.SelectedItem.ItemId));
                                    listModel.Insert(0, new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Model });
                                }
                                else
                                {
                                    listModel = new ObservableCollection<ItemComboBox>
                                                {
                                                    new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Model }
                                                };
                                }

                                itemSearchModel.ListSourceItems.SetListSourceItem(listModel);
                            }
                        }
                    }

                    // Set select item result of make
                    if (itemSearchModel != null)
                    {
                        itemSearchModel.ListSourceItems.SetSelectedItem(itemSelect.EquipModelId);
                    }

                    // Generate new data for Grid Feature
                    this.AllRegisteredFeature = new ObservableCollection<RegisteredAssetFeatureRowItem>(listFeatureItems);

                    // Distinct data
                    this.AllRegisteredFeature = new ObservableCollection<RegisteredAssetFeatureRowItem>(this.AllRegisteredFeature.GroupBy(x => x.FeatureName).Select(x => x.First()).ToList());

                    this.UpdateDataForFeatureGrid();

                    this.SearchToolViewModel.ChangeToSearchResult();
                }

            }
        }

        /// <summary>
        /// The selected item changed.
        /// </summary>
        /// <param name="itemComboBox">
        /// The item combo box.
        /// </param>
        private async void ResultSearch_SelectedItemChanged(ItemComboBox itemComboBox)
        {
            ItemEquipSearch itemSearchType = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Type);
            ItemEquipSearch itemSearchMake = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Make);
            ItemEquipSearch itemSearchModel = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Model);

            List<RegisteredAssetFeatureRowItem> listFeatureItems = new List<RegisteredAssetFeatureRowItem>();

            if (itemComboBox != null)
            {
                if (itemSearchType != null)
                {
                    // Generate type items when select category.
                    if (itemComboBox.TypeItem == SystemType.Category)
                    {
                        if (this.IsCheckedOut)
                        {
                            if (this.CurrentSearchResultItem != null && itemComboBox.ItemId != this.CurrentSearchResultItem.EquipCategoryId)
                            {
                                switch (this.CurrentStateStatus)
                                {
                                    case EnumStateAndStatus.ActiveIdle:
                                        if (RegisteredAssetFunction.CheckActiveBookCanChangeWithId(this.RegisterAssetId, itemComboBox.ItemId, SystemType.Category))
                                        {
                                            this.IsChangeBookDepreciation = true;

                                            // Show window to notify if user want to change Category Depreciation Default
                                            ConfirmationWindowView confirm = new ConfirmationWindowView();
                                            ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                                            confirmViewModel.Content =
                                                "Asset Category depreciation defaults may have changed. Do you wish to update the current Asset’s depreciation settings ?";
                                            confirmViewModel.Title = "Confirm Save - Asset Register";
                                            confirmViewModel.UseOkCancel = false;
                                            confirm.DataContext = confirmViewModel;

                                            confirm.ShowDialog();
                                            if (confirm.DialogResult == false)
                                            {
                                                this.IsChangeBookDepreciation = false;
                                            }
                                        }

                                        break;
                                    case EnumStateAndStatus.ActiveReturned:
                                        if (RegisteredAssetFunction.CheckActiveBookCanChangeWithId(this.RegisterAssetId, itemComboBox.ItemId, SystemType.Category))
                                        {
                                            this.IsChangeBookDepreciation = true;
                                            this.ShowMessageAsync(
                                                "Asset Category depreciation defaults may have changed. Update the current Asset’s depreciation settings if required.",
                                                "Register Asset");
                                        }

                                        break;
                                    case EnumStateAndStatus.ActiveLive:
                                        this.ShowMessageAsync(
                                                "Asset Category depreciation defaults may have changed update the current Asset’s depreciation settings if required.",
                                                "Register Asset");
                                        break;
                                    case EnumStateAndStatus.ActiveReserved:
                                        this.ShowMessageAsync(
                                                "Asset Category depreciation defaults may have changed update the current Asset’s depreciation settings if required.",
                                                "Register Asset");
                                        break;
                                    case EnumStateAndStatus.Terminated:
                                        this.ShowMessageAsync(
                                                "Asset Category depreciation defaults may have changed update the current Asset’s depreciation settings if required.",
                                                "Register Asset");
                                        break;
                                }
                            }
                            else
                            {
                                this.IsChangeBookDepreciation = false;
                            }
                        }

                        if (itemComboBox.ItemId == 0)
                        {
                            itemSearchType.ListSourceItems.ComboBoxItemList = new ObservableCollection<ItemComboBox>
                                                                            {
                                                                               new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Type }
                                                                            };
                            if (itemSearchMake != null)
                            {
                                itemSearchMake.ListSourceItems.ComboBoxItemList = new ObservableCollection<ItemComboBox>
                                                                            {
                                                                                new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Make }
                                                                            };
                                itemSearchMake.ListSourceItems.SetSelectedItem(0);
                            }

                            if (itemSearchModel != null)
                            {
                                itemSearchModel.ListSourceItems.ComboBoxItemList = new ObservableCollection<ItemComboBox>
                                                                            {
                                                                                new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Model }
                                                                            };

                                itemSearchModel.ListSourceItems.SetSelectedItem(0);
                            }
                        }
                        else
                        {
                            itemSearchType.ListSourceItems.ComboBoxItemList = new ObservableCollection<ItemComboBox>(await RegisteredAssetFunction.GetTypeAssignByCategory(itemComboBox.ItemId));
                            itemSearchType.ListSourceItems.ComboBoxItemList.Insert(0, new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Type });

                            // Get Features items for Grid feature
                            listFeatureItems.AddRange((await RegisteredAssetFunction.GetCategoryFeatureItem(itemComboBox.ItemId)).ConvertAll(x => new RegisteredAssetFeatureRowItem
                            {
                                FeatureId = x.FeatureTypeId,
                                FeatureName = x.FeatureName,
                            }));
                        }

                        itemSearchType.ListSourceItems.SetSelectedItem(0);

                        // Update data for grid Feature
                        this.AllRegisteredFeature = new ObservableCollection<RegisteredAssetFeatureRowItem>(listFeatureItems);
                        this.UpdateDataForFeatureGrid();
                    }

                        // Generate make items when select type.
                        if (itemComboBox.TypeItem == SystemType.Type)
                        {
                            if (this.IsCheckedOut)
                            {
                                if (this.CurrentSearchResultItem != null && itemComboBox.ItemId != this.CurrentSearchResultItem.EquipCategoryId)
                                {
                                    switch (this.CurrentStateStatus)
                                    {
                                        case EnumStateAndStatus.ActiveIdle:
                                            if (RegisteredAssetFunction.CheckActiveBookCanChangeWithId(this.RegisterAssetId, itemComboBox.ItemId, SystemType.Type))
                                            {
                                                this.IsChangeBookDepreciation = true;

                                                // Show window to notify if user want to change Category Depreciation Default
                                                ConfirmationWindowView confirm = new ConfirmationWindowView();
                                                ConfirmmationViewModel confirmViewModel = new ConfirmmationViewModel();
                                                confirmViewModel.Content = "Asset Type depreciation defaults may have changed. Do you wish to update the current Asset’s depreciation settings ?";
                                                confirmViewModel.Title = "Confirm Save - Asset Register";
                                                confirmViewModel.UseOkCancel = false;
                                                confirm.DataContext = confirmViewModel;

                                                confirm.ShowDialog();
                                                if (confirm.DialogResult == false)
                                                {
                                                    this.IsChangeBookDepreciation = false;
                                                }
                                            }

                                            break;
                                        case EnumStateAndStatus.ActiveReturned:
                                            if (RegisteredAssetFunction.CheckActiveBookCanChangeWithId(this.RegisterAssetId, itemComboBox.ItemId, SystemType.Type))
                                            {
                                                this.IsChangeBookDepreciation = true;
                                                this.ShowMessageAsync(
                                                    "Asset Type depreciation defaults may have changed. Update the current Asset’s depreciation settings if required.",
                                                    "Register Asset");
                                            }

                                            break;
                                        case EnumStateAndStatus.ActiveLive:
                                            this.ShowMessageAsync(
                                                    "Asset Type depreciation defaults may have changed update the current Asset’s depreciation settings if required.",
                                                    "Register Asset");
                                            break;
                                        case EnumStateAndStatus.ActiveReserved:
                                            this.ShowMessageAsync(
                                                    "Asset Type depreciation defaults may have changed update the current Asset’s depreciation settings if required.",
                                                    "Register Asset");
                                            break;
                                        case EnumStateAndStatus.Terminated:
                                            this.ShowMessageAsync(
                                                    "Asset Type depreciation defaults may have changed update the current Asset’s depreciation settings if required.",
                                                    "Register Asset");
                                            break;
                                    }
                                }
                                else
                                {
                                    this.IsChangeBookDepreciation = false;
                                }
                            }

                            if (itemComboBox.ItemId == 0)
                            {
                                if (itemSearchMake != null)
                                {
                                    itemSearchMake.ListSourceItems.ComboBoxItemList = new ObservableCollection<ItemComboBox>
                                                                           {
                                                                               new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Make }
                                                                           };
                                    itemSearchMake.ListSourceItems.SetSelectedItem(0);
                                }

                                if (itemSearchModel != null)
                                {
                                    itemSearchModel.ListSourceItems.ComboBoxItemList = new ObservableCollection<ItemComboBox>
                                                                           {
                                                                               new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Model }
                                                                           };
                                    itemSearchModel.ListSourceItems.SetSelectedItem(0);
                                }

                                // Remove Features items for Grid feature
                                listFeatureItems.AddRange((await RegisteredAssetFunction.GetTypeFeatureItem(itemComboBox.ItemId)).ConvertAll(x => new RegisteredAssetFeatureRowItem
                                {
                                    FeatureId = x.FeatureTypeId,
                                    FeatureName = x.FeatureName,
                                }));

                                foreach (var item in listFeatureItems)
                                {
                                    this.AllRegisteredFeature.Remove(this.AllRegisteredFeature.FirstOrDefault(x => x.FeatureId == item.FeatureId));
                                }
                            }
                            else
                            {
                                if (itemSearchMake != null)
                                {
                                    itemSearchMake.ListSourceItems.ComboBoxItemList = new ObservableCollection<ItemComboBox>(await RegisteredAssetFunction.GetMakeAssignByType(itemComboBox.ItemId));
                                    itemSearchMake.ListSourceItems.ComboBoxItemList.Insert(0, new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Make });
                                    itemSearchMake.ListSourceItems.SetSelectedItem(0);
                                }

                                // Get Features items for Grid feature
                                listFeatureItems.AddRange((await RegisteredAssetFunction.GetTypeFeatureItem(itemComboBox.ItemId)).ConvertAll(x => new RegisteredAssetFeatureRowItem
                                {
                                    FeatureId = x.FeatureTypeId,
                                    FeatureName = x.FeatureName,
                                }));

                                foreach (var item in listFeatureItems)
                                {
                                    this.AllRegisteredFeature.Add(item);
                                }

                                // Distinct data
                                this.AllRegisteredFeature = new ObservableCollection<RegisteredAssetFeatureRowItem>(this.AllRegisteredFeature.GroupBy(x => x.FeatureName).Select(x => x.First()).ToList());
                            }

                            // Update data for grid Feature
                            this.UpdateDataForFeatureGrid();
                    }

                    if (itemSearchModel != null)
                    {
                        // Generate model items when select make.
                        if (itemComboBox.TypeItem == SystemType.Make)
                        {
                            if (itemComboBox.ItemId == 0)
                            {
                                itemSearchModel.ListSourceItems.ComboBoxItemList = new ObservableCollection<ItemComboBox>
                                                                                {
                                                                                    new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Model }
                                                                                };
                            }
                            else
                            {
                                itemSearchModel.ListSourceItems.ComboBoxItemList = new ObservableCollection<ItemComboBox>(await RegisteredAssetFunction.GetModelAssignByMake(itemComboBox.ItemId));
                                itemSearchModel.ListSourceItems.ComboBoxItemList.Insert(0, new ItemComboBox { ItemId = 0, Name = "<None>", TypeItem = SystemType.Model });
                            }

                            itemSearchModel.ListSourceItems.SetSelectedItem(0);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The registered asset detail view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void ReigsteredAssetDetailViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsCheckedOut)
            {
                if (e.PropertyName.IndexOf("AssetAcquisitionDate", StringComparison.Ordinal) != -1)
                {
                    if (RegisteredAssetFunction.CheckAcquisitionDateWithId(this.RegisterAssetId, this.AssetAcquisitionDate))
                    {
                        this.IsAcquisitionDateChange = true;
                    }
                }

                if (this.OnPropertiesChangedDetail != null)
                {
                    if (e.PropertyName.IndexOf("NetAssetCost", StringComparison.Ordinal) != -1
                        || e.PropertyName.IndexOf("AssetGst", StringComparison.Ordinal) != -1)
                    {
                        this.TotalAssetCost = this.NetAssetCost + this.AssetGst;
                        this.IsTotalAssetCostChange = true;
                    }

                    this.OnPropertiesChangedDetail(sender, e);
                }
                if (this.ListAssetRegisters != null && this.SelectRegisterDefault != null)
                {
                    if (e.PropertyName.IndexOf("SelectRegisterDefault", StringComparison.Ordinal) != -1)
                    {
                        
                        if (this.SelectedRegisteredAsset != null)
                        {
                            await this.GetDataForReportCompanyComboBox(this.SelectRegisterDefault.ID, this.DynamicComboBoxReportCompany.SelectedItem.ItemId);
                            await this.GetDataForLocationComboBox(this.SelectRegisterDefault.ID, this.SelectedRegisteredAsset.AssetRegisterLocationId);  
                        }
                        else
                        {
                            await this.GetDataForLocationComboBox(this.SelectRegisterDefault.ID, -1);
                            await this.GetDataForReportCompanyComboBox(this.SelectRegisterDefault.ID, -1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The get data for report company combo box.
        /// </summary>
        /// <param name="itemId">
        /// The item id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetDataForReportCompanyComboBox(int itemId, int selectedId)
        {
            int reportDefaultCompanyId;

            if (AssetRegisterFunction.GetCategory() == 5
                && await Authorisation.IsModuleInstalledAsync(Modules.GLModule))
            {
                this.ListEntityRelation =
                    new ObservableCollection<AssetRelationRowItem>(await AssetRegisterFunction.GetListFinancial(itemId));
                if (ListEntityRelation.Any(x => x.NodeId == selectedId && x.IsSelected))
                {
                    reportDefaultCompanyId = selectedId;
                }
                else
                {
                    reportDefaultCompanyId = AssetRegisterFunction.GetDefaultReportingCompany(itemId, 5);
                }

                this.ListEntityRelation.Insert(
                    0,
                    new AssetRelationRowItem
                        {
                            NodeId = -1,
                            NodeName = "<None>",
                        });
                this.ReportCompanyName = "Financier";
                
            }
            else
            {
                this.ListEntityRelation =
                    new ObservableCollection<AssetRelationRowItem>(
                        await AssetRegisterFunction.GetListInternalCompany(itemId));
                if (ListEntityRelation.Any(x => x.NodeId == selectedId && x.IsSelected))
                {
                    reportDefaultCompanyId = selectedId;
                }
                else
                {
                    reportDefaultCompanyId = AssetRegisterFunction.GetDefaultReportingCompany(itemId, 7);
                }
                this.ListEntityRelation.Insert(
                    0,
                    new AssetRelationRowItem
                    {
                        NodeId = -1,
                        NodeName = "<None>",
                    });
                this.ReportCompanyName = "Internal Company";
            }

            ObservableCollection<ItemComboBox> comboItemReportCompany = new ObservableCollection<ItemComboBox>();

            foreach (var report in this.ListEntityRelation)
            {
                comboItemReportCompany.Add(new ItemComboBox
                {
                    ItemId = report.NodeId,
                    Name = report.NodeName,
                    IsChecked = report.IsSelected
                });
            }

            this.DynamicComboBoxReportCompany.ComboBoxItemList = comboItemReportCompany;

            this.DynamicComboBoxReportCompany.SelectedItem = this.DynamicComboBoxReportCompany.ComboBoxItemList.FirstOrDefault(x => x.ItemId.Equals(reportDefaultCompanyId));
            if (this.DynamicComboBoxReportCompany.SelectedItem == null)
            {
                this.DynamicComboBoxReportCompany.SelectedItem = this.DynamicComboBoxReportCompany.ComboBoxItemList.FirstOrDefault(x => x.ItemId == -1);
            }
        }

        /// <summary>
        /// The get data for location combo box.
        /// </summary>
        /// <param name="itemId">
        /// The item id.
        /// </param>
        /// <param name="itemIdDefault">
        /// The item id default.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetDataForLocationComboBox(int itemId, int? itemIdDefault)
        {
            this.ListLocations =
                    new ObservableCollection<AssetRegisterLocationRowItem>(
                        await AssetRegisterFunction.GetLocationsForRegister(itemId));
            this.ListLocations.Insert(
           0,
           new AssetRegisterLocationRowItem
           {
               AssetRegisterLocationID = -1,
               LocationName = "<None>",
           });
            this.SelectLocationDefault = this.ListLocations.FirstOrDefault(x => x.AssetRegisterLocationID.Equals(itemIdDefault));
        }

        /// <summary>
        /// The update contract asset features.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task ValidateAgainstPpsrRegistrationContractAsset()
        {
            if (await Authorisation.IsModuleInstalledAsync(Modules.PPSR))
            {
                // MappedFieldID from PPSRContractAsset 
                List<PPSRContractAsset> ppsrContractAssetList = await RegisteredAssetFunction.GetPpsrContractAsset(this.RegisterAssetId);
                if (ppsrContractAssetList != null)
                {
                    if (ppsrContractAssetList.Count(ppsr => ppsr.MappedFieldID != null && this.AllRegisteredFeature.Select(f => f.FeatureId).Contains((int)ppsr.MappedFieldID)) != 0)
                    {
                        string message = "This asset feature is used in a current PPSR registration as the Serial Number. You are required to modify the record in PPSR.";
                        string title = "Registered Asset";
                        this.ShowMessageAsync(message, title);
                    }
                }

                // RegoMappedFieldID from PPSRContractAssetMotorVehicle 
                List<PPSRContractAssetMotorVehicle> ppsrContractAssetMotorVehicleList = await RegisteredAssetFunction.GetPpsrContractAssetMotorVehicle(this.RegisterAssetId);
                if (ppsrContractAssetMotorVehicleList != null)
                {
                    if (ppsrContractAssetMotorVehicleList.Count(ppsr => ppsr.RegoMappedFieldID != null && this.AllRegisteredFeature.Select(f => f.FeatureId).Contains((int)ppsr.RegoMappedFieldID)) != 0)
                    {
                        string message = "This asset feature is used in a current PPSR registration as the Registration for Motor Vehicle. You are required to modify the record in PPSR.";
                        string title = "Registered Asset";
                        this.ShowMessageAsync(message, title);
                    }
                }

                List<PPSRContractAssetAircraft> ppsrContractAssetAircraftList = await RegisteredAssetFunction.GetPpsrContractAssetAircraft(this.RegisterAssetId);
                
                if (ppsrContractAssetAircraftList != null)
                {
                    // NationalityMappedFieldID from PPSRContractAssetAircraft 
                    if (ppsrContractAssetAircraftList.Count(ppsr => ppsr.NationalityMappedFieldID != null && this.AllRegisteredFeature.Select(f => f.FeatureId).Contains((int)ppsr.NationalityMappedFieldID)) != 0)
                    {
                        string message = "This asset feature is used in a current PPSR registration as the Nationality for Aircraft. You are required to modify the record in PPSR.";
                        string title = "Registered Asset";
                        this.ShowMessageAsync(message, title);
                    }

                    // NationalityCodeAndRegoMarkMappedToFieldID from PPSRContractAssetAircraft
                    if (ppsrContractAssetAircraftList.Count(ppsr => ppsr.NationalityCodeAndRegoMarkMappedFieldID != null && this.AllRegisteredFeature.Select(f => f.FeatureId).Contains((int)ppsr.NationalityCodeAndRegoMarkMappedFieldID)) != 0)
                    {
                        string message = "This asset feature is used in a current PPSR registration as the Nationality Code and Rego Mark for Aircraft. You are required to modify the record in PPSR.";
                        string title = "Registered Asset";
                        this.ShowMessageAsync(message, title);
                    }

                    // ManufacturerModelMappedFieldID from PPSRContractAssetAircraft
                    if (ppsrContractAssetAircraftList.Count(ppsr => ppsr.ManufacturerModelMappedFieldID != null && this.AllRegisteredFeature.Select(f => f.FeatureId).Contains((int)ppsr.ManufacturerModelMappedFieldID)) != 0)
                    {
                        string message = "This asset feature is used in a current PPSR registration as the Manufacturer's Model for Aircraft. You are required to modify the record in PPSR.";
                        string title = "Registered Asset";
                        this.ShowMessageAsync(message, title);
                    }

                    // ManufacturerNameMappedFieldID from PPSRContractAssetAircraft
                    if (ppsrContractAssetAircraftList.Count(ppsr => ppsr.ManufacturerNameMappedFieldID != null && this.AllRegisteredFeature.Select(f => f.FeatureId).Contains((int)ppsr.ManufacturerNameMappedFieldID)) != 0)
                    {
                        string message = "This asset feature is used in a current PPSR registration as the Manufacturer's Name for Aircraft. You are required to modify the record in PPSR.";
                        string title = "Registered Asset";
                        this.ShowMessageAsync(message, title);
                    }
                }
            }
        }

        /// <summary>
        /// The validate against ppsr registration quote asset.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task ValidateAgainstPpsrRegistrationQuoteAsset()
        {
            if (await Authorisation.IsModuleInstalledAsync(Modules.PPSR))
            {
                // MappedFieldID from PPSRQuoteAssetProfileAsset 
                List<PPSRQuoteAssetProfileAsset> ppsrQuoteAssetList = await RegisteredAssetFunction.GetPpsrQuoteAssetProfileAsset(this.RegisterAssetId);
                if (ppsrQuoteAssetList != null)
                {
                    if (ppsrQuoteAssetList.Count(ppsr => ppsr.MappedFieldID != null && this.AllRegisteredFeature.Select(f => f.FeatureId).Contains((int)ppsr.MappedFieldID)) != 0)
                    {
                        string message = "This asset feature is used in a current PPSR registration as the Serial Number. You are required to modify the record in PPSR.";
                        string title = "Registered Asset";
                        this.ShowMessageAsync(message, title);
                    }
                }

                // RegoMappedFieldID from PPSRQuoteAssetProfileAssetMotorVehicle 
                List<PPSRQuoteAssetProfileAssetMotorVehicle> ppsrQuoteAssetMotorVehicleList = await RegisteredAssetFunction.GetPpsrQuoteAssetProfileAssetMotorVehicle(this.RegisterAssetId);
                if (ppsrQuoteAssetMotorVehicleList != null)
                {
                    if (ppsrQuoteAssetMotorVehicleList.Count(ppsr => ppsr.RegoMappedFieldID != null && this.AllRegisteredFeature.Select(f => f.FeatureId).Contains((int)ppsr.RegoMappedFieldID)) != 0)
                    {
                        string message = "This asset feature is used in a current PPSR registration as the Registration for Motor Vehicle. You are required to modify the record in PPSR.";
                        string title = "Registered Asset";
                        this.ShowMessageAsync(message, title);
                    }
                }

                List<PPSRQuoteAssetProfileAssetAircraft> ppsrQuoteAssetAircraftList = await RegisteredAssetFunction.GetPpsrQuoteAssetProfileAssetAircraft(this.RegisterAssetId);
                
                if (ppsrQuoteAssetAircraftList != null)
                {
                    // NationalityMappedFieldID from PPSRContractAssetAircraft 
                    if (ppsrQuoteAssetAircraftList.Count(ppsr => ppsr.NationalityMappedFieldID != null && this.AllRegisteredFeature.Select(f => f.FeatureId).Contains((int)ppsr.NationalityMappedFieldID)) != 0)
                    {
                        string message = "This asset feature is used in a current PPSR registration as the Nationality for Aircraft. You are required to modify the record in PPSR.";
                        string title = "Registered Asset";
                        this.ShowMessageAsync(message, title);
                    }

                    // NationalityCodeAndRegoMarkMappedToFieldID from PPSRContractAssetAircraft
                    if (ppsrQuoteAssetAircraftList.Count(ppsr => ppsr.NationalityCodeAndREgoMarkMappedToFieldID != null && this.AllRegisteredFeature.Select(f => f.FeatureId).Contains((int)ppsr.NationalityCodeAndREgoMarkMappedToFieldID)) != 0)
                    {
                        string message = "This asset feature is used in a current PPSR registration as the Nationality Code and Rego Mark for Aircraft. You are required to modify the record in PPSR.";
                        string title = "Registered Asset";
                        this.ShowMessageAsync(message, title);
                    }

                    // ManufacturerModelMappedFieldID from PPSRContractAssetAircraft
                    if (ppsrQuoteAssetAircraftList.Count(ppsr => ppsr.ManufacturerModelMappedFieldID != null && this.AllRegisteredFeature.Select(f => f.FeatureId).Contains((int)ppsr.ManufacturerModelMappedFieldID)) != 0)
                    {
                        string message = "This asset feature is used in a current PPSR registration as the Manufacturer's Model for Aircraft. You are required to modify the record in PPSR.";
                        string title = "Registered Asset";
                        this.ShowMessageAsync(message, title);
                    }

                    // ManufacturerNameMappedFieldID from PPSRContractAssetAircraft
                    if (ppsrQuoteAssetAircraftList.Count(ppsr => ppsr.ManufacturerNameMappedFieldID != null && this.AllRegisteredFeature.Select(f => f.FeatureId).Contains((int)ppsr.ManufacturerNameMappedFieldID)) != 0)
                    {
                        string message = "This asset feature is used in a current PPSR registration as the Manufacturer's Name for Aircraft. You are required to modify the record in PPSR.";
                        string title = "Registered Asset";
                        this.ShowMessageAsync(message, title);
                    }
                }
            }
        }

        /// <summary>
        /// The validates required length.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public bool ValidatesRequiredLength()
        {
            return RegisteredAssetFunction.ValidatesRequiredLength(this.GetListFeatureToSave());
        }

        /// <summary>
        /// The validate category combobox.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateCategoryCombobox()
        {
            ItemEquipSearch itemSearchCategory = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Category);
            if (itemSearchCategory != null)
            {
                var selectedItem = itemSearchCategory.ListSourceItems.CurrentName;
                if (string.IsNullOrEmpty(selectedItem))
                {
                    return true;
                }

                if (!itemSearchCategory.ListSourceItems.ComboBoxItemList.Select(a => a.Name)
                                .Contains(selectedItem))
                {
                    itemSearchCategory.ListSourceItems.AddNotifyError(
                        "ComboBoxItemList",
                        "The Category is invalid. Please select an option in the drop-down list.");
                    return false;
                }

                itemSearchCategory.ListSourceItems.RemoveNotifyError("ComboBoxItemList");
            }

            return true;
        }

        /// <summary>
        /// The validate type combobox.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateTypeCombobox()
        {
            ItemEquipSearch itemSearchType = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Type);
            if (itemSearchType != null)
            {
                var selectedItem = itemSearchType.ListSourceItems.CurrentName;
                if (string.IsNullOrEmpty(selectedItem))
                {
                    return true;
                }

                if (!itemSearchType.ListSourceItems.ComboBoxItemList.Select(a => a.Name).Contains(selectedItem))
                {
                    itemSearchType.ListSourceItems.AddNotifyError(
                        "ComboBoxItemList",
                        "The Type is invalid. Please select an option in the drop-down list.");
                    return false;
                }

                itemSearchType.ListSourceItems.RemoveNotifyError("ComboBoxItemList");
            }
            return true;
        }

        /// <summary>
        /// The validate make combobox.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateMakeCombobox()
        {
            ItemEquipSearch itemSearchMake = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Make);
            if (itemSearchMake != null)
            {
                var selectedItem = itemSearchMake.ListSourceItems.CurrentName;
                if (string.IsNullOrEmpty(selectedItem))
                {
                    return true;
                }

                if (!itemSearchMake.ListSourceItems.ComboBoxItemList.Select(a => a.Name).Contains(selectedItem))
                {
                    itemSearchMake.ListSourceItems.AddNotifyError(
                        "ComboBoxItemList",
                        "The Make is invalid. Please select an option in the drop-down list.");
                    return false;
                }

                itemSearchMake.ListSourceItems.RemoveNotifyError("ComboBoxItemList");
            }

            return true;
        }

        /// <summary>
        /// The validate model combobox.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateModelCombobox()
        {
            ItemEquipSearch itemSearchModel = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Model);
            if (itemSearchModel != null)
            {
                var selectedItem = itemSearchModel.ListSourceItems.CurrentName;
                if (string.IsNullOrEmpty(selectedItem))
                {
                    return true;
                }

                if (!itemSearchModel.ListSourceItems.ComboBoxItemList.Select(a => a.Name).Contains(selectedItem))
                {
                    itemSearchModel.ListSourceItems.AddNotifyError(
                        "ComboBoxItemList",
                        "The Model is invalid. Please select an option in the drop-down list.");
                    return false;
                }

                itemSearchModel.ListSourceItems.RemoveNotifyError("ComboBoxItemList");
            }

            return true;
        }

        /// <summary>
        /// The is null selected category.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsNullSelectedCategory()
        {
            ItemEquipSearch itemSearchCategory = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Category);
            if (itemSearchCategory != null)
            {
                var selectedItem = itemSearchCategory.ListSourceItems.CurrentName;

                if (string.IsNullOrEmpty(selectedItem))
                {
                    itemSearchCategory.ListSourceItems.AddNotifyError("ComboBoxItemList", "The Category cannot be null.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// The is null selected type.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsNullSelectedType()
        {
            ItemEquipSearch itemSearchType = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Type);
            if (itemSearchType != null)
            {
                var selectedItem = itemSearchType.ListSourceItems.CurrentName;

                if (string.IsNullOrEmpty(selectedItem))
                {
                    itemSearchType.ListSourceItems.AddNotifyError("ComboBoxItemList", "The Type cannot be null.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// The is null selected make.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsNullSelectedMake()
        {
            ItemEquipSearch itemSearchMake = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Make);
            if (itemSearchMake != null)
            {
                var selectedItem = itemSearchMake.ListSourceItems.CurrentName;
                if (string.IsNullOrEmpty(selectedItem))
                {
                    itemSearchMake.ListSourceItems.AddNotifyError("ComboBoxItemList", "The Make cannot be null.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// The is null selected model.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsNullSelectedModel()
        {
            ItemEquipSearch itemSearchModel = this.SearchToolViewModel.ListItemResultSearch.FirstOrDefault(x => x.ItemType == SystemType.Model);
            if (itemSearchModel != null)
            {
                var selectedItem = itemSearchModel.ListSourceItems.CurrentName;
                if (string.IsNullOrEmpty(selectedItem))
                {
                    itemSearchModel.ListSourceItems.AddNotifyError("ComboBoxItemList", "The Model cannot be null.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// The check select internal company.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckSelectInternalCompany()
        {
            ItemComboBox itemCbReportCompany = this.DynamicComboBoxReportCompany.SelectedItem;
            if(itemCbReportCompany != null)
            {
                if (this.IsGlModuleKey && itemCbReportCompany.ItemId == -1)
                {
                    if (AssetRegisterFunction.GetCategory() == 7)
                    {
                        this.DynamicComboBoxReportCompany.AddNotifyError("ComboBoxItemList", "Internal Company not selected. This is a required field for General Ledger.");
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The check select financer.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckSelectFinancer()
        {
            ItemComboBox itemCbReportCompany = this.DynamicComboBoxReportCompany.SelectedItem;
            if (itemCbReportCompany != null)
            {
                if (this.IsGlModuleKey && itemCbReportCompany.ItemId == -1)
                {
                    if (AssetRegisterFunction.GetCategory() == 5)
                    {
                        this.DynamicComboBoxReportCompany.AddNotifyError("ComboBoxItemList", "Financier not selected. This is a required field for General Ledger");
                        return false;
                    }
                }

                if (this.IsGlModuleKey && itemCbReportCompany.ItemId == -1 && AssetRegisterFunction.GetCategory() == 7)
                {
                    this.DynamicComboBoxReportCompany.RemoveNotifyError("ComboBoxItemList");
                }
            }

            return true;
        }

        /// <summary>
        /// The remove notify error dynamic combo box.
        /// </summary>
        public void RemoveNotifyErrorDynamicComboBox()
        {
            this.DynamicComboBoxReportCompany.RemoveNotifyError("ComboBoxItemList");
        }
        #endregion
    }
}
