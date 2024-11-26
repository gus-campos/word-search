
using Xunit;

public class WordSearchTests {

    [Fact]
    public void CreatingWordSearch_HighWordDensity_ExceptionIsThrown() 
    {
        Assert.Throws<WordSearchGenerationException>(() => new WordSearch(new IntDuple(40,40), 400));
    }

    [Fact]
    public void CreatingWordSearch_SmallWordDensity_NoExceptionIsThrown() 
    {
        new WordSearch(new IntDuple(40,40), 10);
    }

    [Fact]
    public void CreatingWordSearch_SmallWordDensity_NumberOfWordsMatch() 
    {
        int wordsNumber = 10;
        WordSearch wordSearch = new WordSearch(new IntDuple(40,40), wordsNumber);

        Assert.Equal(wordsNumber, wordSearch.GetWords().Length);
    }

    [Fact]
    public void CreatingWordSearch_SmallWordDensity_DimensionsMatch() 
    {
        IntDuple dimensions = new(40,40);
        WordSearch wordSearch = new WordSearch(dimensions, 10);
        char[,] table = wordSearch.GetTable();

        Assert.Equal(dimensions, new IntDuple(table.GetLength(0), table.GetLength(1)));
    }

    [Fact]
    public void CreatingWordSearch_SmallWordDensity_AllWordsLettersAreInserted() 
    {
        WordSearch wordSearch = new WordSearch(new IntDuple(40,40), 10);
        Word[] words = wordSearch.GetWords();
        char[,] table = wordSearch.GetTable();

        bool valid = true;

        foreach (Word word in words) 
        {
            foreach (Letter letter in word.GetLetters())
            {
                IntDuple position = letter.GetPosition();

                if (table[position.x, position.y] != letter.GetCharacter())
                {
                    valid = false;
                    break;
                }
            }
        }

        Assert.True(valid);
    }
}