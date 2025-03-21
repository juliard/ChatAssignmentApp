namespace ChatAssignmentApp.Core.Shifts.Models
{
    public class CreateShiftModel
    {
        public DateTime ShiftStart { get; set; }

        public short NumberOfJuniorAgents { get; set; }
        public short NumberOfMidAgents { get; set; }
        public short NumberOfSeniorAgents { get; set; }
        public short NumberOfLeadAgents { get; set; }

        public bool IsOverflowAgentsAvailable { get; set; }

        public int TotalAgents { get => NumberOfJuniorAgents + NumberOfMidAgents + NumberOfSeniorAgents + NumberOfLeadAgents; }
    }
}
