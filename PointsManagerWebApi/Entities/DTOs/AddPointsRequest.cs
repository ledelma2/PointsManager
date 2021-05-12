using Newtonsoft.Json;
using System;

namespace PointsManagerWebApi.Entities.DTOs
{
    /// <summary>
    /// DTO Used to store and organize an add points request json.
    /// </summary>
    public class AddPointsRequest
    {
        [JsonProperty("payer", Required = Required.Always)]
        public string Payer { get; set; }

        [JsonProperty("points", Required = Required.Always)]
        public int Points { get; set; }

        [JsonProperty("timestamp", Required = Required.Always)]
        public DateTime TimeStamp { get; set; }
    }
}
