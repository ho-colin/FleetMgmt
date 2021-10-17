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
            return repo.bestaatVoertuig(voertuig);
        }

        public Voertuig geefVoertuig(Voertuig voertuig) {
            return repo.geefVoertuig(voertuig);
        }

        public IEnumerable<Voertuig> toonVoertuigen() {
            return repo.toonVoertuigen();
        }

        public void updateAantalDeuren(Voertuig voertuig, int aantal) {
            repo.updateAantalDeuren(voertuig, aantal);
        }

        public void updateBestuurder(Voertuig voertuig, Bestuurder bestuurder) {
            repo.updateBestuurder(voertuig, bestuurder);
        }

        public void updateKleur(Voertuig voertuig, string kleur) {
            repo.updateKleur(voertuig, kleur);
        }

        public void verwijderVoertuig(Voertuig voertuig) {
            repo.verwijderVoertuig(voertuig);
        }

        public void voegVoertuigToe(Voertuig voertuig) {
            repo.voegVoertuigToe(voertuig);
        }
    }
}
