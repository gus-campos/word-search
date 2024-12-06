
using System;
using System.IO;

public static class Constants {

    public static int maxTriesAmount = 10;
    public static string VocabularyPath = Path.Combine(AppContext.BaseDirectory, "../../../data/vocabulary.txt");
    public static string[] vocabulary = Constants.LoadVocabulary();
    public static string failMessage  = "Too many words for given dimensions. Reduce the number of words, or increase the word search dimensions.";

    private static string[] LoadVocabulary() {

        /*
        Reads vocabulary from file
        */

        string[] vocabulary = File.ReadAllText(Constants.VocabularyPath).Split("\n");

        if (vocabulary.Length == 0)
                throw new IOException("No words found in the vocabulary data file.");

        return vocabulary;
    } 
}

public static class Input {

    public static int GetIntInput(string message) {

        /*
        Asks for the same int input multiple times, until its valid
        */

        bool valid;
        int validatedInput;
            
        do
        {
            Console.Write(message);
            string rawInput = Console.ReadLine() ?? "";
            valid = int.TryParse(rawInput, out validatedInput);

        } while (!valid);

        return validatedInput;
    }

    public static Coord GetCoordInput(string message) {

        /*
        Asks for the same Coord input multiple times, until its valid
        */

        bool valid;
        int x, y;
            
        do
        {
            Console.Write(message);
            string[] rawInput = (Console.ReadLine() ?? "").Split(" ");

            bool validX = int.TryParse(rawInput[0], out x);
            bool validY = int.TryParse(rawInput[1], out y);
            valid = validX && validY; 

        } while (!valid);

        return new Coord(x, y);
    }
}

public static class Util
{
    private static Random random = new Random();
    public static int GetRandom(int max)
    {
        return Util.random.Next(max);
    }

    public static int GetRandom(int min, int max)
    {
        return Util.random.Next(min, max);
    }

    public static char GetRandomCharacter(bool overwrite=false) {
        
        if (overwrite)
            return '.';
        else 
            return (char) Util.GetRandom('A', 'Z'+1);
    }
}

class Program 
{
    public static void Main() 
    {
        // Getting input

        Coord dimensions = Input.GetCoordInput("Insert a dimension: ");
        int wordsAmount = Input.GetIntInput("Insert a valid words amount: ");

        WordSearch? wordSearch = TryGenWordSearch(dimensions, wordsAmount);

        if (wordSearch == null) {

            Console.WriteLine(Constants.failMessage);
            return;
        }

        wordSearch.PrintTable();
        wordSearch.PrintWords();

        while (true) {
            Console.WriteLine("\n");
            Coord begin = Input.GetCoordInput("Insert begin: ");
            Coord end = Input.GetCoordInput("Insert end: ");
            
            if (wordSearch.checkGuess(begin, end))
                Console.WriteLine(wordSearch.GetWordAt(begin)!.GetText());
        }
    }

    private static WordSearch? TryGenWordSearch(Coord dimensions, int wordsAmount) {

        WordSearch? wordSearch = null;

        bool valid = false;
        int tries = 0;

        // Try for many times to create a word search with the given parameters
        while (!valid) 
        {
            try
            {
                wordSearch = new WordSearch(dimensions, wordsAmount);
                break;
            }

            catch (WordSearchGenerationException)
            {   
                if (++tries > Constants.maxTriesAmount)
                    break;
            }
        }

        return wordSearch!;
    }
}