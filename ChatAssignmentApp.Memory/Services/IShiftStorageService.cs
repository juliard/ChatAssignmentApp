using ChatAssignmentApp.Domain;

namespace ChatAssignmentApp.Memory.Services
{
    public interface IShiftStorageService
    {
        void CreateShift(Shift value);

        Shift? GetShift();
        
        Shift? GetShift(Guid shiftId);

        public List<Shift> GetShifts();

        void DeleteShift(Shift value);
    }
}
