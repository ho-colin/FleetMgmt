using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetMgmt_Business.Objects;

namespace FleetMgmt_Business.Repos {
    public interface IBestuurderRepository {
        Bestuurder voegBestuurderToe(Bestuurder bestuurder);
        void verwijderBestuurder(string rijks);
        IEnumerable<Bestuurder> toonBestuurders(string rijksregisternummer, 
            string naam, string voornamam, DateTime? geboortedatum);
        Bestuurder selecteerBestuurder(string rijks);
        void bewerkBestuurder(Bestuurder bestuurder);
        bool bestaatBestuurder(string rijks);




    }
}
