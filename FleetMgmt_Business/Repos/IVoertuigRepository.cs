﻿using FleetMgmt_Business.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Repos {
    public interface IVoertuigRepository {

        void voegVoertuigToe(Voertuig voertuig);
        void verwijderVoertuig(Voertuig voertuig);
        Voertuig geefVoertuig(string chassisnummer);
        IEnumerable<Voertuig> toonVoertuigen(string merk, string model, string typeVoertuig, string brandstof,
            string kleur, int? aantalDeuren, bool strikt = true);
        bool bestaatVoertuig(string chassisnummer);
        bool bestaatVoertuig(Voertuig voertuig);
        void bewerkVoertuig(Voertuig voertuig, Bestuurder bestuurder);
    }
}
