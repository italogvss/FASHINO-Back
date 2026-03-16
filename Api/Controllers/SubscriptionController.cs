using Application.DTOs.Subscription;
using Application.Interfaces.Services;
using Domain.Common;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SubscriptionController(ISubscriptionService service) : ControllerBase
    {
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Get My Subscriptions",
            Description = "Retorna todas as assinaturas do usuário autenticado."
        )]
        public async Task<ApiResponse<SubscriptionResponse>> GetMySubscriptions([Required] Guid id)
        {
            var response = await service.GetUserSubscriptionsAsync(id);
            return ApiResponse<SubscriptionResponse>.SuccessResult(response, "Assinaturas obtidas com sucesso");
        }

        [HttpPost("sync")]
        [SwaggerOperation(
            Summary = "Sync Subscription",
            Description = "Sincroniza a assinatura do usuário com base nos dados recebidos do front ou RevenueCat."
        )]
        public async Task<ApiResponse<SubscriptionResponse>> SyncSubscription([FromBody] SubscriptionSyncRequest request)
        {
            var userId = HttpContext.GetUserId() ??
                        throw new UnauthorizedAccessException("User ID not found in token.");
            request.UserId = userId;
            var response = await service.SyncSubscriptionAsync(request);
            return ApiResponse<SubscriptionResponse>.SuccessResult(response, "Assinatura sincronizada com sucesso");
        }

        //[HttpPost("webhook")]
        //[SwaggerOperation(
        //    Summary = "RevenueCat Webhook",
        //    Description = "Endpoint público para receber notificações do RevenueCat sobre alterações de assinatura."
        //)]
        //public async Task<IActionResult> RevenueCatWebhook([FromBody] RevenueCatWebhookRequest request)
        //{
        //    await service.HandleRevenueCatWebhookAsync(request);
        //    return Ok();
        //}
    }
}