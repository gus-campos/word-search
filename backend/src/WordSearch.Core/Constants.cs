using System;
using System.IO;

public static class Constants
{

    public static int MaxTriesAmount = 10;
    public static string VocabularyPath = Path.Combine(Directory.GetCurrentDirectory(), "../../../backend/data/vocabulary.txt");
    public static string FailMessage = "Too many words for given dimensions. Reduce the number of words, or increase the word search dimensions.";
    public static string[] Vocabulary = LoadVocabulary();
    public static int MinDimension = 15;

    private static string[] LoadVocabulary()
    {
        /*
        Reads vocabulary from file
        */
        string[] vocabulary = File.ReadAllText(VocabularyPath).Split("\n");


        if (vocabulary.Length == 0)
            throw new IOException("No words found in the vocabulary data file.");

        return vocabulary;
    }
}