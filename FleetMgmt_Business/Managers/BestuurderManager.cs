using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using FleetMgmt_Business.Exceptions;
using System.Collections;

namespace FleetMgmt_Business.Managers {
    public class BestuurderManager : IBestuurderRepository {

        private IBestuurderRepository repo;

        public BestuurderManager(IBestuurderRepository repo) {
            this.repo = repo;
        }

        public bool bestaatBestuurder(int id) {
            if (!repo.bestaatBestuurder(id)) return false;
            else
                return true;
        }

        public void bewerkBestuurder(Bestuurder bestuurder) {
            if (!repo.bestaatBestuurder(bestuurder.Id)) throw new BestuurderException("Bestuurdermanager - bewerkBestuurder - Bestuurder bestaat niet");
            else
                repo.bewerkBestuurder(bestuurder);

        }

        public void geefBestuurder(int id) {
            if (id <= 0) throw new BestuurderException("Bestuurdermanager - geefBestuurder - Bestuurder bestaat niet");
            else
                repo.geefBestuurder(id);
        }

        public IEnumerable<Bestuurder> toonBestuurders() {
            return repo.toonBestuurders();
        }

        public void verwijderBestuurder(int id) {
            if (!repo.bestaatBestuurder(id)) throw new BestuurderException("Bestuurdermanager - verwijderBestuurder - Bestuurder bestaat niet");
            else
                repo.verwijderBestuurder(id);
        }

        public void voegBestuurderToe(Bestuurder bestuurder) {
            if (repo.bestaatBestuurder(bestuurder.Id)) throw new BestuurderException("Bestuurdermanager - voegBestuurderToe - Bestuurder bestaat reeds");
            else
                repo.voegBestuurderToe(bestuurder);
        }
    }
}
