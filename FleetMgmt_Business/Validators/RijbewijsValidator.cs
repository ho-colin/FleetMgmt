using FleetMgmt_Business.Exceptions;
using FleetMgmt_Business.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Validators {
    //COLIN MEERSCHMAN
    public static class RijbewijsValidator {

        public static bool isBevoegd(Bestuurder b, Voertuig v) {

            if (b.rijbewijzen == null) throw new RijbewijsException("RijbewijsValidator : isBevoegd - Bestuurder rijbewijzen is null.");

            foreach(Rijbewijs r in b.rijbewijzen) {
                if (r.Categorie == v.TypeVoertuig.vereistRijbewijs) return true;
            }
            return false;
        }
    }
}
