using Microsoft.AspNetCore.Http;

namespace HIOF.GamingSocial.GameInformation.Models.V2;

public class V2GamesWithUsers
{
    public Guid ProfileId { get; set; }

    public List<int> GameIds { get; set; }

    public V2GamesWithUsers()
    {

    }

}
