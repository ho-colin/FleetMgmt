using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Exceptions
{
    public class RijbewijsManagerException : Exception
    {
        public RijbewijsManagerException(string message) : base(message)
        {
        }

        public RijbewijsManagerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
