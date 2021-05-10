using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointsManagerWebApi.Entities
{
    /// <summary>
    /// Class to represent a user.
    /// </summary>
    public class User
    {
        public List<PointsTransaction> TransactionList { get; private set; }

        /// <summary>
        /// Initializes a new User object. 
        /// </summary>
        /// <param name="transactionList">Desired transaction list to use, creates an empty list if null.</param>
        public User(List<PointsTransaction> transactionList = null)
        {
            if(transactionList == null)
            {
                transactionList = new List<PointsTransaction>();
            }

            TransactionList = transactionList;
        }

        /// <summary>
        /// Adds a transaction to the user's history.
        /// </summary>
        /// <param name="transaction">Transaction to be added.</param>
        public void AddTransaction(PointsTransaction transaction)
        {
            TransactionList.Add(transaction);
        }

        /// <summary>
        /// Spends the user's points.
        /// </summary>
        /// <returns>JArray of JObjects which contain the payer and amount of points used.</returns>
        public JArray SpendPoints()
        {
            return new JArray();
        }

        /// <summary>
        /// Gets all point balances for all payers.
        /// </summary>
        /// <returns>JObject containing the payer name and associated point balance.</returns>
        public JObject GetAllPayersPointBalance()
        {
            return new JObject();
        }
    }
}
