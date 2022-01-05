using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Managers {
    public class RijbewijsManager : IRijbewijsRepository {

        private IRijbewijsRepository repo;

        public RijbewijsManager(IRijbewijsRepository repo) {
            this.repo = repo;
        }

        public bool heeftRijbewijs(RijbewijsEnum r, Bestuurder b) {
            return repo.heeftRijbewijs(r, b);
        }

        public List<Rijbewijs> toonRijbewijzen(Bestuurder b) {
            return repo.toonRijbewijzen(b);
        }

        public void verwijderRijbewijs(RijbewijsEnum r, Bestuurder b) {
            repo.verwijderRijbewijs(r, b);
        }

        public void voegRijbewijsToe(Rijbewijs r, Bestuurder b) {
            repo.voegRijbewijsToe(r, b);
        }
    }
}
