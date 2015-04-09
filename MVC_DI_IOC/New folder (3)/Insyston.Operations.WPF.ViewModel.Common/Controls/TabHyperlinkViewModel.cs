// <copyright file="TabHyperlinkViewModel.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The custom hyperlink.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    /// <summary>
    /// The tab hyperlink view model.
    /// </summary>
    public class TabHyperlinkViewModel : ViewModelUseCaseBase
    {
        /// <summary>
        /// The _custom hyp.
        /// </summary>
        private List<CustomHyperlink> _customHyperlinks;

        /// <summary>
        /// Gets or sets the custom hyperlinks.
        /// </summary>
        public List<CustomHyperlink> CustomHyperlinks
        {
            get
            {
                return this._customHyperlinks;
            }
            set
            {
                this.SetField(ref _customHyperlinks, value, () => CustomHyperlinks);
            }
        }

        /// <summary>
        /// Gets or sets the old custom hyperlink.
        /// </summary>
        public CustomHyperlink OldCustomHyperlink { get; set; }

        /// <summary>
        /// The _custom hyperlinh selected.
        /// </summary>
        private CustomHyperlink _customHyperlinhSelected;

        /// <summary>
        /// Gets or sets the custom hyperlinh selected.
        /// </summary>
        public CustomHyperlink CustomHyperlinhSelected
        {
            get
            {
                return this._customHyperlinhSelected;
            }
            set
            {
                this.SetField(ref _customHyperlinhSelected, value, () => CustomHyperlinhSelected);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabHyperlinkViewModel"/> class.
        /// </summary>
        public TabHyperlinkViewModel()
        {
            _customHyperlinks = new List<CustomHyperlink>();
        }

        /// <summary>
        /// The un lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override Task UnLockAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
       
    }

    /// <summary>
    /// The hyper link action.
    /// </summary>
    public enum HyperLinkAction
    {
        None,
        SummaryState,
        PersonalDetailsState,
        CredentialsState,
        GroupsState,
        PermissionsState,
        Users,
        Groups,
        DetailsState,
        UsersState,
        Details,
        Activity,
        CollectionSettings,
        ColletionQueues,
        SecuritySetting,
        Collectors,
        ResultState,
        AssetClasses,
        AssetCollateralClasses,
        AssetFeatures,
        AssetSetting,
        AssetClassesCategoryDetailState,
        AssetClassesCategoryFeaturesState,
        AssetClassesCategoryAssetTypesState,
        AssignedToState,
        AssetClassesTypeDetailState,
        AssetClassesTypeFeaturesState,
        AssetClassesTypeMakeState,
        AssetRegister,
        DepreciationState,
        DisposalState,
        HistoryState,
    }
}
