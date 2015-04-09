// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetClassesModelViewModelValidation.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the AssetClassesModelViewModelValidation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Validation
{
    using FluentValidation;

    using Insyston.Operations.Bussiness.Assets;
    using Insyston.Operations.WPF.ViewModels.Assets.AssetClasses;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;

    /// <summary>
    /// The asset classes model view model validation.
    /// </summary>
    public class AssetClassesModelViewModelValidation : AbstractValidator<AssetClassesModelViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetClassesModelViewModelValidation"/> class.
        /// </summary>
        public AssetClassesModelViewModelValidation()
        {
            this.RuleFor(viewModel => viewModel.AssetModelDetail.ModelDescription)
                .NotEmpty().WithMessage("Model Description cannot be null.")
                .When(x => x.CurrentStep == Asset.EnumSteps.Save);

            this.RuleFor(viewModel => viewModel.AssetModelDetail.ModelDescription)
                .Must(this.IsDuplicateDescription)
                .WithMessage("Model already exists. Please enter a unique Model description before continuing.")
                .When(x => x.CurrentStep == Asset.EnumSteps.Save);

            this.RuleFor(viewModel => viewModel.AssetModelDetail.DynamicComboBoxMake)
                .Must(this.IsDuplicateMake)
                .WithMessage("Disabled Make already exists. Please enter a unique Make description before continuing.")
                .When(x => x.CurrentStep == Asset.EnumSteps.Save);

            this.RuleFor(viewModel => viewModel.AssetModelDetail.DynamicComboBoxType)
                .Must(this.IsDuplicateType)
                .WithMessage("Disabled Type already exists. Please enter a unique Type description before continuing.")
                .When(x => x.CurrentStep == Asset.EnumSteps.Save);

            this.RuleFor(viewModel => viewModel.AssetModelDetail.DynamicComboBoxCategory)
                .Must(this.IsDuplicateCategory)
                .WithMessage("Disabled Category already exists. Please enter a unique Category description before continuing.")
                .When(x => x.CurrentStep == Asset.EnumSteps.Save);

            this.RuleFor(viewModel => viewModel.AssetModelDetail.DynamicComboBoxMake)
                .Must(this.IsMakeTypeValid)
                .WithMessage("Please enter a valid Make or remove Type.")
                .When(x => x.CurrentStep == Asset.EnumSteps.Save);

            this.RuleFor(viewModel => viewModel.AssetModelDetail.DynamicComboBoxMake)
                .Must(this.IsMakeCategoryValid)
                .WithMessage("Please enter a valid Make or remove Category.")
                .When(x => x.CurrentStep == Asset.EnumSteps.Save);

            this.RuleFor(viewModel => viewModel.AssetModelDetail.DynamicComboBoxType)
                .Must(this.IsTypeValid)
                .WithMessage("Please enter a valid Type or remove Category.")
                .When(x => x.CurrentStep == Asset.EnumSteps.Save);
        }

        /// <summary>
        /// The is make type valid.
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
        private bool IsMakeTypeValid(AssetClassesModelViewModel source, DynamicCheckComboBoxViewModel value)
        {
            if (!string.IsNullOrEmpty(source.AssetModelDetail.DynamicComboBoxType.CurrentName) && string.IsNullOrEmpty(value.CurrentName))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The is make category valid.
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
        private bool IsMakeCategoryValid(AssetClassesModelViewModel source, DynamicCheckComboBoxViewModel value)
        {
            if (!string.IsNullOrEmpty(source.AssetModelDetail.DynamicComboBoxCategory.CurrentName) && string.IsNullOrEmpty(value.CurrentName))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The is type valid.
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
        private bool IsTypeValid(AssetClassesModelViewModel source, DynamicCheckComboBoxViewModel value)
        {
            if (!string.IsNullOrEmpty(source.AssetModelDetail.DynamicComboBoxCategory.CurrentName) && string.IsNullOrEmpty(value.CurrentName))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The is duplicate description.
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
        private bool IsDuplicateDescription(AssetClassesModelViewModel source, string value)
        {
            bool check = AssetModelFunctions.CheckValidateDescription(
                source.SelectedModel.EquipModelId, 
                value, 
                source.IsAdd);
            if (check)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The is duplicate make.
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
        private bool IsDuplicateMake(AssetClassesModelViewModel source, DynamicCheckComboBoxViewModel value)
        {
            bool check = AssetModelFunctions.CheckValidateMake(value.CurrentName);
            if (check)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The is duplicate type.
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
        private bool IsDuplicateType(AssetClassesModelViewModel source, DynamicCheckComboBoxViewModel value)
        {
            bool check = AssetModelFunctions.CheckValidateType(value.CurrentName);
            if (check)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The is duplicate category.
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
        private bool IsDuplicateCategory(AssetClassesModelViewModel source, DynamicCheckComboBoxViewModel value)
        {
            bool check = AssetModelFunctions.CheckValidateCategory(value.CurrentName);
            if (check)
            {
                return true;
            }

            return false;
        }
    }
}
