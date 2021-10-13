using FleetMgmt_Business.Validators;
using FleetMgmt_Business.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FleetMgmt_xUnit.Validators {
    public class Test_NummerplaatValidator {

        [Fact]
        public void Test_Alles_Valid() {
            Assert.True(NummerplaatValidator.isGeldig("1-AAA-123"));
        }

        [Theory]
        [InlineData("A-ABC-123")]
        public void Test_IndexCijferLetter_InValid(string nummerplaat) {
            var ex = Assert.Throws<NummerplaatException>(() => NummerplaatValidator.isGeldig(nummerplaat));
            Assert.Equal("Indexcijfer moet van 1 tot en met 9 zijn!", ex.Message);
        }

        [Theory]
        [InlineData("0-ABC-123")]
        public void Test_IndexCijferNummer_InValid(string nummerplaat) {
            var ex = Assert.Throws<NummerplaatException>(() => NummerplaatValidator.isGeldig(nummerplaat));
            Assert.Equal("Indexcijfer moet groter zijn dan 0!", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("-ABC-123")]
        [InlineData("12-ABC-123")]
        public void Test_Lengte_Invalid(string nummerplaat) {
            var ex = Assert.Throws<NummerplaatException>(() => NummerplaatValidator.isGeldig(nummerplaat));
            Assert.Equal("Nummerplaat moet 9 karakters lang zijn, Voorbeeld: 1-AAA-123", ex.Message);
        }

        [Theory]
        [InlineData("1-123-123")]
        [InlineData("1-AB1-123")]
        [InlineData("1-12A-123")]
        public void Test_Letters_Invalid(string nummerplaat) {
            var ex = Assert.Throws<NummerplaatException>(() => NummerplaatValidator.isGeldig(nummerplaat));
            Assert.Equal("2e set karakters moeten 3 letters zijn!", ex.Message);
        }

        [Theory]
        [InlineData("1-ABC-ABC")]
        [InlineData("1-ABC-AB1")]
        [InlineData("1-ABC-12A")]
        public void Test_Cijfers_Invalid(string nummerplaat) {
            var ex = Assert.Throws<NummerplaatException>(() => NummerplaatValidator.isGeldig(nummerplaat));
            Assert.Equal("3e zet karakters moeten 3 cijfers zijn!", ex.Message);
        }
    }
}
