
using System;
using System.Collections.Generic;
using System.Linq;

class Letter {

    /*
    Represents a WordSearch table letter
    */

    // Properties

    public char character { get; }
    public Coord coord { get; }
    public Word? word { get; }

    // Constructor

    // TODO: Letter passará a guardar word e wordsearch será uma matriz de letters

    public Letter(char character, Coord coord, Word? word=null) {

        this.character = character;
        this.coord = coord;
        this.word = word;
    }

    public void Print() {

        bool found = this.word != null && this.word.GetFound();
        Console.Write(found ? "*  " : this.character + "  ");
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

    // Properties

    private List<Letter> letters = new List<Letter>();
    private Direction direction;
    private Orientation orientation;
    private string text = "";
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
        this.text = wordText;

        this.CreateLetters(dimensions);
    }

    // Public methods

    public bool CollidesWith(Word word) 
    {
        /* 
        Verifies if the two words have any letter coord in commom
        */

        foreach (Letter letter0 in this.letters)
            foreach (Letter letter1 in word.letters)
                if (letter0.coord == letter1.coord)
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

    public bool GetFound() {
        return this.found;
    }

    public void markAsFound() {
        this.found = true;
    }


    // Public methods - static

    public static Orientation GenRandomOrientation() {
        
        int randomIndex = Util.GetRandom(Word.orientations.Length);
        return Word.orientations[randomIndex];
    }

    public static Direction GenRandomDirection() {
        
        int randomIndex = Util.GetRandom(Word.directions.Length);
        return Word.directions[randomIndex];
    }

    public static Word GenRandomWord(Coord dimensions) {

        /*
        Creates a random word for given dimensions of a word search
        */

        Orientation orientation = Word.GenRandomOrientation();
        Direction direction = Word.GenRandomDirection();
        string wordText = Word.GetRandomWordText();

        return new Word(orientation, direction, wordText, dimensions);
    }

    // Private methods

    private static string GetRandomWordText() {

        /*
        Get a random word text from vocabulary
        */

        int randomIndex = Util.GetRandom(Constants.vocabulary.Length);
        return Constants.vocabulary![randomIndex].ToUpper();
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

        int x = Util.GetRandom(dimensions.x - wordMaxOffset.x);
        int y = Util.GetRandom(dimensions.y - wordMaxOffset.y);

        return new Coord(x, y);
    }

    private void CreateLetters(Coord dimensions) {

        /*
        Create each letter of the word with their coord,
        considering it's direction, orientation and word search
        dimensions limitations
        */

        // TODO: Usar exception mais própria
        if (this.text == "")
            throw new NullReferenceException("Word text not defined yet");

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
                                 startPosition.y + i*nextLetterOffset.y),
                word: this                                      
            );

            this.letters.Add(letter);
        }
    }
}