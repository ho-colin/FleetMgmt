using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Exceptions;
using FleetMgmt_Business.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Objects {
    public class Voertuig {

        public BrandstofEnum Brandstof { get; private set; }

        public string Chassisnummer { get; private set; }

        public string Kleur { get; private set; }

        public int AantalDeuren { get; private set; }

        public string Merk { get; private set; }

        public string Model { get; private set; }

        public TypeVoertuig TypeVoertuig { get; private set; }

        public string Nummerplaat { get; private set; }

        public Bestuurder Bestuurder { get; private set; }

        public Voertuig(BrandstofEnum brandstof, string chassisnummer, string kleur, int aantaldeuren, string merk, string model, TypeVoertuig typevoertuig, string nummerplaat) {
            this.Brandstof = brandstof;
            zetChassisnummer(chassisnummer);
            zetKleur(kleur);
            zetAantalDeuren(aantaldeuren);
            zetMerk(merk);
            zetModel(model);
            zetTypeVoertuig(typevoertuig);
            zetNummerplaat(nummerplaat);
        }
        
        private void zetModel(string model) {
            if (string.IsNullOrWhiteSpace(model)) throw new VoertuigException("Voertuig - Model mag niet leeg zijn");
            this.Model = model;
        }

        private void zetMerk(string merk) {
            if (string.IsNullOrWhiteSpace(merk)) throw new VoertuigException("Voertuig - Merk mag niet leeg zijn");
            this.Merk = merk;
        }

        private void zetTypeVoertuig(TypeVoertuig type) {
            if (type == null) throw new VoertuigException("Voertuig - Type voertuig mag niet null zijn");
            this.TypeVoertuig = type;
        }

        private void zetKleur(string kleur){
            if (string.IsNullOrWhiteSpace(kleur)) {
                this.Kleur = null;
            } else this.Kleur = kleur;
        }

        private void zetAantalDeuren(int aantal){
            if (aantal < 1) {
                this.AantalDeuren = 0;
            } else this.AantalDeuren = aantal;
        }

        private void zetChassisnummer(string chassisnummer){
            if (ChassisnummerValidator.isGeldig(chassisnummer)) { 
                this.Chassisnummer = chassisnummer; 
            }           
        }

        private void zetNummerplaat(string nummerplaat){
            if (NummerplaatValidator.isGeldig(nummerplaat)) {
                this.Nummerplaat = nummerplaat;
            }
        }

        public void updateBestuurder(Bestuurder bestuurder) {
            if (bestuurder == this.Bestuurder) throw new VoertuigException("Voertuig: UpdateBestuurder - Geen verschil");
            if (!RijbewijsValidator.isBevoegd(bestuurder, this)) throw new VoertuigException("Voertuig - Bestuurder mist het vereiste rijbewijs!");
            //bestuurder.updateVoertuig(this);
            this.Bestuurder = bestuurder;
            if (bestuurder.Voertuig != this) { bestuurder.updateVoertuig(this); }           
        }

        public void updateTypeVoertuig(TypeVoertuig type) {
            if (this.TypeVoertuig == type) throw new VoertuigException("Voertuig : updateTypeVoertuig - Niks verandert!");
            this.TypeVoertuig = type;
        }


        public void updateAantalDeuren(int aantal){
            if(aantal < 1) {
                this.AantalDeuren = 0;
            }else this.AantalDeuren = aantal;
        }

        public void updateKleur(string kleur){
            if (string.IsNullOrWhiteSpace(kleur)) {
                this.Kleur = null;
            }else this.Kleur = kleur;

        }

        public override string ToString() {
            return $"Merk {Merk}\nModel: {Model}\nChassisnummer: {Chassisnummer}\nNummerplaat: {Nummerplaat}\n" +
                $"Brandstof: {Brandstof}\nType wagen: {TypeVoertuig}\nKleur: {Kleur}\nAantal deuren: {AantalDeuren}";
        }

    }
}
