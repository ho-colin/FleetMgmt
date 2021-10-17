using FleetMgmt_Business.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Repos {
    public interface IVoertuigRepository {

        void voegVoertuigToe(Voertuig voertuig);
        void verwijderVoertuig(Voertuig voertuig);
        IEnumerable<Voertuig> toonVoertuigen();
        Voertuig geefVoertuig(Voertuig voertuig);
        //void bewerkVoertuig(Voertuig voertuig);
        bool bestaatVoertuig(Voertuig voertuig);

        void updateBestuurder(Voertuig voertuig, Bestuurder bestuurder);
        void updateAantalDeuren(Voertuig voertuig, int aantal);
        void updateKleur(Voertuig voertuig, string kleur);
    }
}
