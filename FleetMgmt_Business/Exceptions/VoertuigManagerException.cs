using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Exceptions {
    public class VoertuigManagerException : Exception {
        public VoertuigManagerException(string message) : base(message) {
        }

        public VoertuigManagerException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
