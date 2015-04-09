using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insyston.Operations.WPF.ViewModels.Shell
{
    using System.Windows;
    using System.Windows.Input;

    using Insyston.Operations.Bussiness.Assets.AssetCollateralClasses;
    using Insyston.Operations.Security.Model;
    using Insyston.Operations.WPF.ViewModels.Collections;
    using Insyston.Operations.WPF.ViewModels.Common.Controls;
    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    /// <summary>
    /// The configuration action.
    /// </summary>
    public enum ConfigurationAction
    {
        CollectionSettings,
        CollectionQueueSetting,
        SecuritySetting
    }

    public class ConfigurationViewModel: ViewModelUseCaseBase
    {
        /// <summary>
        /// The groups of setting on screen.
        /// </summary>
        private List<GroupLinkModel> _groupSettings;

        /// <summary>
        /// Gets or sets the groups of setting.
        /// </summary>
        public List<GroupLinkModel> GroupSetting
        {
            get
            {
                return this._groupSettings;
            }
            set
            {
                this.SetField(ref _groupSettings, value, () => GroupSetting);
            }
        }

        /// <summary>
        /// The _custom hyp.
        /// </summary>
        private List<CustomHyperlink> _collectionsHyperlinks;

        /// <summary>
        /// Gets or sets the custom hyperlinks.
        /// </summary>
        public List<CustomHyperlink> CollectionsHyperlinks
        {
            get
            {
                return this._collectionsHyperlinks;
            }
            set
            {
                this.SetField(ref _collectionsHyperlinks, value, () => CollectionsHyperlinks);
            }
        }

        private List<CustomHyperlink> _securityHyperlinks;
        public List<CustomHyperlink> SecurityHyperlinks
        {
            get
            {
                return this._securityHyperlinks;
            }
            set
            {
                this.SetField(ref _securityHyperlinks, value, () => _securityHyperlinks);
            }
        }

        private List<CustomHyperlink> _assetsHyperlinks;

        public List<CustomHyperlink> AssetsHyperlinks
        {
            get
            {
                return this._assetsHyperlinks;
            }
            set
            {
                this.SetField(ref _assetsHyperlinks, value, () => _assetsHyperlinks);
            }
        }

        public void GetHyperlinkWithPermission(Permission collectionPermission, Permission queuesManagermentPermission, Permission securityPermission, bool resultAssetClassPermission, bool resultCollateralPermission, bool resultAssetFeaturePermission, Permission resultAssetRegisterPermission, Permission permissionAssetSetting)
        {
            _collectionsHyperlinks = new List<CustomHyperlink>();
            _securityHyperlinks = new List<CustomHyperlink>();
            _groupSettings = new List<GroupLinkModel>();
            _assetsHyperlinks = new List<CustomHyperlink>();

            if (collectionPermission != null)
            {
                if (collectionPermission.CanSee)
                {
                    _collectionsHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Collection Settings", Action = HyperLinkAction.CollectionSettings, Screen = EnumScreen.Configuration });
                }
                else
                {
                    _collectionsHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Security Permissions are required to access Collection Settings.", IsTextBlockVisible = Visibility.Visible, IsButtonHyperlinkVisible = Visibility.Collapsed });
                }
            }
            else
            {
                _collectionsHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Security Permissions are required to access Collection Settings.", IsTextBlockVisible = Visibility.Visible, IsButtonHyperlinkVisible = Visibility.Collapsed });
            }

            if (queuesManagermentPermission != null)
            {
                if (queuesManagermentPermission.CanSee)
                {
                    _collectionsHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Collection Queues", Action = HyperLinkAction.ColletionQueues, Screen = EnumScreen.Configuration });
                }
                else
                {
                    _collectionsHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Security Permissions are required to access Collection Queues.", IsTextBlockVisible = Visibility.Visible, IsButtonHyperlinkVisible = Visibility.Collapsed });
                }
            }
            else
            {
                _collectionsHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Security Permissions are required to access Collection Queues.", IsTextBlockVisible = Visibility.Visible, IsButtonHyperlinkVisible = Visibility.Collapsed });
            }

            if (securityPermission != null)
            {
                if (securityPermission.CanSee)
                {
                    _securityHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Security Settings", Action = HyperLinkAction.SecuritySetting, Screen = EnumScreen.Configuration });
                }
                else
                {
                    _securityHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Security Permissions are required to access Security Settings.", IsTextBlockVisible = Visibility.Visible, IsButtonHyperlinkVisible = Visibility.Collapsed });
                }
            }
            else
            {
                _securityHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Security Permissions are required to access Security Settings.", IsTextBlockVisible = Visibility.Visible, IsButtonHyperlinkVisible = Visibility.Collapsed });
            }

            if (resultAssetClassPermission)
            {
                _assetsHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Asset Classes", Action = HyperLinkAction.AssetClasses, Screen = EnumScreen.Configuration });
            }
            else
            {
                _assetsHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Security Permissions are required to access Asset Classes.", IsTextBlockVisible = Visibility.Visible, IsButtonHyperlinkVisible = Visibility.Collapsed });
            }

            if (resultCollateralPermission)
            {
                _assetsHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Asset Collateral Classes", Action = HyperLinkAction.AssetCollateralClasses, Screen = EnumScreen.Configuration });
            }
            else
            {
                _assetsHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Security Permissions are required to access Asset Collateral Classes.", IsTextBlockVisible = Visibility.Visible, IsButtonHyperlinkVisible = Visibility.Collapsed });
            }

            if (resultAssetFeaturePermission)
            {
                _assetsHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Asset Features", Action = HyperLinkAction.AssetFeatures, Screen = EnumScreen.Configuration });
            }
            else
            {
                _assetsHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Security Permissions are required to access Asset Features.", IsTextBlockVisible = Visibility.Visible, IsButtonHyperlinkVisible = Visibility.Collapsed });
            }

            if (resultAssetRegisterPermission != null)
            {
                if (resultAssetRegisterPermission.CanSee)
                {
                    _assetsHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Asset Registers", Action = HyperLinkAction.AssetRegister, Screen = EnumScreen.Configuration });
                }
                else
                {
                    _assetsHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Security Permissions are required to access the Asset Register.", IsTextBlockVisible = Visibility.Visible, IsButtonHyperlinkVisible = Visibility.Collapsed });
                }
            }
            if (permissionAssetSetting != null)
            {
                if (permissionAssetSetting.CanSee)
                {
                    _assetsHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Asset Settings", Action = HyperLinkAction.AssetSetting, Screen = EnumScreen.Configuration });
                }
                else
                {
                    _assetsHyperlinks.Add(new CustomHyperlink { HyperlinkHeader = "Security Permissions are required to access Asset Settings.", IsTextBlockVisible = Visibility.Visible, IsButtonHyperlinkVisible = Visibility.Collapsed });
                }
            }

            List<GroupLinkModel> groups = new List<GroupLinkModel>
            {
                new GroupLinkModel { Header = "Assets", Hyperlinks = _assetsHyperlinks },
                new GroupLinkModel { Header = "Collections", Hyperlinks = _collectionsHyperlinks },
                new GroupLinkModel { Header = "Security", Hyperlinks = _securityHyperlinks },
            };

            this.CollectionsHyperlinks = _collectionsHyperlinks;
            this.SecurityHyperlinks = _securityHyperlinks;
            this.GroupSetting = groups;
            this.AssetsHyperlinks = _assetsHyperlinks;
        }

        protected override Task UnLockAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> LockAsync()
        {
            throw new NotImplementedException();
        }
    }
}
