using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Timer = Microwave.Classes.Boundary.Timer;

namespace MicrowaveOven.Test.Integration
{
    [TestFixture]
    public class IT06_BU_CookController_Timer
    {
        //  Denne test er ikke udført, se følgende afsnit i rapporten: "Resultater".

        //Testdriver
        private CookController _cookC;

        //Egentlige klasser
        private Timer _timer;
        private Output _output;

        //Fakes
        private IDisplay _display;
        private IPowerTube _powerTube;
        private IUserInterface _uI;

        //StringWriter
        private StringWriter _sW;

        [SetUp]
        public void Setup()
        {
            _timer = new Timer();
            _output = new Output();

            _display = Substitute.For<IDisplay>();
            _powerTube = Substitute.For<IPowerTube>();
            _uI = Substitute.For<IUserInterface>();
            _cookC = new CookController(_timer, _display, _powerTube, _uI);
            _sW = new StringWriter();
            Console.SetOut(_sW);
        }

        
    }
}
