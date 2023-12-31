using System.Text.Json;

namespace Banking.Shared
{
    public class ErrorDetails
    {
            public int StatusCode { get; set; }
            public IEnumerable<string>? Message { get; set; }
            public override string ToString() => JsonSerializer.Serialize(this);
    }
}