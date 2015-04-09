// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeData.cs" company="TMA Solution">
//   Unit Test
// </copyright>
// <summary>
//   Defines the FakeData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Insyston.Operations.WPF.VModel.Collections.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    using Insyston.Operations.Business.Collections.Model;
    using Insyston.Operations.Business.Common.Model;
    using Insyston.Operations.Model;
    using Insyston.Operations.WPF.ViewModels.Collections;
    using Insyston.Operations.WPF.ViewModels.Common.Commands;
    using Insyston.Operations.WPF.ViewModels.Common.Model;

    /// <summary>
    /// The fake data.
    /// </summary>
    public class FakeData
    {
        /// <summary>
        /// The fake task collection queue.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static async Task<CollectionQueue> FakeTaskCollectionQueue()
        {
            return await Task.Run(
                () => new CollectionQueue
                {
                    ID = 15,
                    AssignmentOrder = 1,
                    QueueName = "QLD",
                    CountQueueItem = 6,
                    ClientFinancialsTypeID = 80
                });
        }

        /// <summary>
        /// The fake list collection queues.
        /// </summary>
        /// <returns>
        /// The <see cref="Collection"/>.
        /// </returns>
        internal static List<CollectionQueue> FakeListCollectionQueues()
        {
            return new List<CollectionQueue>
                       {
                           new CollectionQueue
                               {
                                   ID = 15,
                                   AssignmentOrder = 1,
                                   QueueName = "QLD",
                                   CountQueueItem = 6,
                                   ClientFinancialsTypeID = 80
                               }
                       };
        }

        /// <summary>
        /// The fake collection queue.
        /// </summary>
        /// <returns>
        /// The <see cref="CollectionQueue"/>.
        /// </returns>
        internal static CollectionQueue FakeCollectionQueue()
        {
            return new CollectionQueue
            {
                ID = 15,
                AssignmentOrder = 0,
                QueueName = " ",
                CountQueueItem = 6,
                ClientFinancialsTypeID = 80
            };
        }

        /// <summary>
        /// The fake dropdown lists.
        /// </summary>
        /// <returns>
        /// The <see cref="Collection "/>.
        /// </returns>
        internal static ObservableCollection<DropdownList> FakeDropdownLists()
        {
            return new ObservableCollection<DropdownList> { new DropdownList { ID = 1, Description = "test" } };
        }

        /// <summary>
        /// The fake operator.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static async Task<ObservableCollection<OperatorModel>> FakeOperator()
        {
            return await Task.Run(
                () => new ObservableCollection<OperatorModel>
                    {
                         new OperatorModel { Text = "test", Value = "value" } 
                    });
        }

        /// <summary>
        /// The fake select list.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static async Task<List<SelectListModel>> FakeSelectList()
        {
            return await Task.Run(
               () => new List<SelectListModel>
                       {
                           new SelectListModel { Id = 1, IsSelected = true, Text = "test" }
                       });
        }

        /// <summary>
        /// The fake collections.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static async Task<CollectionQueueFilter> FakeCollections()
        {
            return await Task.Run(
               () => new CollectionQueueFilter
               {
                   CollectionQueueID = -1,
                   IsEqualMaxContractArrearsDays = true,
                   IsEqualMaxClientArrearsDays = true,
                   IsEqualMaxClientArrearsAmount = true,
                   IsEqualMinClientArrearsAmount = true
               });
        }

        /// <summary>
        /// The fake collectors.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static async Task<List<Collectors>> FakeCollectors()
        {
            return await Task.Run(
                () =>
                {
                    var list = new List<Collectors>();
                    list.Add(new Collectors { UserId = 1, UserName = "UN1" });
                    list.Add(new Collectors { UserId = 2, UserName = "UN2" });
                    return list;
                });
        }

        /// <summary>
        /// The fake list queue detail model.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static async Task<List<QueueDetailsModel>> FakeListQueueDetailModel()
        {
            return await Task.Run(
                () => new List<QueueDetailsModel>
                {
                        new QueueDetailsModel
                            {
                                QueueDetailId = 1,
                                QueueEnable = true,
                                AssignOrder = 10,
                                QueueName = "QN1"
                            },
                        new QueueDetailsModel
                            {
                                QueueDetailId = 2,
                                QueueEnable = true,
                                AssignOrder = 11,
                                QueueName = "QN2"
                            }
                });
        }

        /// <summary>
        /// The fake queue detail model.
        /// </summary>
        /// <returns>
        /// The <see cref="QueueDetailsModel"/>.
        /// </returns>
        internal static QueueDetailsModel FakeQueueDetailModel()
        {
            return new QueueDetailsModel
            {
                QueueDetailId = 1,
                QueueEnable = true,
                AssignOrder = 10,
                QueueName = "QN"
            };
        }

        /// <summary>
        /// The fake task queue detail mode.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static async Task<QueueDetailsModel> FakeTaskQueueDetailMode()
        {
            return await
                Task.Run(
                    () =>
                    new QueueDetailsModel
                        {
                            QueueDetailId = 1,
                            QueueEnable = true,
                            AssignOrder = 10,
                            QueueName = "QN"
                        });
        }

        /// <summary>
        /// The fake system constant.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static async Task<List<SystemConstant>> FakeSystemConstant()
        {
            return await Task.Run(
                () => new List<SystemConstant>
                    {
                        new SystemConstant { SystemConstantId = 1, SystemConstantType = 2, SystemDescription = "test", UserDescription = "UD1", Visible = true },
                        new SystemConstant { SystemConstantId = 2, SystemConstantType = 2, SystemDescription = "test", UserDescription = "UD2", Visible = true }
                    });
        }

        /// <summary>
        /// The fakes collection default.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static async Task<CollectionSystemDefault> FakesCollectionDefault()
        {
            return await Task.Run(
                () => new CollectionSystemDefault { ID = 1, FilterOptionID = 729, NoteCategoryID = 1385 });
        }

        /// <summary>
        /// The fake selected queue.
        /// </summary>
        /// <returns>
        /// The <see cref="CollectionAssignmentModel"/>.
        /// </returns>
        internal static CollectionAssignmentModel FakeSelectedQueue()
        {
            return new CollectionAssignmentModel
            {
                Arrears = 10,
                ArrearsDays = 2,
                Assignee = "Alek",
                AssigneeID = 1,
                ClientFinancialType = 1,
                ClientName = "Jason",
                ClientNumber = 1,
                ClientNumberFilter = "Filter 1",
                ContractId = 1,
                CurrentIntroducerNodeID = 1,
                Financier = "ABC",
            };
        }

        /// <summary>
        /// The fake assignment details.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static async Task<QueueAssignmentDetailsModel> FakeAssignmentDetails()
        {
            return await Task.Run(
                () => new QueueAssignmentDetailsModel
                {
                    ClientFinancialType = 1,
                    ContractId = "1",
                    EntityType = "1",
                    AssigneeID = 1,
                    LegalName = "abc",
                    TradingName = "xyz"
                });
        }

        /// <summary>
        /// The fake list assignees.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static async Task<List<CollectionAssignee>> FakeListAssignees()
        {
            return await Task.Run(
                    () =>
                     new List<CollectionAssignee>
                       {
                           new CollectionAssignee { AssigneeID = 1, Name = "a" },
                           new CollectionAssignee { AssigneeID = 2, Name = "b" }
                       });
        }

        /// <summary>
        /// The fake assignment contacts.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static async Task<ObservableCollection<ContactModel>> FakeAssignmentContacts()
        {
            return await Task.Run(
                    () =>
                     new ObservableCollection<ContactModel>
                       {
                           new ContactModel
                               {
                                   Association = "a",
                                   Direct = "b",
                                   Email = "abc@xyz.com",
                                   Mobile = "(023) 345 678",
                                   Name = "Smith",
                                   Switch = "AB"
                               },
                           new ContactModel
                               {
                                   Association = "e",
                                   Direct = "f",
                                   Email = "def@xyz.com",
                                   Mobile = "(123) 543 876",
                                   Name = "Tomson",
                                   Switch = "GH"
                               }
                       });
        }

        /// <summary>
        /// The fake contract summary.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static async Task<List<ContractSummaryModel>> FakeContractSummary()
        {
            return await Task.Run(
                    () =>
                    new List<ContractSummaryModel>
                        {
                            new ContractSummaryModel
                                {
                                    ArrearsDays = 2,
                                    Arrears = 10,
                                    CollectionStatus = "status A",
                                    ContractNumber = "no1",
                                    FollowUpdateDate = DateTime.Now,
                                    InternalCompany = "company A",
                                    Investment = 10,
                                    LastDueDate = DateTime.Now
                                }
                        });
        }

        /// <summary>
        /// The fake collection history.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
#pragma warning disable 1998
        internal static async Task<List<CollectionHistoryModel>> FakeCollectionHistory()
#pragma warning restore 1998
        {
            return new List<CollectionHistoryModel>
                       {
                           new CollectionHistoryModel
                               {
                                   ContractID = 1,
                                   ActionDescription = "Description 1"
                               },
                           new CollectionHistoryModel
                               {
                                   ContractID = 2,
                                   ActionDescription = "Description 2"
                               }
                       };
        }

        /// <summary>
        /// The fake list note task.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
#pragma warning disable 1998
        internal static async Task<List<PrideClientModel>> FakeListNoteTask()
#pragma warning restore 1998
        {
            return new List<PrideClientModel>
                       {
                           new PrideClientModel
                               {
                                   AssignTo = "AT 1",
                                   Category = "category 1",
                                   ID = 1
                               },
                           new PrideClientModel
                               {
                                   AssignTo = "AT 2",
                                   Category = "category 2",
                                   ID = 2
                               }
                       };
        }

        /// <summary>
        /// The fake action command.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionCommand"/>.
        /// </returns>
        internal static ObservableCollection<ActionCommand> FakeActionCommand()
        {
            return new ObservableCollection<ActionCommand>
                       {
                           new ActionCommand
                               {
                                   Parameter =
                                       CollectionsAssignmentViewModel
                                       .EnumSteps.Edit.ToString(),
                                   Command = new Edit()
                               },
                           new ActionCommand
                               {
                                   Parameter =
                                       CollectionsAssignmentViewModel
                                       .EnumSteps.Previous.ToString(),
                                   Command = new Previous()
                               },
                           new ActionCommand
                               {
                                   Parameter =
                                       CollectionsAssignmentViewModel
                                       .EnumSteps.Next.ToString(),
                                   Command = new Next()
                               }
                       };
        }

        /// <summary>
        /// The fake queue detail.
        /// </summary>
        /// <returns>
        /// The <see cref="QueueAssignmentDetailsModel"/>.
        /// </returns>
        internal static QueueAssignmentDetailsModel FakeQueueDetail()
        {
            return new QueueAssignmentDetailsModel { AssigneeID = 1 };
        }

        /// <summary>
        /// The fake filtered items.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>ObservableCollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        internal static ObservableCollection<CollectionAssignmentModel> FakeFilteredItems()
        {
            return new ObservableCollection<CollectionAssignmentModel>
                       {
                           new CollectionAssignmentModel
                               {
                                   Arrears = 10,
                                   ArrearsDays =
                                       2,
                                   Assignee =
                                       "Alek",
                                   AssigneeID =
                                       1,
                                   ClientFinancialType
                                       = 1,
                                   ClientName =
                                       "Jason",
                                   ClientNumber
                                       = 1,
                                   ClientNumberFilter
                                       =
                                       "Filter 1",
                                   ContractId =
                                       1,
                                   CurrentIntroducerNodeID
                                       = 1,
                                   Financier =
                                       "ABC"
                               },
                           new CollectionAssignmentModel
                               {
                                   Arrears = 11,
                                   ArrearsDays =
                                       3,
                                   Assignee =
                                       "Alek",
                                   AssigneeID =
                                       2,
                                   ClientFinancialType
                                       = 2,
                                   ClientName =
                                       "Jason",
                                   ClientNumber
                                       = 2,
                                   ClientNumberFilter
                                       =
                                       "Filter 1",
                                   ContractId =
                                       2,
                                   CurrentIntroducerNodeID
                                       = 2,
                                   Financier =
                                       "ABC"
                               },
                           new CollectionAssignmentModel
                               {
                                   Arrears = 12,
                                   ArrearsDays =
                                       4,
                                   Assignee =
                                       "Alek",
                                   AssigneeID =
                                       3,
                                   ClientFinancialType
                                       = 3,
                                   ClientName =
                                       "Jason",
                                   ClientNumber
                                       = 3,
                                   ClientNumberFilter
                                       =
                                       "Filter 1",
                                   ContractId =
                                       3,
                                   CurrentIntroducerNodeID
                                       = 3,
                                   Financier =
                                       "ABC"
                               }
                       };
        }

        /// <summary>
        /// The fake collections system settings async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
#pragma warning disable 1998
        internal static CollectionSetting FakeCollectionsSystemSettingsAsync()
#pragma warning restore 1998
        {
            return new CollectionSetting
                    {
                        ID = 1,
                        MinimumArrearsAmount = 10,
                        MinimumArrearsDays = 2,
                        MinimumArrearsPercent = 12
                    };
        }

        /// <summary>
        /// The fake collections settings.
        /// </summary>
        /// <returns>
        /// The <see cref="CollectionSetting"/>.
        /// </returns>
        internal static CollectionSetting FakeCollectionsSettings()
        {
            return 
            new CollectionSetting
                    {
                        ID = 1,
                        MinimumArrearsAmount = 10,
                        MinimumArrearsDays = 2,
                        MinimumArrearsPercent = 12
                    };
        }

        /// <summary>
        /// The fake collection system default async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static CollectionSystemDefault FakeCollectionSystemDefaultAsync()
        {
            return new CollectionSystemDefault { ID = 1, FilterOptionID = 2, NoteCategoryID = 1 };
        }

        /// <summary>
        /// The fake all system parameter async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static async Task<List<SystemParam>> FakeAllSystemParameterAsync()
        {
            return await
                Task.Run(
                    () =>
                    new List<SystemParam>
                        {
                            new SystemParam
                                {
                                    Enabled = true,
                                    LastDateModified = DateTime.Now,
                                    LastUserId = 1,
                                    ParamDesc = " ",
                                    ParamElement = " ",
                                    ParamId = 1,
                                    ParamType = 1,
                                    SystemConstantId = 1
                                }
                        });
        }

        /// <summary>
        /// The fake selected action.
        /// </summary>
        /// <returns>
        /// The <see cref="SelectListModel"/>.
        /// </returns>
        internal static SelectListModel FakeSelectedAction()
        {
            return new SelectListModel { Id = 1, IsSelected = true, Text = "abc" };
        }

        /// <summary>
        /// The fake get default follow up date for history.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static async Task<DateTime?> FakeGetDefaultFollowUpDateForHistory()
        {
            return await Task.Run(
                () =>
                {
                    DateTime? result = new DateTime(2014, 1, 1);
                    return result;
                });
        }

        /// <summary>
        /// The fake get contract summary.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal static async Task<List<ContractSummaryModel>> FakeGetContractSummary()
        {
            return
                await
                Task.Run(
                    () =>
                    new List<ContractSummaryModel>
                        {
                            new ContractSummaryModel
                                {
                                    Arrears = 10,
                                    ArrearsDays = 2,
                                    CollectionStatus = "status 1",
                                    ContractNumber = "No 1",
                                    FollowUpdateDate = DateTime.Now,
                                    InternalCompany = "Company A"
                                },
                            new ContractSummaryModel
                                {
                                    Arrears = 11,
                                    ArrearsDays = 3,
                                    CollectionStatus = "status 2",
                                    ContractNumber = "No 2",
                                    FollowUpdateDate = DateTime.Now,
                                    InternalCompany = "Company B"
                                }
                        });
        }
    }
}
