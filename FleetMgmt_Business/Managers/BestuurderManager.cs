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
        public IBestuurderRepository repo;

        public BestuurderManager(IBestuurderRepository repo) {
            this.repo = repo;
        }

        public bool BestaatBestuurder(Bestuurder bestuurder) {
            if (string.IsNullOrEmpty(bestuurder.Voornaam) && string.IsNullOrEmpty(bestuurder.Naam))
                throw new BestuurderException("Bestuurdermanager - BestaatBestuurder - Bestuurder is null");
            else if (!repo.BestaatBestuurder(bestuurder)) return false;
            else
                return true;
        }

        public void BewerkBestuurder(Bestuurder bestuurder) {
            if (string.IsNullOrEmpty(bestuurder.Voornaam) && string.IsNullOrEmpty(bestuurder.Naam))
                throw new BestuurderException("Bestuurdermanager - BewerkBestuurder - Bestuurder is null");
            else if (!repo.BestaatBestuurder(bestuurder))
                throw new BestuurderException("Bestuurdermanager - BewerkBestuurder - Bestuurder bestaat niet");
            else
                repo.BewerkBestuurder(bestuurder);
        }

        public void GeefBestuurder(int id) {
            if (id <= 0) throw new BestuurderException("Bestuurdermanager - GeefBestuurder - Bestuurder bestaat niet");
            else
                GeefBestuurder(id);
        }

        public IEnumerable<Bestuurder> ToonBestuurders() {
            return ToonBestuurders();
        }

        public void VerwijderBestuurder(Bestuurder bestuurder) {
            if (string.IsNullOrEmpty(bestuurder.Voornaam) && string.IsNullOrEmpty(bestuurder.Naam))
                throw new BestuurderException("Bestuurdermanager - VerwijderBestuurder - Bestuurder is null");
            else if (!repo.BestaatBestuurder(bestuurder))
                throw new BestuurderException("Bestuurdermanager - VerwijderBestuurder - Bestuurder bestaat niet");
            else
                repo.VerwijderBestuurder(bestuurder);
        }

        public void VoegBestuurderToe(Bestuurder bestuurder) {
            if (string.IsNullOrEmpty(bestuurder.Voornaam) && string.IsNullOrEmpty(bestuurder.Naam))
                throw new BestuurderException("Bestuurdermanager - VoegBestuurderToe - Bestuurder is null");
            else if (repo.BestaatBestuurder(bestuurder))
                throw new BestuurderException("Bestuurdermanager - VoegBestuurderToe - Bestuurder bestaat al");
            else
                repo.VoegBestuurderToe(bestuurder);
        }
    }
}
