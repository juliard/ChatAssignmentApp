using ChatAssignmentApp.Domain;

namespace ChatAssignmentApp.Memory.Services
{
    public class ShiftStorageService : IShiftStorageService, IDisposable
    {
        private readonly List<Shift> _shifts = new();

        public void CreateShift(
            Shift value)
        {
            if (!_shifts.Any(a => a.ShiftId == value.ShiftId))
                _shifts.Add(value);
        }

        public Shift? GetShift()
        {
            var shift = _shifts.FirstOrDefault();

            if (shift != null)
                return shift;

            return null;
        }

        public Shift? GetShift(
            Guid shiftId)
        {
            var shift = _shifts.FirstOrDefault(a => a.ShiftId == shiftId);

            if (shift != null)
                return shift;

            return null;
        }

        public List<Shift> GetShifts()
        {
            return _shifts;
        }

        public void DeleteShift(
            Shift value)
        {
            _shifts.Remove(value);
        }

        public void Dispose()
        {
            _shifts.Clear();
        }
    }
}
