using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Easel;

public static class Logging
{
    public static event OnLogAdded LogAdded;

    public static void Log(LogType type, string message)
    {
        switch (type)
        {
            case LogType.Debug:
                Debug(message);
                break;
            case LogType.Info:
                Info(message);
                break;
            case LogType.Warn:
                Warn(message);
                break;
            case LogType.Error:
                Error(message);
                break;
            case LogType.Fatal:
                Fatal(message);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    
    public static void Debug(string message)
    {
        LogAdded?.Invoke(LogType.Debug, message);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("[" + GetCaller() + "::Debug]\t");
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

    public static void Fatal(string message)
    {
        LogAdded?.Invoke(LogType.Fatal, message);
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write("[Fatal]\t");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
        throw new EaselException(message);
    }

    private static string GetCaller()
    {
        return new StackFrame(2).GetMethod()?.DeclaringType?.Name;
    }

    public delegate void OnLogAdded(LogType type, string message);

    private static string GetLogMessage(LogType type, string message)
    {
        return "[" + type + "]" + message;
    }

    public enum LogType
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
    
    #region Log file

    public static string LogFilePath { get; private set; }

    private static StreamWriter _stream;
    
    public static void InitializeLogFile(string path)
    {
        LogFilePath = path;
        _stream = new StreamWriter(path, true);
        _stream.AutoFlush = true;
        
        LogAdded += LogFile;
    }

    private static void LogFile(LogType type, string message)
    {
        _stream.WriteLine(DateTime.Now + ": " + GetLogMessage(type, message));
    }

    #endregion
}