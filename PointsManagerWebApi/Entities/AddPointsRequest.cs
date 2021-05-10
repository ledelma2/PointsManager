using System;

namespace PointsManagerWebApi.Entities
{
    /// <summary>
    /// DTO Used to store and organize an add points request json.
    /// </summary>
    public class AddPointsRequest
    {
        public string Payer { get; private set; }

        public int Points { get; private set; }

        public DateTime TimeStamp { get; private set; }
    }
}
