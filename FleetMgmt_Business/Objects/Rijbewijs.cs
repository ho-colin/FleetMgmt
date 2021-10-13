using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetMgmt_Business.Exceptions;

namespace FleetMgmt_Business.Objects {
    public class Rijbewijs {

        public string Categorie { get; private set; }

        public DateTime BehaaldOp { get; private set; }

        public Rijbewijs(string categorie, DateTime behaaldop) {
            ZetCategorie(categorie);
            ZetBehaaldOp(behaaldop);
        }

        public void ZetCategorie(string categorie) {
            if (string.IsNullOrWhiteSpace(categorie)) throw new RijbewijsException("Rijbewijs: Categorienaam mag niet leeg zijn!");
            this.Categorie = categorie;
        }

        public void ZetBehaaldOp(DateTime behaaldop) {
            //Een ongeldige datum heeft altijd een hashcode 0, wanneer de datum dus de hashcode 0 heeft dan is hij ongeldig!
            if (behaaldop.GetHashCode() == 0) throw new RijbewijsException("Rijbewijs: Datum heeft geen geldige waarde!");
            if (behaaldop > DateTime.Now) throw new RijbewijsException("Rijbewijs: Datum mag niet in de toekomst zijn!");
            this.BehaaldOp = behaaldop;
        }

    }
}
