using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Exceptions;

namespace FleetMgmt_xUnit.Objects
{
    public class Test_Bestuurder
    {
        [Fact]
        public void Test_Alles_Valid() {
            Bestuurder b = new Bestuurder("90.02.01-999-02", "Gheysens", "Louis", new DateTime(1933, 12, 11));
            Assert.Equal("90.02.01-999-02", b.Rijksregisternummer);
            Assert.Equal("Gheysens", b.Naam);
            Assert.Equal("Louis", b.Voornaam);
            Assert.Equal(new DateTime(1933, 12, 11), b.GeboorteDatum);
        }

        [Fact]
        public void Test_ZetNaam_Valid() {
            Bestuurder b = new Bestuurder("90.02.01-999-02", "Gheysens", "Louis", new DateTime(1996, 06, 05));
            b.ZetNaam("Gheysens");
            Assert.Equal("Gheysens", b.Naam);
        }

        [Fact]
        public void Test_ZetNaam_InValid() {
            var exc = Assert.Throws<BestuurderException>(() => new Bestuurder("90.02.01-999-02", null, "Louis", new DateTime(1996, 06, 05)));
            Assert.Equal("Bestuurder: naam mag niet leeg zijn!", exc.Message);
        }

        [Fact]
        public void Test_ZetVoornaam_Valid() {
            Bestuurder b = new Bestuurder("90.02.01-999-02", "Gheysens", "Louis", new DateTime(1996, 06, 05));
            b.ZetVoornaam("Louis");
            Assert.Equal("Louis", b.Voornaam);
        }

        [Fact]
        public void Test_ZetVoornaam_InValid() {
           var exc =  Assert.Throws<BestuurderException>(() => new Bestuurder("90.02.01-999-02", "Gheysens", null, new DateTime(1996, 06, 05)));
           Assert.Equal("Bestuurder: voornaam mag niet leeg zijn!", exc.Message);
        }

        [Fact]
        public void Test_ZetGeboorteDatum_Valid() {
            Bestuurder b = new Bestuurder("90.02.01-999-02", "Gheysens", "Louis", new DateTime(1996, 06, 05));
            b.ZetGeboorteDatum(new DateTime(1996, 06, 05));
            Assert.Equal(new DateTime(1996, 06, 05), b.GeboorteDatum);

        }

        [Fact]
        public void Test_ZetGeboorteDatum_inValid() {
            var exc = Assert.Throws<BestuurderException>(() => new Bestuurder("90.02.01-999-02", "Gheysens", "Louis", DateTime.MinValue));
            Assert.Equal("Bestuurder: Datum heeft geen geldige waarde!", exc.Message);
        }

        [Fact]
        public void Test_GeboortedatumIsGeenToekomst() {
            DateTime dt = new DateTime(2050, 12, 12);
            var exc = Assert.Throws<BestuurderException>(() => new Bestuurder("90.02.01-999-02", "Gheysens", "Louis", dt));
            Assert.Equal("Bestuurder: Datum mag niet in de toekomst zijn!", exc.Message);
        }
    }
}
