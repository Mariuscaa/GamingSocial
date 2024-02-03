namespace HIOF.GamingSocial.GUI.Services;

public class CooldownService
{
    private DateTime lastUpdated = DateTime.MinValue;

    public bool CanUpdate()
    {
        return (DateTime.Now - lastUpdated).TotalMinutes >= 5;
    }

    public void Update()
    {
        lastUpdated = DateTime.Now;
    }
}
