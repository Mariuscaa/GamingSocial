using HIOF.GamingSocial.GameInformation.Data;
using HIOF.GamingSocial.GameInformation.Models;
using HIOF.GamingSocial.GameInformation.Models.V2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;


namespace HIOF.GamingSocial.GameInformation.Controllers.V2;

[ApiController]
[Route("V2/GameCollection")]
public class V2GameCollectionController : ControllerBase
{

    private readonly ILogger<V2GameCollectionController> _logger;
    private readonly VideoGameDbContext _db;
    public V2GameCollectionController(ILogger<V2GameCollectionController> logger, VideoGameDbContext db)
    {
        _logger = logger;
        _db = db;
    }
    
    /// <summary>
    /// Adds a game collection for one user to the database
    /// </summary>
    /// <param name="collectionPost"></param>
    /// <returns></returns>
    [HttpPost("GamesToPers")]
    [ProducesResponseType(typeof(V2Result<V2GamesWithUsers>), 200)]
    [ProducesResponseType(typeof(V2Result<V2GamesWithUsers>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V2Result<V2GamesWithUsers>>> CreateCollection(V2PostGameCollection collectionPost)
    {

        //var dbContext = new VideoGameDbContext();
        for (int i = 0; i < collectionPost.GameIds.Count; i++)
        {
            var gameCollection = new ProfileGameCollection()
            {

            ProfileId = collectionPost.ProfileId,
            GameId = collectionPost.GameIds[i],
            };
            _db.ProfileGameCollection.Add(gameCollection);
            await _db.SaveChangesAsync();
        }

        
        var result = new V2Result<V2PostGameCollection>(new V2PostGameCollection()
        {
            ProfileId = collectionPost.ProfileId,
            GameIds = collectionPost.GameIds,
        });
        
        return Ok(result);

    }
    /// <summary>
    /// Gets all the users that have this specific game
    /// </summary>
    /// <param name="spillid"></param>
    /// <returns></returns>
    [HttpGet("GamesWithUsers")]
    [ProducesResponseType(typeof(V2Result<V2UsersWithGames>), 200)]
    [ProducesResponseType(typeof(V2Result<V2UsersWithGames>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V2Result<V2UsersWithGames>>> Get(int spillid)
    {
        //var dbContext = new VideoGameDbContext();

        var responseCollection = await _db.ProfileGameCollection
            .Where(profile => profile.GameId == spillid)
            .Select(profile => new ProfileGameCollection
            {
                ProfileId = profile.ProfileId,
                GameId = profile.GameId,
            })
            .ToListAsync();


        if (responseCollection.Count == 0)
        {
            return BadRequest(new V2Result<V2UsersWithGames>($"there are no Game collections in the database."));
        }

        V2UsersWithGames? responseUser = new V2UsersWithGames();
        responseUser.ProfileIds = new List<Guid>();
        responseUser.GameId = spillid;

        foreach (var collections in responseCollection)
        {
            responseUser.ProfileIds.Add(collections.ProfileId);
        }
        return Ok(new V2Result<V2UsersWithGames>(responseUser));

    }
    /// <summary>
    /// Getts all the games one user have.
    /// </summary>
    /// <param name="profileID"></param>
    /// <returns></returns>
    [HttpGet("GamesFromOneUser")]
    [ProducesResponseType(typeof(V2Result<V2GamesWithUsers>), 200)]
    [ProducesResponseType(typeof(V2Result<V2GamesWithUsers>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V2Result<V2GamesWithUsers>>> GetAllGames(Guid profileID)
    {
        //var dbContext = new VideoGameDbContext();

        var responseCollection = await _db.ProfileGameCollection
            .Where(profile => profile.ProfileId == profileID)
            .Select(profile => new ProfileGameCollection
            {
                ProfileId = profile.ProfileId,
                GameId = profile.GameId,
            })
            .ToListAsync();
        if (responseCollection.Count == 0)
        {
            return BadRequest(new V2Result<V2GamesWithUsers>($"there are no Game collections in the database."));
        }
        
        
        V2GamesWithUsers? responseUser = new V2GamesWithUsers();
        responseUser.ProfileId = profileID;
        responseUser.GameIds = new List<int>();

        foreach (var collections in responseCollection)
        {
            responseUser.GameIds.Add(collections.GameId);
        }
        return Ok(new V2Result<V2GamesWithUsers>(responseUser));
    }
    /// <summary>
    /// Deletes game(s) from a user
    /// </summary>
    /// <param name="collectionDelete"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(V2Result<ProfileGameCollection>), 200)]
    [ProducesResponseType(typeof(V2Result<ProfileGameCollection>), 400)]
    [ProducesResponseType(500)]
    [HttpDelete("DeleteGameFromUser")]
    public async Task<ActionResult<V2Result<ProfileGameCollection>>> DeleteGameFromUser(Guid ProfileId, int GameId)
    {


    //var dbContext = new VideoGameDbContext();
    var responseCollection = await _db.ProfileGameCollection
              .Where(profile => profile.ProfileId == ProfileId)
              .Where(profile => profile.GameId == GameId)
            .Select(profile => new ProfileGameCollection
            {
                ProfileId = profile.ProfileId,
                GameId = profile.GameId,
            })
            .ToListAsync();

        

        if (responseCollection.IsNullOrEmpty())
        {
            return BadRequest(new V2Result<ProfileGameCollection>("Cant't find what you are searching for in the database. Doublecheck that you have filled in the correct information"));
        }





        //for (int i = 0; i < collectionDelete.GameIds.Count; i++)
        //{
        //    responseUser.GameIds.Add(collectionDelete.GameIds[i]);
        //    var gameCollection = new ProfileGameCollection()
        //    {

        //        ProfileId = collectionDelete.ProfileId,
        //        GameId = collectionDelete.GameIds[i],
        //    };
        //    _db.ProfileGameCollection.Remove(gameCollection);
        //    await _db.SaveChangesAsync();
        //}




        var gameCollection = new ProfileGameCollection()
        {

            ProfileId = ProfileId,
            GameId = GameId,
        };

        _db.ProfileGameCollection.Remove(gameCollection);
        await _db.SaveChangesAsync();
        return Ok(new V2Result<ProfileGameCollection>(gameCollection));
    }
}

