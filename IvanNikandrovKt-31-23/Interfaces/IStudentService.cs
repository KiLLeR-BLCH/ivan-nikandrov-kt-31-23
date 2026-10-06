using System.Linq.Expressions;
using IvanIvanovKt_31_20.Database;
using IvanNikandrovKt_31_23.Dto;
using IvanNikandrovKt_31_23.Models;
using Microsoft.EntityFrameworkCore;

namespace IvanNikandrovKt_31_23.Interfaces
{
    public interface IStudentService
    {
        /// <summary>Список студентов с фильтрацией по группе, ФИО и статусу удаления.</summary>
        Task<List<StudentDto>> GetStudentsAsync(StudentFilter filter, CancellationToken ct = default);

        Task<ServiceResult<StudentDto>> AddStudentAsync(StudentRequest request, CancellationToken ct = default);

        Task<ServiceResult<StudentDto>> UpdateStudentAsync(int studentId, StudentRequest request, CancellationToken ct = default);

        /// <summary>Мягкое удаление студента.</summary>
        Task<ServiceResult<bool>> DeleteStudentAsync(int studentId, CancellationToken ct = default);
    }

    public class StudentService : IStudentService
    {
        private static readonly Expression<Func<Student, StudentDto>> ToDto = s =>
            new StudentDto(s.StudentId, s.FirstName, s.LastName, s.GroupId, s.Group!.Name, s.IsDeleted);

        private readonly StudentDbContext _db;

        public StudentService(StudentDbContext db)
        {
            _db = db;
        }

        public async Task<List<StudentDto>> GetStudentsAsync(StudentFilter filter, CancellationToken ct = default)
        {
            var query = _db.Students.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.GroupName))
            {
                var groupName = filter.GroupName.Trim();
                query = query.Where(s => s.Group!.Name == groupName);
            }

            if (!string.IsNullOrWhiteSpace(filter.FullName))
            {
                // Каждое слово из ФИО должно встретиться либо в имени, либо в фамилии
                var parts = filter.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var part in parts)
                {
                    var p = part;
                    query = query.Where(s => s.FirstName!.Contains(p) || s.LastName!.Contains(p));
                }
            }

            if (filter.IsDeleted.HasValue)
                query = query.Where(s => s.IsDeleted == filter.IsDeleted.Value);

            return await query
                .OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
                .Select(ToDto)
                .ToListAsync(ct);
        }

        public async Task<ServiceResult<StudentDto>> AddStudentAsync(StudentRequest request, CancellationToken ct = default)
        {
            var error = await ValidateAsync(request, ct);
            if (error != null)
                return ServiceResult<StudentDto>.Invalid(error);

            var student = new Student
            {
                FirstName = request.FirstName!.Trim(),
                LastName = request.LastName!.Trim(),
                GroupId = request.GroupId
            };

            _db.Students.Add(student);
            await _db.SaveChangesAsync(ct);

            return ServiceResult<StudentDto>.Ok(await GetDtoAsync(student.StudentId, ct));
        }

        public async Task<ServiceResult<StudentDto>> UpdateStudentAsync(int studentId, StudentRequest request, CancellationToken ct = default)
        {
            var student = await _db.Students.FirstOrDefaultAsync(s => s.StudentId == studentId && !s.IsDeleted, ct);
            if (student == null)
                return ServiceResult<StudentDto>.NotFound("Студент не найден");

            var error = await ValidateAsync(request, ct);
            if (error != null)
                return ServiceResult<StudentDto>.Invalid(error);

            student.FirstName = request.FirstName!.Trim();
            student.LastName = request.LastName!.Trim();
            student.GroupId = request.GroupId;
            await _db.SaveChangesAsync(ct);

            return ServiceResult<StudentDto>.Ok(await GetDtoAsync(studentId, ct));
        }

        public async Task<ServiceResult<bool>> DeleteStudentAsync(int studentId, CancellationToken ct = default)
        {
            var student = await _db.Students.FirstOrDefaultAsync(s => s.StudentId == studentId && !s.IsDeleted, ct);
            if (student == null)
                return ServiceResult<bool>.NotFound("Студент не найден");

            student.IsDeleted = true;
            await _db.SaveChangesAsync(ct);

            return ServiceResult<bool>.Ok(true);
        }

        private Task<StudentDto> GetDtoAsync(int studentId, CancellationToken ct)
        {
            return _db.Students.AsNoTracking().Where(s => s.StudentId == studentId).Select(ToDto).FirstAsync(ct);
        }

        private async Task<string?> ValidateAsync(StudentRequest request, CancellationToken ct)
        {
            var firstName = request.FirstName?.Trim();
            var lastName = request.LastName?.Trim();

            if (string.IsNullOrWhiteSpace(firstName))
                return "Имя обязательно";
            if (string.IsNullOrWhiteSpace(lastName))
                return "Фамилия обязательна";
            if (firstName.Length > 100 || lastName.Length > 100)
                return "Имя и фамилия не должны быть длиннее 100 символов";
            if (!await _db.Groups.AnyAsync(g => g.GroupId == request.GroupId && !g.IsDeleted, ct))
                return "Группа не найдена";

            return null;
        }
    }
}
