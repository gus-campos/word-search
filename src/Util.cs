using System;

public static class Input {

    public static int GetIntInput(string message) {

        /*
        Asks for the same int input multiple times, until its valid
        */

        bool valid = false;
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