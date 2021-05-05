using System;
using System.IO;
using Microwave.Classes.Boundary;
using NUnit.Framework;

namespace MicrowaveOven.Test.Integration
{
    [TestFixture]
    public class IT03_BU_PowerTube_Output
    {
        //Integrationstest 03:
        //PowerTube = T
        //Output = X

        private Output _output;
        private PowerTube _powerTube;
        private StringWriter _stringWriter;

        [SetUp]
        public void Setup()
        {
            _output = new Output();
            _powerTube = new PowerTube(_output);
            _stringWriter = new StringWriter();
        }

        [Test]
        public void TurnOff_OutputLine_displays_PowerTubeTurnedOff()
        {
            _powerTube.TurnOn(60);
            _powerTube.TurnOff();
            Assert.That(!_stringWriter.ToString().Contains("off"));
        }

        [TestCase(50)]
        [TestCase(100)]
        [TestCase(600)]
        [TestCase(700)]
        public void TurnOn_OutputLine_displays_PowerTubeWorksWithPower(int power)
        {
            _powerTube.TurnOn(power);
            Assert.That(!_stringWriter.ToString().Contains("" + power));
        }
    }
}