using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruwa.Objects
{
    class Parser
    {
        private Token[] TokenArray;
        private Dictionary<int, Point> HoldPoint = new Dictionary<int, Point>();
        private Dictionary<int, List<Point>> SlidePoint = new Dictionary<int, List<Point>>();
        private Dictionary<int, Point> AirLongPoint = new Dictionary<int, Point>();

        private int Index = 0;
        private Token Peek => TokenArray[Index];
        private Token Pop() => TokenArray[Index++];
        private bool IsEndOfToken => TokenArray.Length <= Index;
        private enum GameObjectType
        {
            Tab = 0,
            Hold = 1,
            Slide = 2,
            AirShort = 10,
            AirLong = 11,
            AirMove = 12
        };
        private enum GameObjectAttibuteType
        {
            Begin = 0,
            End = 1,
            Point = 2,
            Left = 10,
            Center = 11,
            Right = 12
        };
        public Parser(List<Token> tokenList)
        {
            TokenArray = tokenList.ToArray();
        }

        /// <summary>
        /// Predict a Peek token
        /// </summary>
        /// <returns>Return eaten token</returns>
        private Token Eat(TokenType type)
        {
            Token token = Peek;
            if(token.Type == type)
            {
                return Pop();
            }
            throw new Exception($"Unexpected token {token}");
        }

        /// <summary>
        /// Parse tokens
        /// </summary>
        /// <returns>Parsed song</returns>
        public Song Parse()
        {
            Song song = new Song();

            song.SongMetadata = ParseMetadata();
            Eat(TokenType.LineToken);
            song.SongSheet = ParseSheet();

            return song;
        }

        /// <summary>
        /// Remove all dump points
        /// </summary>
        /// <returns>Dump points removed point list</returns>
        public Song PostProcess(Song song)
        {
            Func<GameObject, bool> isNotDump = (x) => x is Holdable holdable ? !holdable.IsDump : true; 
            var gameObject = song.SongSheet.Points;
            var result = gameObject.Where(isNotDump).ToList();

            Song resultSong = new Song();
            resultSong.SongMetadata = song.SongMetadata;
            resultSong.SongSheet = new Sheet();
            resultSong.SongSheet.Points = result;
            return resultSong;
        }
        #region Parse
        private Metadata ParseMetadata()
        {
            Metadata metadata = new Metadata();
            List<BPMData> bpmDataList = new List<BPMData>();

            Eat(TokenType.SongNameKeyword);
            Eat(TokenType.EqualToken);
            var songName = Eat(TokenType.StringLiteral) as StringLiteral;
            metadata.SongName = songName.Value;

            Eat(TokenType.SongComposerKeyword);
            Eat(TokenType.EqualToken);
            var songComposer = Eat(TokenType.StringLiteral) as StringLiteral;
            metadata.SongComposer = songComposer.Value;

            Eat(TokenType.SheetComposerKeyword);
            Eat(TokenType.EqualToken);
            var sheetComposer = Eat(TokenType.StringLiteral) as StringLiteral;
            metadata.SheetComposer = sheetComposer.Value;
            
            while (true)
            {
                if (Peek is LineToken) break;
                bpmDataList.Add(ParseBPMData());
            }
            metadata.BPMDatas = bpmDataList;

            return metadata;
        }
        private BPMData ParseBPMData()
        {
            BPMData bpmData = new BPMData();

            var bar = Eat(TokenType.NumberLiteral) as NumberLiteral;
            bpmData.Bar = bar.Value;

            Eat(TokenType.BarToken);

            var bpm = Eat(TokenType.NumberLiteral) as NumberLiteral;
            bpmData.BPM = bpm.Value;

            return bpmData;
        }
        private Sheet ParseSheet()
        {
            Sheet sheet = new Sheet();
            sheet.Points = new List<GameObject>();

            while (!IsEndOfToken)
            {
                var gameObject = ParseGameObject();
                sheet.Points.Add(gameObject);
            }

            return sheet;
        }
        private GameObject ParseGameObject()
        {
            NumberLiteral TypeToken = Peek as NumberLiteral;
            switch (TypeToken.Value)
            {
                case (int)GameObjectType.Tab:
                    return ParseTab();

                case (int)GameObjectType.Hold:
                    return ParseHold();

                case (int)GameObjectType.Slide:
                    return ParseSlide();

                case (int)GameObjectType.AirShort:
                    return ParseAirShort();

                case (int)GameObjectType.AirLong:
                    return ParseAirLong();

                case (int)GameObjectType.AirMove:
                    return ParseAirMove();

                default:
                    throw new Exception($"Unexcepted Object Type {TypeToken}");
            }
        }
        private Tab ParseTab()
        {
            Tab tab = new Tab();
            tab.BeginPoint = ParsePoint();
            return tab;
        }
        private Hold ParseHold()
        {
            Point point = new Point();
            point = ParsePoint();

            Eat(TokenType.ColonToken);
            NumberLiteral attribute = Eat(TokenType.NumberLiteral) as NumberLiteral;
            Eat(TokenType.CommaToken);

            NumberLiteral id = Eat(TokenType.NumberLiteral) as NumberLiteral;

            // 끝점일 때, 시작점 불러와 반환
            if (HoldPoint.ContainsKey(id.Value))
            {
                Hold hold = new Hold();
                hold.BeginPoint = HoldPoint[id.Value];
                hold.EndPoint = point;

                HoldPoint.Remove(id.Value);
                return hold;
            }

            // 시작점일 때, 점 저장
            else if (attribute.Value == (int)GameObjectAttibuteType.Begin)
            {
                HoldPoint.Add(id.Value, point);
                Hold hold = new Hold();
                hold.IsDump = true;
                return hold;
            }

            // 폭발
            else
            {
                throw new Exception($"Unexcepted end token {point}");
            }
        }
        private Slide ParseSlide()
        {
            Point point = new Point();
            point = ParsePoint();

            Eat(TokenType.ColonToken);
            NumberLiteral attribute = Eat(TokenType.NumberLiteral) as NumberLiteral;
            Eat(TokenType.CommaToken);

            NumberLiteral id = Eat(TokenType.NumberLiteral) as NumberLiteral;

            // 끝점일 때, 값 불러와 시작점, 끝점 기록후 중간 점은 리스트에 담아 리턴
            if (attribute.Value == (int)GameObjectAttibuteType.End 
                && SlidePoint.ContainsKey(id.Value))
            {
                Slide slide = new Slide();
                slide.EndPoint = point;

               
                var slideList = SlidePoint[id.Value];
                slide.BeginPoint = slideList.First();
                slideList.RemoveAt(0);

                slide.Points = slideList;
                SlidePoint.Remove(id.Value);

                return slide;
            }

            // 점일 때, 슬라이드 중간점 리스트에 삽입
            else if (attribute.Value == (int)GameObjectAttibuteType.Point)
            {
                SlidePoint[id.Value].Add(point);
                Slide slide = new Slide();
                slide.IsDump = true;
                return slide;
            }

            // 시작점일 때, 시작점 저장.
            else if (attribute.Value == (int)GameObjectAttibuteType.Begin)
            {                
                SlidePoint[id.Value] = new List<Point>();
                SlidePoint[id.Value].Add(point);

                Slide slide = new Slide();
                slide.IsDump = true;
                return slide;
            }

            // 폭발
            else
            {
                throw new Exception($"Unexcepted end token {point}");
            }
        }
        private AirShort ParseAirShort()
        {
            AirShort airShort = new AirShort();
            airShort.BeginPoint = ParsePoint();
            return airShort;
        }
        private AirLong ParseAirLong()
        {
            Point point = new Point();
            point = ParsePoint();

            Eat(TokenType.ColonToken);
            NumberLiteral attribute = Eat(TokenType.NumberLiteral) as NumberLiteral;
            Eat(TokenType.CommaToken);

            NumberLiteral id = Eat(TokenType.NumberLiteral) as NumberLiteral;

            // 끝점일 때, 시작점 불러와 작성후 리턴
            if (AirLongPoint.ContainsKey(id.Value))
            {
                AirLong airLong = new AirLong();
                airLong.BeginPoint = AirLongPoint[id.Value];
                airLong.EndPoint = point;

                AirLongPoint.Remove(id.Value);
                return airLong;
            }

            // 시작점일 때, 시작점 기록후 덤프값 리턴
            else if (attribute.Value == (int)GameObjectAttibuteType.Begin)
            {
                AirLongPoint.Add(id.Value, point);
                AirLong airLong = new AirLong();
                airLong.IsDump = true;
                return airLong;
            }

            // 시작점이 없는데 끝점이 나올때, 폭발
            else
            {
                throw new Exception($"Unexcepted end token {point}");
            }
        }
        private AirMove ParseAirMove()
        {
            AirMove airMove = new AirMove();
            airMove.BeginPoint = ParsePoint();
            Eat(TokenType.ColonToken);
            var direction = Eat(TokenType.NumberLiteral) as NumberLiteral;


            switch (direction.Value)
            {
                case 0:
                    airMove.Direction = AirMoveDirection.Left;
                    break;
                case 1:
                    airMove.Direction = AirMoveDirection.Center;
                    break;
                case 2:
                    airMove.Direction = AirMoveDirection.Right;
                    break;
                default:
                    throw new Exception($"Unexpected airmove direction {direction.Value}");
            }

            return airMove;
        }
        private Point ParsePoint()
        {
            Point point = new Point();
            Eat(TokenType.NumberLiteral);
            Eat(TokenType.CommaToken);

            NumberLiteral bar = Eat(TokenType.NumberLiteral) as NumberLiteral;
            point.Bar = bar.Value;
            Eat(TokenType.CommaToken);

            NumberLiteral curBeat = Eat(TokenType.NumberLiteral) as NumberLiteral;
            point.CurBeat = curBeat.Value;
            Eat(TokenType.CommaToken);

            NumberLiteral fullBeat = Eat(TokenType.NumberLiteral) as NumberLiteral;
            point.FullBeat = fullBeat.Value;
            Eat(TokenType.CommaToken);

            NumberLiteral postion = Eat(TokenType.NumberLiteral) as NumberLiteral;
            point.Position = postion.Value;
            Eat(TokenType.CommaToken);

            NumberLiteral size = Eat(TokenType.NumberLiteral) as NumberLiteral;
            point.Size = size.Value;

            return point;
        }
        #endregion Parse
    }
}
