
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
    private IntDuple startPosition;
    private string text;
    private Direction direction;
    private Orientation orientation;

    // Static properties

    private static string[] vocabulary = [];

    // Constructor

    public Word(Orientation orientation, Direction direction, IntDuple dimensions) {

        // Load vocabulary, if not loaded yet
        if (vocabulary.Length == 0) 
        {
            Word.vocabulary = File.ReadAllText(Constants.VocabularyPath).Split("\n");
            if (Word.vocabulary.Length == 0)
                throw new IOException("No words found in the vocabulary data file.");
        }

        this.orientation = orientation;
        this.direction = direction;

        Random random = new Random();

        // Choosing random word from the vocabulary
        this.text = Word.vocabulary[random.Next(Word.vocabulary.Length)].ToUpper();

        // Creating Letter instances, and their positions
        IntDuple offset = new(0,0);
        IntDuple increment = new(0,0);

        switch (this.orientation) {

            case Orientation.HORIZONTAL:
                offset = new IntDuple(0, text.Length); 
                increment = new IntDuple(0, 1);
                break;

            case Orientation.VERTICAL:
                offset = new IntDuple(text.Length, 0);
                increment = new IntDuple(1, 0);
                break;

            case Orientation.DIAGONAL:
                offset = new IntDuple(text.Length, text.Length);
                increment = new IntDuple(1, 1);
                break;
        }

        startPosition = new IntDuple(random.Next(dimensions.x - offset.x),
                                     random.Next(dimensions.y - offset.y));

        // Reversing word text, if necessary
        string insertedText = this.text;
        if (this.direction == Direction.REVERSE)
            insertedText = string.Join("", this.text.ToCharArray().Reverse());
    
        // Creating each letter of the word
        for (int i=0; i<this.text.Length; i++)
        {
            Letter letter = new Letter(
                
                insertedText[i], 
                new IntDuple(startPosition.x + i*increment.x, 
                             startPosition.y + i*increment.y)                                        
            );

            letters.Add(letter);
        }
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