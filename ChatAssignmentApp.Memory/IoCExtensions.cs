using ChatAssignmentApp.Memory.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChatAssignmentApp.Memory
{
    public static class IoCExtensions
    {
        public static void InjectMemory(this IServiceCollection services)
        {
            services.AddSingleton<IShiftStorageService, ShiftStorageService>();
        }
    }
}
