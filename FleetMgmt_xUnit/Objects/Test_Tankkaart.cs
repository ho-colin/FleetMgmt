using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Exceptions;
using FleetMgmt_Business.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FleetMgmt_xUnit.Objects {
    public class Test_Tankkaart {

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Test_Kaartnummer_TeLaag_InValid(int kaartnummer) {
            DateTime toekomst = new DateTime(2025, 10, 13);
            List<TankkaartBrandstof> dummyBrandstoffen = new List<TankkaartBrandstof>() { TankkaartBrandstof.Benzine };
            var ex = Assert.Throws<TankkaartException>(() => new Tankkaart(kaartnummer, toekomst, "6969", null, dummyBrandstoffen, false));
            Assert.Equal("Kaartnummer moet hoger zijn dan 1!", ex.Message);
        }

        [Fact]
        public void Test_geldigheidsDatum_Verleden_InValid() {
            DateTime verleden = new DateTime(2020, 10, 15);
            List<TankkaartBrandstof> dummyBrandstoffen = new List<TankkaartBrandstof>() { TankkaartBrandstof.Benzine };
            var ex = Assert.Throws<TankkaartException>(() => new Tankkaart(10, verleden, "6969", null, dummyBrandstoffen, false));
            Assert.Equal("Geldigheidsdatum moet groter zijn dan vandaag!", ex.Message);
        }

        [Theory]
        [InlineData("A536")]
        [InlineData("ABCD")]
        [InlineData("12A4")]
        public void Test_Pincode_Invalid(string pincode) {
            DateTime toekomst = new DateTime(2025, 10, 13);
            List<TankkaartBrandstof> dummyBrandstoffen = new List<TankkaartBrandstof>() { TankkaartBrandstof.Benzine };
            var ex = Assert.Throws<TankkaartException>(() => new Tankkaart(10, toekomst, pincode, null, dummyBrandstoffen, false));
            Assert.Equal("Pincode mag alleen bestaan uit cijfers!", ex.Message);
        }

        [Fact]
        public void Test_Pincode_Valid() {
            DateTime toekomst = new DateTime(2025, 10, 13);
            string pincode = "1243";
            List<TankkaartBrandstof> dummyBrandstoffen = new List<TankkaartBrandstof>() { TankkaartBrandstof.Benzine };
            Tankkaart tankkaart = new Tankkaart(10, toekomst, pincode, null, dummyBrandstoffen, false);
            Assert.Equal(pincode, tankkaart.Pincode);
        }

        [Fact]
        public void Test_Kaartnummer_Valid() {
            DateTime toekomst = new DateTime(2025, 10, 13);
            int kaartnummer = 1;
            List<TankkaartBrandstof> dummyBrandstoffen = new List<TankkaartBrandstof>() { TankkaartBrandstof.Benzine };
            Tankkaart tankkaart = new Tankkaart(kaartnummer, toekomst, "6969", null, dummyBrandstoffen, false);
            Assert.Equal(kaartnummer, tankkaart.KaartNummer);
        }

        [Fact]
        public void Test_Geblokkeerd_Valid() {
            DateTime toekomst = new DateTime(2025, 10, 13);
            int kaartnummer = 1;
            List<TankkaartBrandstof> dummyBrandstoffen = new List<TankkaartBrandstof>() { TankkaartBrandstof.Benzine };
            Tankkaart tankkaart = new Tankkaart(kaartnummer, toekomst, "6969", null, dummyBrandstoffen, false);

            tankkaart.zetGeblokkeerd(true);

            Assert.True(tankkaart.Geblokkeerd);
        }

    }
}
