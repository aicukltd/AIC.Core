namespace AIC.Core.Messaging.Delegates;

using AIC.Core.Messaging.Models.Contracts;

public delegate Task MessageReceived<in TMessage>(TMessage message) where TMessage : IMessage;

public delegate Task MessageReceived<in TMessage, TPayload>(TMessage message) where TMessage : IMessage<TPayload>;