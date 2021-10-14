using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetMgmt_Business.Objects;

namespace FleetMgmt_Business.Repos {
    public interface IBestuurderRepository {
        void VoegBestuurderToe(Bestuurder bestuurder);
        void VerwijderBestuurder(Bestuurder bestuurder);
        IEnumerable<Bestuurder> ToonBestuurders();
        void GeefBestuurder(int id);
        void BewerkBestuurder(Bestuurder bestuurder);
        bool BestaatBestuurder(Bestuurder bestuurder);


    }
}
