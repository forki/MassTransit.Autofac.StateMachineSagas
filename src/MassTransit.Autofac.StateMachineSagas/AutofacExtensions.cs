using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Automatonymous;
using MassTransit.Internals.Extensions;
using MassTransit.Saga;

namespace MassTransit.AutofacIntegration
{
    public static class AutofacExtensions
    {
        public static void LoadConsumers(this IReceiveEndpointConfigurator configurator, ILifetimeScope scope,
            string name = "message")
        {
            var concreteTypes = FindTypes<IConsumer>(scope, r => !r.HasInterface<ISaga>());
            foreach (var concreteType in concreteTypes)
                ConsumerConfiguratorCache.Configure(concreteType, configurator, scope, name);
        }

        public static void LoadConsumers(this IReceiveEndpointConfigurator configurator, IComponentContext context,
            string name = "message")
        {
            var scope = context.Resolve<ILifetimeScope>();

            LoadConsumers(configurator, scope, name);
        }

        public static void LoadStateMachineSagas(this IReceiveEndpointConfigurator configurator, ILifetimeScope scope,
           string name = "message")
        {
            var stateMachines = scope.ComponentRegistry.Registrations
                .SelectMany(r => r.Services.OfType<IServiceWithType>(), (r, s) => new {r, s})
                .Where(x => x.s.ServiceType.HasInterface(typeof(SagaStateMachine<>)))
                .Select(x => GetArgumentType(x.s.ServiceType))
                .Where(x => x != null)
                .ToList();
            stateMachines.ForEach(x =>
            {
                StateMachineSagaConfiguratorCache.Configure(x, configurator, scope, name);
            });
        }

        public static void LoadStateMachineSagas(this IReceiveEndpointConfigurator configurator, IComponentContext context,
            string name = "message")
        {
            var scope = context.Resolve<ILifetimeScope>();

            LoadStateMachineSagas(configurator, scope, name);
        }

        private static Type GetArgumentType(Type serviceType)
        {
            return ((TypeInfo)serviceType)
                .ImplementedInterfaces
                    .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(SagaStateMachine<>))
                ?.GenericTypeArguments
                    .First(t => typeof (SagaStateMachineInstance).IsAssignableFrom(t));
        }

        private static IList<Type> FindTypes<T>(IComponentContext scope, Func<Type, bool> filter)
        {
            return scope.ComponentRegistry.Registrations
                .SelectMany(r => r.Services.OfType<IServiceWithType>(), (r, s) => new {r, s})
                .Where(rs => rs.s.ServiceType.HasInterface<T>())
                .Select(rs => rs.s.ServiceType)
                .Where(filter)
                .ToList();
        }
    }
}