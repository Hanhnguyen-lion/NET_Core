using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApi.Data;
using MyApi.DTOs;
using MyApi.Models;

namespace MyApi.Controllers{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersContextDb _context;

        // Constructor injecting the ApplicationDbContext
        public UsersController(IConfiguration configuration)
        {
            _context = new UsersContextDb(configuration) ;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(){

            var users = await Task.Run(() => _context.GetUsers());

            return Ok(users);
        }

        [HttpPost("SaveClient")]
        public async Task<IActionResult> SaveClient(Client client){
            await Task.Run(() => _context.InsertClient(client));

            return Ok();
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(User user){
            await Task.Run(() => _context.DeleteUser(user.Email??""));
            return Ok();
        }

        // Registers a new user.
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            // Validate the incoming model.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string? email = registerDto?.Email?.ToLower();
            // Check if the email already exists.
            var existingUser = await Task.Run(() => _context.GetUser(email ?? ""));

            if (existingUser != null)
            {
                return Conflict(new { message = "Email is already registered." });
            }

            // Hash the password using BCrypt.
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto?.Password);

            // Create a new user entity.
            var newUser = new User
            {
                Firstname = registerDto?.Firstname,
                Lastname = registerDto?.Lastname,
                Email = email,
                Password = hashedPassword
            };

            // Add the new user to the database.
            await Task.Run(() => _context.InsertUser(newUser));

            // // Optionally, assign a default role to the new user.
            // // For example, assign the "User" role.
            // var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            // if (userRole != null)
            // {
            //     var newUserRole = new UserRole
            //     {
            //         UserId = newUser.Id,
            //         RoleId = userRole.Id
            //     };
            //     _context.UserRoles.Add(newUserRole);
            //     await _context.SaveChangesAsync();
            // }

            return CreatedAtAction(nameof(GetProfile), new { id = newUser.Id }, new { message = "User registered successfully." });
        }

        // Retrieves the authenticated user's profile.
        [HttpGet("GetProfile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            // Extract the user's email from the JWT token claims.
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email);
            if (emailClaim == null)
            {
                return Unauthorized(new { message = "Invalid token: Email claim missing." });
            }

            string userEmail = emailClaim.Value;

            var user = await Task.Run(() => this._context.GetUser(userEmail.ToLower()));
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Map the user entity to ProfileDTO.
            var profile = new ProfileDTO
            {
                Id = user.Id,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname
                // ,
                // Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };
            return Ok(profile);
        }

        // Updates the authenticated user's profile.
        [HttpPut("UpdateProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO updateDto)
        {
            // Validate the incoming model.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Extract the user's email from the JWT token claims.
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email);
            if (emailClaim == null)
            {
                return Unauthorized(new { message = "Invalid token: Email claim missing." });
            }

            string userEmail = emailClaim.Value;

            // Retrieve the user from the database.
            var user = await Task.Run(() => _context.GetUser(userEmail.ToLower()));

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Update fields if provided.
            if (!string.IsNullOrEmpty(updateDto.Firstname))
            {
                user.Firstname = updateDto.Firstname;
            }

            if (!string.IsNullOrEmpty(updateDto.Lastname))
            {
                user.Lastname = updateDto.Lastname;
            }

            if (!string.IsNullOrEmpty(updateDto.Email))
            {
                // Check if the new email is already taken by another user.
                var emailExists = await Task.Run(() => _context.GetUsers()
                    .Any(u => u?.Email?.ToLower() == updateDto.Email.ToLower() && u.Id != user.Id)) ;

                if (emailExists)
                {
                    return Conflict(new { message = "Email is already in use by another account." });
                }

                user.Email = updateDto.Email;
            }

            if (!string.IsNullOrEmpty(updateDto.Password))
            {
                // Hash the new password before storing.
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(updateDto.Password);
                user.Password = hashedPassword;
            }

            // Save the changes to the database.
            await Task.Run(() => _context.UpdateUser(user));

            return Ok(new { message = "Profile updated successfully." });
        }
    }
}