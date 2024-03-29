﻿using System;
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
    //LOUIS GHEYSENS
    public class BestuurderManager : IBestuurderRepository {

        private IBestuurderRepository repo;

        public BestuurderManager(IBestuurderRepository repo) {
            this.repo = repo;
        }

        public bool bestaatBestuurder(string rijksregisterNummer) {
            try {
                return repo.bestaatBestuurder(rijksregisterNummer);
            }catch(Exception ex) {
                throw new BestuurderManagerException("BestuurderManager: bestaatBestuurder -", ex);
            }
        }


        public void bewerkBestuurder(Bestuurder bestuurder) {
            try {
                repo.bewerkBestuurder(bestuurder);
            }catch(Exception ex) {
                throw new BestuurderManagerException("BestuurderManager: bewerkBestuurder -", ex);
            }

        }

        public IEnumerable<Bestuurder> toonBestuurders(string rijksregisterNummer, string achterNaam, string voorNaam, DateTime? geboortedatum, int? tankkaartId, string rijbewijs) {
            try {
                return repo.toonBestuurders(rijksregisterNummer, achterNaam, voorNaam, geboortedatum, tankkaartId, rijbewijs);
            }
            catch (Exception ex) {
                throw new BestuurderManagerException("BestuurderManager: toonBestuurders - "+ex.Message, ex);
            }
        }

        public Bestuurder selecteerBestuurder(string rijksregisterNummer) {
            try {
                if (!repo.bestaatBestuurder(rijksregisterNummer)) throw new BestuurderException("BestuurderManager: selecteerBestuurder - bestuurder bestaat niet!");
                return repo.selecteerBestuurder(rijksregisterNummer);
            }catch(Exception ex) {
                throw new BestuurderManagerException("BestuurderManager: SelecteerBestuurder - gefaald", ex);
            }
        }

        public void verwijderBestuurder(Bestuurder bestuurder) {
            try {
                repo.verwijderBestuurder(bestuurder);
            }
            catch(Exception ex) {
                throw new BestuurderManagerException("BestuurderManager: verwijderBestuurder - gefaald", ex);
            }

        }

        public Bestuurder voegBestuurderToe(Bestuurder bestuurder) {
            try {
                if (this.bestaatBestuurder(bestuurder.Rijksregisternummer)) throw new BestuurderException("BestuurderManager : Bestuurder me dit rijksregisternummer bestaat al!");
                return repo.voegBestuurderToe(bestuurder);
            }catch(Exception ex) {
                throw new BestuurderException("BestuurderManager: VoegBestuurderToe - gefaald", ex);
            }
        }
    }
}
