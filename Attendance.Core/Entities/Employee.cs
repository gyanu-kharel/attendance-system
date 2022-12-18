using Attendance.Core.Common;


namespace Attendance.Core.Entities
{
    public class Employee : BaseEntity, IAuditableEntity
    {
        #region Constructors

        public Employee()
        {
            CreatedOn = DateTime.UtcNow;
            IsActive = true;
        }

        #endregion

        #region Properties
        public string Name { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsActive { get; set; }
        public Guid ApplicationUserId { get; set; }

        // Navigation
        public virtual ApplicationUser ApplicationUser { get; set; }

        #endregion
    }
}
