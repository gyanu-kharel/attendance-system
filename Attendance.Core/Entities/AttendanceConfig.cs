using Attendance.Core.Common;

namespace Attendance.Core.Entities
{
    public class AttendanceConfig : BaseEntity
    {
        // follows 24 hour clock 
        public TimeSpan PunchInTime { get; set; }
        public TimeSpan PunchOutTime { get; set; }
        public bool IsActive { get; set; }
    }
}
