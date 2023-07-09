using PasswordGenerator.BLL.CustomException;

namespace PasswordGenerator.BLL.Utils
{
    public static class ValidationExtention
    {
        public static string ThrowIfNullOrEmpty(this string obj, string argumentName)
        {
            if (string.IsNullOrEmpty(obj))
            {
                throw new ArgumentNullException(argumentName);
            }

            return obj;
        }

        public static T ThrowIfNull<T>(this T obj, string argumentName)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(argumentName);
            }

            return obj;
        }

        public static int ThrowIfNotStrictPositive(this int obj, string argumentName)
        {
            if (obj == default || obj < 1)
            {
                throw new InvalidIdException(argumentName);
            }

            return obj;
        }
    }
}
