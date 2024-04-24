using JwtSignFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JwtSignFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    
    public class SignInController : ControllerBase
    {
        [HttpPost]
        public IActionResult SignIn()
        {
            
            return Ok("Admin girdi");
        }



    }
}
