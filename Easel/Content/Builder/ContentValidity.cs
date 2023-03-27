using System;

namespace Easel.Content.Builder;

public class ContentValidity
{
    public readonly bool IsValid;

    public readonly Exception Exception;

    public ContentValidity(bool isValid, Exception exception)
    {
        IsValid = isValid;
        Exception = exception;
    }
}