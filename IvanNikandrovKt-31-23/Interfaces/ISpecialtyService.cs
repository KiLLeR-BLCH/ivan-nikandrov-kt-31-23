using System.Linq.Expressions;
using IvanIvanovKt_31_20.Database;
using IvanNikandrovKt_31_23.Dto;
using IvanNikandrovKt_31_23.Models;
using Microsoft.EntityFrameworkCore;

namespace IvanNikandrovKt_31_23.Interfaces
{
    public interface ISpecialtyService
    {
        /// <summary>Список специальностей с фильтрацией по названию и коду.</summary>
        Task<List<SpecialtyDto>> GetSpecialtiesAsync(SpecialtyFilter filter, CancellationToken ct = default);

        Task<ServiceResult<SpecialtyDto>> AddSpecialtyAsync(SpecialtyRequest request, CancellationToken ct = default);

        Task<ServiceResult<SpecialtyDto>> UpdateSpecialtyAsync(int specialtyId, SpecialtyRequest request, CancellationToken ct = default);
    }

    public class SpecialtyService : ISpecialtyService
    {
        private static readonly Expression<Func<Specialty, SpecialtyDto>> ToDto = s =>
            new SpecialtyDto(s.SpecialtyId, s.Title, s.Code);

        private readonly StudentDbContext _db;

        public SpecialtyService(StudentDbContext db)
        {
            _db = db;
        }

        public async Task<List<SpecialtyDto>> GetSpecialtiesAsync(SpecialtyFilter filter, CancellationToken ct = default)
        {
            var query = _db.Specialties.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.Title))
            {
                var title = filter.Title.Trim();
                query = query.Where(s => s.Title!.Contains(title));
            }

            if (!string.IsNullOrWhiteSpace(filter.Code))
            {
                var code = filter.Code.Trim();
                query = query.Where(s => s.Code!.Contains(code));
            }

            return await query.OrderBy(s => s.Code).Select(ToDto).ToListAsync(ct);
        }

        public async Task<ServiceResult<SpecialtyDto>> AddSpecialtyAsync(SpecialtyRequest request, CancellationToken ct = default)
        {
            var error = await ValidateAsync(request, 0, ct);
            if (error != null)
                return ServiceResult<SpecialtyDto>.Invalid(error);

            var specialty = new Specialty
            {
                Title = request.Title!.Trim(),
                Code = request.Code!.Trim()
            };

            _db.Specialties.Add(specialty);
            await _db.SaveChangesAsync(ct);

            return ServiceResult<SpecialtyDto>.Ok(await GetDtoAsync(specialty.SpecialtyId, ct));
        }

        public async Task<ServiceResult<SpecialtyDto>> UpdateSpecialtyAsync(int specialtyId, SpecialtyRequest request, CancellationToken ct = default)
        {
            var specialty = await _db.Specialties.FirstOrDefaultAsync(s => s.SpecialtyId == specialtyId, ct);
            if (specialty == null)
                return ServiceResult<SpecialtyDto>.NotFound("Специальность не найдена");

            var error = await ValidateAsync(request, specialtyId, ct);
            if (error != null)
                return ServiceResult<SpecialtyDto>.Invalid(error);

            specialty.Title = request.Title!.Trim();
            specialty.Code = request.Code!.Trim();
            await _db.SaveChangesAsync(ct);

            return ServiceResult<SpecialtyDto>.Ok(await GetDtoAsync(specialtyId, ct));
        }

        private Task<SpecialtyDto> GetDtoAsync(int specialtyId, CancellationToken ct)
        {
            return _db.Specialties.AsNoTracking().Where(s => s.SpecialtyId == specialtyId).Select(ToDto).FirstAsync(ct);
        }

        /// <param name="currentSpecialtyId">Id изменяемой специальности (0 при добавлении).</param>
        private async Task<string?> ValidateAsync(SpecialtyRequest request, int currentSpecialtyId, CancellationToken ct)
        {
            var title = request.Title?.Trim();
            var code = request.Code?.Trim();

            if (string.IsNullOrWhiteSpace(title))
                return "Название специальности обязательно";
            if (string.IsNullOrWhiteSpace(code))
                return "Код специальности обязателен";
            if (title.Length > 150)
                return "Название специальности не должно быть длиннее 150 символов";
            if (code.Length > 20)
                return "Код специальности не должен быть длиннее 20 символов";
            if (await _db.Specialties.AnyAsync(s => s.Code == code && s.SpecialtyId != currentSpecialtyId, ct))
                return "Специальность с таким кодом уже существует";

            return null;
        }
    }
}
