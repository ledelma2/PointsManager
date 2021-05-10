using NUnit.Framework;
using PointsManagerWebApi.Entities;
using System;
using System.Collections.Generic;

namespace PointsManagerTests
{
    [TestFixture]
    public class PointsManagerTests
    {
        private static DateTime[] DateTimeCases = new DateTime[]
        {
            DateTime.MinValue,
            DateTime.Now,
            DateTime.MaxValue
        };

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AddTransaction_UserTransactionListEmpty_TransactionIsAddedToList(
            [Values(null, "", "FakeCompany", "FakeCompany with punctuation 'L.L.C.")] string payer, 
            [Range(-10, 10, 1)] int points, 
            [ValueSource("DateTimeCases")] DateTime timestamp)
        {
            User user = new User();
            PointsTransaction expected = new PointsTransaction();
            expected.Payer = payer;
            expected.Points = points;
            expected.TimeStamp = timestamp;

            user.AddTransaction(expected);
            PointsTransaction actual = user.TransactionList[0];

            Assert.AreEqual(expected, actual);
        }
    }
}