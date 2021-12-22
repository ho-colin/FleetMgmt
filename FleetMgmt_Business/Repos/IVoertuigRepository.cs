using FleetMgmt_Business.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Repos {
    public interface IVoertuigRepository {

        Voertuig voegVoertuigToe(Voertuig voertuig);
        void verwijderVoertuig(Voertuig voertuig);
        Voertuig geefVoertuig(string chassisnummer);
        IEnumerable<Voertuig> toonVoertuigen(string merk, string model, string typeVoertuig, string brandstof,
            string kleur, int? aantalDeuren, string bestuurderId);
        bool bestaatVoertuig(string chassisnummer);
        bool bestaatVoertuig(Voertuig voertuig);
        void bewerkVoertuig_GeenBestuurder(Voertuig voertuig);
        void bewerkVoertuig_BestuurderToevoegen(Voertuig voertuig);
        void bewerkVoertuig_BestuurderVerwijderen(Voertuig voertuig);
        void bewerkVoertuig_BestuurderWisselen(Voertuig voertuig);
    }
}
