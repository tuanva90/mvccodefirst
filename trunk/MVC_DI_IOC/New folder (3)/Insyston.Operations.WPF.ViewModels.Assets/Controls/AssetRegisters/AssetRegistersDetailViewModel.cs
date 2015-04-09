// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetRegistersDetailViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   The asset registers detail view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetRegisters
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
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Assets.Validation;
    using Insyston.Operations.WPF.ViewModels.Common;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model.DynamicGridView;
    using Insyston.Operations.WPF.ViewModels.Common.WindowManager;

    using WPFDynamic.ViewModels.Controls;

    /// <summary>
    /// The asset registers detail view model.
    /// </summary>
    public class AssetRegistersDetailViewModel : ViewModelUseCaseBase
    {
        #region Variables
        /// <summary>
        /// The source image add.
        /// </summary>
        private const string SourceImageAdd =
            @"pack://application:,,,/Insyston.Operations.WPF.Views.Common;component\Images\AddToolbar.png";

        /// <summary>
        /// The source image delete.
        /// </summary>
        private const string SourceImageDelete =
            @"pack://application:,,,/Insyston.Operations.WPF.Views.Common;component\Images\DeleteToolbar.png";

        /// <summary>
        /// The register name.
        /// </summary>
        private string _registerName;

        /// <summary>
        /// The _is internal only.
        /// </summary>
        private bool _isInternalOnly;

        /// <summary>
        /// The _default entity relation.
        /// </summary>
        private vwEntityRelation _defaultEntityRelation;

        /// <summary>
        /// The _company finacial.
        /// </summary>
        private List<vwEntityRelation> _companyFinancial;

        /// <summary>
        /// The _list locations.
        /// </summary>
        private ObservableCollection<AssetRegisterLocationRowItem> _listLocations;

        /// <summary>
        /// The _select defaul.
        /// </summary>
        private AssetRelationRowItem _selectDefault;

        /// <summary>
        /// The _default report name.
        /// </summary>
        private string _defaultReportName;

        /// <summary>
        /// The _assigned report name.
        /// </summary>
        private string _assignedReportName;

        /// <summary>
        /// The _select all report name.
        /// </summary>
        private string _selectAllReportName;

        /// <summary>
        /// The _report name.
        /// </summary>
        private string _reportName;

        /// <summary>
        /// The _list entity relation.
        /// </summary>
        private List<AssetRelationRowItem> _listEntityRelationForGrid;

        /// <summary>
        /// The _list entity relation.
        /// </summary>
        private ObservableCollection<AssetRelationRowItem> _listEntityRelation;

        /// <summary>
        /// The _asset register locations.
        /// </summary>
        private ObservableCollection<AssetRegisterLocationRowItem> _assetRegisterLocations;


        /// <summary>
        /// The _dynamic register assign view model.
        /// </summary>
        private DynamicGridViewModel _dynamicRegisterAssignViewModel;

        /// <summary>
        /// The _dynamic register location view model.
        /// </summary>
        private DynamicGridViewModel _dynamicRegisterLocationViewModel;

        /// <summary>
        /// The _is select all row type.
        /// </summary>
        private bool _isSelectAllRowRegister;

        /// <summary>
        /// The _register id header.
        /// </summary>
        private string _registerIdHeader;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetRegistersDetailViewModel"/> class.
        /// </summary>
        public AssetRegistersDetailViewModel()
        {
            this.Validator = new AssetRegistersViewModelDetailValidation();
            this.PropertyChanged += this.AssetRegisterDetailViewModel_PropertyChanged;

            this.DynamicRegisterLocationViewModel = new DynamicGridViewModel(typeof(AssetRegisterLocationRowItem));
            this.DynamicRegisterLocationViewModel.RowDetailTemplateKey = "DetailLocationRowTemplate";

            this.DynamicRegisterAssignViewModel = new DynamicGridViewModel(typeof(AssetRelationRowItem));
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether is edit mode.
        /// </summary>
        public bool IsEditMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is change checked type.
        /// </summary>
        public bool IsChangeCheckedRegister { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is gl module key on p.
        /// </summary>
        public bool IsGlModuleKeyOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is change item checked type.
        /// </summary>
        public bool IsChangeItemCheckedRegister { get; set; }

        /// <summary>
        /// Gets or sets the locations.
        /// </summary>
        public List<LocationItem> Locations { get; set; }

        /// <summary>
        /// Gets or sets the selected register.
        /// </summary>
        public AssetRegister SelectedRegister { get; set; }

        /// <summary>
        /// Gets or sets the register name.
        /// </summary>
        public string RegisterName
        {
            get
            {
                return this._registerName;
            }

            set
            {
                this.SetField(ref this._registerName, value, () => this.RegisterName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is internal only.
        /// </summary>
        public bool IsInternalOnly
        {
            get
            {
                return this._isInternalOnly;
            }

            set
            {
                this.SetField(ref this._isInternalOnly, value, () => this.IsInternalOnly);
            }
        }

        /// <summary>
        /// Gets or sets the default entity relation.
        /// </summary>
        public vwEntityRelation DefaultEntityRelation
        {
            get
            {
                return this._defaultEntityRelation;
            }

            set
            {
                this.SetField(ref this._defaultEntityRelation, value, () => this.DefaultEntityRelation);
            }
        }

        /// <summary>
        /// Gets or sets the company finacial.
        /// </summary>
        public List<vwEntityRelation> CompanyFinacial
        {
            get
            {
                return this._companyFinancial;
            }

            set
            {
                this.SetField(ref this._companyFinancial, value, () => this.CompanyFinacial);
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
        /// Gets or sets the select default.
        /// </summary>
        public AssetRelationRowItem SelectDefault
        {
            get
            {
                return this._selectDefault;
            }

            set
            {
                this.SetField(ref this._selectDefault, value, () => this.SelectDefault);
            }
        }

        /// <summary>
        /// Gets or sets the default report name.
        /// </summary>
        public string DefaultReportName
        {
            get
            {
                return this._defaultReportName;
            }

            set
            {
                this.SetField(ref this._defaultReportName, value, () => this.DefaultReportName);
            }
        }

        /// <summary>
        /// Gets or sets the assigned report name.
        /// </summary>
        public string AssignedReportName
        {
            get
            {
                return this._assignedReportName;
            }

            set
            {
                this.SetField(ref this._assignedReportName, value, () => this.AssignedReportName);
            }
        }

        /// <summary>
        /// Gets or sets the select all report name.
        /// </summary>
        public string SelectAllReportName
        {
            get
            {
                return this._selectAllReportName;
            }

            set
            {
                this.SetField(ref this._selectAllReportName, value, () => this.SelectAllReportName);
            }
        }

        /// <summary>
        /// Gets or sets the report name.
        /// </summary>
        public string ReportName
        {
            get
            {
                return this._reportName;
            }

            set
            {
                this.SetField(ref this._reportName, value, () => this.ReportName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is new register.
        /// </summary>
        public bool IsNewRegister { get; set; }

        /// <summary>
        /// Gets or sets the list entity relation for grid.
        /// </summary>
        public List<AssetRelationRowItem> ListEntityRelationForGrid
        {
            get
            {
                return this._listEntityRelationForGrid;
            }

            set
            {
                this.SetField(ref this._listEntityRelationForGrid, value, () => this.ListEntityRelationForGrid);
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
        /// Gets or sets the asset register locations.
        /// </summary>
        public ObservableCollection<AssetRegisterLocationRowItem> AssetRegisterLocations
        {
            get
            {
                return this._assetRegisterLocations;
            }

            set
            {
                this.SetField(ref this._assetRegisterLocations, value, () => this.AssetRegisterLocations);
            }
        }

        /// <summary>
        /// Gets or sets the asset register locations.
        /// </summary>
        public List<AssetRegisterLocationRowItem> OldRegisterLocations { get; set; }

        /// <summary>
        /// Gets or sets the dynamic register assign view model.
        /// </summary>
        public DynamicGridViewModel DynamicRegisterAssignViewModel
        {
            get
            {
                return this._dynamicRegisterAssignViewModel;
            }

            set
            {
                this.SetField(ref this._dynamicRegisterAssignViewModel, value, () => this.DynamicRegisterAssignViewModel);
            }
        }

        /// <summary>
        /// Gets or sets the dynamic register location view model.
        /// </summary>
        public DynamicGridViewModel DynamicRegisterLocationViewModel
        {
            get
            {
                return this._dynamicRegisterLocationViewModel;
            }

            set
            {
                this.SetField(ref this._dynamicRegisterLocationViewModel, value, () => this.DynamicRegisterLocationViewModel);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is select all row type.
        /// </summary>
        public bool IsSelectAllRowRegister
        {
            get
            {
                return this._isSelectAllRowRegister;
            }

            set
            {
                this.IsChangeCheckedRegister = true;
                if (!this.IsChangeItemCheckedRegister)
                {
                    this.DynamicRegisterAssignViewModel.IsSelectAllRow = value;
                }

                this.SetField(ref this._isSelectAllRowRegister, value, () => this.IsSelectAllRowRegister);
                this.IsChangeCheckedRegister = false;
            }
        }

        /// <summary>
        /// Gets or sets the register id header.
        /// </summary>
        public string RegisterIdHeader
        {
            get
            {
                return this._registerIdHeader;
            }

            set
            {
                this.SetField(ref this._registerIdHeader, value, () => this.RegisterIdHeader);
            }
        }

        /// <summary>
        /// Gets or sets the id selected register validate.
        /// </summary>
        public int IdSelectedRegisterValidate { get; set; }

        #endregion

        #region Public Method

        /// <summary>
        /// The get data source for add screen.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetDataSourceForAddScreen()
        {
            if (await Security.Authorisation.IsModuleInstalledAsync(Modules.GLModule))
            {
                this.IsGlModuleKeyOn = true;
            }
            else
            {
                this.IsGlModuleKeyOn = false;
            }
            this.IsEditMode = false;
            this.RegisterName = string.Empty;
            this.RegisterIdHeader = string.Empty;
            this.IsInternalOnly = false;
            this.IsNewRegister = true;
            if (AssetRegisterFunction.GetCategory() == 5 && this.IsGlModuleKeyOn)
            {
                this.ListEntityRelation = new ObservableCollection<AssetRelationRowItem>(await AssetRegisterFunction.GetFinancier());
                this.ListEntityRelationForGrid = this.ListEntityRelation.ToList();
                this.ListEntityRelation.Insert(0, new AssetRelationRowItem { NodeName = "<None>", NodeId = -1, });
                this.SelectDefault = this.ListEntityRelation.FirstOrDefault(x => x.NodeId.Equals(-1));
                this.DefaultReportName = "Default Financier";
                this.ReportName = "Financier";
                this.SelectAllReportName = "Select All Financiers";
                this.AssignedReportName = "Assigned Financiers";
            }
            else
            {
                this.ListEntityRelation = new ObservableCollection<AssetRelationRowItem>(await AssetRegisterFunction.GetInternalCompany());
                this.ListEntityRelationForGrid = this.ListEntityRelation.ToList();
                this.ListEntityRelation.Insert(0, new AssetRelationRowItem { NodeName = "<None>", NodeId = -1, });
                this.SelectDefault = this.ListEntityRelation.FirstOrDefault(x => x.NodeId.Equals(-1));
                this.DefaultReportName = "Default Internal Company";
                this.ReportName = "Internal Company";
                this.SelectAllReportName = "Select All Internal Companies";
                this.AssignedReportName = "Assigned Internal Companies";
            }

            // Load locatin grid
            // this.DynamicRegisterAssignViewModel = new DynamicGridViewModel(typeof(AssetRelationRowItem));
            this.DynamicRegisterAssignViewModel.GridColumns = new List<DynamicColumn>
                                                                      {
                                                                          new DynamicColumn
                                                                              {
                                                                                      ColumnName = "NodeName",
                                                                                      Header = this.ReportName.ToUpper(),
                                                                                      IsSelectedColumn = true, Width = 290, MinWidth = 95,
                                                                              },
                                                                      };
            await this.PopulateDataForLocationGridAddNew();
            this.DynamicRegisterAssignViewModel.GridDataRows = this.ListEntityRelationForGrid.ToList<object>();
            this.DynamicRegisterAssignViewModel.IsEnableHoverRow = false;

            this.DynamicRegisterAssignViewModel.LoadRadGridView();
            if (this.DynamicRegisterAssignViewModel.SelectedItems != null && this.DynamicRegisterAssignViewModel.MembersTable.Rows != null
             && this.DynamicRegisterAssignViewModel.SelectedItems.Count() == this.DynamicRegisterAssignViewModel.MembersTable.Rows.Count())
            {
                this._isSelectAllRowRegister = true;
                this.OnPropertyChanged(() => this.IsSelectAllRowRegister);
            }
            else
            {
                this._isSelectAllRowRegister = false;
                this.OnPropertyChanged(() => this.IsSelectAllRowRegister);
            }
        }

        /// <summary>
        /// The get detail data source.
        /// </summary>
        /// <param name="itemSelectedId">
        /// The item selected id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task GetDetailDataSource(int itemSelectedId)
        {
            if (await Security.Authorisation.IsModuleInstalledAsync(Modules.GLModule))
            {
                this.IsGlModuleKeyOn = true;
            }
            else
            {
                this.IsGlModuleKeyOn = false;
            }

            this.IsEditMode = true;
            this.IdSelectedRegisterValidate = itemSelectedId;
            this.SelectedRegister = await AssetRegisterFunction.GetAssetRegisterDetail(itemSelectedId);
            var selectedRegister = this.SelectedRegister;
            if (selectedRegister != null)
            {
                this.RegisterName = selectedRegister.RegisterName;
                this.IsInternalOnly = selectedRegister.InternalOnly;
                this.IsNewRegister = false;
                this.RegisterIdHeader = "Register ID: " + selectedRegister.ID;
                if (AssetRegisterFunction.GetCategory() == 5 && this.IsGlModuleKeyOn)
                {
                    this.ReportName = "Financier";
                    this.DefaultReportName = "Default Financier";
                    this.SelectAllReportName = "Select All Financiers";
                    this.AssignedReportName = "Assigned Financiers";
                    this.DefaultEntityRelation = await AssetRegisterFunction.GetFinancial(selectedRegister.DefaultReportingFinancierNodeID);
                    this.ListEntityRelation = new ObservableCollection<AssetRelationRowItem>(await AssetRegisterFunction.GetListFinancial(selectedRegister.ID));
                    this.ListEntityRelationForGrid = this.ListEntityRelation.ToList();
                    this.ListEntityRelation.Insert(
                        0,
                        new AssetRelationRowItem
                            {
                                NodeName = "<None>",
                                NodeId = -1,
                            });
                    this.SelectDefault = this.ListEntityRelation.FirstOrDefault(x => x.NodeId.Equals(selectedRegister.DefaultReportingFinancierNodeID));
                }
                else
                {
                    this.ReportName = "Internal Company";
                    this.DefaultReportName = "Default Internal Company";
                    this.SelectAllReportName = "Select All Internal Company";
                    this.AssignedReportName = "Assigned Internal Companies";
                    this.DefaultEntityRelation = await AssetRegisterFunction.GetInternalCompany(selectedRegister.DefaultReportingInternalCoyNodeID);
                    this.ListEntityRelation = new ObservableCollection<AssetRelationRowItem>(await AssetRegisterFunction.GetListInternalCompany(selectedRegister.ID));
                    this.ListEntityRelationForGrid = this.ListEntityRelation.ToList();
                    this.ListEntityRelation.Insert(
                        0,
                        new AssetRelationRowItem
                            {
                                NodeName = "<None>",
                                NodeId = -1,
                            });
                    this.SelectDefault = this.ListEntityRelation.FirstOrDefault(x => x.NodeId.Equals(selectedRegister.DefaultReportingInternalCoyNodeID));
                }

                // Load Location Grid
                await this.PopulateDataForLocationGrid(selectedRegister);
            }

            this.DynamicRegisterAssignViewModel.GridColumns = new List<DynamicColumn>
                                                                      {
                                                                          new DynamicColumn
                                                                              {
                                                                                  ColumnName = "NodeName",
                                                                                  Header = this.ReportName.ToUpper(),
                                                                                  IsSelectedColumn = true,
                                                                                  Width = 290,
                                                                                  MinWidth = 95,
                                                                              },
                                                                      };
            this.DynamicRegisterAssignViewModel.GridDataRows = this.ListEntityRelationForGrid.ToList<object>();
            this.DynamicRegisterAssignViewModel.IsEnableHoverRow = false;
            this.DynamicRegisterAssignViewModel.LoadRadGridView();
            if (this.DynamicRegisterAssignViewModel.SelectedItems != null && this.DynamicRegisterAssignViewModel.MembersTable.Rows != null
                && this.DynamicRegisterAssignViewModel.SelectedItems.Count() == this.DynamicRegisterAssignViewModel.MembersTable.Rows.Count())
            {
                this._isSelectAllRowRegister = true;
                this.OnPropertyChanged(() => this.IsSelectAllRowRegister);
            }
            else
            {
                this._isSelectAllRowRegister = false;
                this.OnPropertyChanged(() => this.IsSelectAllRowRegister);
            }
        }

        /// <summary>
        /// The delete item change.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void DeleteItemChange(object item, EventArgs e)
        {
                AssetRegisterLocationRowItem newlocation;
                newlocation = (AssetRegisterLocationRowItem)item;
                if (this.AssetRegisterLocations != null)
                {
                    AssetRegisterLocationRowItem removedItem =
                        this.AssetRegisterLocations.FirstOrDefault(l => l.GuidId == newlocation.GuidId);
                    if (removedItem != null)
                    {
                        this.AssetRegisterLocations.Remove(removedItem);
                    }
                }

            // Behavior for Add toolbar button
            if (this.IsAllLocations())
            {
                // Add button should be collapse when dropdownlist Locations Name don't have any record
                this.HandleAddToolbarButton(false);
            }
            else
            {
                if (!this.IsExistedNewItem())
                {
                    // new record was updated, Add button should be visible
                    this.HandleAddToolbarButton(true);
                }
            }

            // Behavior for Delete toolbar button
            if (this.AssetRegisterLocations != null)
            {
                // No record, Delete toolbar button should be collapse
                if (!this.AssetRegisterLocations.Any())
                {
                    this.HandleDeleteToolbarButton(false);
                }
                else
                {
                    this.HandleDeleteToolbarButton(true);
                }
            }
        }

        /// <summary>
        /// The update item change.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void UpdateItemChange(object item, EventArgs e)
        {
            if (!this.IsNewRegister)
            {
                AssetRegisterLocationRowItem newlocation;
                newlocation = (AssetRegisterLocationRowItem)item;

                // newlocation.IsUpdated = true;
                if (newlocation.IsNewRecord)
                {
                    // Add new record into list 
                    newlocation.GuidId = Guid.NewGuid();
                    newlocation.AssetRegisterID = this.SelectedRegister.ID;
                    newlocation.IsNewRecord = false;

                    // AssetRegisterFunction.AddNewAssetLocationItem(this.SelectedRegister.ID, newlocation);
                    this.AssetRegisterLocations.Add(newlocation);
                }
                else
                {
                    newlocation.AssetRegisterID = this.SelectedRegister.ID;
                    AssetRegisterLocationRowItem editLocation =
                        this.AssetRegisterLocations.FirstOrDefault(x => x.GuidId == newlocation.GuidId);
                    if (editLocation != null)
                    {
                        int i = this.AssetRegisterLocations.IndexOf(editLocation);
                        this.AssetRegisterLocations[i].GuidId = newlocation.GuidId;
                        this.AssetRegisterLocations[i].AssetRegisterLocationID = newlocation.AssetRegisterLocationID;
                        this.AssetRegisterLocations[i].Enabled = newlocation.Enabled;
                    }

                    // AssetRegisterFunction.UpdateLocationItem(newlocation, editLocation);
                }
            }
            else
            {
                AssetRegisterLocationRowItem newlocation;
                newlocation = (AssetRegisterLocationRowItem)item;
                if (this.AssetRegisterLocations == null)
                {
                    this.AssetRegisterLocations = new ObservableCollection<AssetRegisterLocationRowItem>();
                }

                AssetRegisterLocationRowItem editLocation =
                        this.AssetRegisterLocations.FirstOrDefault(x => x.GuidId == newlocation.GuidId);
                if (editLocation != null)
                {
                    int i = this.AssetRegisterLocations.IndexOf(editLocation);
                    this.AssetRegisterLocations[i].GuidId = newlocation.GuidId;
                    this.AssetRegisterLocations[i].AssetRegisterLocationID = newlocation.AssetRegisterLocationID;
                    this.AssetRegisterLocations[i].Enabled = newlocation.Enabled;
                }
                else
                {
                    // Add new record into list 
                    newlocation.GuidId = Guid.NewGuid();
                    newlocation.IsNewRecord = false;

                    this.AssetRegisterLocations.Add(
                        new AssetRegisterLocationRowItem
                        {
                            AssetRegisterLocationID = newlocation.AssetRegisterLocationID,
                            GuidId = newlocation.GuidId,
                            Enabled = newlocation.Enabled,
                        });
                }
            }

            // Behavior for Add toolbar button
            if (this.IsAllLocations())
            {
                // Add button should be collapse when dropdownlist Locations Name don't have any record
                this.HandleAddToolbarButton(false);
            }
            else
            {
                if (!this.IsExistedNewItem())
                {
                    // new record was updated, Add button should be visible
                    this.HandleAddToolbarButton(true);
                }
            }

            // Behavior for Delete toolbar button
            if (this.AssetRegisterLocations != null)
            {
                // No record, Delete toolbar button should be collapse
                if (!this.AssetRegisterLocations.Any())
                {
                    this.HandleDeleteToolbarButton(false);
                }
                else
                {
                    this.HandleDeleteToolbarButton(true);
                }
            }
        }

        /// <summary>
        /// The load data for location grid.
        /// </summary>
        /// <param name="selectedRegister">
        /// The selected register.
        /// </param>
        public async void LoadDataForLocationGrid(AssetRegister selectedRegister)
        {
            await this.PopulateDataForLocationGrid(selectedRegister);
            this.DynamicRegisterLocationViewModel.ToolbarVisibilityChanged =
                       Visibility.Visible;
            this.DynamicRegisterLocationViewModel.IsEnableHoverRow = true;
        }

        /// <summary>
        /// The cancel item change.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void CancelItemChange(object item, EventArgs e)
        {
            AssetRegisterLocationRowItem newlocation;
            newlocation = (AssetRegisterLocationRowItem)item;
            if (newlocation.IsNewRecord)
            {
                // int index = this.DynamicRegisterLocationViewModel.GetIndexOfGrid("GuidID", newlocation.GuidId);

                // Delete new record when click cancel
                this.DynamicRegisterLocationViewModel.DeleteRow(0);
            }

            // Behavior for Add toolbar button
            if (this.IsAllLocations())
            {
                // Add button should be collapse when dropdownlist Locations Name don't have any record
                this.HandleAddToolbarButton(false);
            }
            else
            {
                if (!this.IsExistedNewItem())
                {
                    // new record was updated, Add button should be visible
                    this.HandleAddToolbarButton(true);
                }
            }

            // Behavior for Delete toolbar button
            if (this.AssetRegisterLocations != null)
            {
                // No record, Delete toolbar button should be collapse
                if (!this.AssetRegisterLocations.Any())
                {
                    this.HandleDeleteToolbarButton(false);
                }
                else
                {
                    this.HandleDeleteToolbarButton(true);
                }
            }
        }

        /// <summary>
        /// The validate location record.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateLocationRecord(object item)
        {
            AssetRegisterLocationRowItem newlocation;
            newlocation = (AssetRegisterLocationRowItem)item;
            if (newlocation.AssetRegisterLocationID == 0)
            {
                if (string.IsNullOrEmpty(newlocation.CurrentName))
                {
                    newlocation.AddNotifyError("AssetRegisterLocationID", "You must select a Location to Add/Update.");
                    TelerikWindowManager.Alert("Asset Register Location", "You must select a Location to Add/Update.");
                }
                else
                {
                    if (this.AssetRegisterLocations.Select(x => x.LocationName).Contains(newlocation.CurrentName))
                    {
                        newlocation.AddNotifyError("AssetRegisterLocationID", "Location name has been duplicated.");
                        TelerikWindowManager.Alert("Asset Register Location", "Location name has been duplicated.");
                    }
                    else
                    {
                        newlocation.AddNotifyError("AssetRegisterLocationID", "The location is invalid, please select an option in the drop-down list.");
                        TelerikWindowManager.Alert("Asset Register Location", "The location is invalid, please select an option in the drop-down list.");
                    }
                }
            }
            else
            {
                if (
                    this.AssetRegisterLocations.Select(x => x.AssetRegisterLocationID)
                        .Contains(newlocation.AssetRegisterLocationID) && newlocation.IsNewRecord)
                {
                    newlocation.AddNotifyError("AssetRegisterLocationID", "Location name has been duplicated.");
                    TelerikWindowManager.Alert("Asset Register Location", "Location name has been duplicated.");
                }
                else
                {
                    newlocation.RemoveNotifyError("AssetRegisterLocationID");
                }
            }

            return newlocation.HasErrors;
        }

        /// <summary>
        /// The raise clicked toolbar command grid.
        /// </summary>
        public void RaiseClickedToolbarCommandGrid()
        {
            foreach (var item in this.DynamicRegisterLocationViewModel.ToolbarCommandGridViewModel.CustomToolbarCommands)
            {
                item.ToolbarCommandClicked = this.ClickedToolbarCommandGrid;
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
                confirmViewModel.Title = "Confirm Save - Asset Register";
                confirm.DataContext = confirmViewModel;

                confirm.ShowDialog();
                if (confirm.DialogResult == false)
                {
                    canProceed = false;
                }
            }

            return canProceed;
        }

        #endregion

        #region Protected Methods

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
        /// The populate data for location grid.
        /// </summary>
        /// <param name="selectedRegister">
        /// The selected register.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task PopulateDataForLocationGrid(AssetRegister selectedRegister)
        {
            bool isCanAdd = true;
            bool isCanDelete = true;
            this.Locations = await AssetRegisterFunction.GetLocationsFromSystemParam();

            // Load List locations
            List<AssetRegisterLocationRowItem> assetLocations =
                await AssetRegisterFunction.GetLocationsForRegister(selectedRegister.ID);

            // this.OldRegisterLocations = assetLocations;
            this.AssetRegisterLocations = new ObservableCollection<AssetRegisterLocationRowItem>(assetLocations);
            
            this.OldRegisterLocations = new List<AssetRegisterLocationRowItem>();

            // set data for OldRegisterLocations
            this.AssetRegisterLocations.ForEach(a => this.OldRegisterLocations.Add(new AssetRegisterLocationRowItem
                                                                                       {
                                                                                          AssetRegisterLocationID  = a.AssetRegisterLocationID,
                                                                                          Enabled = a.Enabled,
                                                                                          GuidId = a.GuidId,
                                                                                          AssetRegisterID = a.AssetRegisterID,
                                                                                          Found = a.Found,
                                                                                          IsNewRecord = a.IsNewRecord,
                                                                                          LocationName = a.LocationName,
                                                                                          IsUpdated = a.IsUpdated,
                                                                                          IsExpandDetail = a.IsExpandDetail,
                                                                                          IsSelected = a.IsSelected,
                                                                                          IsRadioSelected = a.IsRadioSelected,
                                                                                          ListLocations = a.ListLocations,
                                                                                       }));
            foreach (var a in this.AssetRegisterLocations)
            {
                a.PropertyChanged += this.AssetRegisterDetailViewModel_PropertyChanged;
            }

            // Check to handle Add and Delete toolbar button
            if (this.AssetRegisterLocations.Count() == this.Locations.Count())
            {
                isCanAdd = false;
            }

            if (!this.AssetRegisterLocations.Any())
            {
                isCanDelete = false;
            }

            // Load Toolbar Command Grid
            // Handle visibility Toolbar Command Grid
            this.DynamicRegisterLocationViewModel.ToolbarCommandGridViewModel = new ToolbarCommandGridViewModel();
            List<CustomToolbarCommand> toolbarCommandCustoms = new List<CustomToolbarCommand>
                                                         {
                                                             new CustomToolbarCommand
                                                                 {
                                                                     SourceImage = SourceImageAdd,
                                                                     Key = EnumToolbarAction.Add,
                                                                     ToolbarCommandName = "Add New Record",
                                                                     ToolbarCommandVisibilityChange = isCanAdd ? Visibility.Visible : Visibility.Collapsed
                                                                 },
                                                             new CustomToolbarCommand
                                                                 {
                                                                     SourceImage = SourceImageDelete,
                                                                     Key = EnumToolbarAction.Delete,
                                                                     ToolbarCommandName = "Delete",
                                                                     ToolbarCommandVisibilityChange = isCanDelete ? Visibility.Visible : Visibility.Collapsed
                                                                 }
                                                       };
            this.DynamicRegisterLocationViewModel.ToolbarCommandGridViewModel.CustomToolbarCommands =
                toolbarCommandCustoms;
            this.DynamicRegisterLocationViewModel.ToolbarVisibilityChanged = Visibility.Collapsed;

            // Load Locations Grid
            this.DynamicRegisterLocationViewModel.IsEnableHoverRow = false;
            this.DynamicRegisterLocationViewModel.IsShowGroupPanel = false;
            this.DynamicRegisterLocationViewModel.RowDetailTemplateKey = "DetailLocationRowTemplate";
            this.DynamicRegisterLocationViewModel.GridColumns = new List<DynamicColumn>
                                                                                             {
                                                                                                 new DynamicColumn { ColumnName = "AssetRegisterLocationID", Header = "ID", MinWidth = 30, TextAlignment = TextAlignment.Center, HeaderTextAlignment = TextAlignment.Center, Width = 100 },
                                                                                                 new DynamicColumn { ColumnName = "LocationName", Header = "LOCATION NAME", MinWidth = 100, ColumnTemplate = ViewModels.Common.Enums.RadGridViewEnum.ColumnEditDelSelectedHoverTemplate, IsSelectedColumn = true, Width = 350, HeaderTextAlignment = TextAlignment.Left },
                                                                                                 new DynamicColumn { ColumnName = "Enabled",  Header = "ENABLED", MinWidth = 100, ColumnTemplate = ViewModels.Common.Enums.RadGridViewEnum.ColumnCheckedTemplate, HeaderTextAlignment = TextAlignment.Center }
                                                                                             };

            this.DynamicRegisterLocationViewModel.GridDataRows = this.AssetRegisterLocations.ToList<object>();
            this.DynamicRegisterLocationViewModel.LoadRadGridView();
            this.DynamicRegisterLocationViewModel.DeletedItemChanged -= this.DeleteItemChange;
            this.DynamicRegisterLocationViewModel.DeletedItemChanged += this.DeleteItemChange;

            this.DynamicRegisterLocationViewModel.UpdatedItemChanged -= this.UpdateItemChange;
            this.DynamicRegisterLocationViewModel.UpdatedItemChanged += this.UpdateItemChange;

            this.DynamicRegisterLocationViewModel.CanceledItemChanged -= this.CancelItemChange;
            this.DynamicRegisterLocationViewModel.CanceledItemChanged += this.CancelItemChange;

            this.DynamicRegisterLocationViewModel.RowDetailLoading -= this.DynamicRegisterLocationViewModelRowDetailLoading;
            this.DynamicRegisterLocationViewModel.RowDetailLoading += this.DynamicRegisterLocationViewModelRowDetailLoading;

            this.RaiseClickedToolbarCommandGrid();
            this.DynamicRegisterLocationViewModel.ValidateRow = this.ValidateLocationRecord;

            this.DynamicRegisterLocationViewModel.AddedNewItem -= this.AddNewItem;
            this.DynamicRegisterLocationViewModel.AddedNewItem += this.AddNewItem;
        }

        /// <summary>
        /// The add new item.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AddNewItem(object sender, EventArgs e)
        {
            // Hide Add button
            this.HandleAddToolbarButton(false);
        }

        /// <summary>
        /// The handle add new item.
        /// </summary>
        /// <param name="isCanAdd">
        /// The is Can Add.
        /// </param>
        private void HandleAddToolbarButton(bool isCanAdd)
        {
            var customToolbarCommand =
                this.DynamicRegisterLocationViewModel.ToolbarCommandGridViewModel.CustomToolbarCommands.FirstOrDefault(
                    a => a.Key == EnumToolbarAction.Add);
            if (customToolbarCommand != null)
            {
                if (!isCanAdd)
                {
                    customToolbarCommand.ToolbarCommandVisibilityChange = Visibility.Collapsed;
                }
                else
                {
                    customToolbarCommand.ToolbarCommandVisibilityChange = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// The handle delete toolbar button.
        /// </summary>
        /// <param name="isCanDelete">
        /// The is can delete.
        /// </param>
        private void HandleDeleteToolbarButton(bool isCanDelete)
        {
            var customToolbarCommand =
                this.DynamicRegisterLocationViewModel.ToolbarCommandGridViewModel.CustomToolbarCommands.FirstOrDefault(
                    a => a.Key == EnumToolbarAction.Delete);
            if (customToolbarCommand != null)
            {
                if (!isCanDelete)
                {
                    customToolbarCommand.ToolbarCommandVisibilityChange = Visibility.Collapsed;
                }
                else
                {
                    customToolbarCommand.ToolbarCommandVisibilityChange = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// The dynamic register location view model_ row detail loading.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DynamicRegisterLocationViewModelRowDetailLoading(object sender, EventArgs e)
        {
            if (this.AssetRegisterLocations != null)
            {
                AssetRegisterLocationRowItem detailItem = sender as AssetRegisterLocationRowItem;
                if (detailItem != null)
                {
                    List<int> selectedLocation =
                        this.AssetRegisterLocations.Select(a => a.AssetRegisterLocationID).ToList();
                    List<LocationItem> locationItems =
                        this.Locations.Where(
                            l =>
                            l.LocationId == detailItem.AssetRegisterLocationID
                            || !selectedLocation.Contains(l.LocationId)).ToList();

                    detailItem.ListLocations = new ObservableCollection<LocationItem>(locationItems);

                    foreach (var row in this.DynamicRegisterLocationViewModel.MembersTable.Rows)
                    {
                        AssetRegisterLocationRowItem item = row.RowObject as AssetRegisterLocationRowItem;
                        if (item != null && item.GuidId != detailItem.GuidId && item.IsExpandDetail && !item.IsNewRecord)
                        {
                            AssetRegisterLocationRowItem preItem =
                                this.AssetRegisterLocations.FirstOrDefault(a => a.GuidId == item.GuidId);

                            List<int> otherSelectedLocation =
                                this.AssetRegisterLocations.Select(a => a.AssetRegisterLocationID).ToList();

                            List<LocationItem> otherLocationItems =
                                this.Locations.Where(
                                    l =>
                                    preItem != null
                                    && (l.LocationId == preItem.AssetRegisterLocationID
                                        || !otherSelectedLocation.Contains(l.LocationId))).ToList();

                            item.ListLocations = new ObservableCollection<LocationItem>(otherLocationItems);
                            if (preItem != null)
                            {
                                item.AssetRegisterLocationID =
                                    !item.ListLocations.Select(a => a.LocationId)
                                         .Contains(item.AssetRegisterLocationID)
                                        ? preItem.AssetRegisterLocationID
                                        : item.AssetRegisterLocationID;
                            }
                        }
                        else
                        {
                            if (item != null && item.IsNewRecord && !detailItem.IsNewRecord)
                            {
                                List<int> otherSelectedLocation =
                                    this.AssetRegisterLocations.Select(a => a.AssetRegisterLocationID).ToList();

                                List<LocationItem> otherLocationItems =
                                this.Locations.Where(
                                    l => !otherSelectedLocation.Contains(l.LocationId)).ToList();

                                item.ListLocations = new ObservableCollection<LocationItem>(otherLocationItems);

                                if (!item.ListLocations.Select(a => a.LocationId).Contains(item.AssetRegisterLocationID))
                                {
                                    item.AssetRegisterLocationID = 0;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The is all locations.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsAllLocations()
        {
            if (this.Locations.Count() == this.AssetRegisterLocations.Count())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The is existed new item.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsExistedNewItem()
        {
            if (this.DynamicRegisterLocationViewModel.MembersTable.Rows.Count != 0)
            {
                var assetRegisterLocationRowItem =
                    this.DynamicRegisterLocationViewModel.MembersTable.Rows[0].RowObject as AssetRegisterLocationRowItem;
                if (assetRegisterLocationRowItem != null && assetRegisterLocationRowItem.IsNewRecord)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The populate data for location grid add new.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task PopulateDataForLocationGridAddNew()
        {
            this.Locations = await AssetRegisterFunction.GetLocationsFromSystemParam();

            this.AssetRegisterLocations = new ObservableCollection<AssetRegisterLocationRowItem>();

            // Load Toolbar Command Grid

            // Handle visibility Toolbar Command Grid
            this.DynamicRegisterLocationViewModel.ToolbarVisibilityChanged = Visibility.Collapsed;
            this.DynamicRegisterLocationViewModel.ToolbarCommandGridViewModel = new ToolbarCommandGridViewModel();
            List<CustomToolbarCommand> toolbarCommandCustoms = new List<CustomToolbarCommand>
                                                         {
                                                             new CustomToolbarCommand
                                                                 {
                                                                     SourceImage = SourceImageAdd,
                                                                     Key = EnumToolbarAction.Add,
                                                                     ToolbarCommandName = "Add New Record",
                                                                     ToolbarCommandVisibilityChange = Visibility.Visible
                                                                 },
                                                             new CustomToolbarCommand
                                                                 {
                                                                     SourceImage = SourceImageDelete,
                                                                     Key = EnumToolbarAction.Delete,
                                                                     ToolbarCommandName = "Delete",
                                                                     ToolbarCommandVisibilityChange = Visibility.Collapsed
                                                                 }
                                                       };
            this.DynamicRegisterLocationViewModel.ToolbarCommandGridViewModel.CustomToolbarCommands =
                toolbarCommandCustoms;

            this.DynamicRegisterLocationViewModel.IsEnableHoverRow = false;
            this.DynamicRegisterLocationViewModel.IsShowGroupPanel = false;
            this.DynamicRegisterLocationViewModel.ToolbarVisibilityChanged = Visibility.Visible;
            this.DynamicRegisterLocationViewModel.RowDetailTemplateKey = "DetailLocationRowTemplate";
            this.DynamicRegisterLocationViewModel.GridColumns = new List<DynamicColumn>
                                                                                             {
                                                                                                 new DynamicColumn { ColumnName = "AssetRegisterLocationID", Header = "ID", MinWidth = 30, TextAlignment = TextAlignment.Center, HeaderTextAlignment = TextAlignment.Center, Width = 100 },
                                                                                                 new DynamicColumn { ColumnName = "LocationName", Header = "LOCATION NAME", MinWidth = 100, ColumnTemplate = ViewModels.Common.Enums.RadGridViewEnum.ColumnEditDelSelectedHoverTemplate, IsSelectedColumn = true, Width = 350, HeaderTextAlignment = TextAlignment.Left },
                                                                                                 new DynamicColumn { ColumnName = "Enabled",  Header = "ENABLED", MinWidth = 100, ColumnTemplate = ViewModels.Common.Enums.RadGridViewEnum.ColumnCheckedTemplate, HeaderTextAlignment = TextAlignment.Center }
                                                                                             };
            this.DynamicRegisterLocationViewModel.GridDataRows = this.AssetRegisterLocations.ToList<object>();

            this.DynamicRegisterLocationViewModel.LoadRadGridView();
            this.DynamicRegisterLocationViewModel.DeletedItemChanged -= this.DeleteItemChange;
            this.DynamicRegisterLocationViewModel.DeletedItemChanged += this.DeleteItemChange;

            this.DynamicRegisterLocationViewModel.UpdatedItemChanged -= this.UpdateItemChange;
            this.DynamicRegisterLocationViewModel.UpdatedItemChanged += this.UpdateItemChange;

            this.DynamicRegisterLocationViewModel.CanceledItemChanged -= this.CancelItemChange;
            this.DynamicRegisterLocationViewModel.CanceledItemChanged += this.CancelItemChange;

            this.DynamicRegisterLocationViewModel.RowDetailLoading -= this.DynamicRegisterLocationViewModelRowDetailLoading;
            this.DynamicRegisterLocationViewModel.RowDetailLoading += this.DynamicRegisterLocationViewModelRowDetailLoading;

            this.RaiseClickedToolbarCommandGrid();
            this.DynamicRegisterLocationViewModel.ValidateRow = this.ValidateLocationRecord;

            this.DynamicRegisterLocationViewModel.AddedNewItem -= this.AddNewItem;
            this.DynamicRegisterLocationViewModel.AddedNewItem += this.AddNewItem;
        }

        /// <summary>
        /// The clicked toolbar command grid. Handle Step of Toolbar Command Grid
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        private void ClickedToolbarCommandGrid(object item)
        {
            CustomToolbarCommand customToolbarCommand;
            customToolbarCommand = (CustomToolbarCommand)item;
            if (customToolbarCommand.Key == EnumToolbarAction.Delete)
            {
                var selectedItems = this.DynamicRegisterLocationViewModel.SelectedItems;
                if (selectedItems != null)
                {
                    var items = new ObservableCollection<AssetRegisterLocationRowItem>(
                        selectedItems.Cast<AssetRegisterLocationRowItem>());

                    // Behavior for Add toolbar button
                    // Delete data on grid
                    if (items.Count != 0)
                    {
                        List<AssetRegisterLocationRowItem> deleteLocations =
                            this.AssetRegisterLocations.Where(x => items.Select(a => a.GuidId).ToList().Contains(x.GuidId)).ToList();

                        deleteLocations.ForEach(deleteLocation => this.AssetRegisterLocations.Remove(deleteLocation));

                        foreach (var item1 in items)
                        {
                            int index = this.DynamicRegisterLocationViewModel.GetIndexOfGrid("GuidId", item1.GuidId);
                            if (index != -1)
                            {
                                this.DynamicRegisterLocationViewModel.DeleteRow(index);
                            }
                        }

                        if (this.IsAllLocations())
                        {
                            // Add button should be collapse when dropdownlist Locations Name don't have any record
                            this.HandleAddToolbarButton(false);
                        }
                        else
                        {
                            if (!this.IsExistedNewItem())
                            {
                                // new record was updated, Add button should be visible
                                this.HandleAddToolbarButton(true);
                            }
                        }
                    }
                }

                // Behavior for Delete toolbar button
                if (this.AssetRegisterLocations != null)
                {
                    // No record, Delete toolbar button should be collapse
                    if (!this.AssetRegisterLocations.Any())
                    {
                        this.HandleDeleteToolbarButton(false);
                    }
                    else
                    {
                        this.HandleDeleteToolbarButton(true);
                    }
                }
            }
            else
            {
                if (customToolbarCommand.Key == EnumToolbarAction.Add)
                {
                    AssetRegisterLocationRowItem newLocation = new AssetRegisterLocationRowItem
                                                                   {
                                                                       IsExpandDetail = true,
                                                                       IsNewRecord = true,
                                                                   };
                    this.DynamicRegisterLocationViewModel.InsertRow(0, newLocation);
                }
            }
        }

        /// <summary>
        /// The asset register detail view model_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AssetRegisterDetailViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsCheckedOut
                && ((e.PropertyName.IndexOf("RegisterName", StringComparison.Ordinal) != -1)
                || (e.PropertyName.IndexOf("SelectDefault", StringComparison.Ordinal) != -1)
                || (e.PropertyName.IndexOf("IsInternalOnly", StringComparison.Ordinal) != -1)
                || (e.PropertyName.IndexOf(
                    "DynamicRegisterAssignViewModel.IsCheckItemChanged",
                    StringComparison.Ordinal) != -1)
                || (e.PropertyName.IndexOf("AssetRegisterLocations", StringComparison.Ordinal) != -1)
                || (e.PropertyName.IndexOf("AssetRegisterLocationID", StringComparison.Ordinal) != -1)))
            {
                this.IsChanged = true;
            }

            if (this.IsCheckedOut && (e.PropertyName.IndexOf("DynamicRegisterAssignViewModel.IsCheckItemChanged", StringComparison.Ordinal) != -1) && !this.IsChangeCheckedRegister)
            {
                this.IsChangeItemCheckedRegister = true;
                if (this.DynamicRegisterAssignViewModel.SelectedItems.Count() != this.DynamicRegisterAssignViewModel.MembersTable.Rows.Count())
                {
                    this.IsSelectAllRowRegister = false;
                }
                else
                {
                    this.IsSelectAllRowRegister = true;
                }

                this.IsChangeItemCheckedRegister = false;
            }
        }

        #endregion
    }
}
