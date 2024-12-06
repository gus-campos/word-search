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