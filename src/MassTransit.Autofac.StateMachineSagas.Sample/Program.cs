using System;
using Autofac;
using Automatonymous;
using MassTransit.Autofac.StateMachineSagas.Sample.Elements;
using MassTransit.AutofacIntegration;
using MassTransit.Saga;

namespace MassTransit.Autofac.StateMachineSagas.Sample
{
    class Program
    {
        static void Main()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TestSagaStateMachine>().AsImplementedInterfaces().AsSelf().SingleInstance();
            builder.RegisterGeneric(typeof (InMemorySagaRepository<>)).As(typeof(ISagaRepository<>));
            var container = builder.Build();

            var busControl = Bus.Factory.CreateUsingInMemory(x =>
            {
                x.ReceiveEndpoint("test", c =>
                {
                    c.LoadStateMachineSagas(container);
                });
            });

            busControl.Start();

            var sagaId = Guid.NewGuid();

            busControl.Publish(new StartSaga() {Data = "Test", Id = sagaId });

            Console.WriteLine("Message published, press Enter to continue");
            Console.ReadLine();

            busControl.Publish(new MoveForward() {Message = "Moving forward", SagaId = sagaId});

            Console.WriteLine("Message published, press Enter to continue");
            Console.ReadLine();

            busControl.Stop();
        }
    }
}
