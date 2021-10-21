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

        public Brandstof Brandstof { get; private set; }

        public string Chassisnummer { get; private set; }

        public string Kleur { get; private set; }

        public int AantalDeuren { get; private set; }

        public string Merk { get; private set; }

        public string Model { get; private set; }

        public string TypeVoertuig { get; private set; }

        public string Nummerplaat { get; private set; }

        public Bestuurder Bestuurder { get; private set; }

        public Voertuig(Brandstof brandstof, string chassisnummer, string kleur, int aantaldeuren, string merk, string model, string typevoertuig, string nummerplaat) {
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
            if (string.IsNullOrWhiteSpace(model)) throw new VoertuigException("Voertuig : zetModel - Model mag niet leeg zijn");
            this.Model = model;
        }

        private void zetMerk(string merk) {
            if (string.IsNullOrWhiteSpace(merk)) throw new VoertuigException("Voertuig : zetMerk -  Merk mag niet leeg zijn");
            this.Merk = merk;
        }

        private void zetTypeVoertuig(string type) {
            if (string.IsNullOrWhiteSpace(type)) throw new VoertuigException("Voertuig : zetTypeVoertuig - Type voertuig mag niet leeg zijn");
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
            if (bestuurder == Bestuurder) throw new VoertuigException("Voertuig : updateBestuurder - Geen verschil");
            bestuurder.updateVoertuig(this);
            this.Bestuurder = bestuurder;
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
