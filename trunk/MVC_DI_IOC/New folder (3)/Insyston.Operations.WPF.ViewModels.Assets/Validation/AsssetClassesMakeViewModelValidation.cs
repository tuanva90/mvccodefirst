// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsssetClassesMakeViewModelValidation.cs" company="">
//   
// </copyright>
// <summary>
//   The assset classes make view model validation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Validation
{
    using FluentValidation;

    using Insyston.Operations.WPF.ViewModels.Assets.AssetClasses;

    /// <summary>
    /// The assset classes make view model validation.
    /// </summary>
    public class AsssetClassesMakeViewModelValidation : AbstractValidator<AssetClassesMakeViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsssetClassesMakeViewModelValidation"/> class.
        /// </summary>
        public AsssetClassesMakeViewModelValidation()
        {
            this.RuleFor(viewModel => viewModel.AssetMakeDetailViewModel).SetValidator(new AssetClassesMakeDetailViewModelValidation());
        }
    }
}
