using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmg_Data.Exceptions {
    public class BestuurderRepositoryException : Exception {
        public BestuurderRepositoryException(string message) : base(message) {
        }

        public BestuurderRepositoryException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
