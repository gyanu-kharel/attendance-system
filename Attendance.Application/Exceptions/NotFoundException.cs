namespace Attendance.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string msg) : base(msg)
        {

        }

        public NotFoundException(string msg, object obj) : base(msg)
        {

        }
    }
}
