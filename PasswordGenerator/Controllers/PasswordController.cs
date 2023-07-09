using Microsoft.AspNetCore.Mvc;
using PasswordGenerator.BLL.Models;
using PasswordGenerator.BLL.Services.Interfaces;

namespace PasswordGenerator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PasswordController : Controller
    {
        private readonly IPasswordHandler _passwordHandler;

        public PasswordController(IPasswordHandler passwordHandler)
        {
            _passwordHandler = passwordHandler;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult CheckPassword(string code)
        {
            try
            {
                var result = _passwordHandler.CheckPasswordTotp(code);

                return Ok(result);
            }
            catch (Exception ex) { return Ok(ex.Message); }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult CheckPasswordHmac(UserDetails userDetails, string code)
        {
            try
            {
                var result = _passwordHandler.CheckPasswordHmac(userDetails, code);

                return Ok(result);
            }
            catch (Exception ex) { return Ok(ex.Message); }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult GeneratePasswordWithHmac([FromBody] UserDetails userDetails)
        {
            try
            {
                var result = _passwordHandler.GenerateOneTimePasswordWithHmac(userDetails);

                return Ok(result);
            }
            catch (Exception ex) { return Ok(ex.Message); }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult GeneratePasswordWithTotpLib([FromBody] UserDetails userDetails)
        {
            try
            {
                var result = _passwordHandler.GenerateOneTimePasswordWithTotpLib(userDetails);

                return Ok(result);
            }
            catch (Exception ex) { return Ok(ex.Message); }
        }
    }
}
