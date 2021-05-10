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
        public List<AddPointsRequest> RawTransactionList { get; private set; }

        /// <summary>
        /// Initializes a new User object.
        /// </summary>
        /// <param name="transactionList">Desired transaction list to use, creates an empty list if null.</param>
        public User(IEnumerable<AddPointsRequest> transactionList = null)
        {
            if(transactionList == null)
            {
                RawTransactionList = new List<AddPointsRequest>();
            }
            else
            {
                RawTransactionList = new List<AddPointsRequest>(transactionList);
            }
        }

        /// <summary>
        /// Adds a transaction to the user's history.
        /// </summary>
        /// <param name="addPointsRequest">Transaction to be added.</param>
        public void AddTransaction(AddPointsRequest addPointsRequest)
        {
            RawTransactionList.Add(addPointsRequest);
        }

        /// <summary>
        /// Spends the user's points and updates the transaction list for each spend request.
        /// </summary>
        /// <returns>JArray of JObjects which contain the payer and amount of points used.</returns>
        public JArray SpendPoints(SpendRequest spendRequest)
        {
            return new JArray();
        }

        /// <summary>
        /// Gets all point balances for all payers.
        /// </summary>
        /// <returns>JObject containing the payer name and associated point balance.</returns>
        public JObject GetAllPayersPointBalance()
        {
            JObject response = new JObject();
            Dictionary<string, int> payerPointBalances = new Dictionary<string, int>();

            foreach(AddPointsRequest request in RawTransactionList)
            {
                string payer = request.Payer;
                int points = request.Points;
                if(payerPointBalances.ContainsKey(payer))
                {
                    payerPointBalances[payer] += points;
                }
                else
                {
                    payerPointBalances.Add(payer, points);
                }
            }

            foreach(string key in payerPointBalances.Keys)
            {
                response.Add(new JProperty(key, payerPointBalances[key]));
            }

            return response;
        }
    }
}
