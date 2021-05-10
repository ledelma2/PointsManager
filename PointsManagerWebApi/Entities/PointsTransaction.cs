using System;

namespace PointsManagerWebApi.Entities
{
    /// <summary>
    /// DTO Used to store and organize a transaction.
    /// </summary>
    public class PointsTransaction
    {
        public string Payer { get; set; }

        public int Points { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
