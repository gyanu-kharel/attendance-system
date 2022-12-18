namespace Attendance.Application.Common
{
    /// <summary>
    /// General response format for each APIs.
    /// </summary>
    public class ApiResponse
    {
        public Error Errors { get; set; }
    }

    // TODO: Will keep on adding different fields in this response format
    public class Error
    {
        public string Message { get; set; }
    }
}
