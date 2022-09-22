using System;
using System.Collections.Generic;
using System.Globalization;
using Easel.Math;

namespace Easel.GUI.BBCode;

public static class BBCodeParser
{
    public static BBCodeInstruction[] Parse(string text)
    {
        List<BBCodeInstruction> instructions = new List<BBCodeInstruction>();
        bool inBrackets = false;
        string bracketsText = "";
        string tempText = "";

        for (int c = 0; c < text.Length; c++)
        {
            char chr = text[c];
            switch (chr)
            {
                case '[' when c <= 0 || text[c - 1] != '\\':
                    instructions.Add(new TextInstruction(tempText));
                    tempText = "";
                    inBrackets = true;
                    break;
                case ']' when inBrackets:
                    inBrackets = false;
                    instructions.Add(ParseTag(bracketsText));
                    bracketsText = "";
                    break;
                case '\\' when c < text.Length - 1 && text[c + 1] == '[':
                    break;
                default:
                    if (inBrackets)
                        bracketsText += chr;
                    else
                        tempText += chr;
                    break;
            }
        }
        
        instructions.Add(new TextInstruction(tempText));

        return instructions.ToArray();
    }

    private static BBCodeInstruction ParseTag(string tag)
    {
        bool exiting = tag.StartsWith("/");
        int startPos = exiting ? 1 : 0;
        string tagObject = "";
        
        for (int c = startPos; c < tag.Length; c++)
        {
            char chr = tag[c];
            switch (chr)
            {
                case '=':
                case ' ':
                    startPos = c + 1;
                    goto END;
                default:
                    tagObject += chr;
                    break;
            }
        }
        
        END: ;

        switch (tagObject)
        {
            case "b":
                return new BBCodeInstruction(InstructionType.Bold, exiting);
            case "i":
                return new BBCodeInstruction(InstructionType.Italic, exiting);
            case "u":
                return new BBCodeInstruction(InstructionType.Underline, exiting);
            case "color":
                if (exiting)
                    return new ColorInstruction(Color.Transparent, true);
                int hexValue = int.Parse(tag[startPos..].TrimStart('#'), NumberStyles.HexNumber);
                Color color = new Color(((uint) hexValue << 8) | 255);
                return new ColorInstruction(color, false);
        }

        return null;
    }
}