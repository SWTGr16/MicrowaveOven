using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework.Internal.Execution;

namespace MicrowaveOven.Test.Integration
{
    [TestFixture]
    public class T7_TD_CookController
    {
        private UserInterface _userInterface;
        private IDoor _door;
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private ITimer _timer;
        private IPowerTube _powerTube;
        private CookController _cookController;
        private IDisplay _display;
        private ILight _light;
        private IOutput _output;
        private StringWriter _stringWriter;
        

        [SetUp]
        public void Setup()
        {
            _door = Substitute.For<IDoor>();
            _powerButton = Substitute.For<IButton>();
            _timeButton = Substitute.For<IButton>();
            _startCancelButton = Substitute.For<IButton>();
            _timer = Substitute.For<ITimer>();
            _output = Substitute.For<IOutput>();
            _powerTube = Substitute.For<IPowerTube>();
            _display = Substitute.For<IDisplay>();
            _light = Substitute.For<ILight>();
            _cookController = new CookController(_timer, _display, _powerTube); 
            _userInterface = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cookController);
            _cookController.UI = _userInterface;
            _stringWriter = new StringWriter();
            Console.SetOut(_stringWriter);

        }

        [TestCase(1,2)]
        [TestCase(14, 4)]
        [TestCase(15, 4)]
        [TestCase(30, 4)]
        public void TimeAndPowerSet_StartSession(int power, int time)
        {
            _door.Opened += Raise.EventWith(_door, EventArgs.Empty);
            _door.Closed += Raise.EventWith(_door, EventArgs.Empty);

            int numberOfPowerPressed = 0; // Lokal variabel til monitorering af mere end 14 tryk på Power. Overgang fra 700 -> 50 W. 

            for (int i = 0; i < power; i++)
            {
                _powerButton.Pressed += Raise.EventWith(_powerButton, EventArgs.Empty);
                numberOfPowerPressed++;
                if (numberOfPowerPressed > 14)
                {
                    numberOfPowerPressed = 1;
                }
            }

            for (int i = 0; i < time; i++)
            {
                _timeButton.Pressed += Raise.EventWith(_timeButton, EventArgs.Empty);
            }
            
            _startCancelButton.Pressed += Raise.EventWith(_startCancelButton, EventArgs.Empty);

            _timer.Received(1).Start((60*time));
            _powerTube.Received(1).TurnOn((50*numberOfPowerPressed));
        }

        [TestCase(1, 2,60,1)]
        [TestCase(14, 4, 60, 3)]
        [TestCase(15, 4, 60, 3)]
        [TestCase(20, 4, 60, 3)]
        public void TimeAndPowerSet_Cooking_ShowRemainingTime(int power, int time, int ticks, int remainingTime)
        {
            _door.Opened += Raise.EventWith(_door, EventArgs.Empty);
            _door.Closed += Raise.EventWith(_door, EventArgs.Empty);

            int numberOfPowerPressed = 0;

            for (int i = 0; i < power; i++)
            {
                _powerButton.Pressed += Raise.EventWith(_powerButton, EventArgs.Empty);

                numberOfPowerPressed++;

                if (numberOfPowerPressed > 14)
                {
                    numberOfPowerPressed = 1;
                }
            }

            for (int i = 0; i < time; i++)
            {
                _timeButton.Pressed += Raise.EventWith(_timeButton, EventArgs.Empty);
            }

            _startCancelButton.Pressed += Raise.EventWith(_startCancelButton, EventArgs.Empty);

            _timer.Received(1).Start((60 * time));
            _powerTube.Received(1).TurnOn((50 * numberOfPowerPressed));


            for (int i = 0; i < ticks; i++)
            {
                _timer.TimerTick += Raise.EventWith(_timer, EventArgs.Empty);
            }

            _display.Received(1).ShowTime(remainingTime,0);

        }

        [TestCase(1,2)]
        [TestCase(14, 30)]
        [TestCase(15, 2)]
        public void SetPowerAndTime_Cooking_SessionExpires(int power, int time)
        {
            _door.Opened += Raise.EventWith(_door, EventArgs.Empty);
            _door.Closed += Raise.EventWith(_door, EventArgs.Empty);
            for (int i = 0; i < power; i++)
            {
                _powerButton.Pressed += Raise.EventWith(_powerButton, EventArgs.Empty);
            }

            for (int i = 0; i < time; i++)
            {
                _timeButton.Pressed += Raise.EventWith(_timeButton, EventArgs.Empty);
            }

            _startCancelButton.Pressed += Raise.EventWith(_startCancelButton, EventArgs.Empty);

            _timer.Expired += Raise.EventWith(_timer, EventArgs.Empty);

            _powerTube.Received(1).TurnOff();
            _display.Received(1).Clear();
        }

        [TestCase(1,2 )]
        [TestCase(15,2)]
        public void EX3_CookingInSession_StartCancelButtonPressed(int power, int time)
        {
            _door.Opened += Raise.EventWith(_door, EventArgs.Empty);
            _door.Closed += Raise.EventWith(_door, EventArgs.Empty);
            
            int numberOfPowerPressed = 0;

            for (int i = 0; i < power; i++)
            {
                _powerButton.Pressed += Raise.EventWith(_powerButton, EventArgs.Empty);
                numberOfPowerPressed++;
                if (numberOfPowerPressed > 14)
                {
                    numberOfPowerPressed = 1;
                }
            }

            for (int i = 0; i < time; i++)
            {
                _timeButton.Pressed += Raise.EventWith(_timeButton, EventArgs.Empty);
            }

            _startCancelButton.Pressed += Raise.EventWith(_startCancelButton, EventArgs.Empty);

            _timer.Received(1).Start((60 * time));
            _powerTube.Received(1).TurnOn((50 * numberOfPowerPressed));

            _startCancelButton.Pressed += Raise.EventWith(_startCancelButton, EventArgs.Empty); // Afbryder session ved tryk på Cancel
            _timer.Received(1).Stop();
            _powerTube.Received(1).TurnOff();
            _display.Received(1).Clear();
            _light.Received(2).TurnOff(); // Hvorfor Received 2 gange? Første gang fra linje 157???
            
        }

        [TestCase(1,2)]
        [TestCase(14, 2)]
        [TestCase(15, 2)]
        public void EX4_CookingInSession_DoorOpened(int power, int time)
        {
            _door.Opened += Raise.EventWith(_door, EventArgs.Empty);
            _door.Closed += Raise.EventWith(_door, EventArgs.Empty);


            int numberOfPowerPressed = 0;

            for (int i = 0; i < power; i++)
            {
                _powerButton.Pressed += Raise.EventWith(_powerButton, EventArgs.Empty);

                numberOfPowerPressed++;

                if (numberOfPowerPressed > 14)
                {
                    numberOfPowerPressed = 1;
                }
            }

            for (int i = 0; i < time; i++)
            {
                _timeButton.Pressed += Raise.EventWith(_timeButton, EventArgs.Empty);
            }

            _startCancelButton.Pressed += Raise.EventWith(_startCancelButton, EventArgs.Empty);

            _timer.Received(1).Start((60 * time));
            _powerTube.Received(1).TurnOn((50 * numberOfPowerPressed));

            _door.Opened += Raise.Event(); // Døren åbnes
            _powerTube.Received(1).TurnOff(); 
            _display.Received(1).Clear();
        }

        


    }
}
