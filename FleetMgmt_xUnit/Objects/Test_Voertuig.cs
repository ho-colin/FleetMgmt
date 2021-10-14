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
    public class Test_Voertuig {
        [Fact]
        public void Test_Ctor_Valid() {
            Voertuig v = new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123");
            Assert.Equal(Brandstof.Elektrisch, v.Brandstof);
            Assert.Equal("1HGBH41JXMN109186", v.Chassisnummer);
            Assert.Equal("Blauw", v.Kleur);
            Assert.Equal(5, v.AantalDeuren);
            Assert.Equal("Volkswagen", v.Merk);
            Assert.Equal("Golf", v.Model);
            Assert.Equal("Hatchback", v.Model);
            Assert.Equal("1-AAA-123", v.Model);
        }
        [Theory]
        [InlineData("1IOQH41JXMN109186", "Volkswagen", "Golf", "Hatchback", "1-AAA-123")]
        [InlineData("1HGBH41JXMN109186", null, "Golf", "Hatchback", "1-AAA-123")]
        [InlineData("1HGBH41JXMN109186", "Volkswagen", null, "Hatchback", "1-AAA-123")]
        [InlineData("1HGBH41JXMN109186", "Volkswagen", "Golf", null, "1-AAA-123")]
        [InlineData("1HGBH41JXMN109186", "Volkswagen", "Golf", "Hatchback", "B-AAA-123")]
        public void Test_Ctor_Invalid(string chassisnummer, string merk, string model, string typevoertuig, string nummerplaat) {
            var ex = Assert.Throws<VoertuigException>(() => new Voertuig(Brandstof.Elektrisch, chassisnummer,
                "Blauw", 5, merk, model, typevoertuig, nummerplaat));
        }

        [Fact]
        public void Test_UpdateBestuurder() {
            Voertuig v = new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123");
            Bestuurder b = new Bestuurder("90.02.01-999-02", "Colpaert", "Pieter", new DateTime(1990, 02, 27));
            v.updateBestuurder(b);
            Assert.Equal(b, v.Bestuurder);
        }
        [Fact]
        public void Test_UpdateAantalDeuren_Valid() {
            Voertuig v = new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123");
            v.updateAantalDeuren(3);
            Assert.Equal(3, v.AantalDeuren);
        }
        [Fact]
        public void Test_UpdateKleur_Valid() {
            Voertuig v = new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123");
            v.updateKleur("Rood");
            Assert.Equal("Rood", v.Kleur);
        }

    }
}
