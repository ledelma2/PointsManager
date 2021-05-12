using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointsManagerWebApi.Exceptions
{
    /// <summary>
    /// Class to represent an exception when there is a negative points balance for a particular payer.
    /// </summary>
    public class NegativeBalanceException : Exception
    {
        public NegativeBalanceException()
        {
        }

        public NegativeBalanceException(string message)
            : base(message)
        {
        }

        public NegativeBalanceException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
