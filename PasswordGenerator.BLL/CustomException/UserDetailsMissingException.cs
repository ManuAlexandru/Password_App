using System.Runtime.Serialization;

namespace PasswordGenerator.BLL.CustomException
{
    public class UserDetailsMissingException : Exception
    {
        public UserDetailsMissingException()
        {
        }

        public UserDetailsMissingException(string? message) : base(message)
        {
        }

        public UserDetailsMissingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UserDetailsMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
