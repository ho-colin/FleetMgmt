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

        public bool bestaatBestuurder(Bestuurder bestuurder) {
            if (string.IsNullOrEmpty(bestuurder.Voornaam) && string.IsNullOrEmpty(bestuurder.Naam))
                throw new BestuurderException("Bestuurdermanager - BestaatBestuurder - Bestuurder is null");
            else if (!repo.bestaatBestuurder(bestuurder)) return false;
            else
                return true;
        }

        public void bewerkBestuurder(Bestuurder bestuurder) {
            if (string.IsNullOrEmpty(bestuurder.Voornaam) && string.IsNullOrEmpty(bestuurder.Naam))
                throw new BestuurderException("Bestuurdermanager - BewerkBestuurder - Bestuurder is null");
            else if (!repo.bestaatBestuurder(bestuurder))
                throw new BestuurderException("Bestuurdermanager - BewerkBestuurder - Bestuurder bestaat niet");
            else
                repo.bewerkBestuurder(bestuurder);
        }

        public void geefBestuurder(int id) {
            if (id <= 0) throw new BestuurderException("Bestuurdermanager - GeefBestuurder - Bestuurder bestaat niet");
            else
                repo.geefBestuurder(id);
        }

        public IEnumerable<Bestuurder> toonBestuurders() {
            return repo.toonBestuurders();
        }

        public void verwijderBestuurder(Bestuurder bestuurder) {
            if (string.IsNullOrEmpty(bestuurder.Voornaam) && string.IsNullOrEmpty(bestuurder.Naam))
                throw new BestuurderException("Bestuurdermanager - VerwijderBestuurder - Bestuurder is null");
            else if (!repo.bestaatBestuurder(bestuurder))
                throw new BestuurderException("Bestuurdermanager - VerwijderBestuurder - Bestuurder bestaat niet");
            else
                repo.verwijderBestuurder(bestuurder);
        }

        public void voegBestuurderToe(Bestuurder bestuurder) {
            if (string.IsNullOrEmpty(bestuurder.Voornaam) && string.IsNullOrEmpty(bestuurder.Naam))
                throw new BestuurderException("Bestuurdermanager - VoegBestuurderToe - Bestuurder is null");
            else if (repo.bestaatBestuurder(bestuurder))
                throw new BestuurderException("Bestuurdermanager - VoegBestuurderToe - Bestuurder bestaat al");
            else
                repo.voegBestuurderToe(bestuurder);
        }
    }
}
