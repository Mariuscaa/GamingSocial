using HIOF.GamingSocial.ProfileInformation.Model;
using HIOF.GamingSocial.ProfileInformation.Model.V2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HIOF.GamingSocial.ProfileInformation.Data;
using System.Reflection;

namespace HIOF.GamingSocial.ProfileInformation.Controllers.V2;

/// <summary>
/// Handles profile information.
/// </summary>
[ApiController]
[Route("V2/Profile")]
public class V2ProfileController : ControllerBase
{

    private readonly ILogger<V2ProfileController> _logger;
    private readonly ProfileInformationDbContext _db;
    public V2ProfileController(ILogger<V2ProfileController> logger, ProfileInformationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>a
    /// Created a new profile in the local database.
    /// </summary>
    /// <param name="postProfile">An object with the following properties: Name, Bio, Country, Age, PhotoUrl (optional).</param>
    /// <returns>An object with the result of the POST attempt.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(V2Result<V2Profile>), 200)]
    [ProducesResponseType(typeof(V2Result<V2Profile>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V2Result<V2Profile>>> CreateProfile(V2PostProfile postProfile)
    {
        var profile = new Profile
        {
            ProfileGuid = Guid.NewGuid(),
            Name = postProfile.Name,
            Bio = postProfile.Bio,
            Country = postProfile.Country,
            Age = postProfile.Age,
            PhotoUrl = postProfile.PhotoUrl,
        };


        if (postProfile.Name.Length > 50)
        {
            return BadRequest(new V2Result<V2Profile>($"You have exceeded the character limit for the Name!" +
                $" You cannot exceed 50 characters"));
        }
        else if (postProfile.Name == " ")
        {
            return BadRequest(new V2Result<V2Profile>($"You have forgotten to input any characters for Name!" +
                $" You cannot input 0 characters"));
        }

        if (postProfile.Bio.Length > 500)
        {
            return BadRequest(new V2Result<V2Profile>($"You have exceeded the character limit for the Bio!" +
                $"You cannot exceed 500 characters"));
        }
        else if (postProfile.Bio == " ")
        {
            return BadRequest(new V2Result<V2Profile>($"You have forgotten to input any characters for Bio!" +
                $" You cannot input 0 characters"));
        }


        if (postProfile.Country.Length > 56)
        {
            return BadRequest(new V2Result<V2Profile>($"You have exceeded the character limit for the Country!" +
                $"You cannot exceed 56 characters"));
        }
        else if (postProfile.Country == " ")
        {
            return BadRequest(new V2Result<V2Profile>($"You have forgotten to input any characters for Country!" +
                $" You cannot input 0 characters"));
        }

        if (postProfile.Age.ToString().Length > 3)
        {
            return BadRequest(new V2Result<V2Profile>($"You have exceeded the character limit for the Age!" +
                $"You cannot exceed 3 characters"));
        }
        else if (postProfile.Age == 0)
        {
            return BadRequest(new V2Result<V2Profile>($"You have forgotten to input any characters for Age!" +
                $" You cannot input 0 characters"));
        }



        if (!postProfile.PhotoUrl.IsNullOrEmpty())
        {
            if (postProfile.PhotoUrl.Length > 2048)
            {
                return BadRequest(new V2Result<V2Profile>($"You have exceeded the character limit for the PhotoUrl!" +
                    $"You cannot exceed 2048 characters"));
            }
        }
        _db.Profile.Add(profile);
        await _db.SaveChangesAsync();

        var result = new V2Result<V2Profile>(new V2Profile
        {
            ProfileGuid = profile.ProfileGuid,
            Name = postProfile.Name,
            Bio = postProfile.Bio,
            Country = postProfile.Country,
            Age = postProfile.Age,
            PhotoUrl = postProfile.PhotoUrl,
        });

        return Ok(result);
    }

    /// <summary>
    /// Gets multiple profiles from the database. By default it gets all users but can be filtered with parameters.
    /// </summary>
    /// <param name="nameSearch">a string referring to the name of the user.</param>
    /// <returns>A list of profiles, each with the following properties: Guid, Name, Bio, Country, Age, PhotoUrl.</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(V2Result<V2Profile>), 200)]
    [ProducesResponseType(typeof(V2Result<V2Profile>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V2Result<IEnumerable<V2Profile>>>> Get(string? nameSearch = null)
    {
        if (nameSearch == null)
        {
            var responseProfiles = await _db.Profile
            .Select(profile => new V2Profile
            {
                ProfileGuid = profile.ProfileGuid,
                Name = profile.Name,
                Bio = profile.Bio,
                Country = profile.Country,
                Age = profile.Age,
                PhotoUrl = profile.PhotoUrl
            })
            .ToListAsync();

            return new V2Result<IEnumerable<V2Profile>>(responseProfiles);
        }

        else
        {
            var responseProfiles = await _db.Profile
                .Where(profile => profile.Name.Contains(nameSearch))
                .Select(profile => new V2Profile
                {
                    ProfileGuid = profile.ProfileGuid,
                    Name = profile.Name,
                    Bio = profile.Bio,
                    Country = profile.Country,
                    Age = profile.Age,
                    PhotoUrl = profile.PhotoUrl
                })
                .ToListAsync();

            if (!responseProfiles.Any())
            {
                return NotFound(new V2Result<IEnumerable<V2Profile>>($"No profiles found with the search `{nameSearch}`"));
            }

            return new V2Result<IEnumerable<V2Profile>>(responseProfiles);
        }
    }

    /// <summary>
    /// Gets information about a single profile from the local database.
    /// </summary>
    /// <param name="profileGuid">A unique GUID for a profile.</param>
    /// <returns>A single profile with the following properties: Id, Name, Bio, Country, Age, PhotoUrl.</returns>
    [HttpGet("{profileGuid}")]
    [ProducesResponseType(typeof(V2Result<V2Profile>), 200)]
    [ProducesResponseType(typeof(V2Result<V2Profile>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V2Result<V2Profile>>> GetSingleProfile(Guid profileGuid)
    {
        var responseProfile = await _db.Profile
            .Where(profile => profile.ProfileGuid == profileGuid)
            .Select(profile => new V2Profile
            {
                ProfileGuid = profile.ProfileGuid,
                Name = profile.Name,
                Bio = profile.Bio,
                Country = profile.Country,
                Age = profile.Age,
                PhotoUrl = profile.PhotoUrl
            })
            .SingleOrDefaultAsync();

        if (responseProfile == null)
        {
            return BadRequest(new V2Result<V2Profile>($"No profiles found with the search `{profileGuid}`"));
        }

        return new V2Result<V2Profile>(responseProfile);
    }
    
    /// <summary>
    /// Deletes a single profile from the database.
    /// </summary>
    /// <param name="profileGuid">The Guid of the profile which shall be deleted.</param>
    /// <returns>Returns the deleted object.</returns>
    [HttpDelete("")]
    [ProducesResponseType(typeof(V2Result<V2Profile>), 200)]
    [ProducesResponseType(typeof(V2Result<V2Profile>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V2Result<V2Profile>>> Delete(Guid profileGuid)
    {
        try
        {
            var response = await _db.Profile
                .Where(profile => profile.ProfileGuid == profileGuid)
                .Select(profile => new Profile
                {
                    ProfileGuid = profile.ProfileGuid,
                    Name = profile.Name,
                    Bio = profile.Bio,
                    Country = profile.Country,
                    PhotoUrl= profile.PhotoUrl
                })
                .SingleAsync();

            _db.Profile.Remove(response);
            await _db.SaveChangesAsync();

            var deletedProfile = new V2Profile()
            {
                ProfileGuid = response.ProfileGuid,
                Name = response.Name,
                Bio = response.Bio,
                Country = response.Country,
                PhotoUrl = response.PhotoUrl
            };

            return Ok(new V2Result<V2Profile>(deletedProfile));
        }
        catch (Exception ex)
        {
            return BadRequest(new V2Result<V2Profile>("Could not find any profile with a Guid equal to " + profileGuid));
        }
    }
}