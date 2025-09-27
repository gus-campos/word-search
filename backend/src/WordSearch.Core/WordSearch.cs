
using System;
using System.Collections.Generic;
using System.Linq;

public class WordSearchGenerationException : Exception 
{
    public WordSearchGenerationException(string message) : base(message) {}
}

public class WordSearch {

    /* Represents a WordSearch page */

    // Properties

    public Coord Dimensions { get; set; }
    public List<Word> Words { get; set; } = new List<Word>();
    public Letter[,] Table { get; set; }

    // Constructor 
    
    public WordSearch(Coord dimensions, int wordsAmount) {

        bool debugMode = true;

        if (dimensions.x < Constants.MinDimension || dimensions.y < Constants.MinDimension)
            throw new System.Exception("Nenhum lado deve ser menor que " + Constants.MinDimension);

        this.Table = new Letter[dimensions.x, dimensions.y];
        this.Dimensions = dimensions;

        for (int i = 0; i < dimensions.x; i++)
        {
            for (int j = 0; j < dimensions.y; j++)
            {
                char randomChar = Util.GetRandomCharacter();
                this.Table[i,j] = new Letter(debugMode ? char.ToLower(randomChar) : randomChar, new Coord(i,j));
            }
        }

        // Creating and inserting words
        for (int i=0; i<wordsAmount; i++)
            this.InsertRandomWord();
    }

    // Public methods

    public Letter[,] GetTable() {

        return this.Table;
    }

    public void PrintTable() {

        /* 
        Prints the word search table, character per character 
        */

        void PrintColumnsIndexes(int n) {

            Console.Write("   ");
            
            for (int j=0; j<n; j++)
                Console.Write($"{j:D2} ");
            
            Console.WriteLine("\n");
        }

        void PrintRowIndex(int i) {
            Console.Write($"{i:D2}  ");
        }

        Console.WriteLine("\n\n============= Word Search =============\n\n");

        // Imprimir índices das colunas
        PrintColumnsIndexes(Dimensions.x);

        for (int i=0; i<Dimensions.x; i++)
        {
            PrintRowIndex(i);

            // Print letters
            for (int j=0; j<Dimensions.y; j++)
                this.Table[i,j].Print();

            Console.WriteLine("");
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

        foreach (Word word in this.Words)
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

        foreach (Word word in this.Words)
            if (!word.GetFound())
                return false;
            
        return true;
    }

    // Getters

    public Word[] GetWords() {

        /* Get the text of each word in the table */

        Word[] words = new Word[this.Words.Count];
        
        for (int i=0; i<words.Length; i++)
            words[i] = this.Words[i];
        
        return words;
    }

    // Private methods

    private void InsertWord(Word word) {

        /* Inserts each letter of the word in the table */

        List<Letter> letters = word.GetLetters();

        foreach (Letter letter in letters)
            this.Table[letter.coord.x, letter.coord.y] = letter;

        this.Words.Add(word);
    }

    private bool ValidWordInsert(Word word) {

        /* 
        Verifies if a word is valid to be added to the WordSearch
        It will be valid if:

            * It doesn't collides with any word already inserted
            * It doesnt't have the same word text as any word already inserted 
        */

        foreach (Word tableWord in this.Words)
        {
            if (tableWord.GetText() == word.GetText())
                return false; 

            if (tableWord.CollidesWith(word))
                return false;
        }

        return true;
    }

    private void InsertRandomWord() {

        bool valid;
        int tries = 0;

        // For many times, try to pick a word and fit it inside the word search
        do 
        {
            // If too many tries are made, throw an Exception
            tries++;
            if (tries > Constants.MaxTriesAmount)
                throw new WordSearchGenerationException("WordSearch too hard to create");

            // Create word
            Word word = Word.GenRandomWord(this.Dimensions);

            valid = this.ValidWordInsert(word);

            if (valid) 
                this.InsertWord(word);

        } while (!valid);
    }
}