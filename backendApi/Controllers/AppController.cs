using backendApi.Data;
using backendApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppController : ControllerBase
    {
        private readonly backendApiDbContext _context;
        public AppController(backendApiDbContext context)
        {
            _context = context;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            var authenticatedUser = _context.Users.SingleOrDefault(u => u.Email == user.Email && u.Password == user.Password);

            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(authenticatedUser);
            var userGames = _context.Games.Where(g => g.Id == authenticatedUser.UserId).ToList();

            return Ok(new { Token = token, UserGames = userGames });
        }


        [HttpGet("games")]
        public IActionResult GetGames()
        {
            var games = _context.Games.ToList();
            return Ok(games);
        }

        [HttpGet("games/{id}")]
        public IActionResult GetGame(int id)
        {
            var game = _context.Games.Find(id);

            if (game == null)
            {
                return NotFound();
            }

            return Ok(game);
        }

        [HttpPost("games")]
        public IActionResult CreateGame([FromBody] Game game)
        {
            _context.Games.Add(game);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
        }

        [HttpPut("games/{id}")]
        public IActionResult UpdateGame(int id, [FromBody] Game updatedGame)
        {
            var existingGame = _context.Games.Find(id);

            if (existingGame == null)
            {
                return NotFound();
            }

            existingGame.Name = updatedGame.Name;
            existingGame.Genre = updatedGame.Genre;


            _context.Entry(existingGame).State= Microsoft.EntityFrameworkCore.EntityState.Modified;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("games/{id}")]
        public IActionResult DeleteGame(int id)
        {
            var game = _context.Games.Find(id);

            if (game == null)
            {
                return NotFound();
            }

            _context.Games.Remove(game);
            _context.SaveChanges();

            return NoContent();
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secure_key_with_at_least_128_bits"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.UserId.ToString()),
            // Add other claims as needed
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
