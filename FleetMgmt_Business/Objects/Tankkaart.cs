using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Objects {
    public class Tankkaart {

        public string KaartNummer { get; set; }

        public DateTime GeldigheidsDatum { get; set; }

        public string Pincode { get; set; }

        public Bestuurder InBezitVan { get; set; }

    }
}
