

using System;
using System.Collections.Generic;

public static class PrintWordSearch
{ 
    public static void PrintTable(WordSearch wordSearch)
    {

        /* 
        Prints the word search table, character per character 
        */

        void PrintColumnsIndexes(int n)
        {

            Console.Write("   ");

            for (int j = 0; j < n; j++)
                Console.Write($"{j:D2} ");

            Console.WriteLine("\n");
        }

        void PrintRowIndex(int i)
        {
            Console.Write($"{i:D2}  ");
        }

        Console.WriteLine("\n\n============= Word Search =============\n\n");

        // Imprimir índices das colunas
        PrintColumnsIndexes(wordSearch.Dimensions.x);

        for (int i = 0; i < wordSearch.Dimensions.x; i++)
        {
            PrintRowIndex(i);

            // Print letters
            for (int j = 0; j < wordSearch.Dimensions.y; j++)
                wordSearch.Table[i, j].Print();

            Console.WriteLine("");
        }

        PrintWords(wordSearch);
    }

    private static void PrintWords(WordSearch wordSearch)
    {

        /*
        Prints all words to be found
        */

        Console.WriteLine();

        // Getting not found words texts
        List<string> foundWordsText = new();
        List<string> notFoundWordsText = new();

        foreach (Word word in wordSearch.Words)
            if (word.GetFound())
                foundWordsText.Add(word.GetText());
            else
                notFoundWordsText.Add(word.GetText());

        foundWordsText.Sort();
        notFoundWordsText.Sort();

        // Printing

        // Found
        if (foundWordsText.Count > 0)
        {
            Console.WriteLine("Found:");
            foreach (string wordText in foundWordsText)
                Console.WriteLine("\t" + wordText);
        }

        // Separator
        if (foundWordsText.Count > 0 && notFoundWordsText.Count > 0)
        {
            Console.WriteLine();
        }

        // Not found
        if (notFoundWordsText.Count > 0)
        {
            Console.WriteLine("To be found:");
            foreach (string wordText in notFoundWordsText)
                Console.WriteLine("\t" + wordText);
        }

        Console.WriteLine();
    }

}