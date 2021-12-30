using FleetMgmt_Business.Exceptions;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Managers {
    //PIETER COLPAERT
    public class VoertuigManager {

        private IVoertuigRepository repo;

        public VoertuigManager(IVoertuigRepository repo) {
            this.repo = repo;
        }

        public bool bestaatVoertuig(Voertuig voertuig) {
            try {
                if (voertuig == null) throw new VoertuigManagerException("VoertuigManager - bestaatVoertuig - Voertuig is null");
                return repo.bestaatVoertuig(voertuig);

            } catch (Exception ex) {

                throw new VoertuigManagerException("VoertuigManager - bestaatVoertuig", ex);
            }
        }

        public Voertuig geefVoertuig(string chassisnummer) {
            try {
                if (chassisnummer == null) throw new VoertuigManagerException("VoertuigManager - geefVoertuig - Voertuig is null");
                return repo.geefVoertuig(chassisnummer);

            } catch (Exception ex) {

                throw new VoertuigManagerException("VoertuigManager - geefVoertuig", ex);
            }
        }

        public IEnumerable<Voertuig> zoekVoertuigen(string chassisnummer, string merk, string model, string typeVoertuig, string brandstof,
            string kleur, int? aantalDeuren, string bestuurderId) {
            List<Voertuig> voertuigen = new List<Voertuig>();
            try {
                if(chassisnummer != null) {
                    if (repo.bestaatVoertuig(chassisnummer)) voertuigen.Add(repo.geefVoertuig(chassisnummer));
                } else {                   
                        voertuigen.AddRange(repo.toonVoertuigen(merk, model, typeVoertuig, brandstof, kleur, aantalDeuren, bestuurderId));                 
                }
                return voertuigen;

            } catch (Exception ex) {

                throw new VoertuigManagerException("VoertuigManager - toonVoertuigen", ex);
            }
        }
        public void updateVoertuig(Voertuig voertuig) {
            try {
                if (repo.bestaatVoertuig(voertuig.Chassisnummer)) {
                    Voertuig voertuigDB = repo.geefVoertuig(voertuig.Chassisnummer);
                    if (voertuigDB == voertuig) {
                        throw new VoertuigManagerException("VoertuigManager: updateVoertuig - Voertuigen verschillen niet!");
                    } else {
                        if (voertuigDB.Bestuurder == null && voertuig.Bestuurder != null) {
                            repo.bewerkVoertuig_BestuurderToevoegen(voertuig);
                        } else if (voertuigDB.Bestuurder != null && voertuig.Bestuurder == null) {
                            repo.bewerkVoertuig_BestuurderVerwijderen(voertuig);
                        } else if (voertuigDB.Bestuurder != null && voertuig.Bestuurder != null) {
                            repo.bewerkVoertuig_BestuurderWisselen(voertuig);
                        }
                    }
                }
            } catch (Exception ex) {

                throw new VoertuigManagerException("VoertuigManager: updateVoertuig", ex);
            }
        }

        public void verwijderVoertuig(Voertuig voertuig) {
            try {
                if (voertuig == null) throw new VoertuigManagerException("VoertuigManager - verwijderVoertuig - Voertuig is null");
                if (!repo.bestaatVoertuig(voertuig)) throw new VoertuigManagerException("VoertuigManager - verwijderVoertuig - Voertuig bestaat niet");
                repo.verwijderVoertuig(voertuig);

            } catch (Exception ex) {

                throw new VoertuigManagerException("VoertuigManager - verwijderVoertuig", ex);
            }
        }

        public Voertuig voegVoertuigToe(Voertuig voertuig) {
            try {
                if (voertuig == null) throw new VoertuigManagerException("VoertuigManager - voegVoertuigToe - Voertuig is null");
                if (repo.bestaatVoertuig(voertuig)) throw new VoertuigManagerException("VoertuigManager - voegVoertuigToe - Voertuig bestaat al");
                return repo.voegVoertuigToe(voertuig);

            } catch (Exception ex) {

                throw new VoertuigManagerException("VoertuigManager - voegVoertuigToe", ex);
            }
        }
    }
}
