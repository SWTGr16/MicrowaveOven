using System;
using System.IO;
using Microwave.Classes.Boundary;
using NUnit.Framework;

namespace MicrowaveOven.Test.Integration
{
    [TestFixture]
    public class IT02_BU_Display_Output
    {
        //Integrationstest 02:
        //Display = T
        //Output = X

        private Display _display;
        private Output _output;
        private StringWriter _stringWriter;

        [SetUp]
        public void Setup()
        {
            _output = new Output();
            _display = new Display(_output); //Constructer tager én parameter
            _stringWriter = new StringWriter();
            System.Console.SetOut(_stringWriter);
        }

        [TestCase(10)]
        [TestCase(20)]
        [TestCase(30)]
        [TestCase(40)]

        public void ShowPower_OutputLine_displaysCorrectPower(int power)
        {
            Console.SetOut(_stringWriter);
            _display.ShowPower(power);

            Assert.That(_stringWriter.ToString().Contains("" + power));
        }

        public void Clear_OutputLine_displayShowsClear()
        {
            _display.Clear();
            Assert.That(_stringWriter.ToString().Contains("Display cleared"));
        }

        //Display correct time
        [TestCase(2,30)]
        public void ShowTime_OutputLine_displaysCorrectTime(int min, int sec)
        {
            _display.ShowTime(min, sec);
            Assert.That(_stringWriter.ToString().Contains("" + min + ":" + sec));
        }
    }
}