using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Exceptions;
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
            try {
                return repo.heeftRijbewijs(r, b);
            }
            catch(Exception ex) {
                throw new RijbewijsManagerException("RijbewijsManager: heeftRijbewijs - gefaald", ex);
            }
        }

        public Bestuurder toonRijbewijzen(Bestuurder b) {
            try {
                return repo.toonRijbewijzen(b);
            }
            catch(Exception ex) {
                throw new RijbewijsManagerException("RijbewijsManager: toonRijbewijzen - gefaald", ex);
            }
        }

        public void verwijderRijbewijs(RijbewijsEnum r, Bestuurder b) {
            try {
                repo.verwijderRijbewijs(r, b);
            }
            catch(Exception ex) {
                throw new RijbewijsManagerException("RijbewijsManager: verwijderRijbewijs - gefaald", ex);
            }
        }

        public void voegRijbewijsToe(Rijbewijs r, Bestuurder b) {
            try {
                repo.voegRijbewijsToe(r, b);
            }
            catch(Exception ex) {
                throw new RijbewijsManagerException("RijbewijsManager: voegRijbewijsToe - gefaald", ex);
            }
        }
    }
}
