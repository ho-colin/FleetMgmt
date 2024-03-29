﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FleetMgmt_Business.Exceptions;
using FleetMgmt_Business.Objects;

namespace FleetMgmt_xUnit.Objects
{
    public class Test_Bestuurder
    {
        [Fact]
        public void Test_Alles_Valid() {
            Bestuurder b = new Bestuurder("90.02.01-999-02", "Gheysens", "Louis", new DateTime(1933, 12, 11));
            Assert.Equal("90.02.01-999-02", b.Rijksregisternummer);
            Assert.Equal("Gheysens", b.Achternaam);
            Assert.Equal("Louis", b.Voornaam);
            Assert.Equal(new DateTime(1933, 12, 11), b.GeboorteDatum);
        }

        [Fact]
        public void Test_ZetAchterNaam_Valid() {
            Bestuurder b = new Bestuurder("90.02.01-999-02", "Gheysens", "Louis", new DateTime(1996, 06, 05));
            Assert.Equal("Gheysens", b.Achternaam);
        }

        [Fact]
        public void Test_ZetAchterNaam_InValid() {
            var exc = Assert.Throws<BestuurderException>(() => new Bestuurder("90.02.01-999-02", null, "Louis", new DateTime(1996, 06, 05)));
            Assert.Equal("Bestuurder: Achternaam mag niet leeg zijn!", exc.Message);
        }

        [Fact]
        public void Test_ZetVoornaam_Valid() {
            Bestuurder b = new Bestuurder("90.02.01-999-02", "Gheysens", "Louis", new DateTime(1996, 06, 05));
            Assert.Equal("Louis", b.Voornaam);
        }

        [Fact]
        public void Test_ZetVoornaam_InValid() {
           var exc =  Assert.Throws<BestuurderException>(() => new Bestuurder("90.02.01-999-02", "Gheysens", null, new DateTime(1996, 06, 05)));
           Assert.Equal("Bestuurder: Voornaam mag niet leeg zijn!", exc.Message);
        }

        [Fact]
        public void Test_ZetGeboorteDatum_Valid() {
            Bestuurder b = new Bestuurder("90.02.01-999-02", "Gheysens", "Louis", new DateTime(1996, 06, 05));
            Assert.Equal(new DateTime(1996, 06, 05), b.GeboorteDatum);

        }

        [Fact]
        public void Test_ZetGeboorteDatum_inValid() {
            var exc = Assert.Throws<BestuurderException>(() => new Bestuurder("90.02.01-999-02", "Gheysens", "Louis", DateTime.MinValue));
            Assert.Equal("Bestuurder: Datum heeft geen geldige waarde!", exc.Message);
        }

        [Fact]
        public void Test_VoegRijbewijsToe_Valid() {
            Bestuurder b = new Bestuurder("90.02.01-999-02", "Gheysens", "Louis", new DateTime(1933, 12, 11));
            Rijbewijs r = new Rijbewijs("B", new DateTime(1998, 12, 13));
            b.voegRijbewijsToe(r);
            Assert.Contains(r, b.rijbewijzen);
            Rijbewijs rb = new Rijbewijs("G", new DateTime(1998, 06, 28));
            b.voegRijbewijsToe(rb);
            Assert.Equal(2, b.rijbewijzen.Count);
        }
    }
}
