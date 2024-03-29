﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetMgmt_Business.Exceptions;
using System.Globalization;
using FleetMgmt_Business.Validators;

namespace FleetMgmt_Business.Objects {
    //LOUIS GHEYSENS
    public class Bestuurder {

        public string Rijksregisternummer { get; private set; }

        public string Voornaam { get; private set; }

        public string Achternaam { get; private set; }

        public DateTime GeboorteDatum { get; private set; }

        public Tankkaart Tankkaart { get; private set; }

        public Voertuig Voertuig { get; private set; }

        public List<Rijbewijs> rijbewijzen = new List<Rijbewijs>();

        public Bestuurder(string rijksregisternummer, string naam, string voornaam, DateTime geboortedatum, List<Rijbewijs> rijbewijzen) :this(rijksregisternummer,naam,voornaam,geboortedatum) {
            this.rijbewijzen = rijbewijzen;
        }

        public Bestuurder(string rijksregisternummer, string naam, string voornaam, DateTime geboortedatum) {
            zetRijksRegisternummer(rijksregisternummer, geboortedatum);
            zetAchternaam(naam);
            zetVoornaam(voornaam);
            zetGeboorteDatum(geboortedatum);
        }

        private void zetRijksRegisternummer(string rijksregisternummer, DateTime rijksgeboortedatum) {
            if (RijksregisterValidator.isGeldig(rijksregisternummer, rijksgeboortedatum)) {
                this.Rijksregisternummer = rijksregisternummer;
            }
        }

        public void zetAchternaam(string achternaam) {
            if (string.IsNullOrWhiteSpace(achternaam)) throw new BestuurderException("Bestuurder: Achternaam mag niet leeg zijn!");
            this.Achternaam = achternaam;
        }

        public void zetVoornaam(string voornaam) {
            if (string.IsNullOrWhiteSpace(voornaam)) throw new BestuurderException("Bestuurder: Voornaam mag niet leeg zijn!");
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
            string bestuurdersInfo = $"Rijksregisternummer: " +
                $"{this.Rijksregisternummer}\nVoornaam: " +
                $"{this.Voornaam}\nAchternaam: " +
                $"{this.Achternaam}\nGeboortedatum: " +
                $"{this.GeboorteDatum.ToShortDateString()}";

            if (this.rijbewijzen.Count > 0) {
                bestuurdersInfo += "\nRijbewijzen: "+String.Join(',', this.rijbewijzen);
            }
            else {
                bestuurdersInfo += "\nRijbewijzen: Geen";
            }
            if (this.Tankkaart != null) {
                bestuurdersInfo += $" \nTankkaart: {this.Tankkaart.KaartNummer.ToString()}";
            }
            else {
                bestuurdersInfo += $" \nTankkaart: Geen";
            }

            if (this.Voertuig != null) {
                bestuurdersInfo += $" \nVoertuig: {this.Voertuig.Chassisnummer.ToString()}";
            }
            else {
                bestuurdersInfo += $" \nVoertuig: Geen";
            }
            return bestuurdersInfo;
        }


    }
}