using System;
using Automatonymous;

namespace MassTransit.Autofac.StateMachineSagas.Tests.Elements
{
    public class TestSagaStateMachine : MassTransitStateMachine<TestSaga>
    {
        public State Active { get; private set; }
        public State Done { get; private set; }
        public Event<StartSaga> SagaStarted { get; private set; }
        public Event<MoveForward> SagaMoved { get; private set; }

        public TestSagaStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => SagaStarted, x => x.CorrelateBy(d => d.SomeData, c => c.Message.Data).SelectId(c => Guid.NewGuid()));
            Event(() => SagaMoved, x => x.CorrelateById(context => context.Message.SagaId));

            Initially(
                When(SagaStarted).Then(c =>
                {
                    c.Instance.SomeData = c.Data.Data;
                    TestDataCollector.Data.Add(c.Instance.SomeData);
                }).TransitionTo(Active));

            During(Active, When(SagaMoved).Then(c => {}).TransitionTo(Done).Finalize());

            SetCompletedWhenFinalized();
        }
    }
}