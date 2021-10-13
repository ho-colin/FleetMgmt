using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Objects {
    public class Bestuurder {
        public string Rijksregisternummer { get; set; }

        public string Naam { get; set; }

        public string Voornaam { get; set; }

        public DateTime GeboorteDatum { get; set; }

        public Bestuurder(string rijksregisternummer, string naam, string voornaam, DateTime geboortedatum) {
            this.Rijksregisternummer = rijksregisternummer;
            this.Naam = naam;
            this.Voornaam = voornaam;
            this.GeboorteDatum = geboortedatum;
        }
    }
}