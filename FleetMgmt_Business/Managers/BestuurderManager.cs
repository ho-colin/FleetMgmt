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
            try {
                if (!repo.bestaatBestuurder(id)) return false;
                else
                    return true;
            }catch(Exception ex) {
                throw new BestuurderManagerException("BestuurderManager: bestaatBestuurder -", ex);
            }
        }

        public void bewerkBestuurder(Bestuurder bestuurder) {
            try {
                if (!repo.bestaatBestuurder(bestuurder.Id)) throw new BestuurderException("BestuurderManager: bewerkBestuurder - Bestuurder bestaat niet");
                else
                    repo.bewerkBestuurder(bestuurder);
            }catch(Exception ex) {
                throw new BestuurderManagerException("BestuurderManager: bewerkBestuurder -", ex);
            }

        }

        public IEnumerable<(Bestuurder, Tankkaart, Voertuig)> toonBestuurders(string rijksregisternummer, string naam, string voornamam, DateTime geboortedatum, bool strikt = true) {
            try {
                return repo.toonBestuurders(rijksregisternummer, naam, voornamam, geboortedatum);
            }
            catch (Exception ex) {
                throw new BestuurderManagerException("BestuurderManager: toonBestuurders -", ex);
            }
        }

        public void verwijderBestuurder(int id) {
            if (!repo.bestaatBestuurder(id)) throw new BestuurderException("BestuurderManager: verwijderBestuurder - Bestuurder bestaat niet");
            else
                repo.verwijderBestuurder(id);
        }

        public void voegBestuurderToe(Bestuurder bestuurder) {
            if (repo.bestaatBestuurder(bestuurder.Id)) throw new BestuurderException("BestuurderManager: voegBestuurderToe - Bestuurder bestaat reeds");
            else
                repo.voegBestuurderToe(bestuurder);
        }
    }
}
