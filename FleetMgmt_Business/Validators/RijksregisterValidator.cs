using FleetMgmt_Business.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManagement.Validators {
    public static class RijksregisterValidator {

        public static bool isGeldig(string teControleren, DateTime geboorteDatum) {

            //Uniforme invoermethode forceren.
            if (!teControleren.Contains('.') || !teControleren.Contains('-')) throw new RijksregisterException("Rijksregister invoeren met leesteken(s) en koppelteken(s)!");

            //Controleren lengte string, 15 karakters inclusief koppeltekens en leestekens
            if (teControleren.Length != 15) throw new RijksregisterException("Rijksregister moet 15 karakters lang zijn, Voorbeeld: 90.02.01-999-02");

            string[] gesplitst = teControleren.Split(new char[] { '.', '-' });

            // Er zal niet worden gecontroleerd op geslacht sinds deze kan veranderen!
            int negenVoorafGaandeCijfers = int.Parse((gesplitst[0] + gesplitst[1] + gesplitst[2] + gesplitst[3]));

            //IN OF NA 2000
            if (geboorteDatum.Year >= 2000) {
                negenVoorafGaandeCijfers += 2000000000;
            }

            int modulo = negenVoorafGaandeCijfers % 97;
            int controleGetal = int.Parse(gesplitst[4]);

            int vergelijk = 97 - modulo;

            return controleGetal == vergelijk;
        }
    }
}