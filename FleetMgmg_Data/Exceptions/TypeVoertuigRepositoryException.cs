using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmg_Data.Exceptions {
    public class TypeVoertuigRepositoryException : Exception {

        public TypeVoertuigRepositoryException(string message) : base(message) {}

        public TypeVoertuigRepositoryException(string message, Exception innerException) : base(message, innerException) { }
    }
}
