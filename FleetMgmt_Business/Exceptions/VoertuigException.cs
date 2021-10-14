using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Exceptions
{
    public class VoertuigException : Exception
    {
        public VoertuigException(string message) : base(message)
        {
        }

        public VoertuigException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
