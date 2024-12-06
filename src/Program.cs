
using System;
using System.IO;

public static class Constants {

    public static int maxTriesAmount = 10;
    public static string VocabularyPath = Path.Combine(AppContext.BaseDirectory, "../../../data/vocabulary.txt");
    public static string[] vocabulary = Constants.LoadVocabulary();

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

    public static int getIntInput(string message) {

        /*
        Asks for the same input multiple times, until its valid
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
}

class Program 
{
    public static void Main() 
    {
        // Getting input

        int wordsAmount;
        int rows, columns;
        Coord dimensions;
        
        wordsAmount = Input.getIntInput("Insert a valid words amount: ");
        columns = Input.getIntInput("Insert a valid columns amount: ");
        rows = Input.getIntInput("Insert a valid rows amount: ");

        dimensions = new Coord(rows, columns);

        // Creating word search

        WordSearch? wordSearch = null;

        bool valid = false;
        int tries = 0;

        // Try for many times to create a word search with the given parameters
        do 
        {
            try
            {
                wordSearch = new WordSearch(dimensions: dimensions, 
                                            wordsAmount: wordsAmount);
                valid = true;
            }
            catch (WordSearchGenerationException)
            {
                tries++;
                if (tries > Constants.maxTriesAmount)
                {
                    string errorMessage = "Too many words for given dimensions. Reduce the number of words, or increase it's dimensions.";
                    Console.WriteLine(errorMessage);
                    break;
                }
            }

        } while (!valid);

        // Print word search page and words
        if (wordSearch != null)
        {
            wordSearch.PrintTable();
            wordSearch.PrintWords();
        }
    }
}