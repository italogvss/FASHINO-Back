using Application.Interface.Services;
using Sahared.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhooksController(IWebhookService service) : ControllerBase
    {

        [HttpPost("fashn-ai")]
        [SwaggerOperation(
            Summary = "Get job by ID",
            Description = "Returns a single job with details, status, and result URLs.")]
        public async Task<ActionResult> FashnAI(FashnRunResponse response)
        {
            await service.ProcessFashnAiWebhookAsync(response);
            return Ok();
        }
    }
}
