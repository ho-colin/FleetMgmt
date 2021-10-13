using FleetManagement.Checkers;
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
    }
}
