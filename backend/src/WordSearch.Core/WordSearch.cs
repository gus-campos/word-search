
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    public WordSearch(Coord dimensions, List<string> wordsTexts)
    {
        bool debugMode = true;

        if (dimensions.x < Constants.MinDimension || dimensions.y < Constants.MinDimension)
            throw new Exception("Nenhum lado deve ser menor que " + Constants.MinDimension);

        Table = new Letter[dimensions.x, dimensions.y];
        Dimensions = dimensions;

        for (int i = 0; i < dimensions.x; i++)
        {
            for (int j = 0; j < dimensions.y; j++)
            {
                char randomChar = Util.GetRandomCharacter();
                Table[i, j] = new Letter(debugMode ? char.ToLower(randomChar) : randomChar, new Coord(i, j));
            }
        }

        foreach (string wordText in wordsTexts)
            InsertRandomPositionedWord(wordText);

        Console.WriteLine("CHEGOU");
        Console.WriteLine("Palavras" + Words.Count);
    }

    public static async Task<WordSearch> GenWordSearch(Coord dimensions, int wordsAmount, string topic = "")
    {
        List<string> wordsTexts;

        if (topic == "")
        {
            wordsTexts = [];
            for (int i = 0; i < wordsAmount; i++)
                wordsTexts.Add(Word.GetRandomWordText());
        }
        else
        {
            var genAiWords = new GenAiWords("");
            string[] wordsTextsArray = await genAiWords.GenerateWordsGemini(topic, wordsAmount);
            wordsTexts = [..wordsTextsArray];
        }

        return new WordSearch(dimensions, wordsTexts);
    }

    // Public methods

    public Letter[,] GetTable()
    {

        return Table;
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
                Table[i,j].Print();

            Console.WriteLine("");
        }

        PrintWords();
    }

    public void PrintWords() {

        /*
        Prints all words to be found
        */

        Console.WriteLine(); 

        // Getting not found words texts
        List<string> foundWordsText = new();
        List<string> notFoundWordsText = new();

        foreach (Word word in Words)
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
        return GetTable()[coord.x, coord.y].Word;
    }

    public void GuessWordPosition(Coord coord0, Coord coord1) {

        Word? word = GetWordAt(coord0);

        if (word == null)
            return;
 
        // Ordem direta
        if (word.GetLetters().First().Coord == coord0 && word.GetLetters().Last().Coord == coord1)
            word.markAsFound();

        // Ordem inversa
        if (word.GetLetters().Last().Coord == coord0 && word.GetLetters().First().Coord == coord1)
            word.markAsFound();
    }

    public bool CheckWin() {

        foreach (Word word in Words)
            if (!word.GetFound())
                return false;
            
        return true;
    }

    // Getters

    public Word[] GetWords() {

        /* Get the text of each word in the table */

        Word[] words = new Word[Words.Count];
        
        for (int i=0; i<words.Length; i++)
            words[i] = Words[i];
        
        return words;
    }

    // Private methods

    private void InsertWord(Word word) {

        /* Inserts each letter of the word in the table */

        List<Letter> letters = word.GetLetters();

        foreach (Letter letter in letters)
            Table[letter.Coord.x, letter.Coord.y] = letter;

        Words.Add(word);
    }

    private bool ValidWordInsert(Word word) {

        /* 
        Verifies if a word is valid to be added to the WordSearch
        It will be valid if:

            * It doesn't collides with any word already inserted
            * It doesnt't have the same word text as any word already inserted 
        */

        foreach (Word tableWord in Words)
        {
            // Console.WriteLine("1 " + tableWord.coord);
            // Console.WriteLine("2 " + tableWord.coord);

            if (tableWord.GetText() == word.GetText())
                return false; 

            if (tableWord.CollidesWith(word))
                return false;
        }

        return true;
    }

    private void InsertRandomPositionedWord(string wordText) {

        bool valid;
        int tries = 0;

        // For many times, try to pick a word and fit it inside the word search
        do 
        {
            // If too many tries are made, throw an Exception
            tries++;
            if (tries > Constants.MaxTriesAmount)
                throw new WordSearchGenerationException("Couldn't insert the word " + wordText);

            Word word = Word.GenRandomPositionedWord(Dimensions, wordText);

            valid = ValidWordInsert(word);

            if (valid)
                InsertWord(word);

        } while (!valid);
    }
}