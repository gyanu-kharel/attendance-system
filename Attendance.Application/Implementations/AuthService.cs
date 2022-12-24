using Attendance.Application.Common;
using Attendance.Application.Contracts;
using Attendance.Application.Exceptions;
using Attendance.Application.Models.Auth;
using Attendance.Core.Entities;
using Attendance.DataAccess.Persistence.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Attendance.Application.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AttendanceDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOptions<JwtSettings> _jwtSettings;

        public AuthService(
            AttendanceDbContext dbContext, 
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings
            )
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings;
        }


        #region Methods
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto data)
        {
            var user = await _userManager.FindByEmailAsync(data.Email.ToUpper());

            if (user is null)
                throw new ValidationException("Incorrect email/password");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, data.Password);

            if (!isPasswordValid)
                throw new ValidationException("Incorrect email/password");

            var signInResult = await _signInManager.PasswordSignInAsync(user, data.Password, true, false);

            if (!signInResult.Succeeded)
                throw new ValidationException("Incorrect email/password");

            var userClaims = await GetClaimsAsync(user);

            // only use JWT and ignore refresh token at the moment
            return new LoginResponseDto(await GenerateJwtTokenAsync(userClaims));
        }
        public async Task<Guid> CreateUserAsync(string email)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser is not null)
                throw new ValidationException("Email already in use");

            ApplicationUser user = new ApplicationUser()
            {
                Email = email,
                NormalizedEmail = email.ToUpper(),
                UserName = email,
                NormalizedUserName = email.ToUpper(),
                SecurityStamp = DateTime.Now.Ticks.ToString(),
            };

            // TODO: Generate random password and send it to the user's email
            PasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = hasher.HashPassword(user, "employee123");

            await _userManager.CreateAsync(user);

            await _userManager.AddToRoleAsync(user, "EMPLOYEE");

            return user.Id;
        }

        public async Task UpdateUserEmailAsync(Guid userId, string email)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                throw new ValidationException("User not found");

            // check if email is already in use
            var emailExists = await _userManager.FindByEmailAsync(email.ToUpper());
            if (emailExists is not null)
                throw new ValidationException("Email already in user");

            user.Email = email;
            user.NormalizedEmail = email;
            user.UserName = email;
            user.NormalizedUserName = email;

            await _userManager.UpdateAsync(user);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email.ToUpper());

            return user != null;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Create JWT security token with valid claims, issuer and audience
        /// </summary>
        /// <param name="claims"></param>
        /// <returns>string</returns>

        // TODO: Should move to utility library
        private async Task<string> GenerateJwtTokenAsync(List<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.Secret));

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Value.Issuer,
                audience: _jwtSettings.Value.Audience,
                expires: DateTime.Now.AddHours(1),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        /// <summary>
        /// Helper method to return the list of claims associated with the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>List of claims</returns>
        private async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            List<Claim> authClaims = new()
            {
                new Claim("email", user.Email),
                new Claim("uid", user.Id.ToString())
            };

            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            //authClaims.Add(new Claim("role", userRoles.FirstOrDefault()));
            return authClaims;
        }
        #endregion
    }
}
