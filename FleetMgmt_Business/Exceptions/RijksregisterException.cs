using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Exceptions {
    public class RijksregisterException : Exception {

        public RijksregisterException(string message) : base(message) { }

        public RijksregisterException(string message, Exception innerException) : base(message, innerException) { }
    }
}