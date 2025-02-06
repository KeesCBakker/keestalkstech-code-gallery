using System.Text.RegularExpressions;

namespace Ktt.JsonHandlebars;

public partial class InvalidJsonException(Exception ex, string jsonText) : Exception(ParseMessage(ex, jsonText), ex)
{
    public string JsonText { get; } = jsonText;

    public static string ParseMessage(Exception ex, string json, int linesAbove = 3, int linesUnder = 2)
    {
        var message = ex.Message;

        //Example: After parsing a value an unexpected character was encountered: ". Path 'title', line 4, position 4.
        var match = LinePositionRegex().Match(message);

        if (match.Success)
        {
            var line = int.Parse(match.Groups["line"].Value);
            var position = int.Parse(match.Groups["position"].Value);

            // add lines numbers
            var padding = (line + linesUnder).ToString().Length;
            var lines = json.Split("\n")
                .Select((str, index) => $"{(index + 1).ToString().PadLeft(padding, '0')} | {str}")
                .ToList();

            // insert visual identifier
            lines.Insert(line, new string('-', position + padding + 3) + "^");

            // create final output
            var top = Math.Max(0, line - linesAbove);
            message += "\n\n" + string.Join("\n", lines.Take(line + linesUnder + 1).Skip(top));
        }


        return message;
    }

    [GeneratedRegex(@"line (?<line>\d+), position (?<position>\d+)")]
    private static partial Regex LinePositionRegex();
}
