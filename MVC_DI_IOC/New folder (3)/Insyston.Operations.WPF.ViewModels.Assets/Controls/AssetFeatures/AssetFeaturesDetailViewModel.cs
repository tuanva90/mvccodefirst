// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetFeaturesDetailViewModel.cs" company="Insyston">
//   Insyston
// </copyright>
// <summary>
//   Defines the AssetRegistersViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Assets.Controls.AssetFeatures
{
    using System;
    using System.Threading.Tasks;

    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Assets.Validation;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    /// <summary>
    /// The asset features detail view model.
    /// </summary>
    public class AssetFeaturesDetailViewModel : ViewModelUseCaseBase
    {
        #region Variables

        /// <summary>
        /// The _is checked.
        /// </summary>
        private bool _isChecked;

        /// <summary>
        /// The _feature type id.
        /// </summary>
        private int _featureTypeId;

        /// <summary>
        /// The _enabled.
        /// </summary>
        private bool _enabled;

        /// <summary>
        /// The _feature name.
        /// </summary>
        private string _featureName;

        /// <summary>
        /// The _required length.
        /// </summary>
        private int? _requiredLength;

        /// <summary>
        /// The _required length string.
        /// </summary>
        private string _requiredLengthString;

        /// <summary>
        /// The _is new feature type.
        /// </summary>
        private bool _isNewFeatureType;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetFeaturesDetailViewModel"/> class.
        /// </summary>
        public AssetFeaturesDetailViewModel()
        {
            this.Validator = new AssetFeaturesViewModelDetailValidation();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the feature type id.
        /// </summary>
        public int FeatureTypeId
        {
            get
            {
                return this._featureTypeId;
            }

            set
            {
                this.SetField(ref this._featureTypeId, value, () => this.FeatureTypeId);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this._enabled;
            }

            set
            {
                this.SetField(ref this._enabled, value, () => this.Enabled);
            }
        }

        /// <summary>
        /// Gets or sets the required length string.
        /// </summary>
        public string RequiredLengthString
        {
            get
            {
                return this._requiredLengthString;
            }

            set
            {
                this.SetField(ref this._requiredLengthString, value, () => this.RequiredLengthString);
            }
        }

        /// <summary>
        /// Gets or sets the feature name.
        /// </summary>
        public string FeatureName
        {
            get
            {
                return this._featureName;
            }

            set
            {
                this.SetField(ref this._featureName, value, () => this.FeatureName);
            }
        }

        /// <summary>
        /// Gets or sets the required length.
        /// </summary>
        public int? RequiredLength
        {
            get
            {
                return this._requiredLength;
            }

            set
            {
                this.SetField(ref this._requiredLength, value, () => this.RequiredLength);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is new feature type.
        /// </summary>
        public bool IsNewFeatureType
        {
            get
            {
                return this._isNewFeatureType;
            }

            set
            {
                if (value)
                {
                    // SelectedFeatureType.RequiredLength = 0;
                }

                this.SetField(ref this._isNewFeatureType, value, () => this.IsNewFeatureType);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is checked.
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return this._isChecked;
            }

            set
            {
                if (!value)
                {
                    this.RequiredLengthString = string.Empty;
                }

                this.SetField(ref this._isChecked, value, () => this.IsChecked);
            }
        }
        #endregion

        #region Private Method

        #endregion

        #region Public Method

        /// <summary>
        /// The get data asset features detail.
        /// </summary>
        /// <returns>
        /// The <see cref="FeatureType"/>.
        /// </returns>
        public FeatureType GetDataAssetFeaturesDetail()
        {
            FeatureType currentFeatureType = new FeatureType();
            currentFeatureType.FeatureTypeId = this.FeatureTypeId;
            currentFeatureType.FeatureName = this.FeatureName;
            currentFeatureType.Enabled = this.Enabled;
            currentFeatureType.RequiredLength = string.IsNullOrEmpty(this.RequiredLengthString) ? -1 : int.Parse(this.RequiredLengthString);
            currentFeatureType.IsNewFeatureType = this.IsNewFeatureType;
            return currentFeatureType;
        }
        #endregion

        #region Other

        /// <summary>
        /// The un lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Unlock Function
        /// </exception>
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
        /// <exception cref="NotImplementedException">
        /// Lock Function
        /// </exception>
        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
