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

            //IEnumerable<Tankkaart> tk = repo.geefTankkaarten(null,null,null,null,null);

            //foreach(Tankkaart tank in tk) {
            //    Console.WriteLine(tank);
            //}

            //Tankkaart teUpdaten = repo.geefTankkaarten(8, null, null, null, null).ElementAt(0);
            //teUpdaten.zetGeblokkeerd(false);
            //teUpdaten.updatePincode("6969");

            //repo.bewerkTankkaart(teUpdaten);

            //Tankkaart nieuweTankkaart1 = new Tankkaart(new DateTime(2022, 10, 25), "2548", null, null);
            //repo.voegTankkaartToe(nieuweTankkaart1);

            //Tankkaart nieuweTankkaart = new Tankkaart(500,new DateTime(2025, 12, 25), "6869", null, new List<TankkaartBrandstof>() { TankkaartBrandstof.Benzine, TankkaartBrandstof.Diesel}, false);
            //repo.voegTankkaartToe(nieuweTankkaart);

            ////Voeg Bestuurder Toe
            Bestuurder b = new Bestuurder("90.02.01-999-02", "Gheysens", "Louis", new DateTime(1996, 06, 05));
            bm.voegBestuurderToe(b);


            ////Verwijder Bestuurder
            //bm.verwijderBestuurder(b.Id);

            //Update bestuurder

            //Zoek Bestuurder






        }
    }
}
