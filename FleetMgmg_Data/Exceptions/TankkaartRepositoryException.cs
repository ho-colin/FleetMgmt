using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmg_Data.Exceptions {
    public class TankkaartRepositoryException : Exception {
        public TankkaartRepositoryException(string message) : base(message) {
        }

        public TankkaartRepositoryException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
