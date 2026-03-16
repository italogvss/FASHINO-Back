using Application.DTOs.Tests;
using Application.DTOs.User;
using Application.Interfaces.Services;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Development
{
    [ApiController]
    [Route("api/dev/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IAuthAppService _authAppService;

        public TestController(IAuthAppService authAppService)
        {
            _authAppService = authAppService;
        }

        [HttpPost("generate-uid")]
        public async Task<IActionResult> GenerateUid([FromBody] CreateTestUser request)
        {
            var result = await _authAppService.CreateUserTestAsync(request);

            return Ok(ApiResponse<string>.SuccessResult(result, "Login successful"));
        }
    }
}
