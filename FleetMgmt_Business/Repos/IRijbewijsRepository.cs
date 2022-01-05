using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Repos {
    public interface IRijbewijsRepository {

        void voegRijbewijsToe(Rijbewijs r, Bestuurder b);
        void verwijderRijbewijs(RijbewijsEnum r, Bestuurder b);
        bool heeftRijbewijs(RijbewijsEnum r, Bestuurder b);
        List<Rijbewijs> toonRijbewijzen(Bestuurder b);
    }
}
