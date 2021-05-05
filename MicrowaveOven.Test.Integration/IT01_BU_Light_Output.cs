using System.IO;
using NUnit.Framework;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
namespace MicrowaveOven.Test.Integration
{
    [TestFixture]
    public class IT01_BU_Light_Output
    {
        //Integrationtest 01:
        //Light = T
        //Output = X

        private Light _light;
        private Output _output;
        public StringWriter _stringWriter;

        [SetUp]
        public void Setup()
        {
            _output = new Output();
            _light = new Light(_output); //Constructor tager én parameter
            _stringWriter = new StringWriter();
            System.Console.SetOut(_stringWriter);
        }

        [Test]
        public void TurnOn_Output_LightIsTurnedOn()
        {
            _light.TurnOn();
            Assert.That(_stringWriter.ToString().Contains("turned on"));
        }

        [Test]
        public void TurnOff_Output_LightIsTurnedOff()
        {
            _light.TurnOn(); 
            _light.TurnOff();

            Assert.That(_stringWriter.ToString().Contains("turned off"));
        }
    }
}