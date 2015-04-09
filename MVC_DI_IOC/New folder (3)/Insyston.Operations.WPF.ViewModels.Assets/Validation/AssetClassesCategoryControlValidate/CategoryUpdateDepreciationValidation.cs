// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategoryUpdateDepreciationValidation.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   The asset classes category update depreciation view model validation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Validation.AssetClassesCategoryControlValidate
{
    using FluentValidation;
    using FluentValidation.Results;

    using Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.AssetCategory;

    /// <summary>
    /// The asset classes category update depreciation view model validation.
    /// </summary>
    public class AssetClassesCategoryUpdateDepreciationViewModelValidation : AbstractValidator<AssetCategoryUpdateDepreciationViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetClassesCategoryUpdateDepreciationViewModelValidation"/> class.
        /// </summary>
        public AssetClassesCategoryUpdateDepreciationViewModelValidation()
        {
            this.Custom(detail => !detail.ValidateSalvagePercentEmpty() ? new ValidationFailure(string.Empty, "Salvage is required.") : null);
            this.Custom(detail => !detail.ValidateBookEffectiveLifeEmpty() ? new ValidationFailure(string.Empty, "Book Effective Life is required.") : null);
            this.Custom(detail => !detail.ValidateTaxEffectiveLifeEmpty() ? new ValidationFailure(string.Empty, "Tax Effective Life is required.") : null);
            this.Custom(detail => !detail.ValidateBookRatePercentEmpty() ? new ValidationFailure(string.Empty, "Book Rate is required.") : null);
            this.Custom(detail => !detail.ValidateTaxRatePercentEmpty() ? new ValidationFailure(string.Empty, "Tax Rate is required.") : null);
            this.Custom(detail => !detail.ValidateRatePercent() ? new ValidationFailure(string.Empty, "Rate must be < 50% for Diminishing Value Method.") : null);
            this.Custom(detail => !detail.ValidateTaxEffectiveLife() ? new ValidationFailure(string.Empty, "Effective Life must be > 2 years for Diminishing Value Method.") : null);
        }
    }
}
