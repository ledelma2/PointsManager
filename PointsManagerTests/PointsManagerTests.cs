using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PointsManagerWebApi.Entities;
using PointsManagerWebApi.Entities.DTOs;
using PointsManagerWebApi.Exceptions;
using System;
using System.Collections.Generic;

namespace PointsManagerTests
{
    [TestFixture]
    public class PointsManagerTests
    {
        private User user;

        private static DateTime[] DateTimeCases = new DateTime[]
        {
            DateTime.MinValue,
            DateTime.Now,
            DateTime.MaxValue
        };

        private static AddPointsRequest[] AddPointsRequestCase = new AddPointsRequest[]
        {
            new AddPointsRequest
            {
                Payer = "DANNON",
                Points = 1000,
                TimeStamp = DateTime.Parse("2020-11-02T14:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "UNILEVER",
                Points = 200,
                TimeStamp = DateTime.Parse("2020-10-31T11:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "DANNON",
                Points = -500,
                TimeStamp = DateTime.Parse("2020-10-31T15:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "MILLER COORS",
                Points = 10000,
                TimeStamp = DateTime.Parse("2020-11-01T14:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "DANNON",
                Points = 300,
                TimeStamp = DateTime.Parse("2020-10-31T10:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "DANNON",
                Points = 300,
                TimeStamp = DateTime.Parse("2020-10-31T09:00:00Z")
            }
        };

        private static AddPointsRequest[] AddPointsRequestCaseWithMultipleSpends = new AddPointsRequest[]
        {
            new AddPointsRequest
            {
                Payer = "DANNON",
                Points = -100,
                TimeStamp = DateTime.Parse("2020-10-31T16:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "DANNON",
                Points = 1000,
                TimeStamp = DateTime.Parse("2020-11-02T14:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "UNILEVER",
                Points = 200,
                TimeStamp = DateTime.Parse("2020-10-31T11:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "DANNON",
                Points = -500,
                TimeStamp = DateTime.Parse("2020-10-31T15:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "MILLER COORS",
                Points = 10000,
                TimeStamp = DateTime.Parse("2020-11-01T14:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "DANNON",
                Points = 300,
                TimeStamp = DateTime.Parse("2020-10-31T10:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "DANNON",
                Points = 300,
                TimeStamp = DateTime.Parse("2020-10-31T09:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "MILLER COORS",
                Points = -600,
                TimeStamp = DateTime.Parse("2020-11-03T14:00:00Z")
            }
        };


        private static AddPointsRequest[] OverspendPointsRequestCase = new AddPointsRequest[]
        {
            new AddPointsRequest
            {
                Payer = "DANNON",
                Points = 1000,
                TimeStamp = DateTime.Parse("2020-11-02T14:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "UNILEVER",
                Points = 200,
                TimeStamp = DateTime.Parse("2020-10-31T11:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "DANNON",
                Points = -1000,
                TimeStamp = DateTime.Parse("2020-10-31T15:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "MILLER COORS",
                Points = 10000,
                TimeStamp = DateTime.Parse("2020-11-01T14:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "DANNON",
                Points = 300,
                TimeStamp = DateTime.Parse("2020-10-31T10:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "DANNON",
                Points = 300,
                TimeStamp = DateTime.Parse("2020-10-31T09:00:00Z")
            }
        };

        private List<AddPointsRequest> ExpectedSingleSpendCleanTransactionList = new List<AddPointsRequest>
        {
            new AddPointsRequest
            {
                Payer = "DANNON",
                Points = 100,
                TimeStamp = DateTime.Parse("2020-10-31T10:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "UNILEVER",
                Points = 200,
                TimeStamp = DateTime.Parse("2020-10-31T11:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "MILLER COORS",
                Points = 10000,
                TimeStamp = DateTime.Parse("2020-11-01T14:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "DANNON",
                Points = 1000,
                TimeStamp = DateTime.Parse("2020-11-02T14:00:00Z")
            }
        };

        private List<AddPointsRequest> ExpectedMultiSpendCleanTransactionList = new List<AddPointsRequest>
        {
            new AddPointsRequest
            {
                Payer = "UNILEVER",
                Points = 200,
                TimeStamp = DateTime.Parse("2020-10-31T11:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "MILLER COORS",
                Points = 9400,
                TimeStamp = DateTime.Parse("2020-11-01T14:00:00Z")
            },
            new AddPointsRequest
            {
                Payer = "DANNON",
                Points = 1000,
                TimeStamp = DateTime.Parse("2020-11-02T14:00:00Z")
            }
        };

        [SetUp]
        public void Setup()
        {
            user = new User();
        }

        [Test]
        public void AddTransaction_UserTransactionListEmpty_TransactionIsAddedToList(
            [Values(null, "", "FakeCompany", "FakeCompany with punctuation 'L.L.C.")] string payer,
            [Range(-10, 10, 1)] int points,
            [ValueSource("DateTimeCases")] DateTime timestamp)
        {
            AddPointsRequest expectedRequest = new AddPointsRequest
            {
                Payer = payer,
                Points = points,
                TimeStamp = timestamp
            };

            user.AddTransaction(expectedRequest);
            AddPointsRequest actualRequest = user.RawTransactionList[0];

            Assert.AreEqual(expectedRequest, actualRequest);
        }

        [Test]
        public void AddTransaction_UserTransactionListPopulated_TransactionIsAddedToList(
            [Values(null, "", "FakeCompany", "FakeCompany with punctuation 'L.L.C.")] string payer,
            [Range(-10, 10, 1)] int points,
            [ValueSource("DateTimeCases")] DateTime timestamp)
        {
            user = new User(AddPointsRequestCase);
            AddPointsRequest expectedRequest = new AddPointsRequest
            {
                Payer = payer,
                Points = points,
                TimeStamp = timestamp
            };

            user.AddTransaction(expectedRequest);
            AddPointsRequest actualRequest = user.RawTransactionList[6];

            Assert.AreEqual(expectedRequest, actualRequest);
        }

        [Test]
        public void GetAllPayersPointBalance_AddPointsRequestCases_ReportsAccurateBalance()
        {
            user = new User(AddPointsRequestCase);
            JObject actualBalances = user.GetAllPayersPointBalance();

            JObject expectedBalances = new JObject
            {
                new JProperty("DANNON", 1100),
                new JProperty("UNILEVER", 200),
                new JProperty("MILLER COORS", 10000)
            };

            Assert.AreEqual(expectedBalances, actualBalances);
        }

        [Test]
        public void GetAllPayersPointBalance_NoRequests_ReportsNoBalances()
        {
            JObject actualBalances = user.GetAllPayersPointBalance();

            JObject expectedBalances = new JObject();

            Assert.AreEqual(expectedBalances, actualBalances);
        }

        [Test]
        public void CreateCleanTransactionListFromRaw_AddPointsRequestCase_CreatesACleanTransactionList()
        {
            user = new User(AddPointsRequestCase);

            user.CreateCleanTransactionListFromRaw();

            Assert.AreEqual(ExpectedSingleSpendCleanTransactionList.Count, user.CleanTransactionList.Count);

            for(int i = 0; i < ExpectedSingleSpendCleanTransactionList.Count; i++)
            {
                Assert.AreEqual(ExpectedSingleSpendCleanTransactionList[i].Payer, user.CleanTransactionList[i].Payer);
                Assert.AreEqual(ExpectedSingleSpendCleanTransactionList[i].Points, user.CleanTransactionList[i].Points);
                Assert.AreEqual(ExpectedSingleSpendCleanTransactionList[i].TimeStamp, user.CleanTransactionList[i].TimeStamp);
            }
        }

        [Test]
        public void CreateCleanTransactionListFromRaw_AddPointsRequestCaseWithMultipleSpends_CreatesACleanTransactionList()
        {
            user = new User(AddPointsRequestCaseWithMultipleSpends);

            user.CreateCleanTransactionListFromRaw();

            Assert.AreEqual(ExpectedMultiSpendCleanTransactionList.Count, user.CleanTransactionList.Count);

            for (int i = 0; i < ExpectedMultiSpendCleanTransactionList.Count; i++)
            {
                Assert.AreEqual(ExpectedMultiSpendCleanTransactionList[i].Payer, user.CleanTransactionList[i].Payer);
                Assert.AreEqual(ExpectedMultiSpendCleanTransactionList[i].Points, user.CleanTransactionList[i].Points);
                Assert.AreEqual(ExpectedMultiSpendCleanTransactionList[i].TimeStamp, user.CleanTransactionList[i].TimeStamp);
            }
        }

        [Test]
        public void CreateCleanTransactionListFromRaw_NoRequests_DoesNothing()
        {
            user.CreateCleanTransactionListFromRaw();

            Assert.AreEqual(user.RawTransactionList, user.CleanTransactionList);
        }

        [Test]
        public void CreateCleanTransactionListFromRaw_FirstRequestIsASpend_ThrowsNegativeBalanceException(
            [Range(-1000, -1, 1)] int points)
        {
            AddPointsRequest request = new AddPointsRequest
            {
                Payer = "FAKE COMPANY",
                Points = points,
                TimeStamp = DateTime.Now
            };

            user.AddTransaction(request);

            Assert.Throws<NegativeBalanceException>(user.CreateCleanTransactionListFromRaw);
        }

        [Test]
        public void CreateCleanTransactionListFromRaw_NthRequestIsOverspend_ThrowsNegativeBalanceException()
        {
            user = new User(OverspendPointsRequestCase);

            Assert.Throws<NegativeBalanceException>(user.CreateCleanTransactionListFromRaw);
        }

        [Test]
        public void RedeemPoints_InvalidPointRedemptionRequest_ThrowsInvalidPointRedemptionRequestException(
            [Range(-1000, -1, 1)] int pointsToRedeem)
        {
            SpendRequest request = new SpendRequest
            {
                Points = pointsToRedeem
            };

            Assert.Throws<InvalidPointRedemptionRequestException>(() => user.RedeemPoints(request));
        }

        [Test]
        public void RedeemPoints_AddPointsRequestCase_RedeemsPoints()
        {
            user = new User(AddPointsRequestCase);
        }

        [Test]
        public void RedeemPoints_AddPointsRequestCaseWithMultipleSpends_RedeemsPoints()
        {
            user = new User(AddPointsRequestCaseWithMultipleSpends);

        }
    }
}