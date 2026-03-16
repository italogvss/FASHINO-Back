using Application.DTOs.Plan;
using AutoMapper;
using Domain.Entities;
using System.Text.Json;

namespace Application.Mapping
{
    public class PlanMappingProfile : Profile
    {
        public PlanMappingProfile() {
            CreateMap<Plan, PlanDTO>()
            .ForMember(dest => dest.Features, opt => opt.MapFrom(src => ParseFeatures(src.Features)));
            
            CreateMap<PlanPrice, PlanPriceDTO>();
        }
        private static string[] ParseFeatures(string? features)
        {
            if (string.IsNullOrWhiteSpace(features))
                return Array.Empty<string>();

            var trimmed = features.Trim();

            // Tenta desserializar JSON: ["a","b"]
            if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
            {
                try
                {
                    var arr = JsonSerializer.Deserialize<string[]>(trimmed);
                    if (arr != null)
                        return arr.Select(s => s?.Trim() ?? string.Empty)
                                  .Where(s => s.Length > 0)
                                  .ToArray();
                }
                catch
                {
                    // Se falhar, continuará para split por ';' abaixo
                }
            }

            // Caso seja string separada por ponto e vírgula: "a; b; c"
            return trimmed
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .ToArray();
        }
    }
}
