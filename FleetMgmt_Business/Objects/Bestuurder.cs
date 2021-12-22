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

        public string Rijksregisternummer { get; private set; }

        public string Naam { get; private set; }

        public string Voornaam { get; private set; }

        public DateTime GeboorteDatum { get; private set; }

        public Tankkaart Tankkaart { get; private set; }

        public Voertuig Voertuig { get; private set; }

        public List<Rijbewijs> rijbewijzen = new List<Rijbewijs>();

        public Bestuurder(string rijksregisternummer, string naam, string voornaam, DateTime geboortedatum, List<Rijbewijs> rijbewijzen) :this(rijksregisternummer,naam,voornaam,geboortedatum) {
            this.rijbewijzen = rijbewijzen;
        }

        public Bestuurder(string rijksregisternummer, string naam, string voornaam, DateTime geboortedatum) {
            zetRijksRegisternummer(rijksregisternummer, geboortedatum);
            zetNaam(naam);
            zetVoornaam(voornaam);
            zetGeboorteDatum(geboortedatum);
        }

        private void zetRijksRegisternummer(string rijksregisternummer, DateTime rijksgeboortedatum) {
            RijksregisterValidator.isGeldig(rijksregisternummer, rijksgeboortedatum);
            this.Rijksregisternummer = rijksregisternummer;
        }

        private void zetNaam(string naam) {
            if (string.IsNullOrWhiteSpace(naam)) throw new BestuurderException("Bestuurder: naam mag niet leeg zijn!");
            this.Naam = naam;
        }

        private void zetVoornaam(string voornaam) {
            if (string.IsNullOrWhiteSpace(voornaam)) throw new BestuurderException("Bestuurder: voornaam mag niet leeg zijn!");
            this.Voornaam = voornaam;
        }

        private void zetGeboorteDatum(DateTime geboortedatum) {
            //Een ongeldige datum heeft altijd een hashcode 0, wanneer de datum dus de hashcode 0 heeft dan is hij ongeldig!
            if (geboortedatum.GetHashCode() == 0) throw new BestuurderException("Bestuurder: Datum heeft geen geldige waarde!");
            if(geboortedatum >= DateTime.Today) throw new BestuurderException("Bestuurder: Datum mag niet in de toekomst zijn!");
            this.GeboorteDatum = geboortedatum;
        }

        public void updateTankkaart(Tankkaart tankkaart) {
            if (tankkaart == null) {
                if(this.Tankkaart != null) {
                    this.Tankkaart.updateInBezitVan(null);
                }
                this.Tankkaart = null;
                return;
            }
            this.Tankkaart = tankkaart;
            if (tankkaart.InBezitVan != this) { tankkaart.updateInBezitVan(this); }
        }

        public void updateVoertuig(Voertuig voertuig) {
            if (voertuig == this.Voertuig) throw new BestuurderException("Bestuurder: Geen verschil");
            this.Voertuig = voertuig;
            if (voertuig.Bestuurder != this) {
                voertuig.updateBestuurder(this);
            }           
        }


        public void voegRijbewijsToe(Rijbewijs rijbewijs) {
            if (rijbewijzen.Contains(rijbewijs)) throw new BestuurderException("Bestuurder: Rijbewijs al in lijst!");
            rijbewijzen.Add(rijbewijs);
        }

        public void verwijderRijbewijs(Rijbewijs rijbewijs) {
            if (!rijbewijzen.Contains(rijbewijs)) throw new BestuurderException("Bestuurder: Rijbewijs niet lijst!");
            rijbewijzen.Remove(rijbewijs);
        }

        public override string ToString() {
            return $"Naam: {Naam}\nVoornaam: {Voornaam}\nRijksregisternummer: {Rijksregisternummer}\nGeboortedatum: {GeboorteDatum}";
        }
    }
}