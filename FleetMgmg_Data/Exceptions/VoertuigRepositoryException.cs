using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmg_Data.Exceptions {
    public class VoertuigRepositoryException : Exception {
        public VoertuigRepositoryException(string message) : base(message) {
        }

        public VoertuigRepositoryException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
