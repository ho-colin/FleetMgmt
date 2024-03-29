﻿using FleetMgmt_Business.Enums;
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
            Voertuig v = new Voertuig(BrandstofEnum.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", new TypeVoertuig("Hatchback", RijbewijsEnum.B), "1-AAA-123");
            Assert.Equal(BrandstofEnum.Elektrisch, v.Brandstof);
            Assert.Equal("1HGBH41JXMN109186", v.Chassisnummer);
            Assert.Equal("Blauw", v.Kleur);
            Assert.Equal(5, v.AantalDeuren);
            Assert.Equal("Volkswagen", v.Merk);
            Assert.Equal("Golf", v.Model);
            Assert.Equal("Hatchback", v.TypeVoertuig.Type);
            Assert.Equal("1-AAA-123", v.Nummerplaat);
        }

        [Fact]
        public void Test_AantalDeuren_Empty() {
            Voertuig v = new Voertuig(BrandstofEnum.Diesel, "1HGBH41JXMN109186", "Zwart", null, "BMW", "1Serie", new TypeVoertuig("Hatchback", RijbewijsEnum.B), "1-AAA-123");
            Assert.Null(v.AantalDeuren);
        }

        [Fact]
        public void Test_AantalDeuren_0() {
            var ex = Assert.Throws<VoertuigException>(() =>new Voertuig(BrandstofEnum.Diesel, "1HGBH41JXMN109186", "Zwart", 0, "BMW", "1Serie", new TypeVoertuig("Hatchback", RijbewijsEnum.B), "1-AAA-123"));
            Assert.Equal("Voertuig: Een voertuig moet minstens 1 deur hebben.", ex.Message);
        }

        [Fact]
        public void Test_UpdateBestuurder() {
            Voertuig v = new Voertuig(BrandstofEnum.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", new TypeVoertuig("Hatchback", RijbewijsEnum.B), "1-AAA-123");
            Bestuurder b = new Bestuurder("90.02.01-999-02", "Colpaert", "Pieter", new DateTime(1990, 02, 27), new List<Rijbewijs>() { new Rijbewijs("B",DateTime.Now) });
            Bestuurder b1 = new Bestuurder("90.02.01-999-02", "Gheysens", "Louis", new DateTime(1933, 12, 11), new List<Rijbewijs>() { new Rijbewijs("B", DateTime.Now) });
            v.updateBestuurder(b);
            Assert.Equal(b, v.Bestuurder);
            v.updateBestuurder(b1);
            Assert.Equal(b1, v.Bestuurder);
        }

        [Fact]
        public void Test_UpdateAantalDeuren_Valid() {
            Voertuig v = new Voertuig(BrandstofEnum.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", new TypeVoertuig("Hatchback", RijbewijsEnum.B), "1-AAA-123");
            v.updateAantalDeuren(3);
            Assert.Equal(3, v.AantalDeuren);
            v.updateAantalDeuren(5);
            Assert.Equal(5, v.AantalDeuren);
        }
        [Fact]
        public void Test_UpdateKleur_Valid() {
            Voertuig v = new Voertuig(BrandstofEnum.Elektrisch, "1HGBH41JXMN109186", "Blauw", 5, "Volkswagen", "Golf", new TypeVoertuig("Hatchback", RijbewijsEnum.B), "1-AAA-123");
            v.updateKleur("Rood");
            Assert.Equal("Rood", v.Kleur);
            v.updateKleur("Zwart");
            Assert.Equal("Zwart", v.Kleur);
        }


    }
}
