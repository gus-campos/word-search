
using System;
using System.Collections.Generic;

class WordSearchGenerationException : Exception 
{
    public WordSearchGenerationException(string message) : base(message) {}
}

class WordSearch {

    /* Represents a WordSearch page */

    // Properties

    private Coord dimensions;
    private List<Word> words = new List<Word>();
    private Letter[,] table;

    // Constructor 
    
    public WordSearch(Coord dimensions, int wordsAmount) {

        this.table = new Letter[dimensions.x, dimensions.y];
        this.dimensions = dimensions;

        for (int i=0; i<dimensions.x; i++)
            for (int j=0; j<dimensions.y; j++)
                this.table[i,j] = new Letter((char)Util.GetRandom('A', 'Z'+1), new Coord(i,j));

        // Creating and inserting words
        for (int i=0; i<wordsAmount; i++)
        {
            
            
            this.TryToGenWord();
        }   
    }

    // Public methods

    public Letter[,] GetTable() {

        return this.table;
    }

    public void PrintTable() {

        /* Prints the word search table, character per character */

        Console.WriteLine("\n\n============= Word Search =============\n\n");

        for (int i=0; i<dimensions.x; i++)
        {
            for (int j=0; j<dimensions.y; j++)
            {
                Console.Write(this.table[i,j].character + " ");
            }

            Console.WriteLine();
        }
    }

    public void PrintWords() {

        /*
        Prints all words to be found
        */

        Console.WriteLine("\n=> Words to be found:\n");

        string[] wordsText = this.GetWordsText();
        Array.Sort(wordsText);

        foreach (string wordText in wordsText)
            Console.WriteLine(wordText);
    }

    // Getters

    public Word[] GetWords() {

        /* Get the text of each word in the table */

        Word[] words = new Word[this.words.Count];
        
        for (int i=0; i<words.Length; i++)
            words[i] = this.words[i];
        
        return words;
    }

    public string[] GetWordsText() {
        
        /* Get the text of each word in the table */

        string[] wordsText = new string[this.words.Count];
        
        for (int i=0; i<wordsText.Length; i++)
            wordsText[i] = this.words[i].GetText();
        
        return wordsText;
    }

    // Private methods

    private void InsertWord(Word word) {

        /* Inserts each letter of the word in the table */

        List<Letter> letters = word.GetLetters();

        foreach (Letter letter in letters)
            this.table[letter.coord.x, letter.coord.y] = letter;

        this.words.Add(word);
    }

    private bool ValidWordInsert(Word word) {

        /* 
        Verifies if a word is valid to be added to the WordSearch
        It will be valid if:

            * It doesn't collides with any word already inserted
            * It doesnt't have the same word text as any word already inserted 
        */

        foreach (Word tableWord in this.words)
        {
            if (tableWord.GetText() == word.GetText())
                return false; 

            if (tableWord.CollidesWith(word))
                return false;
        }

        return true;
    }

    private void TryToGenWord() {

        bool valid;
        int tries = 0;

        // For many times, try to pick a word and fit it inside the word search
        do 
        {
            // If too many tries are made, throw an Exception
            tries++;
            if (tries > Constants.maxTriesAmount)
                throw new WordSearchGenerationException("WordSearch too hard to create");

            // Create word
            Word word = Word.GenRandomWord(this.dimensions);

            valid = this.ValidWordInsert(word);

            if (valid) 
                this.InsertWord(word);

        } while (!valid);
    }
}