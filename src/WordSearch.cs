
using System;
using System.Collections.Generic;
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
                this.table[i,j] = new Letter(Util.GetRandomCharacter(), new Coord(i,j));

        // Creating and inserting words
        for (int i=0; i<wordsAmount; i++)
            this.GenWord();
    }

    // Public methods

    public Letter[,] GetTable() {

        return this.table;
    }

    public void PrintTable() {

        /* 
        Prints the word search table, character per character 
        */

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
                Letter letter = this.table[i,j];
                bool found = letter.word != null && letter.word.GetFound();
                
                Console.Write(found ? "*  " : letter.character + "  ");
            }

            Console.WriteLine();
        }

        this.PrintWords();
    }

    public void PrintWords() {

        /*
        Prints all words to be found
        */

        Console.WriteLine(); 

        // Getting not found words texts
        List<string> foundWordsText = new();
        List<string> notFoundWordsText = new();

        foreach (Word word in this.words)
            if (word.GetFound())
                foundWordsText.Add(word.GetText());
            else
                notFoundWordsText.Add(word.GetText());
                
        foundWordsText.Sort();
        notFoundWordsText.Sort();

        // Printing

        // Found
        if (foundWordsText.Count() > 0) {
            Console.WriteLine("Found:");
            foreach (string wordText in foundWordsText)
                Console.WriteLine("\t" + wordText);
        }

        // Separator
        if (foundWordsText.Count() > 0 && notFoundWordsText.Count() > 0) {
            Console.WriteLine();
        }

        // Not found
        if (notFoundWordsText.Count() > 0) {
            Console.WriteLine("To be found:");
            foreach (string wordText in notFoundWordsText)
                Console.WriteLine("\t" + wordText);
        }

        Console.WriteLine();
    }

    public Word? GetWordAt(Coord coord) {
        return this.GetTable()[coord.x, coord.y].word;
    }

    public void GuessWordPosition(Coord coord0, Coord coord1) {

        Word? word = this.GetWordAt(coord0);

        if (word == null)
            return;
 
        // Ordem direta
        if (word.GetLetters().First().coord == coord0 && word.GetLetters().Last().coord == coord1)
            word.markAsFound();

        // Ordem inversa
        if (word.GetLetters().Last().coord == coord0 && word.GetLetters().First().coord == coord1)
            word.markAsFound();
    }

    public bool CheckWin() {

        foreach (Word word in this.words)
            if (!word.GetFound())
                return false;
            
        return true;
    }

    // Getters

    public Word[] GetWords() {

        /* Get the text of each word in the table */

        Word[] words = new Word[this.words.Count];
        
        for (int i=0; i<words.Length; i++)
            words[i] = this.words[i];
        
        return words;
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