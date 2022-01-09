using FleetMgmt_Business.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Validators {
    public static class RijksregisterValidator {
        //LOUIS GHEYSENS

        public static bool isGeldig(string teControleren, DateTime geboorteDatum) {

            if (string.IsNullOrWhiteSpace(teControleren)) throw new RijksregisterException("Rijksregisternummer mag niet leeg zijn!");

            //Uniforme invoermethode forceren.
            if (!teControleren.Contains('.') || !teControleren.Contains('-')) throw new RijksregisterException("Rijksregister invoeren met leesteken(s) en koppelteken(s)!");

            //Controleren of er geen spaties meegegeven zijn
            if (teControleren.Contains(' ')) throw new RijksregisterException("Rijksregisternummers mogen geen spaties bevatten!");

            //Controleren lengte string, 15 karakters inclusief koppeltekens en leestekens
            if (teControleren.Length != 15) throw new RijksregisterException("Rijksregister moet 15 karakters lang zijn, Voorbeeld: 90.02.01-999-02");

            //Kijken of er geen Letters (A-Z) aanwezig zijn in het opgegeven rijksregisternummer
            if (teControleren.Any(char.IsLetter)) throw new RijksregisterException("Er worden geen Letters toegestaan in het rijksregisternummer!");

            string[] gesplitst = teControleren.Split(new char[] { '.', '-' });

            //foreach(string controle in gesplitst) {
            //    if (!controle.All(char.IsNumber)) throw new RijksregisterException("Er worden geen Letters toegestaan in het rijksregisternummer!");
            //}

            // Er zal niet worden gecontroleerd op geslacht sinds deze kan veranderen!
            int negenVoorafGaandeCijfers = int.Parse((gesplitst[0] + gesplitst[1] + gesplitst[2] + gesplitst[3]));

            //IN OF NA 2000
            if (geboorteDatum.Year >= 2000) {
                negenVoorafGaandeCijfers += 2000000000;
            }

            int modulo = negenVoorafGaandeCijfers % 97;
            int controleGetal = int.Parse(gesplitst[4]);

            int vergelijk = 97 - modulo;

            if (controleGetal == vergelijk) {
                return true;
            } else throw new RijksregisterException("Geboortedatum en rijksregisternummer komen niet overeen!");
        }
    }
}