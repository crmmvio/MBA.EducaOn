using MBA.EducaOn.Core.Extensions;
using MBA.EducaOn.Core.Models;
using MBA.EducaOn.GestaoAlunos.Data;
using MBA.EducaOn.GestaoAlunos.Domain;
using MBA.EducaOn.Security.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MBA.EducaOn.Api.Controllers;

[ApiController]
[Route("api/conta")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SecurityDbContext _secContext;
    private readonly AlunoContext _alunosContext;
    private readonly JwtSettings _jwtSettings;

    public AuthController(SignInManager<IdentityUser> signInManager,
                          UserManager<IdentityUser> userManager,
                          SecurityDbContext securityContext,
                          AlunoContext alunosContext,
                          IOptions<JwtSettings> jwtSettings)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _secContext = securityContext;
        _alunosContext = alunosContext;
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// Registra novo Usuario Aluno
    /// </summary>
    /// <param name="registerUser">Informe os dados do Usuario Aluno</param>
    /// <returns></returns>
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Registrar(RegisterUserViewModel registerUser)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var user = new IdentityUser(registerUser.UserName)
        {
            Email = registerUser.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, registerUser.Password);

        if (result.Succeeded)
        {
            //var aluno = new Aluno(user.Id.ToGuid(), registerUser.Name, user.Email);
            //_alunosContext.Alunos.Add(aluno);

            await _secContext.SaveChangesAsync();
            await _signInManager.SignInAsync(user, false);
            return Ok(await GerarJwt(registerUser.Email));
        }
        else
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(item.Code, item.Description);
            }
        }

        return ValidationProblem(detail: "Falha ao registrar o usuário", modelStateDictionary: ModelState);
    }

    /// <summary>
    /// Realiza login de usuario cadastrados
    /// </summary>
    /// <param name="loginUser">Informe os dados do usuario para login</param>
    /// <returns></returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Login(LoginUserViewModel loginUser)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var user = await _userManager.FindByEmailAsync(loginUser.Email);
        if (user != null)
        {
            var result = await _signInManager.PasswordSignInAsync(user, loginUser.Password, false, true);

            if (result.Succeeded)
            {
                return Ok(await GerarJwt(loginUser.Email));
            }
        }


        return Problem("Usuário ou senha incorretos");
    }

    private async Task<string> GerarJwt(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id)
            };

        //Adicionando as roles do usuário como Claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Segredo);

        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtSettings.Emissor,
            Audience = _jwtSettings.Audiencia,
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiracaoHoras),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        });

        var encodedToken = tokenHandler.WriteToken(token);

        return encodedToken;
    }
}
