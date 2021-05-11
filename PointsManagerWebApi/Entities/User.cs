using Newtonsoft.Json.Linq;
using PointsManagerWebApi.Entities.DTOs;
using PointsManagerWebApi.Exceptions;
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
        public List<AddPointsRequest> CleanTransactionList { get; private set; }

        private List<string> payers = new List<string>();

        /// <summary>
        /// Initializes a new User object.
        /// </summary>
        /// <param name="transactionList">Desired transaction list to use, creates an empty list if null.</param>
        public User(IEnumerable<AddPointsRequest> transactionList = null)
        {
            CleanTransactionList = new List<AddPointsRequest>();

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
        /// Redeems the user's points and updates the transaction list for each spend request.
        /// </summary>
        /// <returns>JArray of JObjects which contain the payer and amount of points used.</returns>
        public JArray RedeemPoints(SpendRequest spendRequest)
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

        /// <summary>
        /// Creates the clean transaction list, i.e. orders the raw transaction list by timestamp
        /// and eliminates all requests with negative point values.
        /// </summary>
        public void CreateCleanTransactionListFromRaw()
        {
            Dictionary<string, List<int>> payerPointAdditions = new Dictionary<string, List<int>>();
            List<AddPointsRequest> orderedTransactionList = RawTransactionList.OrderBy(i => i.TimeStamp).ToList();

            for(int i = 0; i < orderedTransactionList.Count; i++)
            {
                string payer = orderedTransactionList[i].Payer;
                int points = orderedTransactionList[i].Points;

                if (!payerPointAdditions.ContainsKey(payer))
                {
                    if (points < 0)
                    {
                        throw new NegativeBalanceException($"Negative balance for Payer: {payer}");
                    }

                    payerPointAdditions.Add(payer, new List<int>() { i });
                }
                else
                {
                    if (points < 0)
                    {
                        //spend request
                        DistributeSpendRequestPoints(orderedTransactionList, payerPointAdditions, i);
                    }
                    else
                    {
                        //add request
                        payerPointAdditions[payer].Add(i);
                    }
                }
            }

            CleanTransactionList = orderedTransactionList;
        }

        /// <summary>
        /// Distrubutes the spend request points to remove the negative point value from the specific transaction and replaces it with 0.
        /// </summary>
        /// <param name="orderedTransactions">Ordered list of transactions.</param>
        /// <param name="indexOfSpendRequest">Index of the particular spend request.</param>
        public void DistributeSpendRequestPoints(IEnumerable<AddPointsRequest> orderedTransactions, Dictionary<string, List<int>> payerPointAdditions, int indexOfSpendRequest)
        {
            List<AddPointsRequest> orderedTransactionsList = new List<AddPointsRequest>(orderedTransactions);
            AddPointsRequest spendRequest = orderedTransactionsList[indexOfSpendRequest];
            int pointsToSpend = spendRequest.Points * -1;

            foreach(int index in payerPointAdditions[spendRequest.Payer])
            {
                AddPointsRequest currentPayerAddPointsRequest = orderedTransactionsList[index];

                if (currentPayerAddPointsRequest.Points > 0)
                {
                    //There are available points to spend on this request
                    currentPayerAddPointsRequest.Points -= pointsToSpend;

                    if(currentPayerAddPointsRequest.Points < 0)
                    {
                        //Have more points to spend on the next transaction, so distrubute max on current request and set in ordered tx list
                        pointsToSpend = currentPayerAddPointsRequest.Points * -1;
                        currentPayerAddPointsRequest.Points = 0;
                    }
                    else
                    {
                        //We have spent all points on the spend request, set spend request points to 0 and update ordered tx list
                        spendRequest.Points = 0;
                    }

                    orderedTransactionsList[payerPointAdditions[spendRequest.Payer][index]] = currentPayerAddPointsRequest;
                }
            }

            if(spendRequest.Points != 0)
            {
                //Went through all prior balances, but we still have points to distrubute which means a negative balance has occured
                throw new NegativeBalanceException($"Negative balance for Payer: {spendRequest.Payer}");
            }

            orderedTransactionsList[indexOfSpendRequest] = spendRequest;
        }
    }
}
