using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmg_Data.Exceptions {
    public class RijbewijsRepositoryException : Exception {
        public RijbewijsRepositoryException(string message) : base(message) { }

        public RijbewijsRepositoryException(string message, Exception innerException) : base(message, innerException) { }
    }
}
