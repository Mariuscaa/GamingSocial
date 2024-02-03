using System.Text.Json.Serialization;

namespace HIOF.GamingSocial.GameInformation.Models.V2
{
    public class V2Result<T>
    {
        public V2Result()
        {
        }

        public V2Result(string error)
        {
            Errors.Add(error);
        }
        public V2Result(T value)
        {
            Value = value;
        }

        public List<string> Errors { get; set; } = new List<string>();

        [JsonIgnore]
        public bool HasErrors => Errors.Any();

        public T Value { get; set; }

    }
}
