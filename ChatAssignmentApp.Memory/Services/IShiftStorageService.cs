using ChatAssignmentApp.Domain;

namespace ChatAssignmentApp.Memory.Services
{
    public interface IShiftStorageService
    {
        void CreateShift(Shift value);

        Shift? GetShift();
        
        Shift? GetShift(Guid shiftId);

        List<Shift> GetShifts();

        bool DoesShiftExist();

        void DeleteShift(Shift value);
    }
}
