using FleetMgmt_Business.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Validators {
    //COLIN MEERSCHMAN
    public static class ChassisnummerValidator {

        public static bool isGeldig(string chassisnummer) {
            //Verwarbare cijfers uitsluiten
           List<char> verbodenTekens = new List<char>() { 'I','O','Q'};

            //Check of opgegeven string niet null of leeg is.
            if (string.IsNullOrWhiteSpace(chassisnummer)) throw new ChassisnummerException("Chassisnummer mag niet leeg zijn!");

            //Als lengte van chassisnummer niet gelijk is aan 17, exception
            if (chassisnummer.Length != 17) throw new ChassisnummerException("Chassisnummer moet 17 karakters lang zijn!");

            //Checken of er een verbodenTeken aanwezig is, zoja exception
            if (verbodenTekens.Any(c => chassisnummer.Contains(c))) throw new ChassisnummerException("Chassisnummer kan geen verwarrende tekens bevatten!");

            //Checken of er lowercase karakter aanwezig is, zoja exception
            if (chassisnummer.Any(c => char.IsLower(c))) throw new ChassisnummerException("Lowercase karakters zijn niet toegestaan!");

            return true;
        }
    }
}
