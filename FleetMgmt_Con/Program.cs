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

            ITankkaartRepository repo = new TankkaartRepository();
            TankkaartManager tm = new TankkaartManager(repo);

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

            Tankkaart nieuweTankkaart = new Tankkaart(new DateTime(2025, 12, 25), "6869", null, new List<string>() { "Diesel","Benzine","Bio-Diesel"} );
            repo.voegTankkaartToe(nieuweTankkaart);






        }
    }
}
