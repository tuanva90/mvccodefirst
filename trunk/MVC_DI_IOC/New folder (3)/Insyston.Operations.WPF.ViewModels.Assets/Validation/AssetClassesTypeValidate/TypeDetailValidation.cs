using FluentValidation;
using FluentValidation.Results;
using Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.AssetType;

namespace Insyston.Operations.WPF.ViewModels.Assets.Validation.AssetClassesTypeValidate
{
    using System.Linq;

    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.AssetCategory;

    public class TypeDetailValidation : AbstractValidator<AssetTypeDetailViewModel>
    {
        public TypeDetailValidation()
        {
            this.RuleFor(detail => detail.TypeName).NotEmpty().WithMessage("Asset Type name is Required.");
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

            this.RuleFor(viewModel => viewModel.TypeName)
                .Must(this.CheckExistTypeName)
                .WithMessage("Type already exists. Please enter a unique Type description before continuing.");
        }


        // Check type name has existed
        public bool CheckExistTypeName(AssetTypeDetailViewModel source, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            using (Entities model = new Entities())
            {
                if (source.IdSelectedValidateName == 0)
                {
                    var query = model.EquipTypes.FirstOrDefault(m => m.Description.Equals(source.TypeName));
                    if (query != null)
                    {
                        return false;
                    }
                }
                else
                {
                    var query = model.EquipTypes.FirstOrDefault(m => m.Description.Equals(source.TypeName));

                    if (query != null && query.EquipTypeId != source.IdSelectedValidateName)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
