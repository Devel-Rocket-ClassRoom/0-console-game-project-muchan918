using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Framework.Engine
{
    public class ScreenBuffer
    {
        private readonly int _width;
        private readonly int _height;
        
        // 그리고 싶은 데이터
        private char[,] _chars;
        private ConsoleColor[,] _fgColors; // 글자 색
        private ConsoleColor[,] _bgColors; // 글자 배경 색
        private readonly StringBuilder _frameBuilder;

        // 글자의 색상을 이스케이프 시퀀스를 통해 정해준다
        // HTML에서 글자 bold하고 기울이고 하는 것처럼 쓰기 위해 선언
        private static readonly int[] s_ansiFg = { 30, 34, 32, 36, 31, 35, 33, 37, 90, 94, 92, 96, 91, 95, 93, 97 };
        private static readonly int[] s_ansiBg = { 40, 44, 42, 46, 41, 45, 43, 47, 100, 104, 102, 106, 101, 105, 103, 107 };

        public int Width => _width / 2;
        public int Height => _height;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr handle, out uint mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr handle, uint mode);

        public ScreenBuffer(int width, int height)
        {
            _width = width * 2;
            _height = height;
            _chars = new char[_height, _width];
            _fgColors = new ConsoleColor[_height, _width];
            _bgColors = new ConsoleColor[_height, _width];
            _frameBuilder = new StringBuilder(_width * _height * 4);
            Clear();
            EnableVirtualTerminalProcessing();
        }

        // 색상 바꾸기 위해 윈도우 설정을 바꿔줘야하는데 그 기능을 하는 API함수
        private static void EnableVirtualTerminalProcessing()
        {
            try
            {
                const int STD_OUTPUT_HANDLE = -11;
                const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

                IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
                if (GetConsoleMode(handle, out uint mode))
                {
                    SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
                }
            }
            catch
            {
                // Non-Windows or unsupported — ANSI may already work
            }
        }

        public void Clear()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _chars[y, x] = ' ';
                    _fgColors[y, x] = ConsoleColor.Gray;
                    _bgColors[y, x] = ConsoleColor.Black;
                }
            }
        }

        // 화면에 출력해주는 함수
        public void SetCell(int x, int y, char ch, ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            int px = 2 * x;
            if (px >= 0 && px < _width && y >= 0 && y < _height)
            {
                _chars[y, px] = ch;
                _fgColors[y, px] = color;
                _bgColors[y, px] = bgColor;
            }
        }

        public void WriteText(int x, int y, string text, ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            for (int i = 0; i < text.Length; i++)
            {
                SetCell(x + i, y, text[i], color, bgColor);
            }
        }

        public void WriteTextCentered(int y, string text, ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            int x = (_width/2/2 - text.Length/2);
            WriteText(x, y, text, color, bgColor);
        }

        public void WriteLines(int x, int y, string[] lines, ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                WriteText(x, y + i, lines[i], color, bgColor);
            }
        }

        public void DrawHLine(int x, int y, int length, char ch = '─', ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            for (int i = 0; i < length; i++)
            {
                SetCell(x + i, y, ch, color, bgColor);
            }
        }

        public void DrawVLine(int x, int y, int length, char ch = '|', ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            for (int i = 0; i < length; i++)
            {
                SetCell(x, y + i, ch, color, bgColor);
            }
        }

        public void DrawBox(int x, int y, int width, int height, ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            SetCell(x, y, '+', color, bgColor);
            SetCell(x + width - 1, y, '+', color, bgColor);
            SetCell(x, y + height - 1, '+', color, bgColor);
            SetCell(x + width - 1, y + height - 1, '+', color, bgColor);

            DrawHLine(x + 1, y, width - 2, '─', color, bgColor);
            DrawHLine(x + 1, y + height - 1, width - 2, '─', color, bgColor);
            DrawVLine(x, y + 1, height - 2, '|', color, bgColor);
            DrawVLine(x + width - 1, y + 1, height - 2, '|', color, bgColor);
        }

        public void FillRect(int x, int y, int width, int height, char ch = ' ', ConsoleColor color = ConsoleColor.Gray, ConsoleColor bgColor = ConsoleColor.Black)
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    SetCell(x + col, y + row, ch, color, bgColor);
                }
            }
        }

        public void Present()
        {
            _frameBuilder.Clear();
            _frameBuilder.Append("\x1b[H");

            ConsoleColor currentFg = (ConsoleColor)(-1);
            ConsoleColor currentBg = (ConsoleColor)(-1);

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    ConsoleColor fg = _fgColors[y, x];
                    ConsoleColor bg = _bgColors[y, x];

                    if (fg != currentFg || bg != currentBg)
                    {
                        _frameBuilder.Append("\x1b[");
                        _frameBuilder.Append(s_ansiFg[(int)fg]);
                        _frameBuilder.Append(';');
                        _frameBuilder.Append(s_ansiBg[(int)bg]);
                        _frameBuilder.Append('m');
                        currentFg = fg;
                        currentBg = bg;
                    }

                    _frameBuilder.Append(_chars[y, x]);
                }

                if (y < _height - 1)
                {
                    _frameBuilder.Append('\n');
                }
            }

            _frameBuilder.Append("\x1b[0m");
            Console.Write(_frameBuilder.ToString()); // 한번에 그리기
        }
    }
}
