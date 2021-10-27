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
        Voertuig geefVoertuig(string chassinummer);
        IEnumerable<Voertuig> toonVoertuigen(string chassinummer, string merk, string model, string typeVoertuig, string brandstof,
            string kleur, int? aantalDeuren, bool strikt = true);        
        bool bestaatVoertuig(Voertuig voertuig);

        void updateBestuurder(Voertuig voertuig, Bestuurder bestuurder);
        void updateAantalDeuren(Voertuig voertuig, int aantal);
        void updateKleur(Voertuig voertuig, string kleur);
    }
}
