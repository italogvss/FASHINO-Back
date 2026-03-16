using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IPlanRepository : IRepository<Plan>
    {
        // Retorna todos os planos que possuem preço na região informada (inclui os preços filtrados pela região)
        Task<IEnumerable<Plan>> GetAllByRegionAsync(string region);

        // Retorna um plano pelo id, incluindo apenas os preços referentes à região informada
        Task<Plan?> GetByIdWithRegionAsync(Guid id, string region);
    }
}
