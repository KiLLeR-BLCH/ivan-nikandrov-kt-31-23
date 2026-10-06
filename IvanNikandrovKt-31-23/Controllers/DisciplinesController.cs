using IvanNikandrovKt_31_23.Dto;
using IvanNikandrovKt_31_23.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IvanNikandrovKt_31_23.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DisciplinesController : ApiBaseController
    {
        private readonly IDisciplineService _disciplineService;

        public DisciplinesController(IDisciplineService disciplineService)
        {
            _disciplineService = disciplineService;
        }

        /// <summary>Список дисциплин (фильтр: название, статус удаления).</summary>
        [HttpPost("list")]
        public async Task<IActionResult> GetDisciplines([FromBody] DisciplineFilter filter, CancellationToken ct)
        {
            return Ok(await _disciplineService.GetDisciplinesAsync(filter, ct));
        }

        [HttpPost]
        public async Task<IActionResult> AddDiscipline([FromBody] DisciplineRequest request, CancellationToken ct)
        {
            return ToResult(await _disciplineService.AddDisciplineAsync(request, ct));
        }

        [HttpPut("{disciplineId:int}")]
        public async Task<IActionResult> UpdateDiscipline(int disciplineId, [FromBody] DisciplineRequest request, CancellationToken ct)
        {
            return ToResult(await _disciplineService.UpdateDisciplineAsync(disciplineId, request, ct));
        }

        [HttpDelete("{disciplineId:int}")]
        public async Task<IActionResult> DeleteDiscipline(int disciplineId, CancellationToken ct)
        {
            return ToResult(await _disciplineService.DeleteDisciplineAsync(disciplineId, ct));
        }
    }
}
