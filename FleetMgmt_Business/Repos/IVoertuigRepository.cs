using FleetMgmt_Business.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Repos {
    public interface IVoertuigRepository {
        //PIETER COLPAERT

        Voertuig voegVoertuigToe(Voertuig voertuig);
        void verwijderVoertuig(Voertuig voertuig);
        Voertuig geefVoertuig(string chassisnummer);
        IEnumerable<Voertuig> toonVoertuigen(string chassisnummer, string merk, string model, string typeVoertuig, string brandstof, string kleur, int? aantalDeuren, string bestuurderId, string nummerplaat);
        bool bestaatVoertuig(string chassisnummer);
        void bewerkVoertuig(Voertuig voertuig);
    }
}
