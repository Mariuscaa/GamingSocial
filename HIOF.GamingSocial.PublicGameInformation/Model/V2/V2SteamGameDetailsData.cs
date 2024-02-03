namespace HIOF.GamingSocial.PublicGameInformation.Model.V2;

/// <summary>
/// The main class to use to deserialize responses from Steam's web API for single game details.
/// </summary>
public class V2SteamGameDetailsData
{
    public Dictionary<string, GameDetails> Games { get; set; }
}

/// <summary>
/// A class to represent the outermost object in the json response.
/// </summary>
public class GameDetails
{
    public bool success { get; set; }
    public Data data { get; set; }
}

/// <summary>
/// A class with all the possible data in a response from steam.
/// </summary>
public class Data
{
    public string type { get; set; }
    public string name { get; set; }
    public int steam_appid { get; set; }
    public int required_age { get; set; }
    public bool is_free { get; set; }
    public string detailed_description { get; set; }
    public string about_the_game { get; set; }
    public string short_description { get; set; }
    public string supported_languages { get; set; }
    public string header_image { get; set; }
    public string website { get; set; }
    public string legal_notice { get; set; }
    public Genre[] genres { get; set; }
    public Release_Date release_date { get; set; }
    public string background { get; set; }
    public string background_raw { get; set; }
}

public class Pc_Requirements
{
    public string minimum { get; set; }
    public string recommended { get; set; }
}

public class Mac_Requirements
{
    public string minimum { get; set; }
    public string recommended { get; set; }
}

public class Linux_Requirements
{
    public string? minimum { get; set; }
    public string? recommended { get; set; }
}

public class Release_Date
{
    public bool coming_soon { get; set; }
    public string date { get; set; }
}

public class Genre
{
    public string id { get; set; }
    public string description { get; set; }
}
