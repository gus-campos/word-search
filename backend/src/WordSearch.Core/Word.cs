
using System;
using System.Collections.Generic;
using System.Linq;

public record Coord(int x, int y);

public class Letter
{

    /*
    Represents a WordSearch table letter
    */

    // Properties

    public char Character { get; }
    public Coord Coord { get; }
    public Word? Word { get; }

    // Constructor
    
    public Letter(char character, Coord coord, Word? word = null)
    {

        Character = character;
        Coord = coord;
        Word = word;
    }

    public void Print()
    {

        bool found = Word != null && Word.GetFound();
        Console.Write(found ? "*  " : Character + "  ");
    }
}

public enum Direction {
    NORMAL,
    REVERSE
}

public enum Orientation {
    HORIZONTAL,
    VERTICAL,
    DIAGONAL
}

public class Word {

    /*
    Represents a WordSearch table word
    */

    // Properties

    public string Text { get; set; } = "";
    
    private readonly List<Letter> letters = new List<Letter>();
    private readonly Direction direction;
    private readonly Orientation orientation;
    private bool found = false;

    // Static
    private static Orientation[] orientations = [

        Orientation.DIAGONAL, 
        Orientation.VERTICAL, 
        Orientation.HORIZONTAL
    ];
    
    private static Direction[] directions = [
        
        Direction.NORMAL, 
        Direction.REVERSE
    ];

    // Constructor

    public Word(Orientation orientation, Direction direction, string wordText, Coord dimensions) {

        this.orientation = orientation;
        this.direction = direction;
        Text = wordText;

        CreateLetters(dimensions);
    }

    // Public methods

    public bool CollidesWith(Word word) 
    {
        /* 
        Verifies if the two words have any letter coord in commom
        */

        foreach (Letter letter0 in letters)
            foreach (Letter letter1 in letters)
                if (letter0.Coord == letter1.Coord)
                    return true;

        return false;
    }

    // Public methods - Getters

    public List<Letter> GetLetters() {
        return letters;
    }

    public string GetText() {
        return Text;
    }

    public bool GetFound() {
        return found;
    }

    public void markAsFound() {
        found = true;
    }


    // Public methods - static

    public static Orientation GenRandomOrientation() {
        
        int randomIndex = Util.GetRandom(orientations.Length);
        return orientations[randomIndex];
    }

    public static Direction GenRandomDirection() {
        
        int randomIndex = Util.GetRandom(directions.Length);
        return directions[randomIndex];
    }

    public static Word GenRandomPositionedWord(Coord dimensions, string wordText) {

        /*
        Creates a random word for given dimensions of a word search
        */

        Orientation orientation = GenRandomOrientation();
        Direction direction = GenRandomDirection();

        return new Word(orientation, direction, wordText, dimensions);
    }

    // Private methods

    public static string GetRandomWordText() {

        /*
        Get a random word text from vocabulary
        */

        int randomIndex = Util.GetRandom(Constants.Vocabulary.Length);
        return Constants.Vocabulary![randomIndex].ToUpper();
    }

    private Coord GetWordSquareDimension() {

        /*
        Get the dimension of the square ocupied by the 
        */

        Coord wordMaxOffset = new(0,0);

        switch (orientation) {

            case Orientation.HORIZONTAL:
                wordMaxOffset = new Coord(0, Text.Length); 
                break;

            case Orientation.VERTICAL:
                wordMaxOffset = new Coord(Text.Length, 0);
                break;

            case Orientation.DIAGONAL:
                wordMaxOffset = new Coord(Text.Length, Text.Length);
                break;
        }

        return wordMaxOffset;
    }

    private Coord GetNextLetterOffset() {

        /*
        Returns the displacement from a letter to the other in the table
        given it's orientation
        */

        Coord nextLetterOffset = new(0,0);

        switch (orientation) {

            case Orientation.HORIZONTAL:
                nextLetterOffset = new Coord(0, 1);
                break;

            case Orientation.VERTICAL:
                nextLetterOffset = new Coord(1, 0);
                break;

            case Orientation.DIAGONAL:
                nextLetterOffset = new Coord(1, 1);
                break;
        }

        return nextLetterOffset;
    }

    private Coord GenStartPosition(Coord dimensions, Coord wordSquareDimension) {

        /* 
        Generates a random start coord given the word search dimensions
        and the greatest coord the word can start
        */

        int x = Util.GetRandom(dimensions.x - wordSquareDimension.x);
        int y = Util.GetRandom(dimensions.y - wordSquareDimension.y);

        return new Coord(x, y);
    }

    private void CreateLetters(Coord dimensions) {

        /*
        Create each letter of the word with their coord,
        considering it's direction, orientation and word search
        dimensions limitations
        */

        if (Text == "")
            throw new NullReferenceException("Word text not defined yet");

        Coord wordSquareDimension = GetWordSquareDimension();
        Coord nextLetterOffset = GetNextLetterOffset();
        Coord startPosition = GenStartPosition(dimensions, wordSquareDimension);        

        // Reversing word text, if necessary
        string formatedText = Text;
        if (direction == Direction.REVERSE)
            formatedText = new string(Text.Reverse().ToArray());
    
        // Creating each letter of the word
        for (int i=0; i<Text.Length; i++)
        {
            Letter letter = new Letter(
                
                character: formatedText[i], 
                coord: new Coord(startPosition.x + i*nextLetterOffset.x, 
                                 startPosition.y + i*nextLetterOffset.y),
                word: this                                      
            );

            letters.Add(letter);
        }
    }
}