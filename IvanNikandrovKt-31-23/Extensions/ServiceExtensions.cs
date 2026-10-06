using IvanNikandrovKt_31_23.Interfaces;

namespace IvanNikandrovKt_31_23.Extensions
{
    /// <summary>Регистрация сервисов, связанных с логикой БД (интерфейс -> реализация).</summary>
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
        {
            // Scoped, потому что сервисы используют StudentDbContext, который тоже живёт на время одного запроса
            services.AddScoped<ISpecialtyService, SpecialtyService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IDisciplineService, DisciplineService>();
            services.AddScoped<IGradeService, GradeService>();

            return services;
        }
    }
}
