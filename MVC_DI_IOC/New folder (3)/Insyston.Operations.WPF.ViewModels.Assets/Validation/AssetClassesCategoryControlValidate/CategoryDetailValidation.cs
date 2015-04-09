// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategoryDetailValidation.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the CategoryDetailValidation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Validation.AssetClassesCategoryControlValidate
{
    using System.Linq;

    using FluentValidation;
    using FluentValidation.Results;

    using Insyston.Operations.Bussiness.Assets.AssetClasses;
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.AssetCategory;

    public class CategoryDetailValidation : AbstractValidator<AssetCategoryDetailViewModel>
    {
        public CategoryDetailValidation()
        {
            this.RuleFor(detail => detail.CategoryName).NotEmpty().WithMessage("Category name is Required.");
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

            this.RuleFor(viewModel => viewModel.CategoryName)
                .Must(this.CheckExistCategoryName)
                .WithMessage("Category already exists. Please enter a unique Category description before continuing.");

            this.RuleFor(viewModel => viewModel.IsCategoryEnable)
                .Must(this.CheckIfEnable)
                .WithMessage("Default Category cannot be disabled.");
        }

        private bool CheckIfEnable(AssetCategoryDetailViewModel source, bool value)
        {
            if (source.IdSelectedValidateName.Equals(AssetClassesCategoryFunctions.GetDefaultCategoryId())
                && value == false)
            {
                return false;
            }

            return true;
        }

        public bool CheckExistCategoryName(AssetCategoryDetailViewModel source, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            using (Entities model = new Entities())
            {
                if (source.IdSelectedValidateName == 0)
                {
                    var query = model.EquipCategories.FirstOrDefault(m => m.Description.Equals(source.CategoryName));
                    if (query != null)
                    {
                        return false;
                    }
                }
                else
                {
                    var query = model.EquipCategories.FirstOrDefault(m => m.Description.Equals(source.CategoryName));

                    if (query != null && query.EquipCatId != source.IdSelectedValidateName)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
