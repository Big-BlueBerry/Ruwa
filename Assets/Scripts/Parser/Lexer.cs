using System;
using System.Collections.Generic;

namespace Ruwa.Objects
{
    class Lexer
    {
        private string SheetFile;

        private int Index = 0;
        private char Peek => SheetFile[Index];

        private char Pop() => SheetFile[Index++];
        private bool IsEof => SheetFile.Length <= Index;
        private bool IsWhitespace(char letter)
        {
            if (letter == ' ') return true;
            else if (letter == '\t') return true;
            else if (letter == '\r') return true;
            else if (letter == '\n') return true;
            else if (letter == '\t') return true;
            else return false;
        }

        public List<Token> TokenList = new List<Token>();

        public Lexer(string sheetFile)
        {
            SheetFile = sheetFile;
        }

        public List<Token> Lex()
        {
            while (!IsEof)
            {
                switch (Peek)
                {
                    case '~':
                        Pop();
                        TokenList.Add(new BarToken());
                        break;
                    
                    case '-':
                        LineComsumer();
                        break;

                    case ',':
                        Pop();
                        TokenList.Add(new CommaToken());
                        break;

                    case ':':
                        Pop();
                        TokenList.Add(new ColonToken());
                        break;

                    case '\n':
                    case '\r':
                    case '\t':
                    case ' ':
                        SpaceComsumer();
                        break;

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        LexNumber();
                        break;

                    case '\"':
                        LexString();
                        break;

                    case '=':
                        Pop();
                        TokenList.Add(new EqualToken());
                        break;

                    default:
                        if (char.IsLetterOrDigit(Peek))
                        {
                            LexKeyword();
                        }
                        else
                        {
                            throw new Exception($"Undeclared letter: {Peek}");
                        }
                        break;
                }
            }

            return TokenList;
        }
        private void SpaceComsumer()
        {
            while (!IsEof && IsWhitespace(Peek))
            {
                Pop();
            }
        }
        private void LineComsumer()
        {
            while (Peek == '-') Pop();
            TokenList.Add(new LineToken());
        }
        private void LexKeyword()
        {
            int beginIndex = Index;
            int endIndex = Index;
            while (!IsEof)
            {
                if (Peek != '_' && !char.IsLetterOrDigit(Peek)) break;
                Pop();
                endIndex++;
            }
            int lenght = endIndex - beginIndex;
            string text = SheetFile.Substring(beginIndex, lenght);

            switch (text)
            {
                case "song_name":
                    TokenList.Add(new SongNameKeyword());
                    break;
                case "song_composer":
                    TokenList.Add(new SongComposerKeyword());
                    break;
                case "sheet_composer":
                    TokenList.Add(new SheetComposerKeyword());
                    break;
                default:
                    throw new Exception($"{text}는 뭐죠??");
            }

        }
        private void LexNumber()
        {
            int beginIndex = Index;
            int endIndex = Index;
            while (!IsEof && char.IsDigit(Peek))
            {
                Pop();
                endIndex++;
            }
            int lenght = endIndex - beginIndex;
            TokenList.Add(new ValueLiteral<int>(int
                .Parse(SheetFile
                .Substring(beginIndex, lenght))));
        }
        private void LexString()
        {
            Pop();
            int beginIndex = Index;
            int endIndex = Index;
            while (Peek != '\"')
            {
                if (IsEof)
                {
                    throw new Exception("아직 스트링이 끝난게 아닌데.... 파일이 끝났네요....");
                }
                Pop();
                endIndex++;
            }
            Pop();
            int lenght = endIndex - beginIndex;
            TokenList.Add(new ValueLiteral<string>(SheetFile.Substring(beginIndex, lenght)));
        }
    }
}