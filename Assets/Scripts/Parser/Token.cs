namespace Ruwa.Objects
{
    public enum TokenType
    {
        SongNameKeyword,
        SongComposerKeyword,
        SheetComposerKeyword,
        BarToken,
        LineToken,
        ColonToken,
        CommaToken,
        EqualToken,
        EmptyToken,
        NumberLiteral,
        StringLiteral,
    }
    abstract class Token
    {
        public TokenType Type { get; set; }
        public Token() => Type = TokenType.EmptyToken;
    }
    class SongNameKeyword : Token
    {
        public SongNameKeyword()
        {
            Type = TokenType.SongNameKeyword;
        }
        public override string ToString()
        {
            return "SongNameKeyword";
        }
    }
    class SongComposerKeyword : Token
    {
        public SongComposerKeyword()
        {
            Type = TokenType.SongComposerKeyword;
        }
        public override string ToString()
        {
            return "SongComposerKeyword";
        }
    }
    class SheetComposerKeyword : Token
    {
        public SheetComposerKeyword()
        {
            Type = TokenType.SheetComposerKeyword;
        }
        public override string ToString()
        {
            return "SheetComposerKeyword";
        }
    }
    class BarToken : Token
    {
        public BarToken()
        {
            Type = TokenType.BarToken;
        }
        public override string ToString()
        {
            return "Bar";
        }
    }
    class LineToken : Token
    {
        public LineToken()
        {
            Type = TokenType.LineToken;
        }
        public override string ToString()
        {
            return "Line";
        }
    }
    class ColonToken : Token
    {
        public ColonToken()
        {
            Type = TokenType.ColonToken;
        }
        public override string ToString()
        {
            return "Colon";
        }
    }
    class CommaToken : Token
    {
        public CommaToken()
        {
            Type = TokenType.CommaToken;
        }
        public override string ToString()
        {
            return "Comma";
        }
    }
    class EqualToken : Token
    {
        public EqualToken()
        {
            Type = TokenType.EqualToken;
        }
        public override string ToString()
        {
            return "Equal";
        }
    }
    class NumberLiteral : Token
    {
        public int Value { get; set; }
        public NumberLiteral(int value)
        {
            Type = TokenType.NumberLiteral;
            Value = value;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
    class StringLiteral : Token
    {
        public string Value { get; set; }
        public StringLiteral(string value)
        {
            Type = TokenType.StringLiteral;
            Value = value;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
