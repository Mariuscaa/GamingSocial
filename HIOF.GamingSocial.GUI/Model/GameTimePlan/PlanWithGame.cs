namespace HIOF.GamingSocial.GUI.Model;

public class PlanWithGame
{
    public V1GameTimePlan GameTimePlan { get; set; }
    public V3VideoGameInformation Game { get; set; }

    public PlanWithGame()
    {
    }

    public PlanWithGame(V1GameTimePlan gameTimePlan, V3VideoGameInformation game)
    {
        GameTimePlan = gameTimePlan;
        Game = game;
    }
}
