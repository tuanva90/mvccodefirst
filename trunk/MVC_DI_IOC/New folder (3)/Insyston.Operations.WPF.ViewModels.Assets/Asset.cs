// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumSteps.cs" company="">
//   
// </copyright>
// <summary>
//   The enum steps.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets
{
    /// <summary>
    /// The asset enum steps.
    /// </summary>
    public class Asset
    {
        /// <summary>
        /// The enum steps.
        /// </summary>
        public enum EnumSteps
        {
            Start,
            DetailsState,
            AssignedToState,
            GridSummaryState,
            GridContentState,
            Add,
            BulkUpdate,
            SelectedFeatureType,
            Edit,
            Save,
            SaveBulkUpdate,
            SaveAndAdd,
            Delete,
            Cancel,
            AssignFeatureState,
            CancelBulkUpdate,
            CancelAssignFeature,
            Error,
            EditBulkUpdate,
            SelectedCollateral,
            MainViewState,
            MainContentState,
            AssetClassesCategoryDetailState,
            AssetClassesCategoryFeaturesState,
            AssetClassesCategoryAssetTypesState,
            AssetClassesCategoryAssignFeaturesState,
            AssetClassesCategoryUpdateDepreciationState,
            SelectedItem,
            AssignType,
            AssignFeature,
            SelectedAssetClassesCategoryItem,
            AssetClassesCategoryAssignTypesState,
            SaveAssignFeature,
            SaveAssignTypes,
            SaveUpdateDepreciation,
            CancelAssignTypes,
            CancelUpdateDepreciation,
            EditAssignFeature,
            EditAssignTypes,
            EditUpdateDepreciation,
            ErrorUpdateDepreciation,
            AssetClassesTypeDetailState,
            AssetClassesTypeFeaturesState,
            AssetClassesTypeMakeState,
            AssetClassesTypeAssignFeaturesState,
            AssetClassesTypeUpdateDepreciationState,
            SelectedAssetClassesTypeItem,
            AssetClassesTypeAssignMakeState,
            SaveAssignMake,
            CancelAssignMake,
            EditAssignMake,
            Dispose,
            None,
            AssignModel,
            SelectedMake,
            DetailState,
            BulkState,
            CancelAssignModel,
            SaveAssignModel,
            EditAssignModel,
            CancelAdd,
            ItemLocked,
            EditRegisterSummary,
            CancelRegisterSummary,
            SaveRegisterSummary,
            SelectRegister,
            SelectModel,
            Copy,
            CurrentGroup,
            EditRegisterDetail,
            CancelRegisterDetail,
            SaveRegisterDetail,
            SelectRegisteredAsset,
            HistoryState,
            Activate,
            Transfer,
            DepreciationState,
            DisposalState,
            SaveTransfer
        }

        public enum CollateralFieldID
        {
            MappedTo = 1,
            ContractMappedField = 2,
            QuoteMappedField = 3,
            MappedField = 4,
            AppendedTo = 5,
            AppendedToContractMappedField = 6,
            AppendedToQuoteMappedField = 7,
            AppendedToMappedField = 8,
            MotorVehicleRegoMappedTo = 9,
            MotorVehicleRegoMappedField = 10,
            MotorVehicleRegoContractMappedField = 11,
            MotorVehicleRegoQuoteMappedField = 12,
            AircraftNationalityMappedTo = 13,
            AircraftNationalityMappedField = 14,
            AircraftNationalityContractMappedField = 15,
            AircraftNationalityQuoteMappedField = 16,
            AircraftNationalityCodeRegoMarkMappedTo = 17,
            AircraftNationalityCodeRegoMarkMappedField = 18,
            AircraftNationalityCodeRegoMarkContractMappedField = 19,
            AircraftNationalityCodeRegoMarkQuoteMappedField = 20,
            AircraftManufacturerModelMappedTo = 21,
            AircraftManufacturerModelMappedField = 22,
            AircraftManufacturerModelContractMappedField = 23,
            AircraftManufacturerModelQuoteMappedField = 24,
            AircraftManufacturerNameMappedTo = 25,
            AircraftManufacturerNameMappedField = 26,
            AircraftManufacturerNameContractMappedField = 27,
            AircraftManufacturerNameQuoteMappedField = 28,
            RegistrationEndDate = 29,
            SerialNumberType = 30,
        }
    }
}