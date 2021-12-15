using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Objects {
    public class TypeVoertuig {

        //Unique Identifier
        public string Type { get; private set; }

        public RijbewijsEnum vereistRijbewijs { get; private set; }

        public TypeVoertuig(string type, RijbewijsEnum vereistRijbewijs) {
            zetType(type);
            zetRijbewijs(vereistRijbewijs);
        }

        private void zetType(string type) {
            if (string.IsNullOrWhiteSpace(type)) throw new TypeVoertuigException("TypeVoertuig : zetType - Type mag niet null zijn!");
            this.Type = type;
        }

        private void zetRijbewijs(RijbewijsEnum rijbewijs) {
            this.vereistRijbewijs = rijbewijs;
        }

        public void updateRijbewijs(RijbewijsEnum rijbewijs) {
            if (this.vereistRijbewijs == rijbewijs) throw new TypeVoertuigException("TypeVoertuig : updateRijbewijs - Rijbewijs mag niet hetzelfde zijn.");
            this.vereistRijbewijs = rijbewijs;
        }

        public override string ToString() {
            return $"[Type] {this.Type}, Rijbewijs: {this.vereistRijbewijs.ToString()}";
        }
    }
}
