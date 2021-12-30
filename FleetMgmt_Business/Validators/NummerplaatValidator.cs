using FleetMgmt_Business.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Validators {
    //COLIN MEERSCHMAN
    public static class NummerplaatValidator {

        private static int nummerplaatLengte = 9;

        public static bool isGeldig(string teValideren) {
            //Lengte belgische nummerplaat = 9 karakters, 1 indexcijfer + 3 letters + 3 cijfers en 2 koppeltekens
            //Voorbeeld geldige nummerplaat = 1-AAA-123

            if (string.IsNullOrWhiteSpace(teValideren) || teValideren.Length != nummerplaatLengte) throw new NummerplaatException("Nummerplaat moet 9 karakters lang zijn, Voorbeeld: 1-AAA-123");

            //Splitsen opgegeven string door koppelteken
            string[] gesplitstDoorKoppelteken = teValideren.Split('-');
            //array[0] is indexcijfer (1-9)
            if (gesplitstDoorKoppelteken[0].Length != 1 || !gesplitstDoorKoppelteken[0].All(char.IsNumber)) throw new NummerplaatException("Indexcijfer moet van 1 tot en met 9 zijn!");

            //array[0] moet tussen 1 tot en met 9 zijn
            int indexcijfer = int.Parse(gesplitstDoorKoppelteken[0]);
            if (indexcijfer < 1) throw new NummerplaatException("Indexcijfer moet groter zijn dan 0!");

            //array[1] is 3 letters (A-Z)
            if (gesplitstDoorKoppelteken[1].Length != 3 || !gesplitstDoorKoppelteken[1].All(char.IsLetter)) throw new NummerplaatException("2e set karakters moeten 3 letters zijn!");

            //array[2] is 3 cijfers (0-9)
            if (gesplitstDoorKoppelteken[2].Length != 3 || !gesplitstDoorKoppelteken[2].All(char.IsDigit)) throw new NummerplaatException("3e zet karakters moeten 3 cijfers zijn!");

            //Als alle bovenstaande checks kloppen is het een geldige nummerplaat
            return true;
        }
    }
}