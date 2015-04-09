// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueActivityUnitTest.cs" company="TMA Solutions">
//   2014
// </copyright>
// <summary>
//   The queue activity unit test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.VModel.Collections.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The queue activity unit test.
    /// </summary>
    [TestClass]
    public class QueueActivityUnitTest
    {
        /*/// <summary>
        /// The test get default follow up date.
        /// </summary>
        [TestMethod]
        public void Test_GetDefaultFollowUpDate_GetCorrectDate()
        {
            Mock.Arrange(() => Application.Current.Dispatcher.InvokeAsync(null))
                .IgnoreArguments()
                .DoNothing();

            var queueActivityMock = Mock.Create<QueueActivityViewModel>(Behavior.CallOriginal);

            Mock.Arrange(() => QueueAssignmentFunctions.GetDefaultFollowUpDateForHistory(Arg.AnyInt))
                .Returns(FakeData.FakeGetDefaultFollowUpDateForHistory);

            Mock.Arrange(() => queueActivityMock.SelectedAction).Returns(FakeData.FakeSelectedAction);

            var getDefaultFollowUpDate = new PrivateAccessor(queueActivityMock);
            getDefaultFollowUpDate.CallMethod("GetDefaultFollowUpDate");

            //Task task = Task.FromResult(getDefaultFollowUpDate.CallMethod("GetDefaultFollowUpDate"));
            //await Task.WhenAll(task);

            // Assert.IsTrue(queueActivityMock.FollowUpDate != null);
            Assert.IsTrue(
                queueActivityMock.FollowUpDate != null
                && (queueActivityMock.FollowUpDate.Value.Day == 1
                    && queueActivityMock.FollowUpDate.Value.Month == 1
                    && queueActivityMock.FollowUpDate.Value.Year == 2014));
        }*/
    }
}
