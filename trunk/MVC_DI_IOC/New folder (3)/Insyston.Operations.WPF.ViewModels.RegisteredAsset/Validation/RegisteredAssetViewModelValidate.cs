// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisteredAssetViewModelValidate.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   The registered asset view model validate.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.RegisteredAsset.Validation
{
    using FluentValidation;

    /// <summary>
    /// The registered asset view model validate.
    /// </summary>
    public class RegisteredAssetViewModelValidate : AbstractValidator<RegisteredAssetViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredAssetViewModelValidate"/> class.
        /// </summary>
        //public RegisteredAssetViewModelValidate()
        //{
        //    this.RuleFor(viewModel => viewModel.RegisteredAssetDetailViewModel).SetValidator(new RegisteredAssetDetailViewModelValidate());
        //}
    }
}
