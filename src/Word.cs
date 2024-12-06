
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public record Coord(int x, int y);

struct Letter {

    /*
    Represents a WordSearch table letter
    */

    // Properties

    public char character { get; }
    public Coord coord { get; }

    // Constructor

    public Letter(char character, Coord coord) {

        this.character = character;
        this.coord = coord;
    }
}

enum Direction {
    NORMAL,
    REVERSE
}

enum Orientation {
    HORIZONTAL,
    VERTICAL,
    DIAGONAL
}

class Word {

    /*
    Represents a WordSearch table word
    */

    // Static properties

    private static string[]? vocabulary = null;
    
    // Properties

    private List<Letter> letters = new List<Letter>();
    private Direction direction;
    private Orientation orientation;
    private string text;    


    // Constructor

    public Word(Orientation orientation, Direction direction, Coord dimensions) {

        if (vocabulary == null) 
            Word.vocabulary = this.LoadVocabulary();

        this.orientation = orientation;
        this.direction = direction;

        this.text = this.GetRandomWordText();
        this.CreateLetters(dimensions);
    }

    // Public methods

    public bool CollidesWith(Word word) 
    {
        /* 
        Verifies if the two words have any letter coord in commom
        */

        foreach (Letter letter1 in this.letters)
            foreach (Letter letter2 in word.letters)
                if (letter1.coord == letter2.coord)
                    return true;

        return false;
    }

    // Public methods - Getters

    public List<Letter> GetLetters() {
        return this.letters;
    }

    public string GetText() {
        return this.text;
    }

    // Private methods

    private string[] LoadVocabulary() {

        /*
        Reads vocabulary from file
        */

        string[] vocabulary = File.ReadAllText(Constants.VocabularyPath).Split("\n");

        if (vocabulary.Length == 0)
                throw new IOException("No words found in the vocabulary data file.");

        return vocabulary;
    }

    private string GetRandomWordText() {

        /*
        Get a random word text from vocabulary
        */

        return Word.vocabulary![Util.GetRandom(Word.vocabulary.Length)].ToUpper();
    }

    private Coord GetWordMaxOffset() {

        /*
        Get the greatest coord that a word can start given it's length
        so it can fit in the wordsearch
        */

        Coord wordMaxOffset = new(0,0);

        switch (this.orientation) {

            case Orientation.HORIZONTAL:
                wordMaxOffset = new Coord(0, text.Length); 
                break;

            case Orientation.VERTICAL:
                wordMaxOffset = new Coord(text.Length, 0);
                break;

            case Orientation.DIAGONAL:
                wordMaxOffset = new Coord(text.Length, text.Length);
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

        switch (this.orientation) {

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

    private Coord GenStartPosition(Coord dimensions, Coord wordMaxOffset) {

        /* 
        Generates a random start coord given the word search dimensions
        and the greatest coord the word can start
        */

        return new Coord(Util.GetRandom(dimensions.x - wordMaxOffset.x),
                         Util.GetRandom(dimensions.y - wordMaxOffset.y));
    }

    private void CreateLetters(Coord dimensions) {

        /*
        Create each letter of the word with their coord,
        considering it's direction, orientation and word search
        dimensions limitations
        */

        Coord wordMaxOffset = this.GetWordMaxOffset();
        Coord nextLetterOffset = this.GetNextLetterOffset();
        Coord startPosition = this.GenStartPosition(dimensions, wordMaxOffset);        

        // Reversing word text, if necessary
        string formatedText = this.text;
        if (this.direction == Direction.REVERSE)
            formatedText = new string(this.text.Reverse().ToArray());
    
        // Creating each letter of the word
        for (int i=0; i<this.text.Length; i++)
        {
            Letter letter = new Letter(
                
                character: formatedText[i], 
                coord: new Coord(startPosition.x + i*nextLetterOffset.x, 
                                    startPosition.y + i*nextLetterOffset.y)                                        
            );

            this.letters.Add(letter);
        }
    }
}