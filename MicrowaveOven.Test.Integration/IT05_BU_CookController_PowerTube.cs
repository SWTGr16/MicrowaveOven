using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NUnit.Framework;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MicrowaveOven.Test.Integration
{
    [TestFixture]
    public class IT05_BU_CookController_PowerTube
    {
        //Testdriver
        private CookController _cookC;

        //Egentlige klasser
        private PowerTube _powerTube;
        private Output _output;

        //Fakes
        private IDisplay _display;
        private ITimer _timer;
        private IUserInterface _uI;

        //StringWriter
        private StringWriter _sW;

        [SetUp]
        public void Setup()
        {
            _output = new Output();
            _powerTube = new PowerTube(_output);

            _display = Substitute.For<IDisplay>();
            _timer = Substitute.For<ITimer>();
            _uI = Substitute.For<IUserInterface>();
            _cookC = new CookController(_timer, _display, _powerTube, _uI);
            _sW = new StringWriter();
            Console.SetOut(_sW);
        }

        #region PowerTube On
        [Test]
        public void Start_Cooking5060__PowerTube_On__ThrowsNothing()
        {
            Assert.That(() => _cookC.StartCooking(50, 60), Throws.Nothing);
        }


        [Test]
        public void Start_Cooking5060__PowerTube_On__PowerTube_IsOn()
        {
            _cookC.StartCooking(50, 60);
            Assert.That(_sW.ToString().Contains("PowerTube works with"));
        }

        [TestCase(49)]
        [TestCase(701)]
        public void Start_CookingPower60__PowerTube_On__ThrowsFromArgumentOutOfRangeException(int power)
        {
            Assert.That(() => _cookC.StartCooking(power, 60), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Start_Cooking5060__PowerTube_On__PowerTube_AlreadyOn_ThrowsFromApplicationException()
        {
            _cookC.StartCooking(50, 60);
            Assert.That(() => _cookC.StartCooking(50, 60), Throws.TypeOf<ApplicationException>());
        }
        #endregion

        #region PowerTube off
        [Test]
        public void Timer_Expired__PowerTube_Off()
        {
            _cookC.StartCooking(50, 60);
            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(_sW.ToString().Contains("off"));
        }

        [Test]
        public void Stop_Cooking5060__PowerTube_Off()
        {
            _cookC.StartCooking(50, 60);
            _cookC.Stop();

            Assert.That(_sW.ToString().Contains("off"));
        }
        #endregion

    }
}
