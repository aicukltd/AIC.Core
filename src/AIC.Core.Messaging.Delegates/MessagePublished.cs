namespace AIC.Core.Messaging.Delegates;

using AIC.Core.Messaging.Models.Contracts;

public delegate Task MessagePublished<in TMessage>(TMessage message) where TMessage : IMessage;

public delegate Task MessagePublished<in TMessage, TPayload>(TMessage message) where TMessage : IMessage<TPayload>;