﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetMgmt_Business.Objects;

namespace FleetMgmt_Business.Repos {
    public interface IBestuurderRepository {
        Bestuurder voegBestuurderToe(Bestuurder bestuurder);
        void verwijderBestuurder(int id);
        IEnumerable<Bestuurder> toonBestuurders(string rijksregisternummer, 
            string naam, string voornamam, DateTime? geboortedatum);
        Bestuurder selecteerBestuurder(int id);
        void bewerkBestuurder(Bestuurder bestuurder);
        bool bestaatBestuurder(int id);




    }
}
