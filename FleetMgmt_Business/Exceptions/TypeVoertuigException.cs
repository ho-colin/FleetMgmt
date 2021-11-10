using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Exceptions {
    public class TypeVoertuigException : Exception {
        public TypeVoertuigException(string message) : base(message) {
        }

        public TypeVoertuigException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
