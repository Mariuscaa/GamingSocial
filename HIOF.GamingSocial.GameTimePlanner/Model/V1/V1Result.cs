using System.Text.Json.Serialization;

namespace HIOF.GamingSocial.GameTimePlan.Model;

/// <summary>
/// Represents a result object of the API.
/// </summary>
/// <typeparam name="T">The type of the value held by the result object.</typeparam>    
public class V1Result<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="V1Result{T}"/> class.
    /// </summary>
    public V1Result()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="V1Result{T}"/> class with a value.
    /// </summary>
    /// <param name="value">The value to be held by the result object.</param>
    public V1Result(T value)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="V1Result{T}"/> class with an error message.
    /// </summary>
    /// <param name="error">The error message to be added to the <see cref="Errors"/> list.</param>
    public V1Result(string error)
    {
        Errors.Add(error);
    }

    /// <summary>
    /// Gets or sets the list of error messages.
    /// </summary>
    public List<string> Errors { get; set; } = new List<string>();

    /// <summary>
    /// Indicates whether the <see cref="V1Result{T}"/> contains errors or not.
    /// </summary>
    [JsonIgnore]
    public bool HasErrors => Errors.Any();

    /// <summary>
    /// Gets or sets the value held by the <see cref="V1Result{T}"/>.
    /// </summary>
    public T Value { get; set; }
}
