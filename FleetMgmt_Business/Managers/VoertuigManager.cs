﻿using FleetMgmt_Business.Exceptions;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Managers {
    //PIETER COLPAERT -> VOLLEDIG REFACTOR: COLIN MEERSCHMAN
    public class VoertuigManager : IVoertuigRepository {

        private IVoertuigRepository repo;

        public VoertuigManager(IVoertuigRepository repo) {
            this.repo = repo;
        }

        public bool bestaatVoertuig(string chassisnummer) {
            try {
                return repo.bestaatVoertuig(chassisnummer);
            } catch (Exception ex) {
                throw new VoertuigManagerException(ex.Message,ex);
            }
        }

        public bool bestaatVoertuigNmrPlaat(string nummerplaat) {
            return repo.bestaatVoertuigNmrPlaat(nummerplaat);
        }

        public void bewerkVoertuig(Voertuig voertuig) {
            try {
                repo.bewerkVoertuig(voertuig);
            } catch (Exception ex) {
                throw new VoertuigManagerException(ex.Message, ex);
            }
        }

        public Voertuig geefVoertuig(string chassisnummer) {
            try {
                return repo.geefVoertuig(chassisnummer);
            } catch (Exception ex) {
                throw new VoertuigManagerException(ex.Message, ex);
            }
        }

        public IEnumerable<Voertuig> toonVoertuigen(string chassisnummer, string merk, string model, string typeVoertuig, string brandstof, string kleur, int? aantalDeuren, string bestuurderId, string nummerplaat) {
            try {
                return repo.toonVoertuigen(chassisnummer, merk, model, typeVoertuig, brandstof, kleur, aantalDeuren, bestuurderId, nummerplaat);
            } catch (Exception ex) {
                throw new VoertuigManagerException(ex.Message, ex);
            }
        }

        public void verwijderVoertuig(Voertuig voertuig) {
            try {
                repo.verwijderVoertuig(voertuig);
            } catch (Exception ex) {
                throw new VoertuigManagerException(ex.Message,ex);
            }
        }

        public Voertuig voegVoertuigToe(Voertuig voertuig) {
            try {
                if (this.bestaatVoertuig(voertuig.Chassisnummer)) throw new VoertuigManagerException("Voertuig met dit chassisnummer bestaat al!");
                if (this.bestaatVoertuigNmrPlaat(voertuig.Nummerplaat)) throw new VoertuigManagerException("Voertuig met deze nummerplaat bestaat al!");
                return repo.voegVoertuigToe(voertuig);
            } catch (Exception ex) {
                throw new VoertuigManagerException(ex.Message, ex);
            }
        }
    }
}
