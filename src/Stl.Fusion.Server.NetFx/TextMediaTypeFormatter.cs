using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace Stl.Fusion.Server;

// purpose and implementation is taken from https://stackoverflow.com/questions/25631970/how-to-post-plain-text-to-asp-net-web-api-endpoint
// without it RestEase client wraps result of type Task<string> in quotes.
public class TextMediaTypeFormatter : MediaTypeFormatter
{
    public TextMediaTypeFormatter()
    {
        SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
    }

    public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
    {
        var memoryStream = new MemoryStream();
        await readStream.CopyToAsync(memoryStream).ConfigureAwait(false);
        return System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    public override Task WriteToStreamAsync(Type type, object? value, Stream writeStream, HttpContent content, System.Net.TransportContext transportContext, System.Threading.CancellationToken cancellationToken)
    {
        if (value == null)
            return Task.CompletedTask;
        var buff = System.Text.Encoding.UTF8.GetBytes(value.ToString()!);
        return writeStream.WriteAsync(buff, 0, buff.Length, cancellationToken);
    }

    public override bool CanReadType(Type type)
    {
        return type == typeof(string);
    }

    public override bool CanWriteType(Type type)
    {
        return type == typeof(string);
    }
}
