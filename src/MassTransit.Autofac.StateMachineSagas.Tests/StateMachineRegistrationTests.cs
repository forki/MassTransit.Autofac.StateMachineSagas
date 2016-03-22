using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Automatonymous;
using MassTransit.Autofac.StateMachineSagas.Tests.Elements;
using MassTransit.AutofacIntegration;
using MassTransit.Saga;
using Xunit;

namespace MassTransit.Autofac.StateMachineSagas.Tests
{
    public class StateMachineRegistrationTests
    {
        [Fact]
        public async Task RegisterStateMachines()
        {
            TestDataCollector.Data.Clear();

            var builder = new ContainerBuilder();
            builder.RegisterSagaStateMachines(typeof(TestSagaStateMachine).Assembly);
            builder.RegisterGeneric(typeof (InMemorySagaRepository<>)).As(typeof(ISagaRepository<>));
            var container = builder.Build();

            var busControl = Bus.Factory.CreateUsingInMemory(x =>
            {
                x.ReceiveEndpoint("test", c =>
                {
                    c.LoadStateMachineSagas(container);
                });
            });

            var handle = await busControl.StartAsync();
            await busControl.Publish(new StartSaga() {Data = "Test"});

            Thread.Sleep(500);

            Assert.Equal(1, TestDataCollector.Data.Count);
            await handle.StopAsync();
        }
    }
}