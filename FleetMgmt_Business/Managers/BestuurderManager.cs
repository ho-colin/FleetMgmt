using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using FleetMgmt_Business.Exceptions;
using System.Collections;
using System.Diagnostics;

namespace FleetMgmt_Business.Managers {
    public class BestuurderManager : IBestuurderRepository {

        private IBestuurderRepository repo;

        public BestuurderManager(IBestuurderRepository repo) {
            this.repo = repo;
        }

        public bool bestaatBestuurder(string rijks) {
            try {
                return repo.bestaatBestuurder(rijks);
            }catch(Exception ex) {
                throw new BestuurderManagerException("BestuurderManager: bestaatBestuurder -", ex);
            }
        }


        public void bewerkBestuurder(Bestuurder bestuurder) {
            try {
                if (!repo.bestaatBestuurder(bestuurder.Rijksregisternummer)) throw new BestuurderException("BestuurderManager: bewerkBestuurder - Bestuurder bestaat niet");
                else
                    repo.bewerkBestuurder(bestuurder);
            }catch(Exception ex) {
                throw new BestuurderManagerException("BestuurderManager: bewerkBestuurder -", ex);
            }

        }

        public IEnumerable<Bestuurder> toonBestuurders(string rijksregisternummer, string naam, string voornamam, DateTime? geboortedatum) {
            try {
                return repo.toonBestuurders(rijksregisternummer, naam, voornamam, geboortedatum);
            }
            catch (Exception ex) {
                throw new BestuurderManagerException("BestuurderManager: toonBestuurders - gefaald", ex);
            }
        }

        public Bestuurder selecteerBestuurder(string rijks) {
            try {
                if (string.IsNullOrWhiteSpace(rijks)) throw new BestuurderException("selecteerBestuurder: selecteerBestuurder - id is 0");
                if (!repo.bestaatBestuurder(rijks)) throw new BestuurderException("BestuurderManager: selecteerBestuurder - bestuurder bestaat niet!");
                return repo.selecteerBestuurder(rijks);
            }catch(Exception ex) {
                throw new BestuurderManagerException("BestuurderManager: SelecteerBestuurder - gefaald", ex);
            }
        }

        public void verwijderBestuurder(string rijks) {
            if (!repo.bestaatBestuurder(rijks)) throw new BestuurderException("BestuurderManager: verwijderBestuurder - Bestuurder bestaat niet");
            else
                repo.verwijderBestuurder(rijks);
            Console.Write("Id werd verwijderd!");
        }

        public Bestuurder voegBestuurderToe(Bestuurder bestuurder) {
            if (repo.bestaatBestuurder(bestuurder.Rijksregisternummer)) throw new BestuurderException("BestuurderManager: voegBestuurderToe - Bestuurder bestaat reeds");
            try {
                return repo.voegBestuurderToe(bestuurder);
                Console.Write($"{bestuurder.Voornaam} is toegevoegd!");
            }catch(Exception ex) {
                throw new BestuurderException("BestuurderManager: VoegBestuurderToe - gefaald", ex);
            }
        }
    }
}
