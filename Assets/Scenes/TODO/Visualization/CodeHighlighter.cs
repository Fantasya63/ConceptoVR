using System.Text.RegularExpressions;

public static class CodeHighlighter
{
    // ====== COLORS ======
    private const string KeywordColor = "#569CD6";
    private const string TypeColor = "#4EC9B0";
    private const string StringColor = "#CE9178";
    private const string NumberColor = "#B5CEA8";
    private const string CommentColor = "#6A9955";
    private const string FunctionColor = "#DCDCAA";
    private const string DefaultColor = "#D4D4D4";

    // ====== KEYWORDS ======
    private static readonly string[] Keywords =
    {
        "public", "private", "protected", "class", "Node", "void", "int", "var",
        "float", "double", "bool", "string", "if", "else",
        "for", "while", "return", "new", "static", "using",
        "namespace", "null", "true", "false"
    };

    public static string Highlight(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";

        // MULTILINE COMMENT
        input = Regex.Replace(input,
            @"/\*.*?\*/",
            match => Wrap(match.Value, CommentColor),
            RegexOptions.Singleline);

        // SINGLE LINE COMMENT
        input = Regex.Replace(input,
            @"//.*",
            match => Wrap(match.Value, CommentColor));

        // STRINGS
        input = Regex.Replace(input,
            "\".*?\"",
            match => Wrap(match.Value, StringColor));

        // NUMBERS
        input = Regex.Replace(input,
            @"\b\d+(\.\d+)?\b",
            match => Wrap(match.Value, NumberColor));

        // KEYWORDS
        foreach (var keyword in Keywords)
        {
            input = Regex.Replace(input,
                $@"\b{keyword}\b",
                Wrap(keyword, KeywordColor));
        }

        // FUNCTION CALLS (identifier followed by parenthesis)
        input = Regex.Replace(input,
            @"\b([a-zA-Z_][a-zA-Z0-9_]*)\s*(?=\()",
            match => Wrap(match.Value, FunctionColor));

        return input;
    }

    private static string Wrap(string text, string color)
    {
        return $"<color={color}>{text}</color>";
    }
}