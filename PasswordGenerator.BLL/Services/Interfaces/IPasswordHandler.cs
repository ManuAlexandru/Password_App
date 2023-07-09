using PasswordGenerator.BLL.Models;

namespace PasswordGenerator.BLL.Services.Interfaces
{
    public interface IPasswordHandler
    {
        string CheckPasswordTotp(string code);
        public Boolean CheckPasswordHmac(UserDetails userDetails, string otp);
        public string GenerateOneTimePasswordWithHmac(UserDetails userDetails);
        public string GenerateOneTimePasswordWithTotpLib(UserDetails userDetails);
    }
}