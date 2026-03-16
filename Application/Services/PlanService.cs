using Application.DTOs.Plan;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services
{
    public class PlanService : IPlanService
    {
        // CORRIGIDO: separador em constante para evitar inconsistência entre métodos
        private const string FeatureSeparator = ";";
        private readonly IMapper _mapper;
        private readonly IPlanRepository _repository;

        public PlanService(IPlanRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task AddPlan(IEnumerable<PlanDTO> plans)
        {
            if (!plans.Any()) throw new ArgumentNullException(nameof(plans));

            foreach (var planDTO in plans)
            {


                var features = planDTO.Features != null
                    ? string.Join(FeatureSeparator, planDTO.Features)
                    : null;

                var plan = new Plan(planDTO.Name, planDTO.Description, features);

                if (planDTO.Prices != null)
                {
                    foreach (var p in planDTO.Prices)
                    {
                        var price = new PlanPrice(plan, p.Region, p.Currency, p.PriceMonthly, p.PriceYearly);
                        plan.Prices.Add(price);
                    }
                }

                await _repository.AddAsync(plan);
            }
            await _repository.SaveChangesAsync();
        }

        public async Task DeletePlan(Guid id)
        {
            var plan = await _repository.GetByIdAsync(id);
            if (plan == null)
                throw new NotFoundException("Plano não encontrado");

            await _repository.DeleteAsync(plan);
            await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<PlanResponse>> GetAllPlansByRegion(string region)
        {
            var plans = await _repository.GetAllByRegionAsync(region);

            if (!plans.Any())
                return Enumerable.Empty<PlanResponse>();

            return plans.Select(p =>
            {
                var price = p.Prices.First(); // já vem filtrado por região do repository

                return new PlanResponse
                {
                    Name = p.Name,
                    Description = p.Description ?? string.Empty,
                    Features = p.Features?.Split(FeatureSeparator) ?? Array.Empty<string>(),
                    Currency = price.Currency,
                    PriceMonthly = price.PriceMonthly,
                    PriceYearly = price.PriceYearly
                };
            });
        }

        public async Task UpdatePlan(Guid id, PlanDTO planDTO)
        {
            var plan = await _repository.GetByIdAsync(id);
            if (plan == null)
                throw new NotFoundException("Plano não encontrado");

            plan.UpdateDescription(planDTO.Description);

            // CORRIGIDO: usa a mesma constante de separador
            var features = planDTO.Features != null
                ? string.Join(FeatureSeparator, planDTO.Features)
                : null;

            plan.UpdateFeatures(features);

            if (planDTO.Prices != null && planDTO.Prices.Any())
            {
                foreach (var priceDto in planDTO.Prices)
                {
                    var existing = plan.Prices.FirstOrDefault(pp =>
                        pp.Region == priceDto.Region && pp.Currency == priceDto.Currency);

                    if (existing != null)
                    {
                        existing.UpdatePrices(priceDto.PriceMonthly, priceDto.PriceYearly);
                    }
                    else
                    {
                        var newPrice = new PlanPrice(plan, priceDto.Region, priceDto.Currency, priceDto.PriceMonthly, priceDto.PriceYearly);
                        plan.Prices.Add(newPrice);
                    }
                }
            }

            await _repository.UpdateAsync(plan);
            await _repository.SaveChangesAsync();
        }
    }
}