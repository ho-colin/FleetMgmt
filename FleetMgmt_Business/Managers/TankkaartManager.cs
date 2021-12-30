using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using FleetMgmt_Business.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetMgmt_Business.Enums;

namespace FleetMgmt_Business.Managers {
    //COLIN MEERSCHMAN
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

        public IEnumerable<Tankkaart> geefTankkaarten(int? id, DateTime? geldigheidsDatum, string bestuurder, bool? geblokkeerd, TankkaartBrandstof? brandstof) {
            try {
                return repo.geefTankkaarten(id, geldigheidsDatum, bestuurder, geblokkeerd, brandstof);
            } catch (Exception ex) {
                throw new TankkaartException("TankkaartManager - geefTankkaarten",ex);
            }
        }

        public Tankkaart selecteerTankkaart(int id) {
            if (!bestaatTankkaart(id)) throw new TankkaartException("TankkaarManager : selecteerTankkaart - Tankkaart bestaat niet!");
            return repo.selecteerTankkaart(id);
        }

        public void verwijderTankkaart(int id) {
            try {
                repo.verwijderTankkaart(id);
            } catch (Exception ex) {
                throw new TankkaartException("TankkaartManager - verwijderTankkaart",ex);
            }
        }

        public Tankkaart voegTankkaartToe(Tankkaart tankkaart) {
            try {
               return repo.voegTankkaartToe(tankkaart);
            } catch (Exception ex) {
                throw new TankkaartException("TankkaartManager - voegTankkaartToe",ex);
            }
        }
    }
}
