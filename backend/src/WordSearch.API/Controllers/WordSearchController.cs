using Microsoft.AspNetCore.Mvc;

public record WordSearchRequest(int Rows, int Columns, int numberOfWords, string topic);
public record WordSearchResponse(char[][] Grid, string[] Words);

[ApiController]
[Route("api/[controller]")]
public class WordSearchController : ControllerBase
{
    [HttpPost]
    public async Task<WordSearchResponse> CreateWordSearch([FromBody] WordSearchRequest request)
    {
        WordSearch? wordSearch = null;

        // Try for many times to create a word search with the given parameters
        int tries = 0;
        while (tries <= Constants.MaxTriesAmount)
        {
            try
            {
                wordSearch = await WordSearch.GenWordSearch(
                    new Coord(request.Rows, request.Columns),
                    request.numberOfWords,
                    request.topic
                );

                break;
            }

            catch (WordSearchGenerationException ex)
            {
                tries++;
                Console.WriteLine(ex.Message);
            }
        }

        if (wordSearch == null)
            throw new WordSearchGenerationException("Não foi possível gerar esse caça palavras.");

        char[][] charGrid = new char[wordSearch.Table.GetLength(0)][];
        string[] stringWords = [.. wordSearch.Words.Select(w => w.GetText())];

        for (int i = 0; i < wordSearch.Table.GetLength(0); i++)
        {
            charGrid[i] = new char[wordSearch.Table.GetLength(1)];

            for (int j = 0; j < wordSearch.Table.GetLength(1); j++)
                charGrid[i][j] = wordSearch.Table[i, j].Character;
        }

        var response = new WordSearchResponse(charGrid, stringWords);

        Console.WriteLine("WordSearch generated.");

        return response;
    }
}

