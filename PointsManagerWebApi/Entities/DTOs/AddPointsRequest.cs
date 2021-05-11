using System;

namespace PointsManagerWebApi.Entities.DTOs
{
    /// <summary>
    /// DTO Used to store and organize an add points request json.
    /// </summary>
    public class AddPointsRequest
    {
        public string Payer { get; set; }

        public int Points { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
