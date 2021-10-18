using FleetMgmt_Business.Exceptions;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Managers {
    public class VoertuigManager : IVoertuigRepository {

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

        public Voertuig geefVoertuig(Voertuig voertuig) {
            try {
                if (voertuig == null) throw new VoertuigManagerException("VoertuigManager - geefVoertuig - Voertuig is null");
                return repo.geefVoertuig(voertuig);

            } catch (Exception ex) {

                throw new VoertuigManagerException("VoertuigManager - geefVoertuig", ex);
            }
        }

        public IEnumerable<Voertuig> toonVoertuigen() {
            try {
                return repo.toonVoertuigen();

            } catch (Exception ex) {

                throw new VoertuigManagerException("VoertuigManager - toonVoertuigen", ex);
            }
        }

        public void updateAantalDeuren(Voertuig voertuig, int aantal) {
            try {
                if (voertuig == null) throw new VoertuigManagerException("VoertuigManager - updateAantalDeuren - Voertuig is leeg");
                if (!repo.bestaatVoertuig(voertuig)) throw new VoertuigManagerException("VoertuigManager - updateAantalDeuren - Voertuig bestaat niet");
                repo.updateAantalDeuren(voertuig, aantal);

            } catch (Exception ex) {

                throw new VoertuigManagerException("VoertuigManager - updateAantalDeuren", ex);
            }
        }

        public void updateBestuurder(Voertuig voertuig, Bestuurder bestuurder) {
            try {
                if (voertuig == null || bestuurder == null) throw new VoertuigManagerException("VoertuigManager - updateBestuurder - Voertuig/Bestuurder is leeg");
                if (!repo.bestaatVoertuig(voertuig)) throw new VoertuigManagerException("VoertuigManager - updateBestuurder - Voertuig bestaat niet");
                repo.updateBestuurder(voertuig, bestuurder);

            } catch (Exception ex) {

                throw new VoertuigManagerException("VoertuigManager - updateBestuurder", ex);
            }
        }

        public void updateKleur(Voertuig voertuig, string kleur) {
            try {
                if (voertuig == null || kleur == null) throw new VoertuigManagerException("VoertuigManager - updateKleur - Voertuig/Kleur is leeg");
                if (!repo.bestaatVoertuig(voertuig)) throw new VoertuigManagerException("VoertuigManager - updateKleur - Voertuig bestaat niet");
                repo.updateKleur(voertuig, kleur);

            } catch (Exception ex) {

                throw new VoertuigManagerException("VoertuigManager - updateKleur", ex);
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

        public void voegVoertuigToe(Voertuig voertuig) {
            try {
                if (voertuig == null) throw new VoertuigManagerException("VoertuigManager - voegVoertuigToe - Voertuig is null");
                if (repo.bestaatVoertuig(voertuig)) throw new VoertuigManagerException("VoertuigManager - voegVoertuigToe - Voertuig bestaat al");
                repo.voegVoertuigToe(voertuig);

            } catch (Exception ex) {

                throw new VoertuigManagerException("VoertuigManager - voegVoertuigToe", ex);
            }
        }
    }
}
