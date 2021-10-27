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

        public bool bestaatTankkaart(int id) {
            try {
                return repo.bestaatTankkaart(id);
            } catch (Exception ex) {
                throw new TankkaartException("TankkaartManager - bestaatTankkaart",ex);
            }
        }

        public void bewerkTankkaart(Tankkaart tankkaart) {
            try {
                repo.bewerkTankkaart(tankkaart);
            } catch (Exception ex) {
                throw new TankkaartException("TankkaartManager - bewerkTankkaart",ex);
            }
        }

        public Tankkaart geefTankkaart(int id) {
            try {
                return repo.geefTankkaart(id);
            } catch (Exception ex) {
                throw new TankkaartException("TankkaartManager - geefTankkaart",ex);
            }
        }

        public IEnumerable<Tankkaart> geefTankkaarten(int id, DateTime geldigheidsDatum, string bestuurderId, bool geblokkeerd) {
            try {
                return repo.geefTankkaarten(id, geldigheidsDatum, bestuurderId, geblokkeerd);
            } catch (Exception ex) {
                throw new TankkaartException("TankkaartManager - geefTankkaarten",ex);
            }
        }

        public void verwijderTankkaart(int id) {
            try {
                repo.verwijderTankkaart(id);
            } catch (Exception ex) {
                throw new TankkaartException("TankkaartManager - verwijderTankkaart",ex);
            }
        }

        public void voegTankkaartToe(Tankkaart tankkaart) {
            try {
                repo.voegTankkaartToe(tankkaart);
            } catch (Exception ex) {
                throw new TankkaartException("TankkaartManager - voegTankkaartToe",ex);
            }
        }
    }
}
