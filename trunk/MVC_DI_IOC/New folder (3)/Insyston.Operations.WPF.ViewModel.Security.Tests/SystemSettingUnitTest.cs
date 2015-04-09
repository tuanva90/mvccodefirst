// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemSettingUnitTest.cs" company="LXM Pty Ltd Trading as Insyston">
//   Copyright (c) LXM Pty Ltd Trading as Insyston. All rights reserved.
// </copyright>
// <summary>
//   The system setting unit test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.ViewModels.Security.Tests
{
    using System.Windows;

    using Insyston.Operations.Business.Security;
    using Insyston.Operations.ObjectMap;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Telerik.JustMock;

    /// <summary>
    /// The system setting unit test.
    /// </summary>
    [TestClass]
    public class SystemSettingUnitTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemSettingUnitTest"/> class.
        /// </summary>
        public SystemSettingUnitTest()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Map.InitialiseMap();
        }

        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }
        #region Unit Tests

        /// <summary>
        /// The test on step sync start password strength.
        /// Expect data loaded, check PasswordStrength
        /// </summary>
        [TestMethod]
        public void TestOnStepSyncStart_PasswordStrength()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock enviroment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                        .IgnoreArguments()
                        .DoNothing();

                    // Create mock
                    var viewModel = Mock.Create<SystemSettingViewModel>(new Constructor(), Behavior.CallOriginal);

                    Mock.SetupStatic(typeof(SystemSettingsFunctions), StaticConstructor.Mocked);
                    Mock.Arrange(() => SystemSettingsFunctions.ReadSystemSettingsAsync())
                        .Returns(FakeDataSecurity.FakeDataSecuritySystemSetting.FakeSystemSetting());

                    var expected = FakeDataSecurity.FakeDataSecuritySystemSetting.FakeSystemSetting();
                    await viewModel.OnStepAsync("Start");

                    var actual = viewModel.SystemSettings;

                    // Check PasswordStrength is loaded
                    Assert.AreEqual(expected.Result.PasswordStrength, actual.PasswordStrength);
                });
        }

        /// <summary>
        /// The test save on set action commands async.
        /// Expect ActionCommand when Save is Edit
        /// </summary>
        [TestMethod]
        public void TestOnStepSyncSave_SetActionCommandsAsyncActionCommand()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                    {
                        // Mock enviroment for test
                        Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();

                        // Create mock
                        var viewModel = Mock.Create<SystemSettingViewModel>(new Constructor(), Behavior.CallOriginal);

                        viewModel.IsCheckedOut = true;
                        Mock.Arrange(() => viewModel.HasErrors).Returns(false);
                        Mock.Arrange(() => viewModel.CanEdit).Returns(true);
                        Mock.Arrange(() => viewModel.SystemSettings).Returns(FakeDataSecurity.FakeDataSecuritySystemSetting.FakeASystemSetting());
                        Mock.SetupStatic(typeof(SystemSettingsFunctions), StaticConstructor.Mocked);
                        Mock.Arrange(() => SystemSettingsFunctions.UpdateSystemSettings(viewModel.SystemSettings))
                            .IgnoreArguments();

                        await viewModel.OnStepAsync("Save");

                        // Assert
                        Assert.AreEqual(false, viewModel.IsCheckedOut);

                        // Expect ActionCommand when Save is Edit
                        Assert.AreEqual("Edit", viewModel.ActionCommands[0].Parameter);
                    });
        }

        /// <summary>
        /// The test edit on set action commands async.
        /// Expect ActionCommand when Edit is Cancel, Save
        /// </summary>
        [TestMethod]
        public void TestOnStepSyncEdit_SetActionCommandsAsync()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock enviroment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();

                    // Create mock
                    var viewModel = Mock.Create<SystemSettingViewModel>(new Constructor(), Behavior.CallOriginal);

                    viewModel.IsCheckedOut = false;

                    await viewModel.OnStepAsync("Edit");

                    // Assert
                    Assert.AreEqual(true, viewModel.IsCheckedOut);

                    // Expect ActionCommand when Edit is Cancel, Save
                    Assert.AreEqual(2, viewModel.ActionCommands.Count);
                });
        }

        /// <summary>
        /// The test cancel on set action commands async.
        /// Expect ActionCommand when Save is Edit and SystemSetting PasswordStrength == "@X123"
        /// </summary>
        [TestMethod]
        public void TestOnStepSyncCancel_SetActionCommandsAsync_PasswordStrength()
        {
            GeneralThreadAffineContext.Run(
                async () =>
                {
                    // Mock enviroment for test
                    Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                            .IgnoreArguments()
                            .DoNothing();

                    // Create mock
                    var viewModel = Mock.Create<SystemSettingViewModel>(new Constructor(), Behavior.CallOriginal);

                    viewModel.IsCheckedOut = true;
                    Mock.Arrange(() => viewModel.HasErrors).Returns(false);
                    Mock.Arrange(() => viewModel.CanEdit).Returns(true);
                    Mock.Arrange(() => viewModel.SystemSettings).Returns(FakeDataSecurity.FakeDataSecuritySystemSetting.FakeASystemSetting());
                    Mock.SetupStatic(typeof(SystemSettingsFunctions), StaticConstructor.Mocked);
                    Mock.Arrange(() => SystemSettingsFunctions.ReadSystemSettingsAsync()).Returns(FakeDataSecurity.FakeDataSecuritySystemSetting.FakeSystemSetting());

                    await viewModel.OnStepAsync("Cancel");

                    Assert.AreEqual(false, viewModel.IsCheckedOut);

                    // Expect ActionCommand when Save is Edit
                    Assert.AreEqual("Edit", viewModel.ActionCommands[0].Parameter);

                    // Expect SystemSetting PasswordStrength == "@X123"
                    Assert.AreEqual("@X123", viewModel.SystemSettings.PasswordStrength);
                });
        }
        #endregion
    }
}
