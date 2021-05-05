using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NUnit.Framework;
using NSubstitute;
using System;
using System.IO;

namespace MicrowaveOven.Test.Integration
{
    [TestFixture]
    public class IT04_BU_CookController_Display
    {
        //Testdriver
        private CookController _cookC;

        //Egentlige klasser
        private Display _display;
        private Output _output;

        //Fakes
        private IPowerTube _powerTube;
        private ITimer _timer;

        //StringWriter
        private StringWriter _sW;

        [SetUp]
        public void Setup()
        {
            _output = new Output();
            _display = new Display(_output);

            _powerTube = Substitute.For<IPowerTube>();
            _timer = Substitute.For<ITimer>();
            _cookC = new CookController(_timer, _display, _powerTube);
            _sW = new StringWriter();
            Console.SetOut(_sW);
        }

        [TestCase("00", "30")]
        [TestCase("01", "01")]
        [TestCase("01", "33")]
        [TestCase("02", "00")]
        [TestCase("02", "27")]
        public void TimeRemaining_On_Display_In_MinutesAndSeconds(string min, string sec)
        {
            _cookC.StartCooking(50, 60);
            _timer.TimeRemaining.Returns(Convert.ToInt32(min) * 60 + Convert.ToInt32(sec));
            _timer.TimerTick += Raise.EventWith(this, EventArgs.Empty);

            Assert.That(_sW.ToString().Contains(min));
            Assert.That(_sW.ToString().Contains(sec));
        }
    }
}
