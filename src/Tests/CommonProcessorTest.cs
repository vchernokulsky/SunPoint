using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intems.Devices;
using Intems.Devices.ErrorProcessing;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Tests
{
    [TestFixture]
    public class CommonProcessorTest
    {
        private MockRepository _repository;
        private IErrorProcessor _errorProcessor;

        [SetUp]
        public void Initialization()
        {
            _repository = new MockRepository();
            _errorProcessor = _repository.DynamicMock<IErrorProcessor>();
        }

        [Test]
        public void ProcessFailedDevice()
        {
            var cp = new CommonProcessor();
            var result = new PackageProcessResult(){Type =  AnswerType.Error, ErrorType = ErrorType.DeviceFailed};

            using (_repository.Record())
            {
                //_errorProcessor.Raise(x => x.Error += null, cp, new ErrorProcessArgs());
                IEventRaiser raiser = _errorProcessor.GetEventRaiser(x => x.Error += null);
                raiser.Raise(_errorProcessor, new ErrorProcessArgs());
            }
            using (_repository.Playback())
            {
                cp.ProcessError(result);
            }
        }

    }
}
