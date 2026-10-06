using System.Linq.Expressions;
using IvanIvanovKt_31_20.Database;
using IvanNikandrovKt_31_23.Dto;
using IvanNikandrovKt_31_23.Models;
using Microsoft.EntityFrameworkCore;

namespace IvanNikandrovKt_31_23.Interfaces
{
    public interface IDisciplineService
    {
        /// <summary>Список дисциплин с фильтрацией по названию и статусу удаления.</summary>
        Task<List<DisciplineDto>> GetDisciplinesAsync(DisciplineFilter filter, CancellationToken ct = default);

        Task<ServiceResult<DisciplineDto>> AddDisciplineAsync(DisciplineRequest request, CancellationToken ct = default);

        Task<ServiceResult<DisciplineDto>> UpdateDisciplineAsync(int disciplineId, DisciplineRequest request, CancellationToken ct = default);

        /// <summary>Мягкое удаление дисциплины.</summary>
        Task<ServiceResult<bool>> DeleteDisciplineAsync(int disciplineId, CancellationToken ct = default);
    }

    public class DisciplineService : IDisciplineService
    {
        private static readonly Expression<Func<Discipline, DisciplineDto>> ToDto = d =>
            new DisciplineDto(d.DisciplineId, d.Name, d.IsDeleted);

        private readonly StudentDbContext _db;

        public DisciplineService(StudentDbContext db)
        {
            _db = db;
        }

        public async Task<List<DisciplineDto>> GetDisciplinesAsync(DisciplineFilter filter, CancellationToken ct = default)
        {
            var query = _db.Disciplines.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                var name = filter.Name.Trim();
                query = query.Where(d => d.Name!.Contains(name));
            }

            if (filter.IsDeleted.HasValue)
                query = query.Where(d => d.IsDeleted == filter.IsDeleted.Value);

            return await query.OrderBy(d => d.Name).Select(ToDto).ToListAsync(ct);
        }

        public async Task<ServiceResult<DisciplineDto>> AddDisciplineAsync(DisciplineRequest request, CancellationToken ct = default)
        {
            var error = await ValidateAsync(request, 0, ct);
            if (error != null)
                return ServiceResult<DisciplineDto>.Invalid(error);

            var discipline = new Discipline
            {
                Name = request.Name!.Trim()
            };

            _db.Disciplines.Add(discipline);
            await _db.SaveChangesAsync(ct);

            return ServiceResult<DisciplineDto>.Ok(await GetDtoAsync(discipline.DisciplineId, ct));
        }

        public async Task<ServiceResult<DisciplineDto>> UpdateDisciplineAsync(int disciplineId, DisciplineRequest request, CancellationToken ct = default)
        {
            var discipline = await _db.Disciplines.FirstOrDefaultAsync(d => d.DisciplineId == disciplineId && !d.IsDeleted, ct);
            if (discipline == null)
                return ServiceResult<DisciplineDto>.NotFound("Дисциплина не найдена");

            var error = await ValidateAsync(request, disciplineId, ct);
            if (error != null)
                return ServiceResult<DisciplineDto>.Invalid(error);

            discipline.Name = request.Name!.Trim();
            await _db.SaveChangesAsync(ct);

            return ServiceResult<DisciplineDto>.Ok(await GetDtoAsync(disciplineId, ct));
        }

        public async Task<ServiceResult<bool>> DeleteDisciplineAsync(int disciplineId, CancellationToken ct = default)
        {
            var discipline = await _db.Disciplines.FirstOrDefaultAsync(d => d.DisciplineId == disciplineId && !d.IsDeleted, ct);
            if (discipline == null)
                return ServiceResult<bool>.NotFound("Дисциплина не найдена");

            discipline.IsDeleted = true;
            await _db.SaveChangesAsync(ct);

            return ServiceResult<bool>.Ok(true);
        }

        private Task<DisciplineDto> GetDtoAsync(int disciplineId, CancellationToken ct)
        {
            return _db.Disciplines.AsNoTracking().Where(d => d.DisciplineId == disciplineId).Select(ToDto).FirstAsync(ct);
        }

        private async Task<string?> ValidateAsync(DisciplineRequest request, int currentDisciplineId, CancellationToken ct)
        {
            var name = request.Name?.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return "Название дисциплины обязательно";
            if (name.Length > 150)
                return "Название дисциплины не должно быть длиннее 150 символов";
            if (await _db.Disciplines.AnyAsync(d => d.Name == name && !d.IsDeleted && d.DisciplineId != currentDisciplineId, ct))
                return "Дисциплина с таким названием уже существует";

            return null;
        }
    }
}
