/*
ChessLib, a chess data structure library

MIT License

Copyright (c) 2017-2023 Rudy Alex Kohn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace Rudzoft.ChessLib.PGN;

public sealed class NonRegexPgnParser : IPgnParser
{
    public async IAsyncEnumerable<PgnGame> ParseFile(string pgnFile, CancellationToken cancellationToken = default)
    {
        await using var fileStream = new FileStream(pgnFile, FileMode.Open, FileAccess.Read);
        using var streamReader = new StreamReader(fileStream);

        var currentGameTags = new Dictionary<string, string>();
        var currentGameMoves = new List<PgnMove>();
        var inMoveSection = false;

        string line;
        while ((line = streamReader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                inMoveSection = currentGameTags.Count > 0;
                continue;
            }

            if (inMoveSection)
            {
                var words = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                var currentMoveNumber = 0;

                foreach (var word in words)
                {
                    if (word.Contains('.'))
                    {
                        if (int.TryParse(word.TrimEnd('.'), out var moveNumber))
                        {
                            currentMoveNumber = moveNumber;
                        }
                    }
                    else if (char.IsLetter(word[0]))
                    {
                        if (currentGameMoves.Count == 0 || !string.IsNullOrEmpty(currentGameMoves[^1].BlackMove))
                        {
                            currentGameMoves.Add(new PgnMove(currentMoveNumber, word, string.Empty));
                        }
                        else
                        {
                            var lastMove = currentGameMoves[^1];
                            currentGameMoves[^1] = lastMove with { BlackMove = word };
                        }
                    }
                    else if (word.Contains("1-0") || word.Contains("0-1") || word.Contains("1/2-1/2") ||
                             word.Contains('*'))
                    {
                        yield return new PgnGame(currentGameTags, currentGameMoves);
                        currentGameTags = new Dictionary<string, string>();
                        currentGameMoves = new List<PgnMove>();
                        inMoveSection = false;
                    }
                }
            }
            else
            {
                if (line.StartsWith("[") && line.EndsWith("]") && line.Contains("\""))
                {
                    var firstSpaceIndex = line.IndexOf(' ');
                    var firstQuoteIndex = line.IndexOf('"');
                    var lastQuoteIndex = line.LastIndexOf('"');

                    if (firstSpaceIndex > 0 && firstQuoteIndex > firstSpaceIndex &&
                        lastQuoteIndex > firstQuoteIndex)
                    {
                        var tagName = line.Substring(1, firstSpaceIndex - 1).Trim();
                        var tagValue = line.Substring(firstQuoteIndex + 1, lastQuoteIndex - firstQuoteIndex - 1)
                            .Trim();

                        currentGameTags[tagName] = tagValue;
                    }
                }
            }
        }

        if (currentGameTags.Count > 0 && currentGameMoves.Count > 0)
        {
            yield return new PgnGame(currentGameTags, currentGameMoves);
        }
    }
}