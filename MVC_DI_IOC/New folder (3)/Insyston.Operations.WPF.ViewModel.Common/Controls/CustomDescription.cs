// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomDescription.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   Defines the CustomDescription type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Common.Controls
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows;

    using Insyston.Operations.WPF.ViewModels.Common.Interfaces;

    /// <summary>
    /// The custom description.
    /// </summary>
    public class CustomDescription : ViewModelUseCaseBase
    {
         /// <summary>
        /// The _source image.
        /// </summary>
        private string _sourceImage;

        /// <summary>
        /// Gets or sets the source image.
        /// </summary>
        public string SourceImage
        {
            get
            {
                return _sourceImage;
            }

            set
            {
                this.SetField(ref _sourceImage, value, () => SourceImage);
            }
        }

        /// <summary>
        /// The _header.
        /// </summary>
        private string _header;

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string Header
        {
            get
            {
                return _header;
            }

            set
            {
                this.SetField(ref _header, value, () => Header);
            }
        }

        /// <summary>
        /// The _content module.
        /// </summary>
        private string _contentModule;

        /// <summary>
        /// Gets or sets the content module.
        /// </summary>
        public string ContentModule
        {
            get
            {
                return _contentModule;
            }

            set
            {
                this.SetField(ref _contentModule, value, () => ContentModule);
            }
        }

        /// <summary>
        /// The _bullet text list.
        /// </summary>
        private ObservableCollection<string> _bulletTextList;

        /// <summary>
        /// Gets or sets the bullet text list.
        /// </summary>
        public ObservableCollection<string> BulletTextList
        {
            get
            {
                return _bulletTextList;
            }

            set
            {
                this.SetField(ref _bulletTextList, value, () => BulletTextList);
            }
        }

        /// <summary>
        /// The _content details.
        /// </summary>
        private string _contentDetails;

        /// <summary>
        /// Gets or sets the content details.
        /// </summary>
        public string ContentDetails
        {
            get
            {
                return _contentDetails;
            }

            set
            {
                this.SetField(ref _contentDetails, value, () => ContentDetails);
            }
        }

        /// <summary>
        /// Gets or sets the width box.
        /// </summary>
        public int WidthBox { get; set; }

        /// <summary>
        /// Gets or sets the width box.
        /// </summary>
        public int HeightBox { get; set; }

        /// <summary>
        /// Gets or sets the width image.
        /// </summary>
        public int WidthImage { get; set; }

        /// <summary>
        /// Gets or sets the header style.
        /// </summary>
        public Style HeaderStyle { get; set; }

        /// <summary>
        /// Gets or sets the visibility border.
        /// </summary>
        public int BorderThicknessValue { get; set; }

        /// <summary>
        /// Gets or sets the content module style.
        /// </summary>
        public Style ContentModuleStyle { get; set; }

        /// <summary>
        /// Gets or sets the visibility content.
        /// </summary>
        public Visibility VisibilityContent { get; set; }

        /// <summary>
        /// The un lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override Task UnLockAsync()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The lock async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override Task<bool> LockAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
