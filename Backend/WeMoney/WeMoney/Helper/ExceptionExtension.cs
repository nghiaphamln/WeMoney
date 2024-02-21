namespace WeMoney.Helper;

public static class ExceptionExtension
{
    public static string GetTrimmedStackTrace(this string stackTrace)
    {
        var lines = stackTrace.Split('\n');
        var relevantLines = lines.Where(line => line.Contains(".cs:") || line.Contains(".vb:"));
        return string.Join('\n', relevantLines);
    }
}