using System.Text.Json.Serialization;

namespace HIOF.GamingSocial.PublicGameInformation.Model.V1;


/// <summary>
/// Represents a result object for version 2 of the API.
/// </summary>
/// <typeparam name="T">The type of the value held by the result object.</typeparam>    
public class V2Result<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="V2Result{T}"/> class.
    /// </summary>
    public V2Result()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="V2Result{T}"/> class with a value.
    /// </summary>
    /// <param name="value">The value to be held by the result object.</param>
    public V2Result(T value)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="V2Result{T}"/> class with an error message.
    /// </summary>
    /// <param name="error">The error message to be added to the <see cref="Errors"/> list.</param>
    public V2Result(string error)
    {
        Errors.Add(error);
    }

    /// <summary>
    /// Gets or sets the list of error messages.
    /// </summary>
    public List<string> Errors { get; set; } = new List<string>();

    /// <summary>
    /// Indicates whether the <see cref="V2Result{T}"/> contains errors or not.
    /// </summary>
    [JsonIgnore]
    public bool HasErrors => Errors.Any();

    /// <summary>
    /// Gets or sets the value held by the <see cref="V2Result{T}"/>.
    /// </summary>
    public T Value { get; set; }
}
