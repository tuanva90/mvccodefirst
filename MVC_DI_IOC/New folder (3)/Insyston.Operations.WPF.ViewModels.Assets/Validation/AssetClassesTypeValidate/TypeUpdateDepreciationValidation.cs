using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.AssetType;

namespace Insyston.Operations.WPF.ViewModels.Assets.Validation.AssetClassesTypeValidate
{
    public class TypeUpdateDepreciationValidation : AbstractValidator<AssetTypeUpdateDepreciationViewModel>
    {
        public TypeUpdateDepreciationValidation()
        {
            this.RuleFor(detail => detail.CategoryName).NotEmpty().WithMessage("Asset Type name is Required.");
            Custom(detail =>
            {
                return !detail.ValidateSalvagePercentEmpty() ? new ValidationFailure("", "Salvage is required.") : null;
            });
            Custom(detail =>
            {
                return !detail.ValidateBookEffectiveLifeEmpty() ? new ValidationFailure("", "Book Effective Life is required.") : null;
            });
            Custom(detail =>
            {
                return !detail.ValidateTaxEffectiveLifeEmpty() ? new ValidationFailure("", "Tax Effective Life is required.") : null;
            });
            Custom(detail =>
            {
                return !detail.ValidateBookRatePercentEmpty() ? new ValidationFailure("", "Book Rate is required.") : null;
            });
            Custom(detail =>
            {
                return !detail.ValidateTaxRatePercentEmpty() ? new ValidationFailure("", "Tax Rate is required.") : null;
            });
            Custom(detail =>
            {
                return !detail.ValidateRatePercent() ? new ValidationFailure("", "Rate must be < 50% for Diminishing Value Method.") : null;
            });
            Custom(detail =>
            {
                return !detail.ValidateTaxEffectiveLife() ? new ValidationFailure("", "Effective Life must be > 2 years for Diminishing Value Method.") : null;
            });
        }
    }
}
