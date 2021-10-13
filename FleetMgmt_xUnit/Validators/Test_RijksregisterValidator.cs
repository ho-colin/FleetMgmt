using FleetManagement.Validators;
using FleetMgmt_Business.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FleetMgmt_xUnit.Validators {
    public class Test_RijksregisterValidator {

        [Fact]
        public void Test_Alles_Valid() {
            Assert.True(RijksregisterValidator.isGeldig("90.02.01-999-02", new DateTime(1990, 2, 1)));
        }

        [Fact]
        public void Test_Alles_Invalid() {
            Assert.False(RijksregisterValidator.isGeldig("90.02.15-999-36", new DateTime(1995, 12, 10)));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Test_InputNull_Invalid(string rijksregisternummer) {
            var ex = Assert.Throws<RijksregisterException>(() => RijksregisterValidator.isGeldig(rijksregisternummer, new DateTime(1990, 2, 1)));
            Assert.Equal("Rijksregisternummer mag niet leeg zijn!", ex.Message);
        }

        [Theory]
        [InlineData("90 02.01-999-02")]
        [InlineData("90 02 01.999-02")]
        [InlineData("90.02.01 999-02")]
        public void Test_InputSpatie_Invalid(string rijksregisternummer) {
            var ex = Assert.Throws<RijksregisterException>(() => RijksregisterValidator.isGeldig(rijksregisternummer, new DateTime(1990, 2, 1)));
            Assert.Equal("Rijksregisternummers mogen geen spaties bevatten!", ex.Message);
        }

        [Theory]
        [InlineData("90.02.01-999-025")]
        [InlineData("90.02.01-999-0")]
        public void Test_InputLengte_Invalid(string rijksregisternummer) {
            var ex = Assert.Throws<RijksregisterException>(() => RijksregisterValidator.isGeldig(rijksregisternummer, new DateTime(1990, 2, 1)));
            Assert.Equal("Rijksregister moet 15 karakters lang zijn, Voorbeeld: 90.02.01-999-02", ex.Message);
        }

        [Theory]
        [InlineData("90.02.01-ABC-02")]
        [InlineData("AB.02.01-999-02")]
        public void Test_InputLetter_Invalid(string rijksregisternummer) {
            var ex = Assert.Throws<RijksregisterException>(() => RijksregisterValidator.isGeldig(rijksregisternummer, new DateTime(1990, 2, 1)));
            Assert.Equal("Er worden geen Letters toegestaan in het rijksregisternummer!", ex.Message);
        }


    }
}
