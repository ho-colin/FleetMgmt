using FleetMgmt_Business.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Objects {
    public class Tankkaart {

        public int KaartNummer { get; private set; }

        public DateTime GeldigheidsDatum { get; private set; }

        public string Pincode { get; private set; }

        public Bestuurder InBezitVan { get; private set; }

        public bool Geblokkeerd { get; private set; }

        public List<string> Brandstoffen { get; private set; }

        public Tankkaart(int kaartnummer, DateTime geldigheidsdatum, string pincode, Bestuurder inbezitvan, List<string> brandstoffen) :this(geldigheidsdatum, pincode, inbezitvan, brandstoffen) {
            zetKaartnummer(kaartnummer);
        }

        public Tankkaart(DateTime geldigheidsdatum, string pincode, Bestuurder inbezitvan, List<string> brandstoffen) {
            zetBrandstoffen(brandstoffen);
            zetGeldigheidsDatum(geldigheidsdatum);
            zetPincode(pincode);
            updateInBezitVan(inbezitvan);

            zetGeblokkeerd(false);
        }


        public void updateInBezitVan(Bestuurder bestuurder) {
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
            if (geldigheidsdatum < DateTime.Now) throw new TankkaartException("Geldigheidsdatum moet groter zijn dan vandaag!");

            this.GeldigheidsDatum = geldigheidsdatum;
        }

        public void voegBrandstofToe(string brandstof) {
            if (this.Brandstoffen == null) {
                zetBrandstoffen(new List<String>() { brandstof });
            } else if (!this.Brandstoffen.Contains(brandstof)) {
                this.Brandstoffen.Add(brandstof);
            } else throw new TankkaartException("Brandstof staat al in lijst!");
        }

        public void verwijderBrandstof(string brandstof) {
            if (this.Brandstoffen == null) throw new TankkaartException("Brandstof lijst is leeg!");
            if (Brandstoffen.Contains(brandstof)) {
                Brandstoffen.Remove(brandstof);
            } else throw new TankkaartException("Brandstof niet gevonden in lijst!");
        }

        public void updatePincode(string pincode) {
            this.zetPincode(pincode);
        }


        public void zetBrandstoffen(List<string> brandstoffen) {
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

        private void zetKaartnummer(int kaartnummer) {
            if (kaartnummer < 1) throw new TankkaartException("Kaartnummer moet hoger zijn dan 1!");

            this.KaartNummer = kaartnummer;
        }

        public override string ToString() {

            StringBuilder print = new StringBuilder($"Id: {this.KaartNummer}, GeldigheidsDatum: {this.GeldigheidsDatum.ToShortDateString()}, Geblokkeerd: {this.Geblokkeerd}");

            if (!string.IsNullOrWhiteSpace(Pincode)) print.Append($", PinCode: {this.Pincode}");

            if (Brandstoffen.Count > 0) {
                print.Append(", Brandstoffen: ");
                for (int i = 0; i < this.Brandstoffen.Count; i++) {
                    if (i > 0 && i < this.Brandstoffen.Count) print.Append(", ");
                    print.Append(this.Brandstoffen.ElementAt(i));
                }
            }

            if (this.InBezitVan != null) print.Append($", Eigenaar: {this.InBezitVan.Id}, {this.InBezitVan.Voornaam} {this.InBezitVan.Naam}");
            

            return print.ToString();
        }

        public override bool Equals(object obj) {
            return obj is Tankkaart tankkaart &&
                   KaartNummer == tankkaart.KaartNummer &&
                   GeldigheidsDatum == tankkaart.GeldigheidsDatum &&
                   Pincode == tankkaart.Pincode &&
                   EqualityComparer<Bestuurder>.Default.Equals(InBezitVan, tankkaart.InBezitVan) &&
                   Geblokkeerd == tankkaart.Geblokkeerd &&
                   EqualityComparer<List<string>>.Default.Equals(Brandstoffen, tankkaart.Brandstoffen);
        }

        public override int GetHashCode() {
            return HashCode.Combine(KaartNummer, GeldigheidsDatum, Pincode, InBezitVan, Geblokkeerd, Brandstoffen);
        }
    }
}
