// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisteredAssetDetailViewModelValidate.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   The registered asset detail view model validate.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.RegisteredAsset.Validation
{
    using System;
    using FluentValidation;
    using FluentValidation.Results;
    using Insyston.Operations.WPF.ViewModels.RegisteredAsset.RegisteredAssetDetail;

    /// <summary>
    /// The registered asset detail view model validate.
    /// </summary>
    public class RegisteredAssetDetailViewModelValidate : AbstractValidator<RegisteredAssetDetailViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredAssetDetailViewModelValidate"/> class.
        /// </summary>
        public RegisteredAssetDetailViewModelValidate()
        {
            this.RuleFor(viewModel => viewModel.NetAssetCost).GreaterThan(0).WithMessage("Asset Cost must be greater than zero for an Activated Asset").When(x => x.IsInActiveAsset);
            this.RuleFor(viewModel => viewModel.DynamicComboBoxReportCompany.SelectedItem.ItemId).NotEqual(-1).WithMessage("The Internal Company/Financier is a required field for an Activated Asset").When(x => x.IsInActiveAsset);
            this.RuleFor(viewModel => viewModel.SelectRegisterDefault.ID).NotEqual(-1).WithMessage("The Asset Register is a required field for an Activated Asset").When(x => x.IsInActiveAsset);
            this.RuleFor(viewModel => viewModel.NetAssetCost).GreaterThan(0).WithMessage("Asset Cost must be greater than zero.");
            this.RuleFor(viewModel => viewModel.AssetGst).GreaterThanOrEqualTo(0).WithMessage("GST must be greater than or equal to zero.");
            this.Custom(detail => !detail.ValidateCategoryCombobox() ? new ValidationFailure(string.Empty, "The Category is invalid. Please select an option in the drop-down list.") : null);
            this.Custom(detail => !detail.ValidateTypeCombobox() ? new ValidationFailure(string.Empty, "The Type is invalid. Please select an option in the drop-down list.") : null);
            this.Custom(detail => !detail.ValidateMakeCombobox() ? new ValidationFailure(string.Empty, "The Make is invalid. Please select an option in the drop-down list.") : null);
            this.Custom(detail => !detail.ValidateModelCombobox() ? new ValidationFailure(string.Empty, "The Model is invalid. Please select an option in the drop-down list.") : null);
            this.Custom(detail => !detail.IsNullSelectedCategory() ? new ValidationFailure(string.Empty, "The Category cannot be null.") : null);
            this.Custom(detail => !detail.IsNullSelectedType() ? new ValidationFailure(string.Empty, "The Type cannot be null.") : null);
            this.Custom(detail => !detail.IsNullSelectedMake() ? new ValidationFailure(string.Empty, "The Make cannot be null.") : null);
            this.Custom(detail => !detail.IsNullSelectedModel() ? new ValidationFailure(string.Empty, "The Model cannot be null.") : null);
            this.Custom(detail => !detail.CheckSelectFinancer() ? new ValidationFailure(string.Empty, "Financier not selected. This is a required field for General Ledger.") : null);
            this.Custom(detail => !detail.CheckSelectInternalCompany() ? new ValidationFailure(string.Empty, "Internal Company not selected. This is a required field for General Ledger.") : null);
        }

        /// <summary>
        /// The check exist register name.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="effectDate">
        /// The effect date.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckEffectDate(RegisteredAssetDetailViewModel source, DateTime? effectDate)
        {
            if (effectDate != null)
            {
                if (source.AssetAcquisitionDate > effectDate)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
