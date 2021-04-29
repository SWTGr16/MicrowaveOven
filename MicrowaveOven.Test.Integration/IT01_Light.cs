using System.IO;
using NUnit.Framework;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
namespace MicrowaveOven.Test.Integration
{
    [TestFixture]
    public class IT01_Light
    {
        private Light _light;
        private Output _output;
        public StringWriter _stringWriter;

        [SetUp]
        public void Setup()
        {
            _output = new Output();
            _light = new Light(_output); //Constructor har én parameter
            _stringWriter = new StringWriter();
        }

        [Test]
        public void isOn_isTrue_TurnOn_Output_LightIsTurnedOn()
        {
            _light.TurnOn();
            Assert.That(_stringWriter.ToString().Equals("Light is turned on"));
        }

        public void isOn_isFalse_TurnOff_Output_LightIsTurnedOff()
        {
            _light.TurnOff();
            Assert.That(_stringWriter.ToString().Equals("Light is turned off"));
        }
    }
}