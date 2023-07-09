using Microsoft.Extensions.Options;
using OtpNet;
using PasswordGenerator.BLL.Models;
using PasswordGenerator.BLL.Services.Interfaces;
using PasswordGenerator.BLL.Utils;
using System.Security.Cryptography;
using System.Text;

namespace PasswordGenerator.BLL.Services.Implementation
{
    public class PasswordHandler : IPasswordHandler
    {
        private readonly IOptions<PasswordConfiguration> _passwordConfiguration;

        public PasswordHandler(IOptions<PasswordConfiguration> passwordConfiguration)
        {
            _passwordConfiguration = passwordConfiguration.ThrowIfNull(nameof(passwordConfiguration));
        }

        public string CheckPasswordTotp(string code)
        {
            //Validate secret key
            var secretKey = _passwordConfiguration.Value
                                                  .SecretKey
                                                  .ThrowIfNullOrEmpty(nameof(_passwordConfiguration.Value.SecretKey));

            //Convert string secret key to an byte array
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var totp = new Totp(keyBytes);

            bool valid = totp.VerifyTotp(code, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

            return valid ? "Valid" : "Invalid";
        }

        public Boolean CheckPasswordHmac(UserDetails userDetails, string otp)
        {
            ValidateUserDetails(userDetails);

            var secretKey = _passwordConfiguration.Value
                                                  .SecretKey
                                                  .ThrowIfNullOrEmpty(nameof(_passwordConfiguration.Value.SecretKey));

            var validityTime = userDetails.Date;
            string expectedOtp = String.Empty;
            string message = String.Empty;
            byte[] messageBytes;
            byte[] hashBytes;
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            HMACSHA256 hmac = new HMACSHA256(keyBytes);

            for (int i = 0; i < 30; i++)
            {
                message = userDetails.Id + validityTime.ToString("yyyy-MM-ddTHH:mm:ssZ");

                messageBytes = Encoding.UTF8.GetBytes(message);

                hashBytes = hmac.ComputeHash(messageBytes);

                expectedOtp = Base32Encode(hashBytes).Substring(0, 6);

                if (string.Equals(expectedOtp, otp))
                    return true;

                validityTime = validityTime.AddSeconds(1);
            }

            return false;
        }

        /// <summary>
        /// This method generates a one-time password using HMAC-SHA256 encryption.
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>
        public string GenerateOneTimePasswordWithHmac(UserDetails userDetails)
        {
            ValidateUserDetails(userDetails);

            ////Validate secret key
            var secretKey = _passwordConfiguration.Value
                                                  .SecretKey
                                                  .ThrowIfNullOrEmpty(nameof(_passwordConfiguration.Value.SecretKey));
            //Validate the time 
            var validityTime = _passwordConfiguration.Value
                                             .ValidityTime
                                             .ThrowIfNullOrEmpty(nameof(_passwordConfiguration.Value.ValidityTime));

            var time = userDetails.Date.AddSeconds(int.Parse(validityTime));
            string message = userDetails.Id + time.ToString("yyyy-MM-ddTHH:mm:ssZ");
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);

            HMACSHA256 hmac = new(keyBytes);

            byte[] hashBytes = hmac.ComputeHash(messageBytes);
            string otp = Base32Encode(hashBytes).Substring(0, 6);

            return otp;
        }

        /// <summary>
        /// This method generates a one-time password (OTP) using the OtpNet library
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns></returns>
        public string GenerateOneTimePasswordWithTotpLib(UserDetails userDetails)
        {
            ValidateUserDetails(userDetails);

            ////Validate secret key
            var secretKey = _passwordConfiguration.Value
                                                  .SecretKey
                                                  .ThrowIfNullOrEmpty(nameof(_passwordConfiguration.Value.SecretKey));
            //Validate the time 
            var validityTime = _passwordConfiguration.Value
                                             .ValidityTime
                                             .ThrowIfNullOrEmpty(nameof(_passwordConfiguration.Value.ValidityTime));

            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);

            var totp = new Totp(keyBytes, step: int.Parse(validityTime));
            var code = totp.ComputeTotp(userDetails.Date);

            return code;
        }

        private void ValidateUserDetails(UserDetails userDetails)
        {
            _ = userDetails.ThrowIfNull(nameof(userDetails));
            _ = userDetails.Id.ThrowIfNotStrictPositive(nameof(userDetails.Id));
        }

        static string Base32Encode(byte[] data)
        {
            const string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            int digitGroup = 0;
            int index = 0;
            int currentByte = 0;

            // Calculate the number of digit groups
            StringBuilder result = new StringBuilder((data.Length + 7) * 8 / 5);
            int[] digitGroups = new int[(data.Length + 4) / 5];


            // Convert the bytes into groups of 5 bytes
            for (int i = 0; i < data.Length; i++)
            {
                digitGroup = i / 5;// Determine the current digit group
                digitGroups[digitGroup] <<= 8;// Shift the previous bytes in the digit group to the left by 8 bits
                digitGroups[digitGroup] |= data[i];// Add the current byte to the digit group
            }

            // Convert each digit group into a base32 encoded string
            foreach (int group in digitGroups)
            {
                currentByte = group;

                for (int j = 0; j < 8; j++)
                {
                    index = currentByte & 0x1F;// Extract the lowest 5 bits of the current byte
                    result.Append(base32Chars[index]);// Append the corresponding base32 character to the result
                    currentByte >>= 5;// Shift the current byte to the right by 5 bits
                }
            }

            return result.ToString();
        }
    }
}

