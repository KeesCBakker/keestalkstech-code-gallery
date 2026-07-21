using System.Text;
using System.Text.Json;
using HandlebarsDotNet;

namespace Ktt.JsonHandlebars;

public class JsonTextEncoder : ITextEncoder
{
  public void Encode(StringBuilder text, TextWriter target)
  {
    Encode(text.ToString(), target);
  }

  public void Encode(string text, TextWriter target)
  {
    if (string.IsNullOrEmpty(text))
    {
      return;
    }

    var encoded = JsonEncodedText.Encode(text)
        .ToString()
        .Replace("\\u0022", "\\\"")
        .Replace("\\u005C", "\\\\");

    target.Write(encoded);
  }

  public void Encode<T>(T text, TextWriter target) where T : IEnumerator<char>
  {
    var str = text?.ToString();
    if (str == null)
    {
      return;
    }

    Encode(str, target);
  }
}
