using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Objects {
    public class Tankkaart {

        public string KaartNummer { get; private set; }

        public DateTime GeldigheidsDatum { get; private set; }

        public string Pincode { get; private set; }

        public Bestuurder InBezitVan { get; private set; }

        public Tankkaart(string kaartnummer, DateTime geldigheidsdatum, string pincode, Bestuurder inbezitvan) {
            this.KaartNummer = kaartnummer;
            this.GeldigheidsDatum = geldigheidsdatum;
            this.Pincode = pincode;
            this.InBezitVan = inbezitvan;
        }

        public void zetBestuurder(Bestuurder bestuurder) {
            this.InBezitVan = bestuurder;
        }

        public void verwijderBestuurder() {
            this.InBezitVan = null;
        }

    }
}
