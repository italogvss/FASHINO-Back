using Application.DTOs.Plan;
using Application.Interfaces.Services;
using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanController(IPlanService service) : ControllerBase
    {
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create Plan",
            Description = "Cria um novo plano com preços e características informadas."
        )]
        public async Task<ApiResponse<object>> Create([FromBody] IEnumerable<PlanDTO> request)
        {
            await service.AddPlan(request);
            return ApiResponse<object>.SuccessResult(null, "Plano criado com sucesso");
        }

        [HttpGet("list")]
        [SwaggerOperation(
            Summary = "List Plans by Region",
            Description = "Retorna todos os planos que possuem preços para a região informada."
        )]
        public async Task<ApiResponse<IEnumerable<PlanResponse>>> List([FromQuery] string region)
        {
            var result = await service.GetAllPlansByRegion(region);
            return ApiResponse<IEnumerable<PlanResponse>>.SuccessResult(result, "Lista de planos obtida com sucesso");
        }

        

        [HttpPut("{id:guid}")]
        [SwaggerOperation(
            Summary = "Update Plan",
            Description = "Atualiza os dados do plano e os preços informados."
        )]
        public async Task<ApiResponse<object>> Update(Guid id, [FromBody] PlanDTO request)
        {
            await service.UpdatePlan(id, request);
            return ApiResponse<object>.SuccessResult(null, "Plano atualizado com sucesso");
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary = "Delete Plan",
            Description = "Exclui um plano pelo seu ID."
        )]
        public async Task<ApiResponse<object>> Delete(Guid id)
        {
            await service.DeletePlan(id);
            return ApiResponse<object>.SuccessResult(null, "Plano deletado com sucesso");
        }
    }
}
