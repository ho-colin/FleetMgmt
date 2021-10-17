using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetMgmt_Business.Objects;

namespace FleetMgmt_Business.Repos {
    public interface IBestuurderRepository {
        void voegBestuurderToe(Bestuurder bestuurder);
        void verwijderBestuurder(Bestuurder bestuurder);
        IEnumerable<Bestuurder> toonBestuurders();
        void geefBestuurder(int id);
        //void bewerkBestuurder(Bestuurder bestuurder);
        bool bestaatBestuurder(Bestuurder bestuurder);

        void updateTankkaart(Bestuurder bestuurder,Tankkaart tankkaart);
        void updateVoertuig(Bestuurder bestuurder, Voertuig voertuig);

        void voegRijbewijsToe(Bestuurder bestuurder, Rijbewijs rijbewijs);
        void verwijderRijbewijs(Bestuurder bestuurder, Rijbewijs rijbewijs);


    }
}
