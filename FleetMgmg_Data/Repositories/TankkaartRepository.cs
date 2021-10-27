using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmg_Data.Repositories {
    public class TankkaartRepository : ITankkaartRepository {
        public bool bestaatTankkaart(int id) {
            throw new NotImplementedException();
        }

        public void bewerkTankkaart(Tankkaart tankkaart) {
            throw new NotImplementedException();
        }

        public Tankkaart geefTankkaart(int id) {
            throw new NotImplementedException();
        }

        public IEnumerable<Tankkaart> geefTankkaarten(int id, DateTime geldigheidsDatum, string bestuurderId, bool geblokkeerd) {
            throw new NotImplementedException();
        }

        public void verwijderTankkaart(int id) {
            throw new NotImplementedException();
        }

        public void voegTankkaartToe(Tankkaart tankkaart) {
            throw new NotImplementedException();
        }
    }
}
