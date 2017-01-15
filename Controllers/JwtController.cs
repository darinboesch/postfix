using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using postfix.Options;
using postfix.ViewModels;
using AutoMapper;
using postfix.Models.User;
using postfix.Shared.DataAccess;

namespace postfix.Controllers
{
  [Route("api/[controller]")]
  public class JwtController : Controller
  {
    private readonly JwtIssuerOptions _jwtOptions;
    private IPostfixRepository _repository;
    private ILogger<JwtController> _logger;
    private readonly JsonSerializerSettings _serializerSettings;

    public JwtController(IOptions<JwtIssuerOptions> jwtOptions, IPostfixRepository repository, ILogger<JwtController> logger)
    {
      _jwtOptions = jwtOptions.Value;
      ThrowIfInvalidOptions(_jwtOptions);

      _repository = repository;
      _logger = logger;

      _serializerSettings = new JsonSerializerSettings
      {
        Formatting = Formatting.Indented
      };
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromForm] UserViewModel vm)
    {
        var user = Mapper.Map<PostfixUser>(vm);
        //var identity = await _repository.GetClaimsIdentity(user, vm.Password);

        var identity = await GetClaimsIdentity(user, vm.Password);
        if (identity == null)
        {
          _logger.LogInformation($"Invalid username ({user.UserName}) or password ({vm.Password})");
          return BadRequest("Invalid credentials");
        }

        var claims = new[]
        {
          new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
          new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
          new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
          identity.FindFirst("PostfixUserLevel")
        };

        // Create the JWT security token and encode it.
        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: _jwtOptions.NotBefore,
            expires: _jwtOptions.Expiration,
            signingCredentials: _jwtOptions.SigningCredentials);

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        // Serialize and return the response
        var response = new
        {
          access_token = encodedJwt,
          expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
        };

        var json = JsonConvert.SerializeObject(response, _serializerSettings);
        return new OkObjectResult(json);
    }

    private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
    {
      if (options == null) throw new ArgumentNullException(nameof(options));

      if (options.ValidFor <= TimeSpan.Zero)
      {
        throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
      }

      if (options.SigningCredentials == null)
      {
        throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
      }

      if (options.JtiGenerator == null)
      {
        throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
      }
    }

    /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
    private static long ToUnixEpochDate(DateTime date)
      => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

    /// <summary>
    /// IMAGINE BIG RED WARNING SIGNS HERE!
    /// You'd want to retrieve claims through your claims provider
    /// in whatever way suits you, the below is purely for demo purposes!
    /// </summary>
    private static Task<ClaimsIdentity> GetClaimsIdentity(PostfixUser user, string password)
    {
      if (user.UserName == "dboesch" &&
          password == "(pipster")
      {
        return Task.FromResult(new ClaimsIdentity(new GenericIdentity(user.UserName, "Token"),
          new[]
          {
            new Claim("PostfixUserLevel", "1")
          }));
      }

    //   if (username is found and password is found, but only guest access)
    //   {
    //     return Task.FromResult(new ClaimsIdentity(new GenericIdentity(user.UserName, "Token"),
    //       new Claim[] { }));
    //   }

      // Credentials are invalid, or account doesn't exist
      return Task.FromResult<ClaimsIdentity>(null);
    }
  }
}