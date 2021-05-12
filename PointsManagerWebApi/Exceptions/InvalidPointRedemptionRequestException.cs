using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointsManagerWebApi.Exceptions
{
    /// <summary>
    /// Class to represent an invalid point redemption request.
    /// </summary>
    public class InvalidPointRedemptionRequestException : Exception
    {
        public InvalidPointRedemptionRequestException()
        {
        }

        public InvalidPointRedemptionRequestException(string message)
            : base(message)
        {
        }

        public InvalidPointRedemptionRequestException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }
}
