using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Exceptions {
    public class TankkaartException : Exception {

        public TankkaartException(string message) : base(message) {}

        public TankkaartException(string message, Exception innerException) : base(message, innerException) {}
    }
}
