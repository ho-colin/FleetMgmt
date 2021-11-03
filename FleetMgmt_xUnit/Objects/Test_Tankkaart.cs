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
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Test_Kaartnummer_Leeg_InValid(string kaartnummer) {
            DateTime toekomst = new DateTime(2025, 10, 13);
            var ex = Assert.Throws<TankkaartException>(() => new Tankkaart(kaartnummer, toekomst, "6969", null, null));
            Assert.Equal("Kaartnummer mag niet leeg zijn!", ex.Message);
        }

        [Theory]
        [InlineData("ABC")]
        public void Test_Kaartnummer_Letter_InValid(string kaartnummer) {
            DateTime toekomst = new DateTime(2025, 10, 13);
            var ex = Assert.Throws<TankkaartException>(() => new Tankkaart(kaartnummer, toekomst, "6969", null, null));
            Assert.Equal("Kaartnummer mogen alleen getallen zijn!", ex.Message);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-1")]
        public void Test_Kaartnummer_TeLaag_InValid(string kaartnummer) {
            DateTime toekomst = new DateTime(2025, 10, 13);
            var ex = Assert.Throws<TankkaartException>(() => new Tankkaart(kaartnummer, toekomst, "6969", null, null));
            Assert.Equal("Kaartnummer moet hoger zijn dan 1!", ex.Message);
        }

        [Fact]
        public void Test_geldigheidsDatum_Verleden_InValid() {
            DateTime verleden = new DateTime(2020, 10, 15);
            var ex = Assert.Throws<TankkaartException>(() => new Tankkaart("10", verleden, "6969", null, null));
            Assert.Equal("Geldigheidsdatum moet groter zijn dan vandaag!", ex.Message);
        }

        [Theory]
        [InlineData("A536")]
        [InlineData("ABCD")]
        [InlineData("12A4")]
        public void Test_Pincode_Invalid(string pincode) {
            DateTime toekomst = new DateTime(2025, 10, 13);
            var ex = Assert.Throws<TankkaartException>(() => new Tankkaart("10", toekomst, pincode, null, null));
            Assert.Equal("Pincode mag alleen bestaan uit cijfers!", ex.Message);
        }

        [Fact]
        public void Test_Pincode_Valid() {
            DateTime toekomst = new DateTime(2025, 10, 13);
            string pincode = "1243";
            Tankkaart tankkaart = new Tankkaart("10", toekomst, pincode, null, null);
            Assert.Equal(pincode, tankkaart.Pincode);
        }

        [Fact]
        public void Test_Kaartnummer_Valid() {
            DateTime toekomst = new DateTime(2025, 10, 13);
            string kaartnummer = "1";
            Tankkaart tankkaart = new Tankkaart(kaartnummer, toekomst, "6969", null, null);
            Assert.Equal(kaartnummer, tankkaart.KaartNummer);
        }

        [Fact]
        public void Test_Geblokkeerd_Valid() {
            DateTime toekomst = new DateTime(2025, 10, 13);
            string kaartnummer = "1";
            Tankkaart tankkaart = new Tankkaart(kaartnummer, toekomst, "6969", null, null);

            tankkaart.zetGeblokkeerd(true);

            Assert.True(tankkaart.Geblokkeerd);
        }

    }
}
