
using Xunit;

public class WordSearchTests {

    [Fact]
    public void CreatingWordSearch_HighWordDensity_ExceptionIsThrown() 
    {
        Assert.Throws<WordSearchGenerationException>(() => new WordSearch(new Coord(40,40), 400));
    }

    [Fact]
    public void CreatingWordSearch_SmallWordDensity_NoExceptionIsThrown() 
    {
        new WordSearch(new Coord(40,40), 10);
    }

    [Fact]
    public void CreatingWordSearch_SmallWordDensity_NumberOfWordsMatch() 
    {
        int wordsNumber = 10;
        WordSearch wordSearch = new WordSearch(new Coord(40,40), wordsNumber);

        Assert.Equal(wordsNumber, wordSearch.GetWords().Length);
    }

    [Fact]
    public void CreatingWordSearch_SmallWordDensity_DimensionsMatch() 
    {
        Coord dimensions = new(40,40);
        WordSearch wordSearch = new WordSearch(dimensions, 10);
        Letter[,] table = wordSearch.GetTable();

        Assert.Equal(dimensions, new Coord(table.GetLength(0), table.GetLength(1)));
    }

    [Fact]
    public void CreatingWordSearch_SmallWordDensity_AllWordsLettersAreInserted() 
    {
        WordSearch wordSearch = new WordSearch(new Coord(40,40), 10);
        Word[] words = wordSearch.GetWords();
        Letter[,] table = wordSearch.GetTable();

        bool valid = true;

        foreach (Word word in words) 
        {
            foreach (Letter letter in word.GetLetters())
            {
                Coord position = letter.coord;

                if (table[position.x, position.y].character != letter.character)
                {
                    valid = false;
                    break;
                }
            }
        }

        Assert.True(valid);
    }
}