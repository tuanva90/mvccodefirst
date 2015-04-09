using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.ViewModels.Assets.Validation
{
    using System.Collections.ObjectModel;

    using FluentValidation;

    using Insyston.Operations.Model;

    /// <summary>
    /// The asset features view model validation.
    /// </summary>
    public class AssetFeaturesViewModelValidation : AbstractValidator<AssetFeaturesViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetFeaturesViewModelValidation"/> class.
        /// </summary>
        public AssetFeaturesViewModelValidation()
        {
            this.RuleFor(viewModel => viewModel.SelectedFeatureType.IsNewFeatureType)
                .Equal(false)
                .When(viewModel => viewModel.CurrentStep == Asset.EnumSteps.Delete)
                .WithMessage("New feature type cannot be removed.");
            this.RuleFor(viewModel => viewModel.SelectedFeatureType.FeatureTypeId)
                .Must(this.IsNotAssignedToAssetClassType)
                .When(viewModel => viewModel.CurrentStep == Asset.EnumSteps.Delete)
                .WithMessage("This Feature cannot be deleted as Feature data exists for an Asset Type. Uncheck the Enable box to disable Feature.");
            this.RuleFor(viewModel => viewModel.SelectedFeatureType.FeatureTypeId)
                .Must(this.IsNotAssignedToAssetClassCategory)
                .When(viewModel => viewModel.CurrentStep == Asset.EnumSteps.Delete)
                .WithMessage("This Feature cannot be deleted as Feature data exists for an Asset Category. Uncheck the Enable box to disable Feature.");
            this.RuleFor(viewModel => viewModel.SelectedFeatureType.FeatureTypeId)
                .Must(this.IsNotAssignedToContractOrQuoteProfile)
                .When(viewModel => viewModel.CurrentStep == Asset.EnumSteps.Delete)
                .WithMessage("This Feature cannot be deleted as Feature data exists. Uncheck the Enable box to disable Feature.");
        }

        /// <summary>
        /// The is state valid.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsNotAssignedToAssetClassType(AssetFeaturesViewModel source, int value)
        {
            if (value != null)
            {
                using (Entities model = new Entities())
                {
                    var types = new ObservableCollection<EquipTypeFeature>(model.EquipTypeFeatures.Where(a => a.FeatureId == value));
                    if (types.Count() != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        private bool IsNotAssignedToAssetClassCategory(AssetFeaturesViewModel source, int value)
        {
            if (value != null)
            {
                using (Entities model = new Entities())
                {
                    var categories = new ObservableCollection<EquipCatFeature>(model.EquipCatFeatures.Where(a => a.FeatureId == value));
                    if (categories.Count() != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        /// <summary>
        /// The is not assigned to contract or quote profile.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsNotAssignedToContractOrQuoteProfile(AssetFeaturesViewModel source, int value)
        {
            if (value != null)
            {
                using (Entities model = new Entities())
                {
                    var contractFeature = new ObservableCollection<ContractAssetFeature>(model.ContractAssetFeatures.Where(a => a.FeatureId == value));
                    if (contractFeature.Count() != 0)
                    {
                        return false;
                    }
                    var quoteFeature = new ObservableCollection<QuoteAssetProfileAssetFeature>(model.QuoteAssetProfileAssetFeatures.Where(a => a.FeatureId == value));
                    if (quoteFeature.Count() != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
