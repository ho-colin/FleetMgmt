using FleetMgmt_Business.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetMgmt_Business.Enums;

namespace FleetMgmt_Business.Objects {
    //COLIN MEERSCHMAN
    public class Tankkaart {

        public int KaartNummer { get; private set; }

        public DateTime GeldigheidsDatum { get; private set; }

        public string Pincode { get; private set; }

        public Bestuurder InBezitVan { get; private set; }

        public bool Geblokkeerd { get; private set; }

        public List<TankkaartBrandstof> Brandstoffen { get; private set; }

        public Tankkaart(int kaartnummer, DateTime geldigheidsdatum, string pincode, Bestuurder inbezitvan, List<TankkaartBrandstof> brandstoffen, 
            bool geblokkeerd) : this(geldigheidsdatum, pincode, inbezitvan, brandstoffen, geblokkeerd) {
            zetKaartnummer(kaartnummer);
        }

        public Tankkaart(DateTime geldigheidsdatum, string pincode, Bestuurder inbezitvan, List<TankkaartBrandstof> brandstoffen, bool geblokkeerd) {
            zetBrandstoffen(brandstoffen);
            zetGeldigheidsDatum(geldigheidsdatum);
            zetPincode(pincode);
            zetGeblokkeerd(geblokkeerd);
            updateInBezitVan(inbezitvan);         
        }

        public Tankkaart(int kaartnummer, DateTime geldigheidsdatum, string pincode) {
            zetKaartnummer(kaartnummer);
            zetGeldigheidsDatum(geldigheidsdatum);
            zetPincode(pincode);
        }

        public void updateInBezitVan(Bestuurder bestuurder) {
            if (Geblokkeerd && bestuurder != null) throw new TankkaartException("Tankkaart is geblokkeerd!");
            if (bestuurder == null) {
                if(this.InBezitVan != null) {
                    this.InBezitVan.updateTankkaart(null);
                }
                this.InBezitVan = null;
                return;
            }
            this.InBezitVan = bestuurder;
            if (bestuurder.Tankkaart != this) { bestuurder.updateTankkaart(this); }          
        }      

        //True is geblokkeerd, False is niet geblokkeerd
        public void zetGeblokkeerd(bool geblokkeerd) {
            this.Geblokkeerd = geblokkeerd;
        }

        public void zetGeldigheidsDatum(DateTime geldigheidsdatum) {
            //if (geldigheidsdatum < DateTime.Now) throw new TankkaartException("Geldigheidsdatum moet groter zijn dan vandaag!");

            this.GeldigheidsDatum = geldigheidsdatum;
        }

        public void voegBrandstofToe(TankkaartBrandstof brandstof) {
            if (this.Brandstoffen == null) {
                zetBrandstoffen(new List<TankkaartBrandstof>() { brandstof });
            } else if (!this.Brandstoffen.Contains(brandstof)) {
                this.Brandstoffen.Add(brandstof);
            } else throw new TankkaartException("Brandstof staat al in lijst!");
        }
      
        public void verwijderBrandstof(TankkaartBrandstof brandstof) {
            if (this.Brandstoffen == null) throw new TankkaartException("Brandstof is null");
            else if (!this.Brandstoffen.Contains(brandstof)) throw new TankkaartException("Brandstof werd niet gevonden!");
            else
                Brandstoffen.Remove(brandstof);
        }

        public void updatePincode(string pincode) {
            this.zetPincode(pincode);
        }
      
        public void zetBrandstoffen(List<TankkaartBrandstof> brandstoffen) {
            if(brandstoffen == null) return;
            if(this.Brandstoffen == null) {
                this.Brandstoffen = brandstoffen;
            } else {
                throw new TankkaartException("Brandstoffen lijst is al aanwezig!");
            }
        }

        private void zetPincode(string pincode) {

            if (string.IsNullOrWhiteSpace(pincode)) {
                this.Pincode = null;
                return;
            }

            if (pincode.Any(char.IsLetter)) throw new TankkaartException("Pincode mag alleen bestaan uit cijfers!");

            this.Pincode = pincode;
        }

        public void zetKaartnummer(int kaartnummer) {
            if (kaartnummer < 1) throw new TankkaartException("Kaartnummer moet hoger zijn dan 1!");

            this.KaartNummer = kaartnummer;
        }

        public override string ToString() {

            string inBezitVan = this.InBezitVan == null ? "Vrij" : $"{this.InBezitVan.Voornaam} {this.InBezitVan.Achternaam}";

            string naarString = $"[Tankkaart] Id: {this.KaartNummer} " +
                $"\n Geblokkeerd: {this.Geblokkeerd}" +
                $"\n Geldig tot: {this.GeldigheidsDatum.ToString("dd/MM/yyyy")}" +
                $"\n Brandstoffen: {string.Join(',', this.Brandstoffen)}" +
                $"\n Gebruikt door: {inBezitVan}";
            return naarString;
        }

        public override bool Equals(object obj) {
            return obj is Tankkaart tankkaart &&
                   KaartNummer == tankkaart.KaartNummer &&
                   GeldigheidsDatum == tankkaart.GeldigheidsDatum &&
                   Pincode == tankkaart.Pincode &&
                   EqualityComparer<Bestuurder>.Default.Equals(InBezitVan, tankkaart.InBezitVan) &&
                   Geblokkeerd == tankkaart.Geblokkeerd &&
                   EqualityComparer<List<TankkaartBrandstof>>.Default.Equals(Brandstoffen, tankkaart.Brandstoffen);
        }

        public override int GetHashCode() {
            return HashCode.Combine(KaartNummer, GeldigheidsDatum, Pincode, InBezitVan, Geblokkeerd, Brandstoffen);
        }
    }
}
