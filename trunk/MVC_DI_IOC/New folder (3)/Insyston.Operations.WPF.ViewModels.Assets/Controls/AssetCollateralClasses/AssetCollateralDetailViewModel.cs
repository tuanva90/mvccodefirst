// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetCollateralDetailViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the AssetRegistersViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetCollateralClasses
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    using Insyston.Operations.Bussiness.Assets;
    using Insyston.Operations.Bussiness.Assets.AssetCollateralClasses;
    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    /// <summary>
    /// The asset collateral detail view model.
    /// </summary>
    public class AssetCollateralDetailViewModel : ViewModelUseCaseBase
    {
        #region Variables

        /// <summary>
        /// The _list collateral detail.
        /// </summary>
        private ObservableCollection<ItemCollateralClassViewModel> _listCollateralDetail;

        /// <summary>
        /// The mapped to system constant i ds.
        /// </summary>
        private readonly List<int> _mappedToSystemConstantIDs = new List<int> { 630, 631, 632 };

        /// <summary>
        /// The motor vehicle rego mapped to system constant ids.
        /// </summary>
        private readonly List<int> _motorVehicleRegoMappedToSystemConstantIDs = new List<int> { 630, 631, 632 };

        /// <summary>
        /// The aircraft nationality mapped to system constant i ds.
        /// </summary>
        private readonly List<int> _aircraftNationalityMappedToSystemConstantIDs = new List<int> { 630, 631 };

        /// <summary>
        /// The aircraft nationality code and rego mark system constant i ds.
        /// </summary>
        private readonly List<int> _aircraftNationalityCodeAndRegoMarkSystemConstantIDs = new List<int> { 630, 631 };

        /// <summary>
        /// The aircraft manufacturer model mapped constant i ds.
        /// </summary>
        private readonly List<int> _aircraftManufacturerModelMappedConstantIDs = new List<int> { 630, 631, 647 };

        /// <summary>
        /// The aircraft manufacturer name mapped to constant i ds.
        /// </summary>
        private readonly List<int> _aircraftManufacturerNameMappedToConstantIDs = new List<int> { 630, 631, 648 };

        /// <summary>
        /// The list show up.
        /// </summary>
        private readonly List<int> _listShowUp = new List<int> { 604, 605, 606, 607, 608 };

        /// <summary>
        /// The _collateral name.
        /// </summary>
        private string _collateralName;

        #endregion

        #region Constructor

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the collateral name.
        /// </summary>
        public string CollateralName
        {
            get
            {
                return this._collateralName;
            }

            set
            {
                this.SetField(ref this._collateralName, value, () => this.CollateralName);
            }
        }

        /// <summary>
        /// Gets or sets the list collateral detail.
        /// </summary>
        public ObservableCollection<ItemCollateralClassViewModel> ListCollateralDetail
        {
            get
            {
                return this._listCollateralDetail;
            }

            set
            {
                this.SetField(ref this._listCollateralDetail, value, () => this.ListCollateralDetail);
            }
        }
        #endregion

        #region Public Method

        /// <summary>
        /// The populate collateral classes.
        /// </summary>
        /// <param name="selectedCollateral">
        /// The selected collateral.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        // ReSharper disable once CSharpWarnings::CS1998
        public async Task PopulateCollateralClasses(PPSRCollateralClass selectedCollateral)
        {
            if (selectedCollateral != null)
            {
                ObservableCollection<AssetCollateralItemDetail> listRegs = this.GetListRegistrationEndDate();
                ObservableCollection<AssetCollateralItemDetail> listMappedTos = this.GetListMappedTo(this._mappedToSystemConstantIDs);
                ObservableCollection<AssetCollateralItemDetail> listMotorVehicleRegos = this.GetListMappedTo(this._motorVehicleRegoMappedToSystemConstantIDs);
                ObservableCollection<AssetCollateralItemDetail> listAircrafts = this.GetListMappedTo(this._aircraftNationalityMappedToSystemConstantIDs);
                ObservableCollection<AssetCollateralItemDetail> listAircraftsCodeAndRegoMarks =
                    this.GetListMappedTo(this._aircraftNationalityCodeAndRegoMarkSystemConstantIDs);
                ObservableCollection<AssetCollateralItemDetail> listAircraftManufacturerModels = this.GetListMappedTo(this._aircraftManufacturerModelMappedConstantIDs);
                ObservableCollection<AssetCollateralItemDetail> listAircraftManufacturerNames = this.GetListMappedTo(this._aircraftManufacturerNameMappedToConstantIDs);
                ObservableCollection<AssetCollateralItemDetail> listSerials = this.GetListSerialNumberType(selectedCollateral.CollateralClassID);
                this.ListCollateralDetail = new ObservableCollection<ItemCollateralClassViewModel>();
                this.ListCollateralDetail.Add(new ItemCollateralClassViewModel
                        {
                            IsShowUp = true,
                            CollateralFieldID = Asset.CollateralFieldID.RegistrationEndDate,
                            Header = "Registration End Date : ",
                            ListComboBox = listRegs,
                            SelectComboBox = listRegs != null && selectedCollateral.RegoEndDateID != null
                                            ? listRegs.FirstOrDefault(x => x.ItemId == selectedCollateral.RegoEndDateID) 
                                            : listRegs.FirstOrDefault(d => d.ItemId == -1)
                        });
                this.ListCollateralDetail.Add(new ItemCollateralClassViewModel
                        {
                            IsShowUp = this.CheckSerialNumberRequired(selectedCollateral.CollateralClassID),
                            CollateralFieldID = Asset.CollateralFieldID.SerialNumberType,
                            Header = "Serial Number Type : ",
                            ListComboBox = listSerials,
                            SelectComboBox = listSerials != null && selectedCollateral.SerialNumberTypeID != null 
                                            ? listSerials.FirstOrDefault(x => x.ItemId == selectedCollateral.SerialNumberTypeID) 
                                            : listSerials.FirstOrDefault(d => d.ItemId == -1)
                        });
                this.ListCollateralDetail.Add(new ItemCollateralClassViewModel
                        {
                            IsShowUp = this.CheckSerialNumberRequired(selectedCollateral.CollateralClassID),
                            CollateralFieldID = Asset.CollateralFieldID.MappedTo,
                            Header = "Mapped To : ",
                            ListComboBox = listMappedTos,
                            ListMultiItem = this.GetListMultiItemMappedTo(selectedCollateral),
                            SelectComboBox = listMappedTos != null && selectedCollateral.MappedToID != null 
                                            ? listMappedTos.FirstOrDefault(x => x.ItemId == selectedCollateral.MappedToID) 
                                            : listMappedTos.FirstOrDefault(d => d.ItemId == -1)
                        });
                this.ListCollateralDetail.Add(new ItemCollateralClassViewModel
                        {
                            IsShowUp = !this.CheckSerialNumberRequired(selectedCollateral.CollateralClassID),
                            CollateralFieldID = Asset.CollateralFieldID.AppendedTo,
                            Header = "Append To Description : ",
                            ListComboBox = listMappedTos,
                            ListMultiItem = this.GetListMultiItemAppendTo(selectedCollateral),
                            SelectComboBox = listMappedTos != null && selectedCollateral.MappedToID != null 
                                            ? listMappedTos.FirstOrDefault(x => x.ItemId == selectedCollateral.MappedToID) 
                                            : listMappedTos.FirstOrDefault(d => d.ItemId == -1)
                        });
                this.ListCollateralDetail.Add(new ItemCollateralClassViewModel
                        {
                            IsShowUp = selectedCollateral.CollateralClassID == 626,
                            CollateralFieldID = Asset.CollateralFieldID.MotorVehicleRegoMappedTo,
                            Header = "Motor Vehicle Rego Mapped To : ",
                            ListComboBox = listMotorVehicleRegos,
                            ListMultiItem = this.GetListMultiItemMotorVehicleRegoMappedTo(selectedCollateral.MotorVehicleRego),
                            SelectComboBox = listMotorVehicleRegos != null && selectedCollateral.MotorVehicleRego != null 
                                            && selectedCollateral.MotorVehicleRego.RegoMappedTo != null
                                             ? listMotorVehicleRegos.FirstOrDefault(x => x.ItemId == selectedCollateral.MotorVehicleRego.RegoMappedTo) 
                                             : listMotorVehicleRegos.FirstOrDefault(d => d.ItemId == -1)
                        });
                this.ListCollateralDetail.Add(new ItemCollateralClassViewModel
                        {
                            IsShowUp = this._listShowUp.Contains(selectedCollateral.CollateralClassID),
                            CollateralFieldID = Asset.CollateralFieldID.AircraftNationalityMappedTo,
                            Header = "Aircraft Nationality Mapped To : ",
                            ListComboBox = listAircrafts,
                            ListMultiItem = this.GetListMultiItemAircraftNationalityMappedTo(selectedCollateral.Aircraft),
                            SelectComboBox = listAircrafts != null && selectedCollateral.Aircraft != null 
                                            && selectedCollateral.Aircraft.NationalityMappedTo != null 
                                            ? listAircrafts.FirstOrDefault(x => x.ItemId == selectedCollateral.Aircraft.NationalityMappedTo) 
                                            : listAircrafts.FirstOrDefault(d => d.ItemId == -1)
                        });
                this.ListCollateralDetail.Add(
                    new ItemCollateralClassViewModel
                        {
                            IsShowUp =
                                this._listShowUp.Contains(
                                    selectedCollateral.CollateralClassID),
                            CollateralFieldID = Asset.CollateralFieldID.AircraftNationalityCodeRegoMarkMappedTo,
                            Header = "Aircraft Nationality Code and Rego Mark Mapped To : ",
                            ListComboBox = listAircraftsCodeAndRegoMarks,
                            ListMultiItem =
                                this
                                .GetListMultiItemAircraftNationalityCodeAndRegoMarkMappedTo
                                (selectedCollateral.Aircraft),
                            SelectComboBox =
                                listAircraftsCodeAndRegoMarks != null
                                && selectedCollateral.Aircraft != null && selectedCollateral.Aircraft.NationalityCodeAndRegoMarkMappedTo != null
                                    ? listAircraftsCodeAndRegoMarks.FirstOrDefault(
                                        x =>
                                        x.ItemId
                                        == selectedCollateral.Aircraft
                                               .NationalityCodeAndRegoMarkMappedTo)
                                    : listAircraftsCodeAndRegoMarks.FirstOrDefault(d => d.ItemId == -1)
                        });
                this.ListCollateralDetail.Add(new ItemCollateralClassViewModel
                        {
                            IsShowUp = this._listShowUp.Contains(selectedCollateral.CollateralClassID),
                            CollateralFieldID = Asset.CollateralFieldID.AircraftManufacturerModelMappedTo,
                            Header = "Aircraft Manufacturer’s Model Mapped To : ",
                            ListComboBox = listAircraftManufacturerModels,
                            ListMultiItem = this.GetListMultiItemAircraftManufacturerModelMappedTo(selectedCollateral.Aircraft),
                            SelectComboBox = listAircraftManufacturerModels != null && selectedCollateral.Aircraft != null 
                                            && selectedCollateral.Aircraft.ManufacturerModelMappedTo != null 
                                            ? listAircraftManufacturerModels.FirstOrDefault(x => x.ItemId == selectedCollateral.Aircraft.ManufacturerModelMappedTo) 
                                             : listAircraftManufacturerModels.FirstOrDefault(d => d.ItemId == -1)
                        });
                this.ListCollateralDetail.Add(
                    new ItemCollateralClassViewModel
                        {
                            IsShowUp =
                                this._listShowUp.Contains(
                                    selectedCollateral.CollateralClassID),
                            CollateralFieldID = Asset.CollateralFieldID.AircraftManufacturerNameMappedTo,
                            Header = "Aircraft Manufacturer’s Name Mapped To : ",
                            ListComboBox = listAircraftManufacturerNames,
                            ListMultiItem =
                                this.GetListMultiItemAircraftManufacturerNameMappedTo(
                                    selectedCollateral.Aircraft),
                            SelectComboBox =
                                listAircraftManufacturerNames != null
                                && selectedCollateral.Aircraft != null && selectedCollateral.Aircraft != null
                                    && selectedCollateral.Aircraft
                                               .ManufacturerNameMappedTo != null
                                    ? listAircraftManufacturerNames.FirstOrDefault(
                                        x =>
                                        x.ItemId
                                        == selectedCollateral.Aircraft
                                               .ManufacturerNameMappedTo)
                                    : listAircraftManufacturerNames.FirstOrDefault(d => d.ItemId == -1)
                        });
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
        /// The get list multi item mapped to.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>ObservableCollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        private ObservableCollection<ItemCollateralClassViewModel> GetListMultiItemMappedTo(PPSRCollateralClass item)
        {
            ObservableCollection<AssetCollateralItemDetail> listContractFeatures = this.GetContractFeatures();
            ObservableCollection<AssetCollateralItemDetail> listCustomeFields = this.GetCustomFields();
            ObservableCollection<ItemCollateralClassViewModel> temp = new ObservableCollection<ItemCollateralClassViewModel>
            {
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.ContractMappedField,
                        Header = "Contract Mapped Field : ", 
                        ItemType = 630, 
                        ListComboBox = listContractFeatures,
                        SelectComboBox = listContractFeatures != null && listContractFeatures.FirstOrDefault(x => x.ItemId == item.MappedFieldID) != null
                                                ? listContractFeatures.FirstOrDefault(x => x.ItemId == item.MappedFieldID) 
                                                : listContractFeatures.FirstOrDefault(x => x.ItemId == -1)
                    },
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.ContractMappedField,
                        Header = "Contract Mapped Field : ", 
                        ItemType = 631, 
                        ListComboBox = listCustomeFields,
                        SelectComboBox = listCustomeFields != null && listCustomeFields.FirstOrDefault(x => x.ItemId == item.MappedFieldID) != null 
                                            ? listCustomeFields.FirstOrDefault(x => x.ItemId == item.MappedFieldID) 
                                            : listCustomeFields.FirstOrDefault(x => x.ItemId == -1)
                    },
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.QuoteMappedField,
                        Header = "Quote Mapped Field : ", 
                        ItemType = 631, 
                        ListComboBox = listCustomeFields,
                        SelectComboBox = listCustomeFields != null && listCustomeFields.FirstOrDefault(x => x.ItemId == item.QuoteMappedFieldID) != null 
                                            ? listCustomeFields.FirstOrDefault(x => x.ItemId == item.QuoteMappedFieldID) 
                                            : listCustomeFields.FirstOrDefault(x => x.ItemId == -1)
                    }
            };

            return temp;
        }

        /// <summary>
        /// The get list multi item motor vehicle rego mapped to.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>ObservableCollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        private ObservableCollection<ItemCollateralClassViewModel> GetListMultiItemMotorVehicleRegoMappedTo(PPSRCollateralClassMotorVehicle item)
        {
            ObservableCollection<AssetCollateralItemDetail> listContractFeatures = this.GetContractFeatures();
            ObservableCollection<AssetCollateralItemDetail> listCustomeFields = this.GetCustomFields();
            ObservableCollection<ItemCollateralClassViewModel> temp = new ObservableCollection<ItemCollateralClassViewModel>
            {
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.MotorVehicleRegoMappedField,
                        Header = "Motor Vehicle Rego Mapped Field : ", 
                        ItemType = 630, 
                        ListComboBox = listContractFeatures,
                        SelectComboBox = listContractFeatures != null && item != null && item.RegoMappedFieldID != null 
                                        && listCustomeFields.FirstOrDefault(x => x.ItemId == item.RegoMappedFieldID) != null
                                            ? listContractFeatures.FirstOrDefault(x => x.ItemId == item.RegoMappedFieldID) 
                                            : listContractFeatures.FirstOrDefault(x => x.ItemId == -1)
                    },
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.MotorVehicleRegoContractMappedField,
                        Header = "Motor Vehicle Rego Contract Mapped Field : ", 
                        ItemType = 631, 
                        ListComboBox = listCustomeFields,
                        SelectComboBox = listCustomeFields != null && item != null && item.RegoMappedFieldID != null 
                                        && listCustomeFields.FirstOrDefault(x => x.ItemId == item.RegoMappedFieldID) != null
                                            ? listCustomeFields.FirstOrDefault(x => x.ItemId == item.RegoMappedFieldID) 
                                            : listCustomeFields.FirstOrDefault(x => x.ItemId == -1)
                    },
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.MotorVehicleRegoContractMappedField,
                        Header = "Motor Vehicle Rego Contract Mapped Field : ", 
                        ItemType = 630, 
                        ListComboBox = listCustomeFields,
                        SelectComboBox = listCustomeFields != null && item != null && item.RegoMappedFieldID != null
                                            && listCustomeFields.FirstOrDefault(x => x.ItemId == item.RegoMappedFieldID) != null
                                            ? listCustomeFields.FirstOrDefault(x => x.ItemId == item.RegoMappedFieldID) 
                                            : listCustomeFields.FirstOrDefault(x => x.ItemId == -1)
                    },
                new ItemCollateralClassViewModel 
                    { 
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.MotorVehicleRegoQuoteMappedField,
                        Header = "Motor Vehicle Rego Quote Mapped Field : ", 
                        ItemType = 631, 
                        ListComboBox = listCustomeFields,
                        SelectComboBox = listCustomeFields != null && item != null && item.RegoQuoteMappedFieldID != null 
                                            && listCustomeFields.FirstOrDefault(x => x.ItemId == item.RegoQuoteMappedFieldID) != null  
                                            ? listCustomeFields.FirstOrDefault(x => x.ItemId == item.RegoQuoteMappedFieldID) 
                                            : listCustomeFields.FirstOrDefault(x => x.ItemId == -1)
                    }
            };

            return temp;
        }

        /// <summary>
        /// The get list multi item aircraft nationality mapped to.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>ObservableCollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        private ObservableCollection<ItemCollateralClassViewModel> GetListMultiItemAircraftNationalityMappedTo(PPSRCollateralClassAircraft item)
        {
            ObservableCollection<AssetCollateralItemDetail> listContractFeatures = this.GetContractFeatures();
            ObservableCollection<AssetCollateralItemDetail> listCustomeFields = this.GetCustomFields();
            ObservableCollection<ItemCollateralClassViewModel> temp = new ObservableCollection<ItemCollateralClassViewModel>
            {
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.AircraftNationalityMappedField,
                        Header = "Aircraft Nationality Mapped Field : ", 
                        ItemType = 630, 
                        ListComboBox = listContractFeatures,
                        SelectComboBox = listContractFeatures != null && item != null && item.NationalityMappedToFieldID != null 
                                            && listContractFeatures.FirstOrDefault(x => x.ItemId == item.NationalityMappedToFieldID) != null
                                            ? listContractFeatures.FirstOrDefault(x => x.ItemId == item.NationalityMappedToFieldID) 
                                            : listContractFeatures.FirstOrDefault(x => x.ItemId == -1)
                    },
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.AircraftNationalityContractMappedField,
                        Header = "Aircraft Nationality Contract Mapped Field : ", 
                        ItemType = 631, 
                        ListComboBox = listCustomeFields,
                        SelectComboBox = listCustomeFields != null && item != null && item.NationalityMappedToFieldID != null 
                                            && listCustomeFields.FirstOrDefault(x => x.ItemId == item.NationalityMappedToFieldID) != null
                                            ? listCustomeFields.FirstOrDefault(x => x.ItemId == item.NationalityMappedToFieldID) 
                                            : listCustomeFields.FirstOrDefault(x => x.ItemId == -1)
                    },
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.AircraftNationalityQuoteMappedField,
                        Header = "Aircraft Nationality Quote Mapped Field : ",
                        ItemType = 631, 
                        ListComboBox = listCustomeFields,
                        SelectComboBox = listCustomeFields != null && item != null && item.NationalityQuoteMappedToFieldID != null 
                                            && listCustomeFields.FirstOrDefault(x => x.ItemId == item.NationalityQuoteMappedToFieldID) != null
                                            ? listCustomeFields.FirstOrDefault(x => x.ItemId == item.NationalityQuoteMappedToFieldID)   
                                            : listCustomeFields.FirstOrDefault(x => x.ItemId == -1)
                    }
            };

            return temp;
        }

        /// <summary>
        /// The get list multi item aircraft nationality code and rego mark mapped to.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>ObservableCollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        private ObservableCollection<ItemCollateralClassViewModel> GetListMultiItemAircraftNationalityCodeAndRegoMarkMappedTo(PPSRCollateralClassAircraft item)
        {
            ObservableCollection<AssetCollateralItemDetail> listContractFeatures = this.GetContractFeatures();
            ObservableCollection<AssetCollateralItemDetail> listCustomeFields = this.GetCustomFields();

            ObservableCollection<ItemCollateralClassViewModel> temp = new ObservableCollection<ItemCollateralClassViewModel>
            {
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.AircraftNationalityCodeRegoMarkMappedField,
                        Header = "Aircraft Nationality Code and Rego Mark Mapped Field : ", 
                        ItemType = 630, 
                        ListComboBox = listContractFeatures,
                        SelectComboBox = listContractFeatures != null && item != null && item.NationalityCodeAndRegoMarkMappedFieldID != null 
                                            && listContractFeatures.FirstOrDefault(x => x.ItemId == item.NationalityCodeAndRegoMarkMappedFieldID) != null
                                            ? listContractFeatures.FirstOrDefault(x => x.ItemId == item.NationalityCodeAndRegoMarkMappedFieldID) 
                                            : listContractFeatures.FirstOrDefault(x => x.ItemId == -1)
                    },
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.AircraftNationalityCodeRegoMarkContractMappedField,
                        Header = "Aircraft Nationality Code and Rego Mark Contract Mapped Field : ", 
                        ItemType = 631, 
                        ListComboBox = listCustomeFields,
                        SelectComboBox = listCustomeFields != null && item != null && item.NationalityCodeAndRegoMarkMappedFieldID != null 
                                            && listCustomeFields.FirstOrDefault(x => x.ItemId == item.NationalityCodeAndRegoMarkMappedFieldID) != null
                                            ? listCustomeFields.FirstOrDefault(x => x.ItemId == item.NationalityCodeAndRegoMarkMappedFieldID) 
                                            : listCustomeFields.FirstOrDefault(x => x.ItemId == -1)
                    },
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true,
                        CollateralFieldID = Asset.CollateralFieldID.AircraftNationalityCodeRegoMarkQuoteMappedField,
                        Header = "Aircraft Nationality Code and Rego Mark Quote Mapped Field : ", 
                        ItemType = 631, 
                        ListComboBox = listCustomeFields,
                        SelectComboBox = listCustomeFields != null && item != null && item.NationalityCodeAndRegoMarkQuoteMappedFieldID != null 
                                            && listCustomeFields.FirstOrDefault(x => x.ItemId == item.NationalityCodeAndRegoMarkQuoteMappedFieldID) != null
                                            ? listCustomeFields.FirstOrDefault(x => x.ItemId == item.NationalityCodeAndRegoMarkQuoteMappedFieldID) 
                                            : listCustomeFields.FirstOrDefault(x => x.ItemId == -1)
                    }
            };

            return temp;
        }

        /// <summary>
        /// The get list multi item aircraft manufacturer model mapped to.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>ObservableCollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        private ObservableCollection<ItemCollateralClassViewModel> GetListMultiItemAircraftManufacturerModelMappedTo(PPSRCollateralClassAircraft item)
        {
            ObservableCollection<AssetCollateralItemDetail> listContractFeatures = this.GetContractFeatures();
            ObservableCollection<AssetCollateralItemDetail> listCustomeFields = this.GetCustomFields();

            ObservableCollection<ItemCollateralClassViewModel> temp = new ObservableCollection<ItemCollateralClassViewModel>
            {
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.AircraftManufacturerModelMappedField,
                        Header = "Aircraft Manufacturer’s Model Mapped To Mapped Field : ", 
                        ItemType = 630, 
                        ListComboBox = listContractFeatures,
                        SelectComboBox = listContractFeatures != null && item != null && item.ManufacturerModelMappedFieldID != null 
                                            && listContractFeatures.FirstOrDefault(x => x.ItemId == item.ManufacturerNameMappedFieldID) != null
                                            ? listContractFeatures.FirstOrDefault(x => x.ItemId == item.ManufacturerNameMappedFieldID) 
                                            : listContractFeatures.FirstOrDefault(x => x.ItemId == -1)
                    },
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true,
                        CollateralFieldID = Asset.CollateralFieldID.AircraftManufacturerModelContractMappedField,
                        Header = "Aircraft Manufacturer’s Model Mapped To Contract Mapped Field : ", 
                        ItemType = 631, 
                        ListComboBox = listCustomeFields,
                        SelectComboBox = listCustomeFields != null && item != null && item.ManufacturerModelMappedFieldID != null 
                                            && listCustomeFields.FirstOrDefault(x => x.ItemId == item.ManufacturerModelMappedFieldID) != null
                                            ? listCustomeFields.FirstOrDefault(x => x.ItemId == item.ManufacturerModelMappedFieldID) 
                                            : listCustomeFields.FirstOrDefault(x => x.ItemId == -1)
                    },
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.AircraftManufacturerModelQuoteMappedField,
                        Header = "Aircraft Manufacturer’s Model Mapped To Quote Mapped Field : ",
                        ItemType = 631, 
                        ListComboBox = listCustomeFields,
                        SelectComboBox = listCustomeFields != null && item != null && item.ManufacturerModelQuoteMappedFieldID != null 
                                            && listCustomeFields.FirstOrDefault(x => x.ItemId == item.ManufacturerModelQuoteMappedFieldID) != null
                                            ? listCustomeFields.FirstOrDefault(x => x.ItemId == item.ManufacturerModelQuoteMappedFieldID) 
                                            : listCustomeFields.FirstOrDefault(x => x.ItemId == -1)
                    }
            };

            return temp;
        }

        /// <summary>
        /// The get list multi item aircraft manufacturer name mapped to.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>ObservableCollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        private ObservableCollection<ItemCollateralClassViewModel> GetListMultiItemAircraftManufacturerNameMappedTo(PPSRCollateralClassAircraft item)
        {
            ObservableCollection<AssetCollateralItemDetail> listContractFeatures = this.GetContractFeatures();
            ObservableCollection<AssetCollateralItemDetail> listCustomeFields = this.GetCustomFields();

            ObservableCollection<ItemCollateralClassViewModel> temp = new ObservableCollection<ItemCollateralClassViewModel>
            {
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true,
                        CollateralFieldID = Asset.CollateralFieldID.AircraftManufacturerNameMappedField,
                        Header = "Aircraft Manufacturer’s Name Mapped To Mapped Field : ", 
                        ItemType = 630, 
                        ListComboBox = listContractFeatures,
                        SelectComboBox = listContractFeatures != null && item != null && item.ManufacturerNameQuoteMappedFieldID != null 
                                            && listContractFeatures.FirstOrDefault(x => x.ItemId == item.ManufacturerNameQuoteMappedFieldID) != null
                                            ? listContractFeatures.FirstOrDefault(x => x.ItemId == item.ManufacturerNameQuoteMappedFieldID) 
                                            : listContractFeatures.FirstOrDefault(x => x.ItemId == -1)
                    },
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.AircraftManufacturerNameContractMappedField,
                        Header = "Aircraft Manufacturer’s Name Mapped To Contract Mapped Field : ", 
                        ItemType = 631, 
                        ListComboBox = listCustomeFields,
                        SelectComboBox = listCustomeFields != null && item != null && item.ManufacturerNameQuoteMappedFieldID != null 
                                            && listCustomeFields.FirstOrDefault(x => x.ItemId == item.ManufacturerNameQuoteMappedFieldID) != null
                                            ? listCustomeFields.FirstOrDefault(x => x.ItemId == item.ManufacturerNameQuoteMappedFieldID) 
                                            : listCustomeFields.FirstOrDefault(x => x.ItemId == -1)
                    },
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.AircraftManufacturerNameQuoteMappedField,
                        Header = "Aircraft Manufacturer’s Name Mapped To Quote Mapped Field : ", 
                        ItemType = 631, 
                        ListComboBox = listCustomeFields,
                        SelectComboBox = listCustomeFields != null && item != null && item.ManufacturerNameQuoteMappedFieldID != null 
                                            && listCustomeFields.FirstOrDefault(x => x.ItemId == item.ManufacturerNameQuoteMappedFieldID) != null
                                            ? listCustomeFields.FirstOrDefault(x => x.ItemId == item.ManufacturerNameQuoteMappedFieldID) 
                                            : listCustomeFields.FirstOrDefault(x => x.ItemId == -1)
                    }
            };

            return temp;
        }

        private ObservableCollection<ItemCollateralClassViewModel> GetListMultiItemAppendTo(PPSRCollateralClass item)
        {
            ObservableCollection<AssetCollateralItemDetail> listContractFeatures = this.GetContractFeatures();
            ObservableCollection<AssetCollateralItemDetail> listCustomeFields = this.GetCustomFields();

            ObservableCollection<ItemCollateralClassViewModel> temp = new ObservableCollection<ItemCollateralClassViewModel>
            {
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true,
                        CollateralFieldID = Asset.CollateralFieldID.AppendedToContractMappedField,
                        Header = "Contract Mapped Field : ", 
                        ItemType = 631, 
                        ListComboBox = listCustomeFields,
                        SelectComboBox = listCustomeFields != null && listCustomeFields.FirstOrDefault(x => x.ItemId == item.MappedFieldID) != null 
                                            ? listCustomeFields.FirstOrDefault(x => x.ItemId == item.MappedFieldID) 
                                            : listCustomeFields.FirstOrDefault(x => x.ItemId == -1)
                    },
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.AppendedToQuoteMappedField,
                        Header = "Quote Mapped Field : ", 
                        ItemType = 631, 
                        ListComboBox = listCustomeFields,
                        SelectComboBox = listCustomeFields != null && listCustomeFields.FirstOrDefault(x => x.ItemId == item.QuoteMappedFieldID) != null 
                                            && listCustomeFields.FirstOrDefault(x => x.ItemId == item.QuoteMappedFieldID) != null
                                            ? listCustomeFields.FirstOrDefault(x => x.ItemId == item.QuoteMappedFieldID) 
                                            : listCustomeFields.FirstOrDefault(x => x.ItemId == -1)
                    },
                new ItemCollateralClassViewModel
                    {
                        IsShowUp = true, 
                        CollateralFieldID = Asset.CollateralFieldID.AppendedToMappedField,
                        Header = "Mapped Field : ", 
                        ItemType = 630, 
                        ListComboBox = listContractFeatures,
                        SelectComboBox = listContractFeatures != null && listContractFeatures.FirstOrDefault(x => x.ItemId == item.MappedFieldID) != null 
                                            ? listContractFeatures.FirstOrDefault(x => x.ItemId == item.MappedFieldID) 
                                            : listContractFeatures.FirstOrDefault(x => x.ItemId == -1)
                    },
            };

            return temp;
        }

        /// <summary>
        /// The get list mapped to.
        /// </summary>
        /// <param name="ids">
        /// The ids.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>ObservableCollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        private ObservableCollection<AssetCollateralItemDetail> GetListMappedTo(List<int> ids)
        {
            ObservableCollection<SystemConstant> mappedTos =
                new ObservableCollection<SystemConstant>(AssetCollateralClassesFunction.GetListSystemConstant(117));

            ObservableCollection<AssetCollateralItemDetail> list = new ObservableCollection<AssetCollateralItemDetail>(
                (from mappedTo in mappedTos
                 join id in ids on mappedTo.SystemConstantId equals id
                 select
                     new AssetCollateralItemDetail
                         {
                             Text = mappedTo.SystemDescription,
                             ItemId = mappedTo.SystemConstantId,
                         }).ToList());
            list.Insert(
                0,
                new AssetCollateralItemDetail
                    {
                        Text = "<None>",
                        ItemId = -1
                    });
            return list;
        }

/*
        private ObservableCollection<AssetCollateralItemDetail> GetListAircraftNationalityMappedTo()
        {
            ObservableCollection<SystemConstant> mappedTos =
                new ObservableCollection<SystemConstant>(AssetCollateralClassesFunction.GetListSystemConstant(117));

            ObservableCollection<AssetCollateralItemDetail> list = new ObservableCollection<AssetCollateralItemDetail>((from mappedTo in mappedTos
                                                                                                                        where mappedTo.SystemConstantId == 630 || mappedTo.SystemConstantId == 631
                                                                                                                        select new AssetCollateralItemDetail { Text = mappedTo.SystemDescription, ItemId = mappedTo.SystemConstantId, })
                    .ToList());
            list.Insert(
                0,
                new AssetCollateralItemDetail
                    {
                        Text = "<None>",
                        ItemId = -1
                    });
            return list;
        }
*/

        /// <summary>
        /// The get list registration end date.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>ObservableCollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        private ObservableCollection<AssetCollateralItemDetail> GetListRegistrationEndDate()
        {
            ObservableCollection<SystemConstant> regs = new ObservableCollection<SystemConstant>(AssetCollateralClassesFunction.GetListSystemConstant(116));
            ObservableCollection<AssetCollateralItemDetail> list = new ObservableCollection<AssetCollateralItemDetail>((from reg in regs
                                                                                                                        select new AssetCollateralItemDetail { Text = reg.SystemDescription, ItemId = reg.SystemConstantId, })
                    .ToList());
            list.Insert(
                0,
                new AssetCollateralItemDetail
                    {
                        Text = "<None>",
                        ItemId = -1
                    });
            return list;
        }

        /// <summary>
        /// The get list serial number type.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>ObservableCollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        private ObservableCollection<AssetCollateralItemDetail> GetListSerialNumberType(int id)
        {
            ObservableCollection<PPSRSerialNumberType> regs = new ObservableCollection<PPSRSerialNumberType>(AssetCollateralClassesFunction.GetListSerialNumberType(id));
            ObservableCollection<AssetCollateralItemDetail> list = new ObservableCollection<AssetCollateralItemDetail>((from reg in regs
                                                                                                                        select new AssetCollateralItemDetail { ItemId = reg.CollateralClassID, Text = reg.SerialNumberDescription, })
                    .ToList());
            list.Insert(
                0,
                new AssetCollateralItemDetail
                    {
                        Text = "<None>",
                        ItemId = -1
                    });
            return list;
        }

        /// <summary>
        /// The get contract features.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>ObservableCollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        private ObservableCollection<AssetCollateralItemDetail> GetContractFeatures()
        {
            ObservableCollection<FeatureType> regs = new ObservableCollection<FeatureType>(AssetFeatureFunction.GetAllFeatureTypes());
            ObservableCollection<AssetCollateralItemDetail> list = new ObservableCollection<AssetCollateralItemDetail>((from reg in regs
                                                                                                                        select new AssetCollateralItemDetail { Text = reg.FeatureName, ItemId = reg.FeatureTypeId, })
                    .ToList());
            list.Insert(
                0,
                new AssetCollateralItemDetail
                    {
                        Text = "<None>",
                        ItemId = -1
                    });
            return list;
        }

        /// <summary>
        /// The get custom fields.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>ObservableCollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        private ObservableCollection<AssetCollateralItemDetail> GetCustomFields()
        {
            ObservableCollection<UserDefinedFieldName> regs = new ObservableCollection<UserDefinedFieldName>(AssetCollateralClassesFunction.GetCustomFields());
            ObservableCollection<AssetCollateralItemDetail> list = new ObservableCollection<AssetCollateralItemDetail>((from reg in regs
                                                                                                                        select new AssetCollateralItemDetail { Text = reg.FieldName, ItemId = reg.ID, })
                    .ToList());
            list.Insert(
                0,
                new AssetCollateralItemDetail
                    {
                        Text = "<None>",
                        ItemId = -1
                    });
            return list;
        }

        /// <summary>
        /// The check serial number required.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CheckSerialNumberRequired(int id)
        {
            return AssetCollateralClassesFunction.GetSerialNumberRequired(id) != 0;
        }
        #endregion

        #region Other
        #endregion
    }
}
