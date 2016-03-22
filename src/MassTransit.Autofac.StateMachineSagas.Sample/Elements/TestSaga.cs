using System;
using Automatonymous;

namespace MassTransit.Autofac.StateMachineSagas.Sample.Elements
{
    public class TestSaga : SagaStateMachineInstance
    {
        public string CurrentState { get; set; }
        public string SomeData { get; set; }
        public Guid CorrelationId { get; set; }
    }
}