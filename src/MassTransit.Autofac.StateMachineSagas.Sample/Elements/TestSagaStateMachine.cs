using System;
using Automatonymous;

namespace MassTransit.Autofac.StateMachineSagas.Sample.Elements
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

            Event(() => SagaStarted, x => x.CorrelateBy(d => d.SomeData, c => c.Message.Data).SelectId(c => c.Message.Id));
            Event(() => SagaMoved, x => x.CorrelateById(context => context.Message.SagaId));

            Initially(
                When(SagaStarted).Then(c =>
                {
                    c.Instance.SomeData = c.Data.Data;
                    Console.WriteLine($"Saga started with data {c.Data.Data}");
                }).TransitionTo(Active));

            During(Active, When(SagaMoved).Then(c =>
            {
                Console.WriteLine($"Saga moved to done with data {c.Data.Message}");
            }).TransitionTo(Done).Finalize());

            SetCompletedWhenFinalized();
        }
    }
}