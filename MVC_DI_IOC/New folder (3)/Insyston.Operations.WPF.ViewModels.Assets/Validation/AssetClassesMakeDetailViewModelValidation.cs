// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetClassesMakeDetailViewModelValidation.cs" company="Insyston">
//  Insyston 
// </copyright>
// <summary>
//   The asset classes make detail view model validation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Validation
{
    using System.Linq;

    using FluentValidation;

    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetClasses.Asset_Make;

    /// <summary>
    /// The asset classes make detail view model validation.
    /// </summary>
    public class AssetClassesMakeDetailViewModelValidation : AbstractValidator<AssetMakeDetailViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetClassesMakeDetailViewModelValidation"/> class.
        /// </summary>
        public AssetClassesMakeDetailViewModelValidation()
        {
            this.RuleFor(viewModel => viewModel.AssetMakeName)
                .NotEmpty().WithMessage("Asset Make Name cannot be null.");

            this.RuleFor(viewModel => viewModel.AssetMakeName)
                .Must(this.CheckExistMakeName)
                .WithMessage("Make already exists. Please enter a unique Make description before continuing.");
        }

        /// <summary>
        /// The check exist make name.
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
        public bool CheckExistMakeName(AssetMakeDetailViewModel source, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            using (Entities model = new Entities())
            {
                if (!source.IsModeEdit)
                {
                    var query = model.EquipMakes.FirstOrDefault(m => m.Description.Equals(source.AssetMakeName));
                    if (query != null)
                    {
                        return false;
                    }
                }
                else
                {
                    var query = model.EquipMakes.FirstOrDefault(m => m.Description.Equals(source.AssetMakeName));

                    if (query != null && query.EquipMakeId != source.ValidateMakeName.EquipMakeId)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
