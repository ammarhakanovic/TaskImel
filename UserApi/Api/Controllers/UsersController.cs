using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApi.Api.Interface;
using UserApi.DataLayer.Entity;
using UserApi.DataLayer.Models;

namespace UserApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize(Roles = "Admin")]

        [HttpGet("getusers")]
        public async Task<IActionResult> GetAll([FromQuery] UserQueryParameters parameters)
        {
            try
            {
                var users = await _userService.GetAllAsync(parameters);
                if (users == null) return NotFound();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]

        [HttpGet("getuser{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                return user == null ? NotFound($"Korisnik sa {id} nije pronadjen.") : Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("createuser")]
        public async Task<IActionResult> Create(User user)
        {
            try
            {
                var created = await _userService.CreateAsync(user);
                return Ok("Korisnik uspjesno kreiran");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("updateuser{id}")]
        public async Task<IActionResult> Update(int id, User user)
        {
            try
            {
                var result = await _userService.UpdateAsync(id, user);
                return Ok("Korisnik uspjesno azuriran!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]

        [HttpDelete("deleteuser{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _userService.DeleteAsync(id);
                return Ok("Korisnik uspjesno izbrisan!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
