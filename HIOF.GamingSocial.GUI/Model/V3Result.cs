using System.Text.Json.Serialization;

namespace HIOF.GamingSocial.GUI.Model
{
    public class V3Result<T>
    {

		public V3Result()
		{

		}
		public V3Result(string error)
        {
            Errors.Add(error);
        }

        public bool Success => !HasErrors;
        public V3Result(T value)
        {
            Value = value;
        }

        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; } = new List<string>();
        
        [JsonIgnore]
        public bool HasErrors => Errors.Any();
        
        public T Value { get; set; }

        }
}
