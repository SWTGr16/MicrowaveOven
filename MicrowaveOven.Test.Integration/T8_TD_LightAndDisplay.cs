using System;
using System.IO;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal.Execution;


namespace MicrowaveOven.Test.Integration
{
    [TestFixture]
    public class T8_TD_LightAndDisplay
    {
        private UserInterface _userInterface;
        private IDoor _door;
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private ICookController _cookController;
        private Display _display;
        private Light _light;
        private Output _output;
        private StringWriter stringWriter;
      


        [SetUp]
        public void Setup()
        {
            _door = Substitute.For<IDoor>();
            _powerButton = Substitute.For<IButton>();
            _timeButton = Substitute.For<IButton>();
            _startCancelButton = Substitute.For<IButton>();
            _cookController = Substitute.For<ICookController>();
            _output = new Output();
            _display = new Display(_output);
            _light = new Light(_output); 
            _userInterface = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cookController);
            stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
        }

        #region Light

        [Test]
        public void DoorOpens_LigthTurnsOn()
        {
            _door.Opened += Raise.EventWith(_door, EventArgs.Empty); // Main Scenario Step 1
            Assert.That(stringWriter.ToString().Contains("turned on")); // Main Scenario Step 2
        }

        [Test]
        public void DoorCloses_LigthTurnsOff()
        {
            _door.Opened += Raise.EventWith(_door, EventArgs.Empty); // Main Scenario Step 1
            _door.Closed += Raise.EventWith(_door, EventArgs.Empty); // Main Scenario Step 4 - Step 2 tested previously
            Assert.That(stringWriter.ToString().Contains("turned off")); // Main Scenario Step 5

        }

        #endregion

        #region Display

        // Testcase input fastlagt på baggrund af BVA
        [TestCase(0,"")] 
        [TestCase(1, "50")]
        [TestCase(14, "700")]
        [TestCase(15, "50")]
        public void PowerButton_PressedInputTimes_OutputEqualsOutput(int input, string output)
        {
            _door.Opened += Raise.EventWith(_door, EventArgs.Empty);
            _door.Closed += Raise.EventWith(_door, EventArgs.Empty);

            for (int i = 0; i < input; i++)
            {
                _powerButton.Pressed += Raise.EventWith(_powerButton, EventArgs.Empty);
            }

            Assert.That(stringWriter.ToString().Contains($"{output}"));
        }

        [Test]
        public void EX1_PowerButtonPressed_StartCancelPressed_DisplayCleared()
        {
            _door.Opened += Raise.EventWith(_door, EventArgs.Empty);
            _door.Closed += Raise.EventWith(_door, EventArgs.Empty);
            _powerButton.Pressed += Raise.EventWith(_powerButton, EventArgs.Empty);
            _startCancelButton.Pressed += Raise.EventWith(_startCancelButton, EventArgs.Empty);

            Assert.That(stringWriter.ToString().Contains("Display cleared"));
        }

        [Test]
        public void EX2_PowerButtonPressed_DoorOpened_LightOn_DisplayCleared()
        {
            _door.Opened += Raise.EventWith(_door, EventArgs.Empty);
            _door.Closed += Raise.EventWith(_door, EventArgs.Empty);
            _powerButton.Pressed += Raise.EventWith(_powerButton, EventArgs.Empty);
            _door.Opened += Raise.EventWith(_door, EventArgs.Empty);

            Assert.Multiple(() => { 
                Assert.That(stringWriter.ToString().Contains("turned on")); 
                Assert.That(stringWriter.ToString().Contains("Display cleared"));
            });
        }

        [TestCase(0, "")]
        [TestCase(5, "05:00")]
        public void TimeButtonPressedInputTimes_OutputEqualsOutputMinutes(int input, string output)
        {
            _door.Opened += Raise.EventWith(_door, EventArgs.Empty);
            _door.Closed += Raise.EventWith(_door, EventArgs.Empty);
            _powerButton.Pressed += Raise.EventWith(_powerButton, EventArgs.Empty);

            for (int i = 0; i < input; i++)
            {
                _timeButton.Pressed += Raise.EventWith(_timeButton, EventArgs.Empty);

            }

            Assert.That(stringWriter.ToString().Contains($"{output}"));
        }

        [Test]
        public void EX2_TimeButtonPressedInputTimes_DoorOpened_DisplayCleared()
        {
            _door.Opened += Raise.EventWith(_door, EventArgs.Empty);
            _door.Closed += Raise.EventWith(_door, EventArgs.Empty);
            _powerButton.Pressed += Raise.EventWith(_powerButton, EventArgs.Empty);
            _timeButton.Pressed += Raise.EventWith(_timeButton, EventArgs.Empty);
            _door.Opened += Raise.EventWith(_door, EventArgs.Empty);


            Assert.Multiple(() => {
                Assert.That(stringWriter.ToString().Contains("turned on"));
                Assert.That(stringWriter.ToString().Contains("Display cleared"));
            });
        }

        #endregion



    }
}