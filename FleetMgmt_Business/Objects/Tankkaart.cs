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

        public Tankkaart(string kaartnummer, DateTime geldigheidsdatum, string pincode, Bestuurder inbezitvan) {
            this.KaartNummer = kaartnummer;
            this.GeldigheidsDatum = geldigheidsdatum;
            this.Pincode = pincode;
            this.InBezitVan = inbezitvan;
        }

    }
}
