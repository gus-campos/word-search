
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Word {

    /*
    Represents a WordSearch table word
    */

    // Properties

    private List<Letter> letters = new List<Letter>();
    private Direction direction;
    private Orientation orientation;
    private string text;    

    // Static properties

    private static string[]? vocabulary = null;
    private static Random random = new();

    // Constructor

    public Word(Orientation orientation, Direction direction, Coord dimensions) {

        if (vocabulary == null) 
            Word.vocabulary = this.LoadVocabulary();

        this.orientation = orientation;
        this.direction = direction;

        this.text = this.GetRandomWordText();
        this.CreateLetters(dimensions);
    }

    private Coord GenStartPosition(Coord dimensions, Coord wordMaxOffset) {

        /* 
        Generates a random start position given the word search dimensions
        */

        return new Coord(random.Next(dimensions.x - wordMaxOffset.x),
                            random.Next(dimensions.y - wordMaxOffset.y));
    }

    private Coord GetWordMaxOffset() {

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

    private void CreateLetters(Coord dimensions) {

        /*
        Create each letter of the word with their position,
        considering it's direction, orientation and word search
        dimensions limitations

        "wordMaxOffset" variable name is obscure
        */

        Coord wordMaxOffset = this.GetWordMaxOffset();
        Coord nextLetterOffset = this.GetNextLetterOffset();
        Coord startPosition = this.GenStartPosition(dimensions, wordMaxOffset);        

        // Reversing word text, if necessary
        string formatedText = this.text;
        if (this.direction == Direction.REVERSE)
            formatedText = string.Join("", this.text.ToCharArray().Reverse());
    
        // Creating each letter of the word
        for (int i=0; i<this.text.Length; i++)
        {
            Letter letter = new Letter(
                
                character: formatedText[i], 
                position: new Coord(startPosition.x + i*nextLetterOffset.x, 
                                    startPosition.y + i*nextLetterOffset.y)                                        
            );

            this.letters.Add(letter);
        }
    }

    private string GetRandomWordText() {

        return Word.vocabulary![Word.random.Next(Word.vocabulary.Length)].ToUpper();
    }

    private string[] LoadVocabulary() {

        string[] vocabulary = File.ReadAllText(Constants.VocabularyPath).Split("\n");

        if (vocabulary.Length == 0)
                throw new IOException("No words found in the vocabulary data file.");

        return vocabulary;
    }

    // Public methods

    public bool CollidesWith(Word word) 
    {
        /* 
        Verifies if the two words have any letter position in commom
        */

        foreach (Letter letter1 in this.letters)
            foreach (Letter letter2 in word.letters)
                if (letter1.GetPosition() == letter2.GetPosition())
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
}