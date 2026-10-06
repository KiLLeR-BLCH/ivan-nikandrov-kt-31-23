using IvanNikandrovKt_31_23.Dto;
using IvanNikandrovKt_31_23.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IvanNikandrovKt_31_23.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : ApiBaseController
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        /// <summary>Список студентов (фильтр: группа, ФИО, статус удаления).</summary>
        [HttpPost("list")]
        public async Task<IActionResult> GetStudents([FromBody] StudentFilter filter, CancellationToken ct)
        {
            return Ok(await _studentService.GetStudentsAsync(filter, ct));
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent([FromBody] StudentRequest request, CancellationToken ct)
        {
            return ToResult(await _studentService.AddStudentAsync(request, ct));
        }

        [HttpPut("{studentId:int}")]
        public async Task<IActionResult> UpdateStudent(int studentId, [FromBody] StudentRequest request, CancellationToken ct)
        {
            return ToResult(await _studentService.UpdateStudentAsync(studentId, request, ct));
        }

        [HttpDelete("{studentId:int}")]
        public async Task<IActionResult> DeleteStudent(int studentId, CancellationToken ct)
        {
            return ToResult(await _studentService.DeleteStudentAsync(studentId, ct));
        }
    }
}
