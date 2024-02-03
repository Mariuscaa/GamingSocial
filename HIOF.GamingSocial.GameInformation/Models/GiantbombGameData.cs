namespace HIOF.GamingSocial.GameInformation.Models;

/// <summary>
/// Old model for Giantbomb data. Kept it to allow for adding games outside of steam in the future.
/// </summary>
public class GiantbombGameData
{
    public string error { get; set; }
    public int limit { get; set; }
    public int offset { get; set; }
    public int number_of_page_results { get; set; }
    public int number_of_total_results { get; set; }
    public int status_code { get; set; }
    public Results results { get; set; }
    public string version { get; set; }

    public 
        GiantbombGameData()
    {
    }
}

public class Results
{
    public string aliases { get; set; }
    public string api_detail_url { get; set; }
    public string date_added { get; set; }
    public string date_last_updated { get; set; }
    public string deck { get; set; }
    public string description { get; set; }
    public string expected_release_day { get; set; }
    public string expected_release_month { get; set; }
    public string expected_release_quarter { get; set; }
    public string expected_release_year { get; set; }
    public string guid { get; set; }
    public int id { get; set; }
    public Image image { get; set; }
    public Image_Tags[] image_tags { get; set; }
    public string name { get; set; }
    public int number_of_user_reviews { get; set; }
    public Original_Game_Rating[] original_game_rating { get; set; }
    public string original_release_date { get; set; }
    public Platform[] platforms { get; set; }
    public string site_detail_url { get; set; }
    public Image1[] images { get; set; }
    public object[] videos { get; set; }
    public object characters { get; set; }
    public Concept[] concepts { get; set; }
    public Developer[] developers { get; set; }
    public object first_appearance_characters { get; set; }
    public First_Appearance_Concepts[] first_appearance_concepts { get; set; }
    public First_Appearance_Locations[] first_appearance_locations { get; set; }
    public First_Appearance_Objects[] first_appearance_objects { get; set; }
    public First_Appearance_People[] first_appearance_people { get; set; }
    public Franchise[] franchises { get; set; }
    public Genre[] genres { get; set; }
    public object killed_characters { get; set; }
    public Location[] locations { get; set; }
    public Object[] objects { get; set; }
    public Person[] people { get; set; }
    public Publisher[] publishers { get; set; }
    public Release[] releases { get; set; }
    public Similar_Games[] similar_games { get; set; }
    public Theme[] themes { get; set; }
}

public class Image
{
    public string icon_url { get; set; }
    public string medium_url { get; set; }
    public string screen_url { get; set; }
    public string screen_large_url { get; set; }
    public string small_url { get; set; }
    public string super_url { get; set; }
    public string thumb_url { get; set; }
    public string tiny_url { get; set; }
    public string original_url { get; set; }
    public string image_tags { get; set; }
}

public class Image_Tags
{
    public string api_detail_url { get; set; }
    public string name { get; set; }
    public int total { get; set; }
}

public class Original_Game_Rating
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
}

public class Platform
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string site_detail_url { get; set; }
    public string abbreviation { get; set; }
}

public class Image1
{
    public string icon_url { get; set; }
    public string medium_url { get; set; }
    public string screen_url { get; set; }
    public string small_url { get; set; }
    public string super_url { get; set; }
    public string thumb_url { get; set; }
    public string tiny_url { get; set; }
    public string original { get; set; }
    public string tags { get; set; }
}

public class Concept
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string site_detail_url { get; set; }
}

public class Developer
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string site_detail_url { get; set; }
}

public class First_Appearance_Concepts
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string site_detail_url { get; set; }
}

public class First_Appearance_Locations
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string site_detail_url { get; set; }
}

public class First_Appearance_Objects
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string site_detail_url { get; set; }
}

public class First_Appearance_People
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string site_detail_url { get; set; }
}

public class Franchise
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string site_detail_url { get; set; }
}

public class Genre
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string site_detail_url { get; set; }
}

public class Location
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string site_detail_url { get; set; }
}

public class Object
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string site_detail_url { get; set; }
}

public class Person
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string site_detail_url { get; set; }
}

public class Publisher
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string site_detail_url { get; set; }
}

public class Release
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string site_detail_url { get; set; }
}

public class Similar_Games
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string site_detail_url { get; set; }
}

public class Theme
{
    public string api_detail_url { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string site_detail_url { get; set; }
}
