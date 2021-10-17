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
            if (!repo.bestaatBestuurder(bestuurder)) return false;
            else
                return true;
        }

        //public void bewerkBestuurder(Bestuurder bestuurder) {
        //    if (string.IsNullOrEmpty(bestuurder.Voornaam) && string.IsNullOrEmpty(bestuurder.Naam))
        //        throw new BestuurderException("Bestuurdermanager - bewerkBestuurder - Bestuurder is null");
        //    else if (!repo.bestaatBestuurder(bestuurder))
        //        throw new BestuurderException("Bestuurdermanager - bewerkBestuurder - Bestuurder bestaat niet");
        //    else
        //        repo.bewerkBestuurder(bestuurder);
        //}

        public void geefBestuurder(int id) {
            if (id <= 0) throw new BestuurderException("Bestuurdermanager - geefBestuurder - Bestuurder bestaat niet");
            else
                repo.geefBestuurder(id);
        }

        public IEnumerable<Bestuurder> toonBestuurders() {
            return repo.toonBestuurders();
        }

        public void updateTankkaart(Bestuurder bestuurder, Tankkaart tankkaart) {
                repo.updateTankkaart(bestuurder, tankkaart);      
        }

        public void updateVoertuig(Bestuurder bestuurder, Voertuig voertuig) {
            repo.updateVoertuig(bestuurder, voertuig);
        }

        public void verwijderBestuurder(Bestuurder bestuurder) {
            if (string.IsNullOrEmpty(bestuurder.Voornaam) && string.IsNullOrEmpty(bestuurder.Naam))
                throw new BestuurderException("Bestuurdermanager - verwijderBestuurder - Bestuurder is null");
            else if (!repo.bestaatBestuurder(bestuurder))
                throw new BestuurderException("Bestuurdermanager - verwijderBestuurder - Bestuurder bestaat niet");
            else
                repo.verwijderBestuurder(bestuurder);
        }

        public void verwijderRijbewijs(Bestuurder bestuurder, Rijbewijs rijbewijs) {
            repo.verwijderRijbewijs(bestuurder, rijbewijs);
        }

        public void voegBestuurderToe(Bestuurder bestuurder) {
            if (string.IsNullOrEmpty(bestuurder.Voornaam) && string.IsNullOrEmpty(bestuurder.Naam))
                throw new BestuurderException("Bestuurdermanager - voegBestuurderToe - Bestuurder is null");
            else if (repo.bestaatBestuurder(bestuurder))
                throw new BestuurderException("Bestuurdermanager - voegBestuurderToe - Bestuurder bestaat al");
            else
                repo.voegBestuurderToe(bestuurder);
        }

        public void voegRijbewijsToe(Bestuurder bestuurder, Rijbewijs rijbewijs) {
            repo.voegRijbewijsToe(bestuurder, rijbewijs);
        }
    }
}
