using System;

namespace MassTransit.Autofac.StateMachineSagas.Sample.Elements
{
    public class StartSaga
    {
        public string Data { get; set; }
        public Guid Id { get; set; }
    }
}