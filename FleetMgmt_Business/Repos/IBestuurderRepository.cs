using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetMgmt_Business.Objects;

namespace FleetMgmt_Business.Repos {
    public interface IBestuurderRepository {
        void voegBestuurderToe(Bestuurder bestuurder);
        void verwijderBestuurder(int id);
        IEnumerable<Bestuurder> toonBestuurders(); //NULLABLE, NIETS INVULLEN IS ALLEMAAL!!! VOORBEELD? = LIJN 28!
        void geefBestuurder(int id);
        void bewerkBestuurder(Bestuurder bestuurder);  //NULLABLE, NIETS INVULLEN IS ALLEMAAL!!! VOORBEELD? = LIJN 28!
        bool bestaatBestuurder(int id);


    }
}
