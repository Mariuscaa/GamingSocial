using System.Text.Json.Serialization;

namespace HIOF.GamingSocial.GameInformation.Models.V2
{
    public class V2VideoGameInformation
    {
        public int Id { get; set; }
        public string GameTitle { get; set; }
        public string GiantbombGuid { get; set; }
        public string GameDescription { get; set; }
    }

    /* Our chosen results
    public class Results
    {
        public int id { get; set; }
        public string aliases { get; set; }
        public string api_detail_url { get; set; }
        public string date_last_updated { get; set; }
        public string deck { get; set; }
        public string description { get; set; }
        public string expected_release_year { get; set; }
        public string guid { get; set; }
        public Image image { get; set; }
        public string name { get; set; }
        public int number_of_user_reviews { get; set; }
        public string original_release_date { get; set; }
        public Platform[] platforms { get; set; }
        public string site_detail_url { get; set; }
        public Developer[] developers { get; set; }
        public Franchise[] franchises { get; set; }
        public Genre[] genres { get; set; }
        public Publisher[] publishers { get; set; }
        public Theme[] themes { get; set; }
    }

    public class Image
    {
        public int id { get; set; }
        public string original_url { get; set; }
    }

    public class Platform
    {
        public int id { get; set; }
        public string api_detail_url { get; set; }
        public string name { get; set; }
        public string site_detail_url { get; set; }
        public string abbreviation { get; set; }
    }

    public class Developer
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


    public class Publisher
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
    }*/

}
