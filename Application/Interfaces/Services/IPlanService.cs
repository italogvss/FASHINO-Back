using Application.DTOs.Plan;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanResponse>> GetAllPlansByRegion(string region);
        Task AddPlan(IEnumerable<PlanDTO> planDTO);
        Task UpdatePlan(Guid id, PlanDTO planDTO);
        Task DeletePlan(Guid id);
    }
}
