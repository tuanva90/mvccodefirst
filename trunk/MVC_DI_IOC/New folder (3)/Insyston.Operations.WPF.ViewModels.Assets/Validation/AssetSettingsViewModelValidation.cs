// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetSettingsViewModelValidation.cs" company="Insyston">
// Insyston  
// </copyright>
// <summary>
//   The asset settings view model validation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Validation
{
    using System.Collections.Generic;
    using System.Linq;

    using FluentValidation;

    using Insyston.Operations.Model;

    /// <summary>
    /// The asset settings view model validation.
    /// </summary>
    public class AssetSettingsViewModelValidation : AbstractValidator<AssetSettingsViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetSettingsViewModelValidation"/> class.
        /// </summary>
        public AssetSettingsViewModelValidation()
        {
            this.RuleFor(viewModel => viewModel.CurrentCategory)
                .NotEmpty().WithMessage(" Default Category cannot be null.");
            this.RuleFor(viewModel => viewModel.CurrentCategory)
                .Must(this.IsCategoryValid)
                .WithMessage("The Default Category is invalid. Please select an option in the drop-down list.");
        }

        /// <summary>
        /// The is category valid.
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
        private bool IsCategoryValid(AssetSettingsViewModel source, string value)
        {
            if (string.IsNullOrEmpty(source.CurrentCategory))
            {
                return true;
            }

            using (Entities model = new Entities())
            {
                List<string> category = (from cate in model.EquipCategories
                                    where cate.Enabled 
                                    select cate.Description).ToList();
                category.Add("<None>");
                var cat = category.FirstOrDefault(x => x.Equals(value));
                if (cat != null)
                {
                    return true;
                }

            return false;
            }
        }
    }
}
