using FleetManagement.Checkers;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Exceptions;
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
            ZetChassisnummer(chassisnummer);
            ZetKleur(kleur);
            ZetAantalDeuren(aantaldeuren);
            this.Merk = merk;
            this.Model = model;
            this.TypeVoertuig = typevoertuig;
            ZetNummerplaat(nummerplaat);
        }
        
        public void ZetKleur(string kleur)
        {
            if (string.IsNullOrWhiteSpace(kleur)) throw new VoertuigException("Voertuig - kleur mag niet leeg zijn");
            Kleur = kleur;
        }
        public void ZetAantalDeuren(int aantal)
        {
            if (aantal < 1) throw new VoertuigException("Voertuig - aantal deuren mag niet minder dan 1 zijn");
            AantalDeuren = aantal;
        }
        public void ZetChassisnummer(string chassisnummer)
        {
            ChassisnummerValidator.isGeldig(chassisnummer);
            Chassisnummer = chassisnummer;
        }
        public void ZetNummerplaat(string nummerplaat)
        {
            NummerplaatValidator.isGeldig(nummerplaat);
            Nummerplaat = nummerplaat;
        }
        public void UpdateBestuurder(Bestuurder bestuurder)
        {
            if(bestuurder == null)
            {
                Bestuurder = null;
            }
            else 
            {
                Bestuurder = bestuurder;
            }            
        }
        public void UpdateAantalDeuren(int aantal)
        {
            if (aantal < 1) throw new VoertuigException("Voertuig - aantal deuren mag niet minder dan 1 zijn");
            AantalDeuren = aantal;
        }
        public void UpdateKleur(string kleur)
        {
            if (string.IsNullOrWhiteSpace(kleur)) throw new VoertuigException("Voertuig - kleur mag niet leeg zijn");
            Kleur = kleur;
        }
    }
}
