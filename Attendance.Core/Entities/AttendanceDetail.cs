using Attendance.Core.Common;

namespace Attendance.Core.Entities
{
    public class AttendanceDetail : BaseEntity
    {
        #region Constructors

        public AttendanceDetail()
        {
            Date = DateTime.UtcNow;
            PunchInTime = DateTime.UtcNow.TimeOfDay;

        }

        #endregion

        #region Properties
        public DateTime? Date { get; set; }
        public TimeSpan? PunchInTime { get; set; }
        public TimeSpan? PunchOutTime { get; set; }
        public string? LatePunchInRemarks { get; set; }
        public string? LatePunchOutRemarks { get; set; }
        public Guid EmployeeId { get; set; }

        // Navigation
        public virtual Employee Employee { get; set; }

        #endregion
    }
}
