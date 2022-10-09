using System;

namespace Easel;

public static class Logging
{
    public static event OnLogAdded LogAdded;
    
    public static void Log(string message)
    {
        LogAdded?.Invoke(LogType.Debug, message);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("[Debug]\t");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
    }
    
    public static void Info(string message)
    {
        LogAdded?.Invoke(LogType.Info, message);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("[Info]\t");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
    }

    public static void Warn(string message)
    {
        LogAdded?.Invoke(LogType.Warn, message);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("[Warn]\t");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
    }
    
    public static void Error(string message)
    {
        LogAdded?.Invoke(LogType.Error, message);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[Error]\t");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
    }

    public static void Critical(string message)
    {
        LogAdded?.Invoke(LogType.Critical, message);
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write("[CRITICAL]\t");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
        throw new EaselException(message);
    }

    public delegate void OnLogAdded(LogType type, string message);

    public enum LogType
    {
        Debug,
        Info,
        Warn,
        Error,
        Critical
    }
}