using FleetMgmt_Business.Exceptions;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Managers {
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

        public Voertuig geefVoertuig(Voertuig voertuig) {
            try {
                if (voertuig == null) throw new VoertuigManagerException("VoertuigManager - geefVoertuig - Voertuig is null");
                return repo.geefVoertuig(voertuig.Chassisnummer);

            } catch (Exception ex) {

                throw new VoertuigManagerException("VoertuigManager - geefVoertuig", ex);
            }
        }

        public IEnumerable<Voertuig> toonVoertuigen(string chassinummer, string merk, string model, string typeVoertuig, string brandstof,
            string kleur, int? aantalDeuren, bool strikt = true) {
            try {
                return repo.toonVoertuigen(chassinummer, merk, model, typeVoertuig, brandstof, kleur, aantalDeuren, strikt);

            } catch (Exception ex) {

                throw new VoertuigManagerException("VoertuigManager - toonVoertuigen", ex);
            }
        }

        public void updateAantalDeuren(Voertuig voertuig, int aantal) {
            try {
                if (voertuig == null) throw new VoertuigManagerException("VoertuigManager - updateAantalDeuren - Voertuig is leeg");
                if (!repo.bestaatVoertuig(voertuig)) throw new VoertuigManagerException("VoertuigManager - updateAantalDeuren - Voertuig bestaat niet");
                Voertuig dbv = repo.geefVoertuig(voertuig.Chassisnummer);
                if (voertuig == dbv) throw new VoertuigManagerException("Voertuigmanager - updateAantalDeuren - Geen verschillen werden toegepast!");
                repo.updateAantalDeuren(voertuig, aantal);

            } catch (Exception ex) {

                throw new VoertuigManagerException("VoertuigManager - updateAantalDeuren", ex);
            }
        }

        public void updateBestuurder(Voertuig voertuig, Bestuurder bestuurder) {
            try {
                if (voertuig == null || bestuurder == null) throw new VoertuigManagerException("VoertuigManager - updateBestuurder - Voertuig/Bestuurder is leeg");
                if (!repo.bestaatVoertuig(voertuig)) throw new VoertuigManagerException("VoertuigManager - updateBestuurder - Voertuig bestaat niet");
                Voertuig dbvr = repo.geefVoertuig(voertuig.Chassisnummer);
                if (voertuig == dbvr) throw new VoertuigManagerException("Voertuigmanager - updateBestuurder - Geen verschillen werden toegepast");
                repo.updateBestuurder(voertuig, bestuurder);

            } catch (Exception ex) {

                throw new VoertuigManagerException("VoertuigManager - updateBestuurder", ex);
            }
        }

        public void updateKleur(Voertuig voertuig, string kleur) {
            try {
                if (voertuig == null || kleur == null) throw new VoertuigManagerException("VoertuigManager - updateKleur - Voertuig/Kleur is leeg");
                if (!repo.bestaatVoertuig(voertuig)) throw new VoertuigManagerException("VoertuigManager - updateKleur - Voertuig bestaat niet");
                Voertuig dbvrt = repo.geefVoertuig(voertuig.Chassisnummer);
                if (voertuig == dbvrt) throw new VoertuigManagerException("Voertuigmanager - updateKleur - Geen verschillen werden toegepast!");
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
