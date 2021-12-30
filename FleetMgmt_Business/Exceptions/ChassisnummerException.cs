using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Exceptions {
    //COLIN MEERSCHMAN
    public class ChassisnummerException : Exception {

        public ChassisnummerException(string message) : base(message) {}

        public ChassisnummerException(string message, Exception innerException) : base(message, innerException) { }
    }
}
