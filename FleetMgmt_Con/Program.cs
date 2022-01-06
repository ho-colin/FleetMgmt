using FleetMgmg_Data.Repositories;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Managers;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FleetMgmt_Con {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello World!");

            //Sectie Tankkaart
            ITankkaartRepository repo = new TankkaartRepository();
            TankkaartManager tm = new TankkaartManager(repo);

            //Sectie Bestuurder
            IBestuurderRepository Brepo = new BestuurderRepository();
            BestuurderManager bm = new BestuurderManager(Brepo);

            //Sectie Rijbewijs
            IRijbewijsRepository rRepo = new RijbewijsRepository();
            RijbewijsManager rm = new RijbewijsManager(rRepo);

            Bestuurder b = bm.selecteerBestuurder("00.10.01.001-68");
            //rm.voegRijbewijsToe(new Rijbewijs("B", DateTime.Today), b);

            Bestuurder metRibba = rm.toonRijbewijzen(b);

            bool heeftDieDa = rm.heeftRijbewijs(RijbewijsEnum.AM, b);

            rm.verwijderRijbewijs(RijbewijsEnum.AM, b);







        }
    }
}
