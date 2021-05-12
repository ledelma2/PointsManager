﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointsManagerWebApi.Entities.DTOs
{
    /// <summary>
    /// DTO Used to store and organize a spend request json.
    /// </summary>
    public class SpendRequest
    {
        [JsonProperty("points", Required = Required.Always)]
        public int Points { get; set; }
    }
}
