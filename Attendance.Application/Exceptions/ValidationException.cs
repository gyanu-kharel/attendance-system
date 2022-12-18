namespace Attendance.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string msg) : base(msg)
        {

        }

        public ValidationException(string msg, object obj) : base(msg)
        {

        }
    }
}
