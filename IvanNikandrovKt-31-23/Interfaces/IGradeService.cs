using IvanIvanovKt_31_20.Database;
using IvanNikandrovKt_31_23.Dto;
using IvanNikandrovKt_31_23.Models;
using Microsoft.EntityFrameworkCore;

namespace IvanNikandrovKt_31_23.Interfaces
{
    public interface IGradeService
    {
        /// <summary>Средний балл по предмету в группе.</summary>
        Task<ServiceResult<AverageGradeDto>> GetGroupDisciplineAverageAsync(GroupDisciplineAverageFilter filter, CancellationToken ct = default);

        /// <summary>Оценки конкретного студента (опционально по одному предмету) и его средний балл.</summary>
        Task<ServiceResult<StudentGradesDto>> GetStudentGradesAsync(StudentGradesFilter filter, CancellationToken ct = default);

        /// <summary>Средний балл по году обучения (курсу).</summary>
        Task<ServiceResult<AverageGradeDto>> GetYearAverageAsync(YearAverageFilter filter, CancellationToken ct = default);

        Task<ServiceResult<GradeDto>> AddGradeAsync(AddGradeRequest request, CancellationToken ct = default);

        Task<ServiceResult<GradeDto>> UpdateGradeAsync(int gradeId, UpdateGradeRequest request, CancellationToken ct = default);

        /// <summary>Дисциплины, по которым студенты введенной группы получили оценку "2".</summary>
        Task<ServiceResult<DisciplinesWithGrade2Dto>> GetDisciplinesWithGrade2Async(DisciplinesWithGrade2Filter filter, CancellationToken ct = default);
    }

    public class GradeService : IGradeService
    {
        private const int MinGrade = 2;
        private const int MaxGrade = 5;

        private const int Grade2 = 2;

        private readonly StudentDbContext _db;

        public GradeService(StudentDbContext db)
        {
            _db = db;
        }

        public async Task<ServiceResult<AverageGradeDto>> GetGroupDisciplineAverageAsync(GroupDisciplineAverageFilter filter, CancellationToken ct = default)
        {
            if (!await _db.Groups.AnyAsync(g => g.GroupId == filter.GroupId && !g.IsDeleted, ct))
                return ServiceResult<AverageGradeDto>.NotFound("Группа не найдена");
            if (!await _db.Disciplines.AnyAsync(d => d.DisciplineId == filter.DisciplineId && !d.IsDeleted, ct))
                return ServiceResult<AverageGradeDto>.NotFound("Дисциплина не найдена");

            var query = _db.Grades.AsNoTracking()
                .Where(g => g.DisciplineId == filter.DisciplineId
                            && g.Student!.GroupId == filter.GroupId
                            && !g.Student!.IsDeleted);

            return ServiceResult<AverageGradeDto>.Ok(await CalculateAverageAsync(query, ct));
        }

        public async Task<ServiceResult<StudentGradesDto>> GetStudentGradesAsync(StudentGradesFilter filter, CancellationToken ct = default)
        {
            var student = await _db.Students.AsNoTracking()
                .Where(s => s.StudentId == filter.StudentId && !s.IsDeleted)
                .Select(s => new { s.StudentId, s.FirstName, s.LastName })
                .FirstOrDefaultAsync(ct);

            if (student == null)
                return ServiceResult<StudentGradesDto>.NotFound("Студент не найден");

            var query = _db.Grades.AsNoTracking()
                .Where(g => g.StudentId == filter.StudentId && !g.Discipline!.IsDeleted);

            if (filter.DisciplineId.HasValue)
                query = query.Where(g => g.DisciplineId == filter.DisciplineId.Value);

            var grades = await query
                .OrderBy(g => g.Discipline!.Name).ThenBy(g => g.GradeId)
                .Select(g => new GradeDto(g.GradeId, g.StudentId, g.DisciplineId, g.Discipline!.Name, g.Value))
                .ToListAsync(ct);

            double? average = grades.Count == 0 ? null : Math.Round(grades.Average(g => g.Value), 2);
            var fullName = $"{student.LastName} {student.FirstName}";

            return ServiceResult<StudentGradesDto>.Ok(new StudentGradesDto(student.StudentId, fullName, average, grades));
        }

        public async Task<ServiceResult<AverageGradeDto>> GetYearAverageAsync(YearAverageFilter filter, CancellationToken ct = default)
        {
            if (filter.Course < 1 || filter.Course > 6)
                return ServiceResult<AverageGradeDto>.Invalid("Курс должен быть от 1 до 6");

            if (filter.DisciplineId.HasValue
                && !await _db.Disciplines.AnyAsync(d => d.DisciplineId == filter.DisciplineId.Value && !d.IsDeleted, ct))
                return ServiceResult<AverageGradeDto>.NotFound("Дисциплина не найдена");

            var query = _db.Grades.AsNoTracking()
                .Where(g => g.Student!.Group!.Course == filter.Course
                            && !g.Student!.IsDeleted
                            && !g.Student!.Group!.IsDeleted
                            && !g.Discipline!.IsDeleted);

            if (filter.DisciplineId.HasValue)
                query = query.Where(g => g.DisciplineId == filter.DisciplineId.Value);

            return ServiceResult<AverageGradeDto>.Ok(await CalculateAverageAsync(query, ct));
        }

        public async Task<ServiceResult<GradeDto>> AddGradeAsync(AddGradeRequest request, CancellationToken ct = default)
        {
            if (request.Value < MinGrade || request.Value > MaxGrade)
                return ServiceResult<GradeDto>.Invalid($"Оценка должна быть от {MinGrade} до {MaxGrade}");
            if (!await _db.Students.AnyAsync(s => s.StudentId == request.StudentId && !s.IsDeleted, ct))
                return ServiceResult<GradeDto>.Invalid("Студент не найден");

            var discipline = await _db.Disciplines.AsNoTracking()
                .FirstOrDefaultAsync(d => d.DisciplineId == request.DisciplineId && !d.IsDeleted, ct);
            if (discipline == null)
                return ServiceResult<GradeDto>.Invalid("Дисциплина не найдена");

            var grade = new Grade
            {
                StudentId = request.StudentId,
                DisciplineId = request.DisciplineId,
                Value = request.Value
            };

            _db.Grades.Add(grade);
            await _db.SaveChangesAsync(ct);

            return ServiceResult<GradeDto>.Ok(new GradeDto(grade.GradeId, grade.StudentId, grade.DisciplineId, discipline.Name, grade.Value));
        }

        public async Task<ServiceResult<GradeDto>> UpdateGradeAsync(int gradeId, UpdateGradeRequest request, CancellationToken ct = default)
        {
            var grade = await _db.Grades
                .Include(g => g.Discipline)
                .Include(g => g.Student)
                .FirstOrDefaultAsync(g => g.GradeId == gradeId, ct);

            if (grade == null || grade.Student!.IsDeleted || grade.Discipline!.IsDeleted)
                return ServiceResult<GradeDto>.NotFound("Оценка не найдена");

            if (request.Value < MinGrade || request.Value > MaxGrade)
                return ServiceResult<GradeDto>.Invalid($"Оценка должна быть от {MinGrade} до {MaxGrade}");

            grade.Value = request.Value;
            await _db.SaveChangesAsync(ct);

            return ServiceResult<GradeDto>.Ok(new GradeDto(grade.GradeId, grade.StudentId, grade.DisciplineId, grade.Discipline!.Name, grade.Value));
        }

        private static async Task<AverageGradeDto> CalculateAverageAsync(IQueryable<Grade> query, CancellationToken ct)
        {
            var count = await query.CountAsync(ct);
            // (double?) нужен, чтобы для пустой выборки вернулся null, а не исключение
            var average = await query.AverageAsync(g => (double?)g.Value, ct);

            return new AverageGradeDto(average.HasValue ? Math.Round(average.Value, 2) : null, count);
        }

        public async Task<ServiceResult<DisciplinesWithGrade2Dto>> GetDisciplinesWithGrade2Async(DisciplinesWithGrade2Filter filter, CancellationToken ct = default)
        {
            var groupName = filter.GroupName?.Trim();
            if (string.IsNullOrWhiteSpace(groupName))
                return ServiceResult<DisciplinesWithGrade2Dto>.Invalid("Название группы обязательно");

            if (!await _db.Groups.AnyAsync(g => g.Name == groupName && !g.IsDeleted, ct))
                return ServiceResult<DisciplinesWithGrade2Dto>.NotFound("Группа не найдена");

            var names = await _db.Grades.AsNoTracking()
                .Where(g => g.Value == Grade2
                            && g.Student!.Group!.Name == groupName
                            && !g.Student!.IsDeleted
                            && !g.Student!.Group!.IsDeleted
                            && !g.Discipline!.IsDeleted)
                .Select(g => g.Discipline!.Name)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync(ct);

            return ServiceResult<DisciplinesWithGrade2Dto>.Ok(new DisciplinesWithGrade2Dto(groupName, string.Join(", ", names)));
        }
    }
}
