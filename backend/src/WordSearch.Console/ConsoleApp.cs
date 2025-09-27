
using System;



class ConsoleApp
{
    public static void Main()
    {
        // Getting input

        Coord dimensions = Input.GetCoordInput("Insert a dimension: ");
        int wordsAmount = Input.GetIntInput("Insert a valid words amount: ");

        WordSearch? wordSearch = TryGenWordSearch(dimensions, wordsAmount);

        if (wordSearch == null)
        {
            Console.WriteLine(Constants.failMessage);
            return;
        }

        PlayWordSearch(wordSearch);
    }

    private static WordSearch? TryGenWordSearch(Coord dimensions, int wordsAmount)
    {

        WordSearch? wordSearch = null;

        int tries = 0;

        // Try for many times to create a word search with the given parameters
        while (tries <= Constants.maxTriesAmount)
        {
            try
            {
                wordSearch = new WordSearch(dimensions, wordsAmount);
                break;
            }

            catch (WordSearchGenerationException)
            {
                tries++;
            }
        }

        return wordSearch!;
    }

    private static void PlayWordSearch(WordSearch wordSearch)
    {

        PrintWordSearch.PrintTable(wordSearch);

        while (!wordSearch.CheckWin())
        {

            Console.WriteLine("=> Try to mark a word: ");

            Coord begin = Input.GetCoordInput("Insert begin: ");
            Coord end = Input.GetCoordInput("Insert end: ");

            wordSearch.GuessWordPosition(begin, end);

            PrintWordSearch.PrintTable(wordSearch);
        }

        Console.WriteLine("================");
        Console.WriteLine("Congratulations!");
        Console.WriteLine("================\n\n");
    }
}