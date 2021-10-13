using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FleetMgmt_Business.Exceptions;
using FleetMgmt_Business.Objects;
using System.Diagnostics;

namespace FleetMgmt_xUnit.Objects
{
    public class Test_Rijbewijs
    {
        [Fact]
        public void Test_Alles_Valid() {
            Rijbewijs rb = new Rijbewijs("B", new DateTime(2019, 03, 13));
            Assert.Equal("B", rb.Categorie);
            Assert.Equal(new DateTime(2019, 03, 13), rb.BehaaldOp);
        }

        [Fact]
        public void Test_ZetCategorie_Valid() {
            Rijbewijs rb = new Rijbewijs("B", new DateTime(2019, 03, 13));
            rb.ZetCategorie("B");
            Assert.Equal("B", rb.Categorie);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Test_ZetCategorie_InValid(string categorie) {
            Rijbewijs rb = new Rijbewijs("B", new DateTime(2019, 03, 13));
            var exc = Assert.Throws<RijbewijsException>(() => rb.ZetCategorie(categorie));
            Assert.Equal("Rijbewijs: Categorienaam mag niet leeg zijn!", exc.Message);
        }

        [Fact]
        public void Test_ZetBehaaldOp_Valid() {
            Rijbewijs rb = new Rijbewijs("B", new DateTime(1996, 06, 05));
            rb.ZetBehaaldOp(new DateTime(1996, 06, 05));
            Assert.Equal(new DateTime(1996, 06, 05), rb.BehaaldOp);
        }

        [Fact]
        public void Test_ZetBehaaldOp_InValid() {
            var date = DateTime.MinValue; 
            var exc = Assert.Throws<RijbewijsException>(() => new Rijbewijs("B", date));
            Assert.Equal("Rijbewijs: Datum heeft geen geldige waarde!", exc.Message);
        }

        [Fact]
        public void Test_ZetBehaaldOpIsGeenToekomst() {
            var exc = Assert.Throws<RijbewijsException>(() => new Rijbewijs("B", new DateTime(2030, 10, 12)));
            Assert.Equal("Rijbewijs: Datum mag niet in de toekomst zijn!", exc.Message);
        }
    }
}
