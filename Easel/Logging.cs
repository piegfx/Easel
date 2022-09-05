using System;

namespace Easel;

public static class Logging
{
    public static void Log(string message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("[Debug]\t");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
    }
    
    public static void Info(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("[Info]\t");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
    }

    public static void Warn(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("[Warn]\t");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
    }
    
    public static void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[Error]\t");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
    }

    public static void Critical(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write("[CRITICAL]\t");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
        throw new EaselException(message);
    }
}