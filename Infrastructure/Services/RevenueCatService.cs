namespace Infrastructure.Services
{
    //public class RevenueCatService : ISubscriptionProviderWebhookService
    //{
    //    private readonly IUserRepository userRepository;
    //    private readonly ISubscriptionRepository subscriptionRepository;
    //    private readonly ISubscriptionEventRepository subscriptionEventRepository;
    //    private readonly IMapper mapper;
    //    private readonly ILogger<RevenueCatService> logger;

    //    public RevenueCatService(
    //        IUserRepository userRepository,
    //        ISubscriptionRepository subscriptionRepository,
    //        ISubscriptionEventRepository subscriptionEventRepository,
    //        IMapper mapper,
    //        ILogger<RevenueCatService> logger)
    //    {
    //        this.userRepository = userRepository;
    //        this.subscriptionRepository = subscriptionRepository;
    //        this.subscriptionEventRepository = subscriptionEventRepository;
    //        this.mapper = mapper;
    //        this.logger = logger;
    //    }

    //    public async Task ProcessWebhookAsync(RevenueCatWebhookDto webhookData)
    //    {
    //        try
    //        {
    //            if (webhookData?.Event == null)
    //            {
    //                logger.LogError("Invalid webhook payload received");
    //                return;
    //            }

    //            var eventData = webhookData.Event;

    //            logger.LogInformation(
    //                "Processing RevenueCat event {EventType} with ID {EventId} for user {UserId}",
    //                eventData.Type, eventData.Id, eventData.AppUserId
    //            );

    //            1.Verificar se o evento já foi processado(idempotência)
    //            var existingEvent = await subscriptionEventRepository
    //                .GetByRevenueCatEventIdAsync(eventData.Id);

    //            if (existingEvent != null)
    //            {
    //                logger.LogInformation("Event {EventId} already processed", eventData.Id);
    //                return;
    //            }
    //            2.Buscar ou criar usuário
    //           var user = await userRepository.GetByIdAsync(Guid.Parse(eventData.AppUserId));
    //            if (user == null)
    //            {
    //                return;
    //            }

    //            3.Processar evento baseado no tipo
    //            await ProcessEventByTypeAsync(user, eventData);

    //        }
    //        catch (Exception ex)
    //        {
    //            logger.LogError(ex, "Error processing webhook event: {Error}", ex.Message);
    //            throw;
    //        }
    //    }
    //    private async Task ProcessEventByTypeAsync(User user, RevenueCatEventDto eventData)
    //    {
    //        var eventType = Enum.Parse<EventType>(eventData.Type, true);

    //        switch (eventType)
    //        {
    //            case EventType.INITIAL_PURCHASE:
    //                await HandleInitialPurchaseAsync(user, eventData);
    //                break;

    //            case EventType.RENEWAL:
    //                await HandleRenewalAsync(user, eventData);
    //                break;

    //            case EventType.CANCELLATION:
    //                await HandleCancellationAsync(user, eventData);
    //                break;

    //            case EventType.UNCANCELLATION:
    //                await HandleUncancellationAsync(user, eventData);
    //                break;

    //            case EventType.EXPIRATION:
    //                await HandleExpirationAsync(user, eventData);
    //                break;

    //            case EventType.BILLING_ISSUE:
    //                await HandleBillingIssueAsync(user, eventData);
    //                break;

    //            case EventType.PRODUCT_CHANGE:
    //                await HandleProductChangeAsync(user, eventData);
    //                break;

    //            case EventType.SUBSCRIPTION_PAUSED:
    //                await HandleSubscriptionPausedAsync(user, eventData);
    //                break;

    //            default:
    //                logger.LogWarning("Unhandled event type: {EventType}", eventData.Type);
    //                break;
    //        }
    //    }

    //    private async Task HandleInitialPurchaseAsync(User user, RevenueCatEventDto eventData)
    //    {
    //        Usar AutoMapper para criar nova assinatura
    //        var subscription = mapper.Map<Subscription>(eventData);
    //        subscription = Subscription.CreateNew(user.Id);

    //        Mapear os dados do evento para a assinatura criada
    //        mapper.Map(eventData, subscription);

    //        await subscriptionRepository.AddAsync(subscription);

    //        Criar evento usando AutoMapper
    //        var subscriptionEvent = mapper.Map<SubscriptionEvent>(eventData);
    //        subscriptionEvent = new SubscriptionEvent(subscription.Id);
    //        mapper.Map(eventData, subscriptionEvent);

    //        await subscriptionEventRepository.AddAsync(subscriptionEvent);

    //        Atualizar usuário com a assinatura
    //        user.UpdateSubscription(subscription.Id);
    //        await userRepository.UpdateAsync(user);
    //    }

    //    private async Task HandleRenewalAsync(User user, RevenueCatEventDto eventData)
    //    {
    //        var subscription = await subscriptionRepository
    //            .GetByOriginalTransactionIdAsync(eventData.OriginalTransactionId);

    //        if (subscription != null)
    //        {
    //            Usar AutoMapper para atualização
    //            var updatedSubscription = mapper.Map<Subscription>(eventData, opts =>
    //            {
    //                opts.Items["ExistingSubscription"] = subscription;
    //            });

    //            subscription.UpdateFromMapper(updatedSubscription);
    //            await subscriptionRepository.UpdateAsync(subscription);

    //            Criar evento
    //            var subscriptionEvent = mapper.Map<SubscriptionEvent>(eventData);
    //            subscriptionEvent = new SubscriptionEvent(subscription.Id);
    //            mapper.Map(eventData, subscriptionEvent);

    //            await subscriptionEventRepository.AddAsync(subscriptionEvent);
    //        }
    //        else
    //        {
    //            Tratamento para resubscription após expiração
    //           await HandleInitialPurchaseAsync(user, eventData);
    //        }
    //    }

    //    private async Task HandleCancellationAsync(User user, RevenueCatEventDto eventData)
    //    {
    //        var subscription = await subscriptionRepository
    //            .GetByOriginalTransactionIdAsync(eventData.OriginalTransactionId);

    //        if (subscription != null)
    //        {
    //            Usar AutoMapper para atualização
    //            var updatedSubscription = mapper.Map<Subscription>(eventData, opts =>
    //            {
    //                opts.Items["ExistingSubscription"] = subscription;
    //            });

    //            subscription.UpdateFromMapper(updatedSubscription);
    //            await subscriptionRepository.UpdateAsync(subscription);

    //            Criar evento
    //            var subscriptionEvent = mapper.Map<SubscriptionEvent>(eventData);
    //            subscriptionEvent = new SubscriptionEvent(subscription.Id);
    //            mapper.Map(eventData, subscriptionEvent);

    //            await subscriptionEventRepository.AddAsync(subscriptionEvent);
    //        }
    //    }

    //    private async Task HandleUncancellationAsync(User user, RevenueCatEventDto eventData)
    //    {
    //        var subscription = await subscriptionRepository
    //            .GetByOriginalTransactionIdAsync(eventData.OriginalTransactionId);

    //        if (subscription != null)
    //        {
    //            Usar AutoMapper para atualização
    //            var updatedSubscription = mapper.Map<Subscription>(eventData, opts =>
    //            {
    //                opts.Items["ExistingSubscription"] = subscription;
    //            });

    //            subscription.UpdateFromMapper(updatedSubscription);
    //            await subscriptionRepository.UpdateAsync(subscription);

    //            Criar evento
    //            var subscriptionEvent = mapper.Map<SubscriptionEvent>(eventData);
    //            subscriptionEvent = new SubscriptionEvent(subscription.Id);
    //            mapper.Map(eventData, subscriptionEvent);

    //            await subscriptionEventRepository.AddAsync(subscriptionEvent);
    //        }
    //    }

    //    private async Task HandleExpirationAsync(User user, RevenueCatEventDto eventData)
    //    {
    //        var subscription = await subscriptionRepository
    //            .GetByOriginalTransactionIdAsync(eventData.OriginalTransactionId);

    //        if (subscription != null)
    //        {
    //            Usar AutoMapper para atualização
    //            var updatedSubscription = mapper.Map<Subscription>(eventData, opts =>
    //            {
    //                opts.Items["ExistingSubscription"] = subscription;
    //            });

    //            subscription.UpdateFromMapper(updatedSubscription);
    //            await subscriptionRepository.UpdateAsync(subscription);

    //            Criar evento
    //            var subscriptionEvent = mapper.Map<SubscriptionEvent>(eventData);
    //            subscriptionEvent = new SubscriptionEvent(subscription.Id);
    //            mapper.Map(eventData, subscriptionEvent);

    //            await subscriptionEventRepository.AddAsync(subscriptionEvent);

    //            Remover assinatura do usuário

    //           user.RemoveSubscription();
    //            await userRepository.UpdateAsync(user);
    //        }
    //    }

    //    private async Task HandleBillingIssueAsync(User user, RevenueCatEventDto eventData)
    //    {
    //        var subscription = await subscriptionRepository
    //            .GetByOriginalTransactionIdAsync(eventData.OriginalTransactionId);

    //        if (subscription != null)
    //        {
    //            Usar AutoMapper para atualização
    //            var updatedSubscription = mapper.Map<Subscription>(eventData, opts =>
    //            {
    //                opts.Items["ExistingSubscription"] = subscription;
    //            });

    //            subscription.UpdateFromMapper(updatedSubscription);
    //            await subscriptionRepository.UpdateAsync(subscription);

    //            Criar evento
    //            var subscriptionEvent = mapper.Map<SubscriptionEvent>(eventData);
    //            subscriptionEvent = new SubscriptionEvent(subscription.Id);
    //            mapper.Map(eventData, subscriptionEvent);

    //            await subscriptionEventRepository.AddAsync(subscriptionEvent);
    //        }
    //    }

    //    private async Task HandleProductChangeAsync(User user, RevenueCatEventDto eventData)
    //    {
    //        var subscription = await subscriptionRepository
    //            .GetByOriginalTransactionIdAsync(eventData.OriginalTransactionId);

    //        if (subscription != null)
    //        {
    //            Para mudança de produto, você pode querer criar uma nova subscription
    //             ou atualizar a existente dependendo da sua lógica de negócio
    //            var updatedSubscription = mapper.Map<Subscription>(eventData, opts =>
    //            {
    //                opts.Items["ExistingSubscription"] = subscription;
    //            });

    //            subscription.UpdateFromMapper(updatedSubscription);
    //            await subscriptionRepository.UpdateAsync(subscription);

    //            Criar evento
    //            var subscriptionEvent = mapper.Map<SubscriptionEvent>(eventData);
    //            subscriptionEvent = new SubscriptionEvent(subscription.Id);
    //            mapper.Map(eventData, subscriptionEvent);

    //            await subscriptionEventRepository.AddAsync(subscriptionEvent);
    //        }
    //    }

    //    private async Task HandleSubscriptionPausedAsync(User user, RevenueCatEventDto eventData)
    //    {
    //        var subscription = await subscriptionRepository
    //            .GetByOriginalTransactionIdAsync(eventData.OriginalTransactionId);

    //        if (subscription != null)
    //        {
    //            var updatedSubscription = mapper.Map<Subscription>(eventData, opts =>
    //            {
    //                opts.Items["ExistingSubscription"] = subscription;
    //            });

    //            subscription.UpdateFromMapper(updatedSubscription);
    //            await subscriptionRepository.UpdateAsync(subscription);

    //            Criar evento
    //            var subscriptionEvent = mapper.Map<SubscriptionEvent>(eventData);
    //            subscriptionEvent = new SubscriptionEvent(subscription.Id);
    //            mapper.Map(eventData, subscriptionEvent);

    //            await subscriptionEventRepository.AddAsync(subscriptionEvent);
    //        }
    //    }
    //}
}
