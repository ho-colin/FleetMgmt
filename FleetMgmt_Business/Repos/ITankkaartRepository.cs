using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Repos {
    //COLIN MEERSCHMAN
    public interface ITankkaartRepository {
        Tankkaart voegTankkaartToe(Tankkaart tankkaart);
        void verwijderTankkaart(int id);
        Tankkaart selecteerTankkaart(int id);
        IEnumerable<Tankkaart> geefTankkaarten(int? id, DateTime? geldigheidsDatum, string bestuurder, bool? geblokkeerd, TankkaartBrandstof? brandstof);
        void bewerkTankkaart(Tankkaart tankkaart);
        bool bestaatTankkaart(int id);
    }
}
