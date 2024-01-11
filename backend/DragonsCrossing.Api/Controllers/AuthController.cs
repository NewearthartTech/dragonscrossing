using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using DragonsCrossing.Core.Services;
using DragonsCrossing.NewCombatLogic;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace DragonsCrossing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    readonly ILogger _logger;
    readonly AuthConfig _myConfig;
    readonly Web3Config _web3Config;
    readonly IPerpetualDbService _perpetualDb;
    readonly IBlockchainService _blockchainService;
    readonly IPublishEndpoint _publishEp;
    readonly IServiceProvider _sp;

    public readonly static string ClaimWalletVarified = @"ClaimWalletVarified";
    public readonly static string ClaimSelectedHeroId = @"SelectedHeroId";
    public readonly static string ClaimSelectedSeasonId = @"SelectedSeasonId";

    public AuthController(
            IPerpetualDbService db,
            IServiceProvider sp,
            IConfiguration config,
            IBlockchainService blockchainService,
            IPublishEndpoint publishEp,
            ILogger<AuthController> logger
            )
    {
        _sp = sp;
        _perpetualDb = db;
        _logger = logger;
        _publishEp = publishEp;
        _myConfig = AuthConfig.CreateFromConfig(config);
        _blockchainService = blockchainService;
        _web3Config = config.GetSection("web3").Get<Web3Config>() ?? new Web3Config();
    }

    /// <summary>
    /// used for frontend to get the nounce that can be signed by metamask
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet()]
    public AuthOptions getAuthOptions(string? userId = null)
    {
        return new AuthOptions
        {
            Web3Nounce = $"Sign in to DCX at {DateTime.Now}",
            contractsForChain = _web3Config.deployedContractsForChain()!,
        };
    }

    /// <summary>
    /// We might already be signed in, and be trying to acquire more claims 
    /// </summary>
    /// <param name="creds"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpPost]
    public async Task<AuthenticatedUser> SignIn([FromBody] AuthCredentials creds)
    {
        if (string.IsNullOrWhiteSpace(creds.Web3Signature))
            throw new ArgumentNullException(nameof(creds.Web3Signature));

        _logger.LogDebug("checking for web3Signature validation");
        if (string.IsNullOrWhiteSpace(creds.WalletAddress))
            throw new ArgumentNullException(creds.WalletAddress);


        //TODO: this is security issue, we need to store the Nounce in Redis or something
        var extratedAddress = Web3Utils.extractAddressFromSignature(
            originalMessage: creds.Options.Web3Nounce ?? "",
            signatureString: creds.Web3Signature
        ).ToLowerInvariant();

        if (extratedAddress != creds.WalletAddress.ToLowerInvariant())
            throw new Exception("Someone is trying to hack address");


        //var player = await _playersService.EnsurePlayer(extratedAddress);
        var collection = _perpetualDb.getCollection<PlayerDto>();
        var player = await collection.Find(p => p.BlockchainPublicAddress == extratedAddress).SingleOrDefaultAsync();

        if(null == player)
        {
            player = new PlayerDto
            {
                BlockchainPublicAddress = extratedAddress,
            };

            await collection.InsertOneAsync(player);
        }

        var claims = new[] { new Claim(ClaimWalletVarified, extratedAddress) };

        return new AuthenticatedUser(player, claims.ToArray(), _myConfig);

    }

    /// <summary>
    /// Used to remove the selected Hero from JWT
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [Authorize]
    [HttpGet("removeHeroClaim")]
    public async Task<AuthenticatedUser> RemoveHeroClaim()
    {
        var playerId = this.GetUserId();
        var collection = _perpetualDb.getCollection<PlayerDto>();
        var player = await collection.Find(p => p.Id == playerId).SingleAsync();

        if (string.IsNullOrEmpty(player.BlockchainPublicAddress))
        {
            throw new Exception($"player {playerId} blockchain address is null");
        }

        var claims = new[] {
            new Claim(ClaimWalletVarified, player.BlockchainPublicAddress),
        };

        return new AuthenticatedUser(player, claims.ToArray(), _myConfig);

    }

    [Authorize]
    [HttpGet("claimHero/{HeroId}")]
    public async Task<AuthenticatedUser> AddHeroClaim(int HeroId)
    {
        var playerId = this.GetUserId();

        var collection = _perpetualDb.getCollection<PlayerDto>();
        var player = await collection.Find(p => p.Id == playerId).SingleAsync();

        if (string.IsNullOrEmpty(player.BlockchainPublicAddress))
        {
            throw new Exception($"player {playerId} blockchain address is null");
        }

        var claims = new List<Claim>(new[] {
            new Claim(ClaimWalletVarified, player.BlockchainPublicAddress),
            new Claim(ClaimSelectedHeroId, HeroId.ToString())
        });


        if (HeroDto.LOANERHEROID != HeroId)
        {

            var hero = await _perpetualDb.getCollection<HeroDto>().Find(c => c.id == HeroId).SingleOrDefaultAsync();
            var ownedHeroIds = await _blockchainService.OwnedHeros(new[] { HeroId }, player.BlockchainPublicAddress);


            if (null != hero?.isLoanedHero)
            {
                if (ownedHeroIds.Contains(HeroId))
                {
                    _logger.LogInformation($"converting loaned hero {HeroId} to normal");
                    await _perpetualDb.getCollection<HeroDto>().UpdateOneAsync(
                        c => c.id == HeroId,
                        Builders<HeroDto>.Update.Unset(h => h.isLoanedHero)
                    );

                    if(0 != hero.seasonId)
                    {
                        _logger.LogInformation($"converting sesonal loaned hero {HeroId} to normal");
                        ISeasonsDbService seasonDb = ActivatorUtilities.CreateInstance<SeasonsDbService>(_sp, hero.seasonId);
                        await seasonDb.getCollection<DbGameState>().UpdateOneAsync(
                            g => g.HeroId == HeroId,
                            Builders<DbGameState>.Update.Unset(g => g.Hero.isLoanedHero)
                        ) ;
                    }

                    
                }
                else
                {
                    if (hero.isLoanedHero.loanedToUserId != playerId)
                    {
                        throw new ExceptionWithCode("You no longer loan this Hero");
                    }
                }
                
            }
            else
            {
                

                if (!ownedHeroIds.Contains(HeroId))
                {
                    throw new ExceptionWithCode("You no longer own this Hero");
                }

            }

            if (null != hero && 0 != hero.seasonId)
            {
                claims.Add(new Claim(ClaimSelectedSeasonId, hero.seasonId.ToString()));

                //setup Hero resets 
                await _publishEp.Publish(new Core.Sagas.ResetHeroIfNeededMessage
                {
                    heroId = HeroId
                });
            }
        }

        return new AuthenticatedUser(player, claims.ToArray(), _myConfig);

    }

    public static string GenerateJWTToken(PlayerDto player, AuthConfig _myConfig, Claim[]? claims = null)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_myConfig.PrivateKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        claims = (claims ?? new Claim[] { }).Concat(new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, player.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim(JwtRegisteredClaimNames.UniqueName, user.details?.primaryEmail??user.details?.phoneNumber),
            }
        ).ToArray();

        var token = new JwtSecurityToken(
        issuer: _myConfig.JwtIssuer,
        audience: _myConfig.JwtIssuer,
        claims: claims,
        expires: DateTime.Now.AddMinutes(_myConfig.JwtExpiryMin),
        signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}

public class AuthCredentials
{
    /// <summary>
    /// The options we are responding to
    /// </summary>
    public AuthOptions Options { get; set; } = new AuthOptions();

    /// <summary>
    /// Signature from metamask
    /// </summary>
    public string? Web3Signature { get; set; }

    public string? WalletAddress { get; set; }
}


public class AuthenticatedUser
{
    [Required]
    public string Jwt { get; set; } = "";

    [Required]
    public PlayerDto Player { get; set; } = new PlayerDto();


    public int? seasonId { get; set; }

    [System.Text.Json.Serialization.JsonConstructor]
    [Newtonsoft.Json.JsonConstructor]
    protected AuthenticatedUser() { }
    //public AuthenticatedUser() { }

    public AuthenticatedUser(PlayerDto user, Claim[] claims, AuthConfig _myConfig)
    {
        Jwt = AuthController.GenerateJWTToken(user, _myConfig, claims);
        Player = user;

        var seasonClaim = claims.Where(c => c.Type == AuthController.ClaimSelectedSeasonId).FirstOrDefault();

        if(null != seasonClaim)
        {
            seasonId = int.Parse( seasonClaim.Value);
        }
            
    }
}

/// <summary>
/// displays the options available for Further Authentication
/// Out Auth here is progressive. vene when a user just uses the software we try to remember who she is
/// </summary>
public class AuthOptions
{
    /// <summary>
    /// A nounce that can be signed by metamask to auth using wallet address
    /// </summary>
    public string? Web3Nounce { get; set; }

    [Required]
    public DeployedContractsForChain contractsForChain { get; set; } = new DeployedContractsForChain();

}

public class AuthConfig
{
    public string PrivateKey { get; set; } = "";
    public string JwtIssuer { get; set; } = @"colourbox";

    public int JwtExpiryMin { get; set; } = 60 * 24 * 7; //7 days

    /// <summary>
    /// When does the OTP Timesout in seconds
    /// </summary>
    public int OtpTimeout { get; set; } = 60 * 5;

    /// <summary>
    /// can resend OTP after how many seconds
    /// </summary>
    public int OtpResendTimeout { get; set; } = 15;

    static string _generatedPrivateKey = string.Empty;

    public static AuthConfig CreateFromConfig(IConfiguration Configuration)
    {
        var config = Configuration.GetSection("login").Get<AuthConfig>();
        if (null == config)
            config = new AuthConfig();

        if (string.IsNullOrEmpty(config.PrivateKey))
        {
            if (string.IsNullOrWhiteSpace(_generatedPrivateKey))
            {
                var rnd = new Random();
                _generatedPrivateKey = new string(Enumerable.Range(0, 64).Select(i => (char)rnd.Next('a', 'z')).ToArray());
            }

            config.PrivateKey = _generatedPrivateKey;
        }

        return config;

    }

}




