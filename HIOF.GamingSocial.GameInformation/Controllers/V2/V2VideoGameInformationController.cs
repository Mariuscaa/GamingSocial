using HIOF.GamingSocial.GameInformation.Data;
using HIOF.GamingSocial.GameInformation.Models;
using HIOF.GamingSocial.GameInformation.Models.V2;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace HIOF.GamingSocial.GameInformation.Controllers.V2
{
    /// <summary>
    /// Handles the data for video game information. 
    /// </summary>
    [ApiController]
    [Route("V2/VideoGameInformation")]
    public class V2VideoGameInformationController : ControllerBase
    {
        //TEST
        private readonly ILogger<V2VideoGameInformationController> _logger;
        private readonly VideoGameDbContext _db;

        public V2VideoGameInformationController(ILogger<V2VideoGameInformationController> logger, VideoGameDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        
        /// <summary>
        /// Gets information about all the videogames in the local database.
        /// </summary>
        /// <returns>A list of videogame objects with error information.</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(V2Result<IEnumerable<V2VideoGameInformation>>), 200)]
        [ProducesResponseType(typeof(V2Result<IEnumerable<V2VideoGameInformation>>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<V2Result<IEnumerable<V2VideoGameInformation>>>> Get()
        {
            //var dbContext = new VideoGameDbContext();
            if(_db.VideoGameInformation.IsNullOrEmpty())
            {
                return BadRequest(new V2Result<IEnumerable<V2VideoGameInformation>>("Database for videogames is currently empty."));
            }

            var responseGames = await _db.VideoGameInformation
                .Select(game => new V2VideoGameInformation
                {
                    Id = game.GameId,
                    GameDescription = game.GameDescription,
                    GameTitle = game.GameTitle,
                    GiantbombGuid = game.GiantbombGuid
                }).ToListAsync();
            return Ok(new V2Result<IEnumerable<V2VideoGameInformation>>(responseGames));
        }

        /// <summary>
        /// Gets information about a single game in the local database. If it is not there, it will call PublicGameInformation.
        /// </summary>
        /// <param name="giantbombGuid">A string with a unique guid from the giant bomb web api.</param>
        /// <returns>Information about a single game.</returns>
        [HttpGet("{giantbombGuid}")]
        [ProducesResponseType(typeof(V2Result<V2VideoGameInformation>), 200)]
        [ProducesResponseType(typeof(V2Result<V2VideoGameInformation>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<V2Result<V2VideoGameInformation>>> GetSingle(string giantbombGuid)
        {
            //var dbContext = new VideoGameDbContext();

            try
            {

                var responseGame = await _db.VideoGameInformation
                    .Where(game => game.GiantbombGuid == giantbombGuid)
                    .Select(game => new V2VideoGameInformation
                    {
                        Id = game.GameId,
                        GameTitle = game.GameTitle,
                        GiantbombGuid = game.GiantbombGuid,
                        GameDescription = game.GameDescription,
                    }).SingleAsync();
                return Ok(new V2Result<V2VideoGameInformation>(responseGame));
            }
            // If not found in database -> call PublicGameInformation.
            catch (InvalidOperationException)
            {
                // https://localhost:7250/V1/PublicGameInformation/GiantbombGame/3030-1
                string url = $"https://localhost:7250/V1/PublicGameInformation/GiantbombGame/{giantbombGuid}";

                using var client = new HttpClient();
                var response = await client.GetAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();

                var giantbombResult = JsonConvert.DeserializeObject<V2Result<GiantbombGameData>>(responseString);
                // Debug.WriteLine("\n" + giantbombResult.Value.ToString());
                if (giantbombResult.Value == null) { 
                    return BadRequest(new V2Result<V2VideoGameInformation>(giantbombResult.Errors[0].ToString()));
                }

                if (giantbombResult.Errors.IsNullOrEmpty()) {
                    // V2GiantbombGameData? gameData = JsonConvert.DeserializeObject<V2GiantbombGameData>(giantbombResult.Value.ToString());
                    var gamePost = new V2PostVideoGameInformation()
                    {
                        GameTitle = giantbombResult.Value.results.name,
                        GameDescription = giantbombResult.Value.results.deck,
                        GiantbombGuid = giantbombResult.Value.results.guid,
                    };
                    var json = JsonConvert.SerializeObject(gamePost);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var postResponse = await client.PostAsync("https://localhost:7296/V2/VideoGameInformation", content);

                    var V2VideoGameInformation = new V2VideoGameInformation()
                    {
                        GameTitle = giantbombResult.Value.results.name,
                        GameDescription = giantbombResult.Value.results.deck,
                        GiantbombGuid = giantbombResult.Value.results.guid,
                    };

                    var result = new V2Result<V2VideoGameInformation>(V2VideoGameInformation);

                    return Ok(result);
                }

                return BadRequest(new V2Result<V2VideoGameInformation>("Unknown error"));
                
            }
        }

        /// <summary>
        /// Adds a videogame to the database.
        /// </summary>
        /// <param name="gamePost">An object with the following fields: GameTitle, GameDescription, GiantbombGuid.</param>
        /// <returns>An object with the result of the POST attempt.</returns>
        [HttpPost("")]
        [ProducesResponseType(typeof(V2Result<V2VideoGameInformation>), 200)]
        [ProducesResponseType(typeof(V2Result<V2VideoGameInformation>), 400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<V2Result<V2VideoGameInformation>>> CreateGame(V2PostVideoGameInformation gamePost)
        {
            //var dbContext = new VideoGameDbContext();

            var videoGameInformation = new VideoGameInformation()
            {
                GameTitle = gamePost.GameTitle,
                GameDescription = gamePost.GameDescription,
                GiantbombGuid = gamePost.GiantbombGuid,
            };

            // Might come back to this version later:
            //Type gameType = videoGameInformation.GetType();
            //foreach (PropertyInfo prop in gameType.GetProperties())
            //{
            //    object value = prop.GetValue(videoGameInformation);
            //    if (value == null)
            //    {
            //        return BadRequest(new V2Result<V2GiantbombGameData>($"You need to have the field '{prop.Name}' filled in correctly"));
            //    }
            //    else if (value.GetType() != prop.PropertyType)
            //    {
            //        return BadRequest(new V2Result<V2GiantbombGameData>($"You have used the wrong datatype. you used`{prop.PropertyType} when you should have used '{value.GetType()}`."));
            //    }
            //};

            if (gamePost.GameTitle.Length >= 200)
            {
                return BadRequest(new V2Result<V2VideoGameInformation>("Game title is too long. Max is 200 characters."));
            }

            if (gamePost.GiantbombGuid.Length >= 11)
            {
                return BadRequest(new V2Result<V2VideoGameInformation>("Giantbomb guid is too long. Max is 11 characters. It usually looks like this: 3030-123"));
            }

            if (gamePost.GameDescription != null)
            {
                if (gamePost.GameDescription.Length >= 450)
                {
                    return BadRequest(new V2Result<V2VideoGameInformation>("Game description is too long. Max is 450 characters."));
                }
            }



            _db.VideoGameInformation.Add(videoGameInformation);
            await _db.SaveChangesAsync();


            var result = new V2Result<V2VideoGameInformation>(new V2VideoGameInformation()
            {
                Id = videoGameInformation.GameId,
                GameTitle = videoGameInformation.GameTitle,
                GiantbombGuid = videoGameInformation.GiantbombGuid,
                GameDescription = videoGameInformation.GameDescription
            });

            return Ok(result);
        }

 

    }
}