using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetMgmt_Business.Objects;

namespace FleetMgmt_Business.Repos {
    //LOUIS GHEYSENS
    public interface IBestuurderRepository {
        Bestuurder voegBestuurderToe(Bestuurder bestuurder);
        void verwijderBestuurder(string rijks);
        IEnumerable<Bestuurder> toonBestuurders(string rijksregisternummer, 
            string achterNaam, string voorNaam, DateTime? geboortedatum);
        Bestuurder selecteerBestuurder(string rijks);
        void bewerkBestuurder(Bestuurder bestuurder);
        bool bestaatBestuurder(string rijks);




    }
}
