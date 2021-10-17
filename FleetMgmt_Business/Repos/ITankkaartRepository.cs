using FleetMgmt_Business.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Repos {
    public interface ITankkaartRepository {
        void voegTankkaartToe(Tankkaart tankkaart);
        void verwijderTankkaart(Tankkaart tankkaart);
        IEnumerable<Tankkaart> geefTankkaarten();
        Tankkaart geefTankkaart(int id);
        //void bewerkTankkaart(Tankkaart tankkaart);
        bool bestaatTankkaart(Tankkaart tankkaart);

        void updateInBezitVan(Tankkaart tankkaart, Bestuurder bestuurder);
        void zetGeblokkeerd(Tankkaart tankkaart, bool geblokkeerd);
        void zetGeldigheidsDatum(Tankkaart tankkaart, DateTime geldigheidsdatum);
        void voegBrandstofToe(Tankkaart tankkaart, string brandstof);
        void updatePincode(Tankkaart tankkaart, string pincode);
    }
}
