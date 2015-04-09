// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetRegistersViewModelDetailValidation.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   The asset registers view model detail validation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Validation
{
    using System.Linq;
    using FluentValidation;

    using Insyston.Operations.Bussiness.Assets.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetRegisters;

    /// <summary>
    /// The asset registers view model detail validation.
    /// </summary>
    public class AssetRegistersViewModelDetailValidation : AbstractValidator<AssetRegistersDetailViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetRegistersViewModelDetailValidation"/> class.
        /// </summary>
        public AssetRegistersViewModelDetailValidation()
        {
            this.RuleFor(viewModel => viewModel.RegisterName)
                .NotEmpty()
                .WithMessage("Register Name cannot be null.");
            this.RuleFor(viewModel => viewModel.RegisterName)
                .Must(this.CheckExistRegisterName)
                .WithMessage("Register already exist. Please enter a unique Register description before continuing.");
            this.RuleFor(viewModel => viewModel.SelectDefault)
                .Must(this.CheckDefaultReporting)
                .WithMessage("{0} cannot be <None>.", x => x.DefaultReportName);
        }

        /// <summary>
        /// The check exist register name.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="registerName">
        /// The register name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckExistRegisterName(AssetRegistersDetailViewModel source, string registerName)
        {
            if (string.IsNullOrEmpty(source.RegisterName))
            {
                return true;
            }

            using (Entities model = new Entities())
            {
                if (!source.IsEditMode)
                {
                    var register = model.AssetRegisters.FirstOrDefault(x => x.RegisterName.Equals(registerName));
                    if (register != null)
                    {
                        return false;
                    }
                }
                else
                {
                    var register = model.AssetRegisters.FirstOrDefault(x => x.RegisterName.Equals(registerName));
                    if (register != null && register.ID != source.IdSelectedRegisterValidate)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The check default reporting.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="selectDefault">
        /// The select default.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckDefaultReporting(AssetRegistersDetailViewModel source, AssetRelationRowItem selectDefault)
        {
            if (source.IsGlModuleKeyOn)
            {
                if (selectDefault.NodeId == -1)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
