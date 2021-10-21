using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using FleetMgmt_Business.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmt_Business.Managers {
    public class TankkaartManager : ITankkaartRepository {

        private ITankkaartRepository repo;

        public TankkaartManager(ITankkaartRepository repo) {
            this.repo = repo;
        }

        public bool bestaatTankkaart(Tankkaart tankkaart) {
            return repo.bestaatTankkaart(tankkaart);
        }

        public Tankkaart geefTankkaart(int id) {
            return repo.geefTankkaart(id);
        }

        public IEnumerable<Tankkaart> geefTankkaarten() {
            return repo.geefTankkaarten();
        }

        public void updateInBezitVan(Tankkaart tankkaart, Bestuurder bestuurder) {
            repo.updateInBezitVan(tankkaart, bestuurder);
        }

        public void updatePincode(Tankkaart tankkaart, string pincode) {
            if (!repo.bestaatTankkaart(tankkaart)) throw new TankkaartException("TankkaartManager : updatePincode - Tankkaart bestaat niet!");
            repo.updatePincode(tankkaart, pincode);
        }

        public void verwijderTankkaart(Tankkaart tankkaart) {
            if (!repo.bestaatTankkaart(tankkaart)) throw new TankkaartException("TankkaartManager : verwijderTankkaart - Tankkaart bestaat niet!");
            repo.bestaatTankkaart((tankkaart));
        }

        public void voegBrandstofToe(Tankkaart tankkaart, string brandstof) {
            repo.voegBrandstofToe(tankkaart, brandstof);
        }

        public void voegTankkaartToe(Tankkaart tankkaart) {
            repo.voegTankkaartToe(tankkaart);
        }

        public void zetGeblokkeerd(Tankkaart tankkaart, bool geblokkeerd) {
            repo.zetGeblokkeerd(tankkaart, geblokkeerd);
        }

        public void zetGeldigheidsDatum(Tankkaart tankkaart, DateTime geldigheidsdatum) {
            repo.zetGeldigheidsDatum(tankkaart, geldigheidsdatum);
        }
    }
}
