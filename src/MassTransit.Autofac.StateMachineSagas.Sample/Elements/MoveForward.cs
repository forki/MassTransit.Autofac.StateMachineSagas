using System;

namespace MassTransit.Autofac.StateMachineSagas.Sample.Elements
{
    public class MoveForward
    {
        public Guid SagaId { get; set; }
        public string Message { get; set; }
    }
}