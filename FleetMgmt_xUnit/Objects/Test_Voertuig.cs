using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Exceptions;
using FleetMgmt_Business.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FleetMgmt_xUnit.Objects
{
    public class Test_Voertuig
    {
        [Fact]
        public void Test_Ctor_Valid()
        {
            Voertuig v = new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123");
            Assert.Equal(Brandstof.Elektrisch,v.Brandstof);
            Assert.Equal("1HGBH41JXMN109186",v.Chassisnummer);
            Assert.Equal("Blauw",v.Kleur);
            Assert.Equal(5,v.AantalDeuren);
            Assert.Equal("Volkswagen",v.Merk);
            Assert.Equal("Golf",v.Model);
            Assert.Equal("Hatchback", v.Model);
            Assert.Equal("1-AAA-123", v.Model);
        }
        [Fact]
        public void Test_ZetKleur_Valid()
        {
            Voertuig v = new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123");
            Assert.Equal("Rood", v.Kleur);
        }
        [Fact]
        public void Test_ZetKleur_Invalid()
        {
            var ex = Assert.Throws<VoertuigException>(() => new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123"));
            Assert.Equal("Voertuig - kleur mag niet leeg zijn", ex.Message);
        }
        [Fact]
        public void Test_ZetAantalDeuren_Valid()
        {
            Voertuig v = new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123");
            Assert.Equal(3, v.AantalDeuren);
        }
        [Fact]
        public void Test_ZetAantalDeuren_Invalid()
        {
            var ex = Assert.Throws<VoertuigException>(() => new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123"));
            Assert.Equal("Voertuig - aantal deuren mag niet minder dan 1 zijn", ex.Message);
        }
        [Fact]
        public void Test_ZetChassisnummer_Valid()
        {
            Voertuig v = new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123");
            Assert.Equal("2WVU52KCMN2182877", v.Chassisnummer);
        }
        [Theory]
        [InlineData("1IOQH41JXMN109186")]
        [InlineData(null)]
        [InlineData("1HGBH41JXMN10")]
        [InlineData("1HGBH41JXMN109186WV5867")]
        [InlineData("1hgBH41JXMN109186")]
        public void Test_ZetChassisnummer_Invalid(string chassisnummer)
        {
            Voertuig v = new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123");
            Assert.False(false);
        }
        [Fact]
        public void Test_ZetNummerplaat_Valid()
        {
            Voertuig v = new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123");
            Assert.Equal("2 - BBB - 321", v.Nummerplaat);
        }
        [Theory]
        [InlineData("A-ABC-123")]
        [InlineData(null)]
        [InlineData("0-ABC-123")]
        [InlineData("1-123-123")]
        [InlineData("1-ABC-ABC")]
        public void Test_ZetNummerplaat_Invalid(string nummerplaat)
        {
            Voertuig v = new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123");
            Assert.False(false);
        }
        [Fact]
        public void Test_UpdateBestuurder()
        {
            Voertuig v = new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123");
            Bestuurder b = new Bestuurder("90.02.01-999-02", "Colpaert", "Pieter", new DateTime(1990, 02, 27));
            Assert.Equal(b, v.Bestuurder);
        }
        [Fact]
        public void Test_UpdateAantalDeuren_Valid()
        {
            Voertuig v = new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123");
            Assert.Equal(3, v.AantalDeuren);
        }
        [Fact]
        public void Test_UpdateAantalDeuren_Invalid()
        {
            var ex = Assert.Throws<VoertuigException>(() => new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123"));
            Assert.Equal("Voertuig - aantal deuren mag niet minder dan 1 zijn", ex.Message);
        }
        [Fact]
        public void Test_UpdateKleur_Valid()
        {
            Voertuig v = new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123");
            Assert.Equal("Rood", v.Kleur);
        }
        [Fact]
        public void Test_UpdateKleur_Invalid()
        {
            var ex = Assert.Throws<VoertuigException>(() => new Voertuig(Brandstof.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", "Hatchback", "1-AAA-123"));
            Assert.Equal("Voertuig - kleur mag niet leeg zijn", ex.Message);
        }

    }
}
