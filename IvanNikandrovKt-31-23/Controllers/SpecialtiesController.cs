using IvanNikandrovKt_31_23.Dto;
using IvanNikandrovKt_31_23.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IvanNikandrovKt_31_23.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpecialtiesController : ApiBaseController
    {
        private readonly ISpecialtyService _specialtyService;

        public SpecialtiesController(ISpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
        }

        /// <summary>Список специальностей (фильтр: название, код).</summary>
        [HttpPost("list")]
        public async Task<IActionResult> GetSpecialties([FromBody] SpecialtyFilter filter, CancellationToken ct)
        {
            return Ok(await _specialtyService.GetSpecialtiesAsync(filter, ct));
        }

        [HttpPost]
        public async Task<IActionResult> AddSpecialty([FromBody] SpecialtyRequest request, CancellationToken ct)
        {
            return ToResult(await _specialtyService.AddSpecialtyAsync(request, ct));
        }

        [HttpPut("{specialtyId:int}")]
        public async Task<IActionResult> UpdateSpecialty(int specialtyId, [FromBody] SpecialtyRequest request, CancellationToken ct)
        {
            return ToResult(await _specialtyService.UpdateSpecialtyAsync(specialtyId, request, ct));
        }
    }
}
