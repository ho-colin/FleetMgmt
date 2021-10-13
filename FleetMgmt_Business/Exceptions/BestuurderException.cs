using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Exceptions
{
    public class BestuurderException : Exception {
        public BestuurderException(string message) : base(message) { }

        public BestuurderException(string message, Exception innerException) : base(message, innerException){ }
    }
}
