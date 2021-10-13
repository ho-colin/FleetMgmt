using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Exceptions {
    public class NummerplaatException : Exception {

        public NummerplaatException(string message) : base(message) { }

        public NummerplaatException(string message, Exception innerException) : base(message, innerException) { }

    }
}