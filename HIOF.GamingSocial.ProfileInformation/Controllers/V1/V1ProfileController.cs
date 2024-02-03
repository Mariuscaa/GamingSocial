using Catel.Data;
using HIOF.GamingSocial.ProfileInformation.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HIOF.GamingSocial.ProfileInformation.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HIOF.GamingSocial.ProfileInformation.Controllers.V1;

/// <summary>
/// Handles profile information.
/// </summary>
[ApiController]
[Route("V1/Profile")]
public class V1ProfileController : ControllerBase
{

    private readonly ILogger<V1ProfileController> _logger;
    private readonly ProfileInformationDbContext _db;
    public V1ProfileController(ILogger<V1ProfileController> logger, ProfileInformationDbContext db)
    {
        _logger = logger;
        _db = db;
    }
    
    /// <summary>
    /// Gets all profiles from the local database.
    /// </summary>
    /// <returns>A list of profiles, each with the following properties: Id, Name, Bio, Country, Age, PhotoUrl.</returns>
    [HttpGet("MultipleProfiles/{nameSearch}")]
    [ProducesResponseType(typeof(V1Result<V1Profile>), 200)]
    [ProducesResponseType(typeof(V1Result<V1Profile>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V1Result<IEnumerable<V1Profile>>>> GetProfiles(string nameSearch)
    {
        

        var responseProfiles = await _db.Profile
            .Where(profile => profile.Name.Contains(nameSearch))
            .Select(profile => new V1Profile
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
            return BadRequest(new V1Result<IEnumerable<V1Profile>>($"No profiles found with the search`{nameSearch}`"));
        }

        return new V1Result<IEnumerable<V1Profile>>(responseProfiles);
    }

    /// <summary>
    /// Gets all profiles from the local database.
    /// </summary>
    /// <returns>A list of profiles, each with the following properties: Id, Name, Bio, Country, Age, PhotoUrl.</returns>
    [HttpGet("AllProfiles")]
    [ProducesResponseType(typeof(V1Result<V1Profile>), 200)]
    [ProducesResponseType(typeof(V1Result<V1Profile>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V1Result<IEnumerable<V1Profile>>>> GetAllProfiles()
    {
        

        var responseProfiles = await _db.Profile
            .Select(profile => new V1Profile
            {
                ProfileGuid = profile.ProfileGuid,
                Name = profile.Name,
                Bio = profile.Bio,
                Country = profile.Country,
                Age = profile.Age,
                PhotoUrl = profile.PhotoUrl
            })
            .ToListAsync();

        if (responseProfiles.Count == 0) 
        {
            return BadRequest(new V1Result<V1Profile>($"There are no users in the database."));
        }

        return new V1Result<IEnumerable<V1Profile>>(responseProfiles);
    }

    

    /// <summary>
    /// Gets information about a single profile from the local database.
    /// </summary>
    /// <param name="Id">A unique GUID for a profile.</param>
    /// <returns>A single profile with the following properties: Id, Name, Bio, Country, Age, PhotoUrl.</returns>
    [HttpGet("{profileId}")]
    [ProducesResponseType(typeof(V1Result<V1Profile>), 200)]
    [ProducesResponseType(typeof(V1Result<V1Profile>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V1Result<V1Profile>>> GetSingleProfile(Guid profileId)
    {
        
        var responseProfile = await _db.Profile
            .Where(profile => profile.ProfileGuid == profileId)
            .Select(profile => new V1Profile
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
            return BadRequest(new V1Result<V1Profile>($"No profiles found with the search`{profileId}`"));
        }
     

        return new V1Result<V1Profile>(responseProfile);
    }

    /// <summary>
    /// Created a new profile in the local database.
    /// </summary>
    /// <param name="postProfile">An object with the following properties: Id, Name, Bio, Country, Age, PhotoUrl.</param>
    /// <returns>An object with the result of the POST attempt.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(V1Result<V1Profile>), 200)]
    [ProducesResponseType(typeof(V1Result<V1Profile>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<V1Result<V1Profile>>> CreateProfile(V1PostProfile? postProfile)
    {
        
        var profile = new Profile
        {
            ProfileGuid = Guid.NewGuid(),
            Name = postProfile.Name,
            Bio = postProfile.Bio,  
            Country = postProfile.Country,
            Age = (int)postProfile.Age,
            PhotoUrl = postProfile.PhotoUrl,
        };
        Type profiletype = profile.GetType();
  
        foreach (PropertyInfo prop in profiletype.GetProperties())
        {
            object value = prop.GetValue(profile);
            if (value == null) 
            {
                return BadRequest(new V1Result<V1Profile>($"You need to have this {prop.Name} filled in correctly"));
            }
            // Den under fungerer ikke, husk å spør på¨øvingstime
            else if (value.GetType() != prop.PropertyType)
            {
                return BadRequest(new V1Result<V1Profile>($"You have used the wrong datatype. you used`{prop.PropertyType} when you should have used {value.GetType()}`"));
            }
        };

        if (postProfile.Name.Length > 50)
        {
            return BadRequest(new V1Result<V1Profile>($"You have exceeded the character limit!" +
                $"You cannot exceed 50 characters"));
        }
        else if (postProfile.Bio.Length > 500)
        {
            return BadRequest(new V1Result<V1Profile>($"You have exceeded the character limit!" +
                $"You cannot exceed 500 characters"));
        }
        else if (postProfile.Country.Length > 56)
        {
            return BadRequest(new V1Result<V1Profile>($"You have exceeded the character limit!" +
                $"You cannot exceed 56 characters"));
        }
        else if (postProfile.Age.ToString().Length > 3)
        {
            return BadRequest(new V1Result<V1Profile>($"You have exceeded the character limit!" +
                $"You cannot exceed 3 characters"));
        }
        else if (postProfile.PhotoUrl.Length > 2048)
        {
            return BadRequest(new V1Result<V1Profile>($"You have exceeded the character limit!" +
                $"You cannot exceed 2048 characters"));
        }
        _db.Profile.Add(profile);
        await _db.SaveChangesAsync();
        
        

        var result = new V1Result<V1Profile>(new V1Profile
        {
            ProfileGuid = profile.ProfileGuid,
            Name = postProfile.Name,
            Bio = postProfile.Bio,
            Country = postProfile.Country,
            Age = (int)postProfile.Age,
            PhotoUrl = postProfile.PhotoUrl,
        });

        return result;
    }

}