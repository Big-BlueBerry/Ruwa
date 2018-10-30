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
        ValueLiteral
    }

    abstract class Token
    {
        public TokenType Type { get; set; }
        public Token() => Type = TokenType.EmptyToken;
        public virtual string ToString() => Type.ToString();
    }

    class SongNameKeyword : Token
    {
        public SongNameKeyword()
        {
            Type = TokenType.SongNameKeyword;
        }
    }
    class SongComposerKeyword : Token
    {
        public SongComposerKeyword()
        {
            Type = TokenType.SongComposerKeyword;
        }
    }
    class SheetComposerKeyword : Token
    {
        public SheetComposerKeyword()
        {
            Type = TokenType.SheetComposerKeyword;
        }
    }

    class BarToken : Token
    {
        public BarToken()
        {
            Type = TokenType.BarToken;
        }
    }
    class LineToken : Token
    {
        public LineToken()
        {
            Type = TokenType.LineToken;
        }
    }
    class ColonToken : Token
    {
        public ColonToken()
        {
            Type = TokenType.ColonToken;
        }
    }
    class CommaToken : Token
    {
        public CommaToken()
        {
            Type = TokenType.CommaToken;
        }
    }
    class EqualToken : Token
    {
        public EqualToken()
        {
            Type = TokenType.EqualToken;
        }
    }

    class ValueLiteral<T> : Token
    {
        public T Value { get; set; }
        public ValueLiteral(T value)
        {
            this.Value = value;
            this.Type = TokenType.ValueLiteral;
        }
        public override string ToString() => $"Value {Value}"; 
    }
}
