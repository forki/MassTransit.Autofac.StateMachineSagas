# OBSOLETE
This project is obsolete since MassTransit is providing similar functionality by the `MassTransit.Automatonymous.Autofac` package.

https://www.nuget.org/packages/MassTransit.Automatonymous.Autofac/

# MassTransit.Autofac.StateMachineSagas
Autofac registration and configuration adapter for Automatonymous state machine Sagas

## Installation

The library is published on nuget.org.

Use `Install-Package MassTransit.Autofac.StateMachineSagas` to install it.

## Usage

When building your container, place this call to the registration sequence:
``` c#
builder.RegisterSagaStateMachines(sagasAssembly); 	// register all state machines
builder.RegisterConsumers(consumersAssembly);		// register consumers (standard MassTransit Autofac integration)
```
where `assembly` is the Assembly where your state machines are located. You can specify as many assemblies to scan as you want.

When configuring your endpoint, add this call:
``` c#
x.ReceiveEndpoint(queueName, c =>
{
    c.LoadConsumers(container);
    c.LoadStateMachineSagas(container);
});
```

As you can see, there is a call to `LoadConsumers(ILifetimeScope scope)` in the code above. It replaces the standard MassTransit Autofac integration call to `LoadFrom(ILifetimeScope scope)` since `LoadFrom` configures both consumers and sagas, and sagas are configured as standard sagas, not as state machine sagas. To avoid duplicate saga registration, use separate calls to register consumers and state machine sagas, as shown above.

Full sample with in-memory bus and saga repository is in the solution.