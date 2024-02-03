using System.Text.Json.Serialization;

namespace HIOF.GamingSocial.BackgroundJob.Models;

public class Result<T>
{
    public Result()
    {
    }
    public Result(string error)
    {
        Errors.Add(error);
    }
    public Result(T value)
    {
        Value = value;
    }

    public List<string> Errors { get; set; } = new List<string>();

    [JsonIgnore]
    public bool HasErrors => Errors.Any();

    public T Value { get; set; }

}
