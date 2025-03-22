using ChatAssignmentApp.Core.Shifts.Commands;
using ChatAssignmentApp.Core.Shifts.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ChatAssignmentApp.Core.Shifts
{
    public static class IoCExtensions
    {
        public static void AddShiftCommands(this IServiceCollection services)
        {
            services.AddTransient<ICreateShiftCommand, CreateShiftCommand>();
            services.AddTransient<IEndShiftCommand, EndShiftCommand>();
            services.AddTransient<IGetShiftsCommand, GetShiftsCommand>();
        }
    }
}
