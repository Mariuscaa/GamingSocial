using HIOF.GamingSocial.ProfileInformation.Model.V3;
using HIOF.GamingSocial.ProfileInformation.Model.External;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HIOF.GamingSocial.ProfileInformation.Data;
using System.Text.Json;

namespace HIOF.GamingSocial.ProfileInformation.Controllers.V3;

/// <summary>
/// Controller that handles creation, editing, deleting and retrieval of profiles
/// </summary>
[ApiController]
[Route("V3/Profile")]
public class V3ProfileController : ControllerBase
{
    private readonly ILogger<V3ProfileController> _logger;
    private readonly ProfileInformationDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="V3ProfileController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="db">The database context.</param>
    public V3ProfileController(ILogger<V3ProfileController> logger, ProfileInformationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Creates a new profile and saves it in the database.
    /// </summary>
    /// <param name="postProfile">An object with the following properties: Name, Bio, Country, Age, PhotoUrl (optional).</param>
    /// <returns>Returns the result of the post, if successfull it returns what was saved to the database</returns>
    [HttpPost]
    [ProducesResponseType(typeof(V3Result<V3Profile>), 200)]
    [ProducesResponseType(typeof(V3Result<V3Profile>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3Profile>>> CreateProfile(V3PostProfile postProfile)
    {
        if (postProfile.UserName.Length > 20)
        {
            _logger.LogWarning($"User tried to create a profile with a username that was too long. " +
                               $"Username: {postProfile.UserName}");
            return BadRequest(new V3Result<V3Profile>($"You have exceeded the character limit for the UserName! " +
                                                      $"You cannot exceed 20 characters"));
        }

        if (postProfile.Name.Length > 50)
        {
            _logger.LogWarning($"User tried to create a profile with a name that was too long. " +
                $"Name: {postProfile.Name}");
            return BadRequest(new V3Result<V3Profile>($"You have exceeded the character limit for the Name! " +
                                                      $"You cannot exceed 50 characters"));
        }

        if (postProfile.Bio.Length > 500)
        {
            _logger.LogWarning($"User tried to create a profile with a bio that was too long. " +
                               $"Bio: {postProfile.Bio}");
            return BadRequest(new V3Result<V3Profile>($"You have exceeded the character limit for the Bio! " +
                                                      $"You cannot exceed 500 characters"));
        }

        if (postProfile.Country.Length > 56)
        {
            _logger.LogWarning($"User tried to create a profile with a country that was too long. " +
                               $"Country: {postProfile.Country}");
            return BadRequest(new V3Result<V3Profile>($"You have exceeded the character limit for the Country! " +
                                                      $"You cannot exceed 56 characters"));
        }

        if (postProfile.Age.ToString().Length > 3)
        {
            _logger.LogWarning($"User tried to create a profile with an age that was too long. " +
                               $"Age: {postProfile.Age}");
            return BadRequest(new V3Result<V3Profile>($"You have exceeded the character limit for the Age!" +
                                                      $"You cannot exceed 3 characters"));
        }

        if (!postProfile.PhotoUrl.IsNullOrEmpty())
        {
            if (postProfile.PhotoUrl.Length > 2048)
            {
                _logger.LogWarning($"User tried to create a profile with a photo url that was too long. " +
                                   $"PhotoUrl: {postProfile.PhotoUrl}");
                return BadRequest(new V3Result<V3Profile>($"You have exceeded the character limit for the PhotoUrl!" +
                                                          $"You cannot exceed 2048 characters"));
            }
        }

        var CheckforProfiles = await _db.Profile
            .Where(profile => profile.UserName == postProfile.UserName)
            .Select(profile => new V3Profile
            {
                ProfileGuid = profile.ProfileGuid,
                UserName = profile.UserName,
                Name = profile.Name,
                Bio = profile.Bio,
                Country = profile.Country,
                Age = profile.Age,
                PhotoUrl = profile.PhotoUrl
            })
            .SingleOrDefaultAsync();

        if (CheckforProfiles != null)
        {
            _logger.LogWarning($"User already exist `{postProfile.UserName}`");
            return BadRequest(new V3Result<V3Profile>($"Could not create profile because it already exists.\n You have to choose a different username. `{postProfile.UserName}`"));
        }
        



        var profile = new Profile
        {
            ProfileGuid = Guid.NewGuid(),
            UserName = postProfile.UserName.Trim(),
            Name = postProfile.Name.Trim(),
            Bio = postProfile.Bio.Trim(),
            Country = postProfile.Country.Trim(),
            Age = postProfile.Age,
            PhotoUrl = postProfile.PhotoUrl,
        };
        _db.Profile.Add(profile);
        await _db.SaveChangesAsync();

        var result = new V3Result<V3Profile>(new V3Profile
        {
            ProfileGuid = profile.ProfileGuid,
            UserName = profile.UserName.Trim(),
            Name = profile.Name.Trim(),
            Bio = profile.Bio.Trim(),
            Country = profile.Country.Trim(),
            Age = profile.Age,
            PhotoUrl = profile.PhotoUrl,
        });

        _logger.LogInformation($"Created profile with Guid: {profile.ProfileGuid}");
        return Ok(result);
    }


    /// <summary>
    /// Gets multiple profiles from the database. By default it gets all users but can be filtered with parameters.
    /// </summary>
    /// <param name="nameSearch">a string referring to the name of the user.</param>
    /// <param name="userName">a string referring to the username of the user.</param>
    /// <returns>A list of profiles, each with the following properties: Guid, Name, Bio, Country, Age, PhotoUrl.</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(V3Result<V3Profile>), 200)]
    [ProducesResponseType(typeof(V3Result<V3Profile>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<IEnumerable<V3Profile>>>> Get(string? nameSearch = null, string? userName = null)
    {
        if (nameSearch == null && userName == null)
        {
            var responseProfiles = await _db.Profile
            .Select(profile => new V3Profile
            {
                ProfileGuid = profile.ProfileGuid,
                UserName = profile.UserName,
                Name = profile.Name,
                Bio = profile.Bio,
                Country = profile.Country,
                Age = profile.Age,
                PhotoUrl = profile.PhotoUrl
            })
            .ToListAsync();

            return Ok(new V3Result<IEnumerable<V3Profile>>(responseProfiles));
        }
        else if (nameSearch != null)
        {
            var responseProfiles = await _db.Profile
                .Where(profile => profile.Name.Contains(nameSearch))
                .Select(profile => new V3Profile
                {
                    ProfileGuid = profile.ProfileGuid,
                    UserName = profile.UserName,
                    Name = profile.Name,
                    Bio = profile.Bio,
                    Country = profile.Country,
                    Age = profile.Age,
                    PhotoUrl = profile.PhotoUrl
                })
                .ToListAsync();

            if (!responseProfiles.Any())
            {
                _logger.LogWarning($"No profiles found with the search `{nameSearch}`");
                return NotFound(new V3Result<IEnumerable<V3Profile>>($"No profiles found with the search `{nameSearch}`"));
            }

            return new V3Result<IEnumerable<V3Profile>>(responseProfiles);
        }
        else if (userName != null)
        {
            var responseProfiles = await _db.Profile
                .Where(profile => profile.UserName.Contains(userName))
                .Select(profile => new V3Profile
                {
                    ProfileGuid = profile.ProfileGuid,
                    UserName = profile.UserName,
                    Name = profile.Name,
                    Bio = profile.Bio,
                    Country = profile.Country,
                    Age = profile.Age,
                    PhotoUrl = profile.PhotoUrl
                })
                .ToListAsync();
            if (!responseProfiles.Any())
            {
                _logger.LogWarning($"No profiles found with the search `{userName}`.");
                return NotFound(new V3Result<IEnumerable<V3Profile>>($"No profiles found with the search `{userName}`."));
            }

            return Ok(new V3Result<IEnumerable<V3Profile>>(responseProfiles));
        }
        else
        {
            _logger.LogWarning($"Cannot be called with both nameSearch and userName (yet).");
            return NotFound(new V3Result<IEnumerable<V3Profile>>($"Cannot be called with both nameSearch and userName (yet)."));
        }
    }



    /// <summary>
    /// Gets information about a single profile from the local database.
    /// </summary>
    /// <param name="UserName">a unique username</param>
    /// <returns>A single profile with the following properties: ProfileGuid, Name, Bio, Country, Age, PhotoUrl.</returns>
    [HttpGet("UserName")]
    [ProducesResponseType(typeof(V3Result<V3Profile>), 200)]
    [ProducesResponseType(typeof(V3Result<V3Profile>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3Profile>>> GetSingleProfileWithUsername(string UserName)
    {
        var responseProfile = await _db.Profile
            .Where(profile => profile.UserName == UserName)
            .Select(profile => new V3Profile
            {
                ProfileGuid = profile.ProfileGuid,
                UserName = profile.UserName,
                Name = profile.Name,
                Bio = profile.Bio,
                Country = profile.Country,
                Age = profile.Age,
                PhotoUrl = profile.PhotoUrl
            })
            .SingleOrDefaultAsync();

        

        if (responseProfile == null)
        {
            _logger.LogWarning($"No profile found with the search `{UserName}`");
            return NotFound(new V3Result<V3Profile>($"No profile found with the search `{UserName}`"));
        }

        return new V3Result<V3Profile>(responseProfile);
    }


    /// <summary>
    /// Gets a users profile from the database by the profileguid
    /// </summary>
    /// <param name="profileGuid">a unique Guid</param>
    /// <returns>Only one user with the different user atributes</returns>
    [HttpGet("{profileGuid}")]
    [ProducesResponseType(typeof(V3Result<V3Profile>), 200)]
    [ProducesResponseType(typeof(V3Result<V3Profile>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3Profile>>> GetSingleProfile(Guid profileGuid)
    {
        var responseProfile = await _db.Profile
            .Where(profile => profile.ProfileGuid == profileGuid)
            .Select(profile => new V3Profile
            {
                ProfileGuid = profile.ProfileGuid,
                UserName = profile.UserName,
                Name = profile.Name,
                Bio = profile.Bio,
                Country = profile.Country,
                Age = profile.Age,
                PhotoUrl = profile.PhotoUrl
            })
            .SingleOrDefaultAsync();

        if (responseProfile == null)
        {
            _logger.LogWarning($"No profile found with the search `{profileGuid}`");
            return NotFound(new V3Result<V3Profile>($"No profile found with the search `{profileGuid}`"));
        }

        return Ok(new V3Result<V3Profile>(responseProfile));
    }

    /// <summary>
    /// The main get for profile discovery with optional parameters. Gets a completely random user when no parameters are given.
    /// </summary>
    /// <param name="profileGuid">The Guid for the profile making the discovery request.</param>
    /// <param name="gamesInCommon">A boolean value for whether the result should be filtered by games that the profile has in common with others.
    ///                                 Note that this parameter is irrelevant if either highlyRatedGames or specificGame is provided.</param>
    /// <param name="highlyRatedGamesInCommon">If set to true, it filters for users with common games with a personal rating of 80+.</param>
    /// <param name="specificGame">An int for an ID of a game to filter by. Can be combined with highlyRatedGames to only find people who have rated the specific game highly.</param>
    /// <returns>A suggestion in the form of a single Guid for another profile.</returns>
    [HttpGet("ProfileDiscovery")]
    [ProducesResponseType(typeof(V3Result<V3Profile>), 200)]
    [ProducesResponseType(typeof(V3Result<V3Profile>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3Profile>>> GetProfile(Guid profileGuid, bool? gamesInCommon = false,
        bool? highlyRatedGamesInCommon = false, int? specificGame = null)
    {
        var profileResult = new V3Result<List<V3Profile>>() { Value = new List<V3Profile>() };
        if (highlyRatedGamesInCommon != false && specificGame != null)
        {
            string url = $"https://localhost:7296/V3/GameCollection?gameId={specificGame}";

            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var profilesWithCommonGameResult = JsonSerializer.Deserialize<V3Result<List<V3ProfileWithGames>>>(responseString, options);

            HashSet<Guid> profileGuidsWithCommonGames = new HashSet<Guid>();

            foreach (var user in profilesWithCommonGameResult.Value)
            {
                if (user.GamesCollection[0].GameRating != null)
                {
                    if (user.GamesCollection[0].GameRating > 80)
                    {
                        profileGuidsWithCommonGames.Add(user.ProfileGuid);
                    }
                }
            }

            // Calls ProfileInformation to make sure that the users exist.
            foreach (var g in profileGuidsWithCommonGames)
            {
                url = $"https://localhost:7087/V3/Profile/{g}";
                response = await client.GetAsync(url);
                responseString = await response.Content.ReadAsStringAsync();

                var singleProfileResult = JsonSerializer.Deserialize<V3Result<V3Profile>>(responseString, options);
                if (singleProfileResult.Value != null)
                {
                    var singleProfile = singleProfileResult.Value;
                    profileResult.Value.Add(singleProfile);
                }
            }

        }
        if (highlyRatedGamesInCommon != false && specificGame == null)
        {
            string url = $"https://localhost:7296/V3/GameCollection/Profile?profileGuid={profileGuid}&onlyRatedGames=true";

            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var profileHasGamesResult = JsonSerializer.Deserialize<V3Result<V3ProfileWithGames>>(responseString, options);

            profileHasGamesResult.Value.GamesCollection.RemoveAll(x => x.GameRating < 80);

            // HashSet is used here to avoid adding duplicate profileGuids when profiles have multiple common games.
            HashSet<Guid> profileGuidsWithCommonGames = new HashSet<Guid>();

            foreach (var game in profileHasGamesResult.Value.GamesCollection)
            {
                url = $"https://localhost:7296/V3/GameCollection?gameId={game.GameId}";
                response = await client.GetAsync(url);
                responseString = await response.Content.ReadAsStringAsync();
                var profilesWithCommonGameResult = JsonSerializer.Deserialize<V3Result<List<V3ProfileWithGames>>>(responseString, options);

                foreach (var user in profilesWithCommonGameResult.Value)
                {
                    if (user.GamesCollection[0].GameRating != null)
                    {
                        if (user.GamesCollection[0].GameRating > 80)
                        {
                            profileGuidsWithCommonGames.Add(user.ProfileGuid);
                        }
                    }
                }
            }
            // Calls ProfileInformation to make sure that the users exist.
            foreach (var g in profileGuidsWithCommonGames)
            {
                url = $"https://localhost:7087/V3/Profile/{g}";
                response = await client.GetAsync(url);
                responseString = await response.Content.ReadAsStringAsync();

                var singleProfileResult = JsonSerializer.Deserialize<V3Result<V3Profile>>(responseString, options);
                if (singleProfileResult.Value != null)
                {
                    var singleProfile = singleProfileResult.Value;
                    profileResult.Value.Add(singleProfile);
                }
            }
        }

        if (gamesInCommon != false && highlyRatedGamesInCommon == false && specificGame == null)
        {
            string url = $"https://localhost:7296/V3/GameCollection/Profile?profileGuid={profileGuid}";
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var profileHasGames = JsonSerializer.Deserialize<V3Result<V3ProfileWithGames>>(responseString, options);

            // HashSet is used here to avoid adding duplicate profileGuids when profiles have multiple common games.
            HashSet<Guid> profileGuidsWithCommonGames = new HashSet<Guid>();

            foreach (var game in profileHasGames.Value.GamesCollection)
            {
                url = $"https://localhost:7296/V3/GameCollection?gameId={game.GameId}";

                response = await client.GetAsync(url);
                responseString = await response.Content.ReadAsStringAsync();

                var profilesWithCommonGameResult = JsonSerializer.Deserialize<V3Result<List<V3ProfileWithGames>>>(responseString, options);
                foreach (var user in profilesWithCommonGameResult.Value)
                {
                    profileGuidsWithCommonGames.Add(user.ProfileGuid);
                }
            }

            // Calls ProfileInformation to make sure that the users exist.
            foreach (var g in profileGuidsWithCommonGames)
            {
                url = $"https://localhost:7087/V3/Profile/{g}";
                response = await client.GetAsync(url);
                responseString = await response.Content.ReadAsStringAsync();

                var singleProfileResult = JsonSerializer.Deserialize<V3Result<V3Profile>>(responseString, options);
                if (singleProfileResult.Value != null)
                {
                    var singleProfile = singleProfileResult.Value;
                    profileResult.Value.Add(singleProfile);
                }
            }
        }

        if (specificGame != null && highlyRatedGamesInCommon == false)
        {
            string url = $"https://localhost:7296/V3/GameCollection?gameId={specificGame}";

            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var profilesWithCommonGameResult = JsonSerializer.Deserialize<V3Result<List<V3ProfileWithGames>>>(responseString, options);

            // Calls ProfileInformation to make sure that the users exist.
            if (profilesWithCommonGameResult.Value != null)
            {
                foreach (var pg in profilesWithCommonGameResult.Value)
                {
                    url = $"https://localhost:7087/V3/Profile/{pg.ProfileGuid}";
                    response = await client.GetAsync(url);
                    responseString = await response.Content.ReadAsStringAsync();

                    var singleProfileResult = JsonSerializer.Deserialize<V3Result<V3Profile>>(responseString, options);
                    if (singleProfileResult.Value != null)
                    {
                        var singleProfile = singleProfileResult.Value;
                        profileResult.Value.Add(singleProfile);
                    }
                }
            }
        }

        if (gamesInCommon == false && highlyRatedGamesInCommon == false && specificGame == null)
        {
            string url = $"https://localhost:7087/V3/Profile";
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            profileResult = JsonSerializer.Deserialize<V3Result<List<V3Profile>>>(responseString, options);
        }


        profileResult.Value.RemoveAll(profile => profile.ProfileGuid == profileGuid);
        if (profileResult.Value.Count == 0)
        {
            _logger.LogWarning($"Could not find any other users with the given search parameters.");
            return NotFound(new V3Result<V3Profile>("Could not find any other users with the given search parameters."));
        }
        Random random = new Random();
        int randomIndex = random.Next(profileResult.Value.Count);

        return Ok(new V3Result<V3Profile>(profileResult.Value[randomIndex]));
    }


    /// <summary>
    /// Updates a single profile from the database.
    /// </summary>
    /// <param name="profileGuid">The Guid of the profile which shall be updated.</param>
    /// <param name="patchProfile">An object containing the properties to be updated. Any property set to null will not be updated.</param>
    /// <returns>Returns the updated object.</returns>
    [HttpPatch("{profileGuid}")]
    [ProducesResponseType(typeof(V3Result<V3Profile>), 200)]
    [ProducesResponseType(typeof(V3Result<V3Profile>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3Profile>>> Patch(Guid profileGuid, V3PatchProfile patchProfile)
    {

        var existingProfile = await _db.Profile
            .SingleOrDefaultAsync(profile => profile.ProfileGuid == profileGuid);

        var existingProfilewithSameName = await _db.Profile
            .SingleOrDefaultAsync(profile => profile.UserName == patchProfile.UserName && profile.ProfileGuid != profileGuid);


        if (existingProfile == null)
        {
            _logger.LogWarning($"No profile found with the search `{profileGuid}`");
            return BadRequest(new V3Result<V3Profile>($"No profile found with the search `{profileGuid}`"));
        }



        if (patchProfile.UserName != null)
        {
            if (existingProfilewithSameName != null)
            {
                _logger.LogWarning($"User already exist `{patchProfile.UserName}`");
                return BadRequest(new V3Result<V3Profile>($"Could not create profile because it already exists.\n You have to choose a different username. `{patchProfile.UserName}`"));
            }
            if (patchProfile.UserName.Length > 20)
            {
                _logger.LogWarning($"User tried to update the profile with a username that was too long. " +
                                   $"Username: {patchProfile.UserName}");
                return BadRequest(new V3Result<V3Profile>($"You have exceeded the character limit for the UserName! " +
                                                          $"You cannot exceed 20 characters"));
            }
            else
            {
                existingProfile.UserName = patchProfile.UserName;
            }
        }

        if (patchProfile.UserName == null)
        {
            patchProfile.UserName = existingProfile.UserName;
        }

        if (patchProfile.Name != null)
        {
            if (patchProfile.Name.Length > 50)
            {
                _logger.LogWarning($"User tried to update the profile with a name that was too long. " +
                    $"Name: {patchProfile.Name}");
                return BadRequest(new V3Result<V3Profile>($"You have exceeded the character limit for the Name! " +
                                                          $"You cannot exceed 50 characters"));
            }
            else
            {
                existingProfile.Name = patchProfile.Name;
            }
        }

        if (patchProfile.Name == null)
        {
            patchProfile.Name = existingProfile.Name;
        }

        if (patchProfile.Bio != null)
        {
            if (patchProfile.Bio.Length > 500)
            {
                _logger.LogWarning($"User tried to update the profile with a bio that was too long. " +
                                   $"Bio: {patchProfile.Bio}");
                return BadRequest(new V3Result<V3Profile>($"You have exceeded the character limit for the Bio! " +
                                                          $"You cannot exceed 500 characters"));
            }
            else
            {
                existingProfile.Bio = patchProfile.Bio;
            }
        }

        if (patchProfile.Bio == null)
        {
            patchProfile.Bio = existingProfile.Bio;
        }

        if (patchProfile.Country != null)
        {
            if (patchProfile.Country.Length > 56)
            {
                _logger.LogWarning($"User tried to update the profile with a country that was too long. " +
                                   $"Country: {patchProfile.Country}");
                return BadRequest(new V3Result<V3Profile>($"You have exceeded the character limit for the Country! " +
                                                          $"You cannot exceed 56 characters"));
            }
            else
            {
                existingProfile.Country = patchProfile.Country;
            }
            
        }

        if (patchProfile.Country == null)
        {
            patchProfile.Country = existingProfile.Country;
        }

        if (patchProfile.Age != null)
        {
            if (patchProfile.Age.ToString().Length > 3)
            {
                _logger.LogWarning($"User tried to update the profile with an age that was too long. " +
                                   $"Age: {patchProfile.Age}");
                return BadRequest(new V3Result<V3Profile>($"You have exceeded the character limit for the Age!" +
                                                          $"You cannot exceed 3 characters"));
            }
            else
            {
                existingProfile.Age = patchProfile.Age.Value;
            }
        }
        if (patchProfile.Age == null)
        {
            patchProfile.Age = existingProfile.Age;
        }

        if (patchProfile.PhotoUrl != null)
        {
            if (!patchProfile.PhotoUrl.IsNullOrEmpty())
            {
                if (patchProfile.PhotoUrl.Length > 2048)
                {
                    _logger.LogWarning($"User tried to update the profile with a photo url that was too long. " +
                                       $"PhotoUrl: {patchProfile.PhotoUrl}");
                    return BadRequest(new V3Result<V3Profile>($"You have exceeded the character limit for the PhotoUrl!" +
                                                              $"You cannot exceed 2048 characters"));
                }
                else
                {
                    existingProfile.PhotoUrl = patchProfile.PhotoUrl;
                }
            }
        }
        if (patchProfile.PhotoUrl == null)
        {
            patchProfile.PhotoUrl = existingProfile.PhotoUrl;
        }


            _db.Profile.Update(existingProfile);
        await _db.SaveChangesAsync();

        var updatedProfile = new V3Profile()
        {
            ProfileGuid = existingProfile.ProfileGuid,
            UserName = existingProfile.UserName,
            Name = existingProfile.Name,
            Bio = existingProfile.Bio,
            Country = existingProfile.Country,
            Age = existingProfile.Age,
            PhotoUrl = existingProfile.PhotoUrl
        };

        _logger.LogInformation($"Updated profile with id `{profileGuid}`");
        return Ok(new V3Result<V3Profile>(updatedProfile));
    }
    
    /// <summary>
    /// Deletes a single profile from the database.
    /// </summary>
    /// <param name="profileGuid">The Guid of the profile which shall be deleted.</param>
    /// <returns>Returns the deleted object.</returns>
    [HttpDelete("")]
    [ProducesResponseType(typeof(V3Result<V3Profile>), 200)]
    [ProducesResponseType(typeof(V3Result<V3Profile>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V3Result<V3Profile>>> Delete(Guid profileGuid)
    {
        try
        {
            var response = await _db.Profile
                .Where(profile => profile.ProfileGuid == profileGuid)
                .Select(profile => new Profile
                {
                    ProfileGuid = profile.ProfileGuid,
                    UserName = profile.UserName,
                    Name = profile.Name,
                    Bio = profile.Bio,
                    Country = profile.Country,
                    Age = profile.Age,
                    PhotoUrl = profile.PhotoUrl
                })
                .SingleAsync();

            _db.Profile.Remove(response);
            await _db.SaveChangesAsync();

            var deletedProfile = new V3Profile()
            {
                ProfileGuid = response.ProfileGuid,
                UserName = response.UserName,
                Name = response.Name,
                Bio = response.Bio,
                Country = response.Country,
                Age = response.Age,
                PhotoUrl = response.PhotoUrl
            };
            _logger.LogInformation($"Deleted profile with id `{profileGuid}`.");
            return Ok(new V3Result<V3Profile>(deletedProfile));
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Could not find any profile with a Guid equal to {profileGuid}.");
            return NotFound(new V3Result<V3Profile>("Could not find any profile with a Guid equal to " + profileGuid));
        }
    }
}