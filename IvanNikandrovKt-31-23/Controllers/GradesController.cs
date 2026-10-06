using IvanNikandrovKt_31_23.Dto;
using IvanNikandrovKt_31_23.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IvanNikandrovKt_31_23.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GradesController : ApiBaseController
    {
        private readonly IGradeService _gradeService;

        public GradesController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        /// <summary>Средний балл по предмету в группе.</summary>
        [HttpPost("group-discipline-average")]
        public async Task<IActionResult> GetGroupDisciplineAverage([FromBody] GroupDisciplineAverageFilter filter, CancellationToken ct)
        {
            return ToResult(await _gradeService.GetGroupDisciplineAverageAsync(filter, ct));
        }

        /// <summary>Оценки конкретного студента (можно указать предмет).</summary>
        [HttpPost("student")]
        public async Task<IActionResult> GetStudentGrades([FromBody] StudentGradesFilter filter, CancellationToken ct)
        {
            return ToResult(await _gradeService.GetStudentGradesAsync(filter, ct));
        }

        /// <summary>Средний балл по году обучения (курсу).</summary>
        [HttpPost("year-average")]
        public async Task<IActionResult> GetYearAverage([FromBody] YearAverageFilter filter, CancellationToken ct)
        {
            return ToResult(await _gradeService.GetYearAverageAsync(filter, ct));
        }

        [HttpPost]
        public async Task<IActionResult> AddGrade([FromBody] AddGradeRequest request, CancellationToken ct)
        {
            return ToResult(await _gradeService.AddGradeAsync(request, ct));
        }

        [HttpPut("{gradeId:int}")]
        public async Task<IActionResult> UpdateGrade(int gradeId, [FromBody] UpdateGradeRequest request, CancellationToken ct)
        {
            return ToResult(await _gradeService.UpdateGradeAsync(gradeId, request, ct));
        }
    }
}
