using FleetMgmt_Business.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Objects {
    public class Voertuig {

        public Brandstof Brandstof { get; set; }

        public string Chassisnummer { get; set; }

        public string Kleur { get; set; }

        public int AantalDeuren { get; set; }

        public string Merk { get; set; }

        public string TypeVoertuig { get; set; }

    }
}
