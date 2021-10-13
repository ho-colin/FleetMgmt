using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Exceptions
{
    public class RijbewijsException: Exception
    {
        public RijbewijsException(string message) : base(message) { }

        public RijbewijsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
