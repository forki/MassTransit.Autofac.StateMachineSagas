using System.Linq;
using System.Reflection;
using Autofac;
using Automatonymous;

namespace MassTransit.AutofacIntegration
{
    public static class RegistrationExtensions
    {
        public static void RegisterSagaStateMachines(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.GetTypeInfo()
                    .ImplementedInterfaces.Any(
                        i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (SagaStateMachine<>)))
                .AsImplementedInterfaces()
                .AsSelf();
        }
    }
}