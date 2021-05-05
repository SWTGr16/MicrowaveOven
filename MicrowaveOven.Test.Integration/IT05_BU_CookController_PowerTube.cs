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
        [TestCase(50, 60)]
        public void Start_Cooking5060__PowerTube_On__ThrowsNothing(int power, int time)
        {
            Assert.That(() => _cookC.StartCooking(power, time), Throws.Nothing);
        }


        [TestCase(50, 60)]
        public void Start_Cooking5060__PowerTube_On__PowerTube_IsOn(int power, int time)
        {
            _cookC.StartCooking(power, time);
            Assert.That(_sW.ToString().Contains("PowerTube works with"));
        }

        [TestCase(45, 60)]
        [TestCase(701, 60)]
        public void Start_CookingPower60__PowerTube_On__ThrowsFromArgumentOutOfRangeException(int power, int time)
        {
            Assert.That(() => _cookC.StartCooking(power, time), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [TestCase(50, 60)]
        public void Start_Cooking5060__PowerTube_On__PowerTube_AlreadyOn_ThrowsFromApplicationException(int power, int time)
        {
            _cookC.StartCooking(power, time);
            Assert.That(() => _cookC.StartCooking(power, time), Throws.TypeOf<ApplicationException>());
        }
        #endregion

        #region PowerTube off
        [TestCase(50, 60)]
        public void Timer_Expired__PowerTube_Off(int power, int time)
        {
            _cookC.StartCooking(power, time);
            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(_sW.ToString().Contains("off"));
        }

        [TestCase(50, 60)]
        public void Stop_Cooking5060__PowerTube_Off(int power, int time)
        {
            _cookC.StartCooking(power, time);
            _cookC.Stop();

            Assert.That(_sW.ToString().Contains("off"));
        }
        #endregion

    }
}
