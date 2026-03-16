using AutoMapper;
using Application.DTOs.Subscription;
using Application.Interfaces.Persistence;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unit;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepository,
            IUserRepository userRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            this.subscriptionRepository = subscriptionRepository;
            this.mapper = mapper;
            this.userRepository = userRepository;
            unit = unitOfWork;
        }

        public async Task<SubscriptionResponse> GetUserSubscriptionsAsync(Guid userId)
        {
            var subscription = await subscriptionRepository.GetByUserId(userId);

            if (subscription == null)
                throw new NotFoundException("Usuario sem assinatura");

            return mapper.Map<SubscriptionResponse>(subscription);

        }

        public async Task<SubscriptionResponse> SyncSubscriptionAsync(SubscriptionSyncRequest request)
        {
            // Busca assinatura existente do usuário
            var user = await userRepository.GetByIdAsync(request.UserId); // stub: UserId do contexto
            if (user == null)
                throw new NotFoundException("Usuário não encontrado.");

            await unit.BeginTransactionAsync();

            Subscription? subscription = user.Subscription;

            try
            {

                if (subscription != null)
                {
                    subscription.Update(request.ProductId, request.ProductId, request.ExpiresAt, SubscriptionStatus.Active);
                }
                else
                {
                    subscription = new Subscription(
                        request.UserId,
                        request.ProductId,
                        request.EntitlementId,
                        request.Credits,
                        DateTime.UtcNow,
                        request.ExpiresAt
                    );
                    user.UpdateSubscription(subscription.Id);
                    await userRepository.UpdateAsync(user);
                    await subscriptionRepository.AddAsync(subscription);

                    await unit.CommitAsync();
                }

            }
            catch (Exception ex)
            {
                await unit.RollbackAsync();
                throw;
            }
            return mapper.Map<SubscriptionResponse>(subscription);
        }

        /// <summary>
        /// Processa notificações enviadas pelo RevenueCat.
        /// </summary>
        public async Task HandleRevenueCatWebhookAsync(RevenueCatWebhookRequest request)
        {
            throw new NotImplementedException();
        }

    }
}