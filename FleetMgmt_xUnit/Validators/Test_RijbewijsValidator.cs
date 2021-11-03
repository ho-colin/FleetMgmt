using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FleetMgmt_xUnit.Validators {
    public class Test_RijbewijsValidator {

        Bestuurder b = new Bestuurder("90.02.01-999-02", "Doe", "John", new DateTime(1990, 2, 1), new List<Rijbewijs>() { new Rijbewijs(RijbewijsEnum.B.ToString(), new DateTime(2020, 2, 15)) });
        Voertuig v = new Voertuig(BrandstofEnum.Benzine, "4Y1SL65848Z411439", "Zwart", 5, "Mercedes", "GLC", new TypeVoertuig("Coupe",RijbewijsEnum.B), "1-ASP-123");

        [Fact]
        public void Test_Alles_Valid() {

        }
    }
}
