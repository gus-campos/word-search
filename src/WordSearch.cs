
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
                this.table[i,j] = new Letter(Util.GetRandomCharacter(true), new Coord(i,j));

        // Creating and inserting words
        for (int i=0; i<wordsAmount; i++)
            this.GenWord();
    }

    // Public methods

    public Letter[,] GetTable() {

        return this.table;
    }

    public void PrintTable() {

        /* Prints the word search table, character per character */

        Console.WriteLine("\n\n============= Word Search =============\n\n");

        // Imprimir índices das colunas
        Console.Write("   ");
        for (int j=0; j<dimensions.x; j++)
            Console.Write($"{j:D2} ");
        Console.WriteLine("\n");

        for (int i=0; i<dimensions.x; i++)
        {
            // Imprimir índices das linhas
            Console.Write($"{i:D2}  ");

            for (int j=0; j<dimensions.y; j++)
            {
                Console.Write(this.table[i,j].character + "  ");
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

    public Word? GetWordAt(Coord coord) {
        return this.GetTable()[coord.x, coord.y].word;
    }

    public bool checkGuess(Coord coord0, Coord coord1) {

        Word? word = this.GetWordAt(coord0);

        if (word == null)
            return false;
 
        // Ordem direta
        if (word.GetLetters().First().coord == coord0 && word.GetLetters().Last().coord == coord1)
            return true;

        // Ordem inversa
        if (word.GetLetters().First().coord == coord0 && word.GetLetters().Last().coord == coord1)
            return true;

        return false;
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

    private void GenWord() {

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