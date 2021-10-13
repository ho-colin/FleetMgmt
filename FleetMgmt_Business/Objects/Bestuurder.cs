using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetMgmt_Business.Exceptions;
using System.Globalization;
using FleetMgmt_Business.Validators;

namespace FleetMgmt_Business.Objects {
    public class Bestuurder {
        public string Rijksregisternummer { get; set; }

        public string Naam { get; set; }

        public string Voornaam { get; set; }

        public DateTime GeboorteDatum { get; set; }

        public Bestuurder(string rijksregisternummer, string naam, string voornaam, DateTime geboortedatum) {
            ZetRijksRegisternummer(rijksregisternummer, geboortedatum);
            ZetNaam(naam);
            ZetVoornaam(voornaam);
            ZetGeboorteDatum(geboortedatum);
        }

        public void ZetRijksRegisternummer(string rijksregisternummer, DateTime rijksgeboortedatum) {
            RijksregisterValidator.isGeldig(rijksregisternummer, rijksgeboortedatum);
            this.Rijksregisternummer = rijksregisternummer;
        }

        public void ZetNaam(string naam) {
            if (string.IsNullOrWhiteSpace(naam)) throw new BestuurderException("Bestuurder: naam mag niet leeg zijn!");
            this.Naam = naam;
        }

        public void ZetVoornaam(string voornaam) {
            if (string.IsNullOrWhiteSpace(voornaam)) throw new BestuurderException("Bestuurder: voornaam mag niet leeg zijn!");
            this.Voornaam = voornaam;
        }

        public void ZetGeboorteDatum(DateTime geboortedatum) {
            //Een ongeldige datum heeft altijd een hashcode 0, wanneer de datum dus de hashcode 0 heeft dan is hij ongeldig!
            if (geboortedatum.GetHashCode() == 0) throw new BestuurderException("Bestuurder: Datum heeft geen geldige waarde!");
            if(geboortedatum > DateTime.Today) throw new BestuurderException("Bestuurder: Datum mag niet in de toekomst zijn!");
            this.GeboorteDatum = geboortedatum;
        }

        public override string ToString() {
            return $"Rijksregisternummer: {Rijksregisternummer}\nNaam: {Naam}\nVoornaam: {Voornaam}\nGeboortedatum: {GeboorteDatum}";
        }

    }
}