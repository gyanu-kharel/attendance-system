using Attendance.DataAccess.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Attendance.DataAccess.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static void AddPersistenceServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDbContext<AttendanceDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DbConnectionString")));
        }
    }
}
