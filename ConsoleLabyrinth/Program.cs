using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleLabyrinth
{
	internal class Program
	{
		private const int Delay = 200;
		private const int Cols = 15;
		private const int Rows = 14;
		private static readonly CursorCoords Origin = new CursorCoords(10, 4);

		private readonly static int[,] Labyrinth = new int[Rows, Cols]
			{
				{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				{ 0, 3, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},
				{ 0, 2, 3, 1, 1, 1, 1, 0, 3, 1, 1, 1, 1, 0, 2},
				{ 0, 2, 2, 1, 1, 3, 1, 0, 2, 1, 1, 3, 1, 1, 2},
				{ 0, 2, 1, 1, 2, 2, 3, 1, 1, 1, 2, 2, 3, 0, 2},
				{ 0, 3, 1, 0, 2, 2, 2, 1, 3, 0, 2, 0, 2, 1, 2},
				{ 0, 2, 3, 1, 2, 2, 1, 2, 2, 3, 1, 3, 1, 0, 2},
				{ 0, 2, 2, 2, 2, 3, 1, 2, 2, 2, 1, 0, 3, 1, 2},
				{ 0, 2, 2, 1, 0, 2, 2, 2, 2, 1, 1, 3, 1, 2, 2},
				{ 0, 2, 1, 1, 1, 0, 2, 0, 3, 0, 2, 2, 2, 0, 2},
				{ 0, 2, 3, 1, 1, 3, 3, 3, 0, 3, 0, 2, 1, 1, 2},
				{ 0, 2, 2, 1, 2, 2, 2, 2, 3, 0, 3, 1, 1, 2, 2},
				{ 0, 2, 1, 0, 2, 0, 2, 0, 2, 1, 0, 3, 0, 0, 2},
				{ 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0},
			};

		private readonly struct CursorCoords
		{
			public readonly int X;
			public readonly int Y;

			public CursorCoords(int x, int y)
			{
				X = x; Y = y;
			}
		}

		public enum Direction
		{
			Top,
			Left,
			Right,
			Bottom,
		}

		private class Traveller
		{
			private readonly int[,] _labyrinth;
			private readonly int _rows;
			private readonly int _cols;

			private readonly CursorCoords _origin;

			private readonly int _minColInside;
			private readonly int _maxColInside;
			private readonly int _minRowInside;
			private readonly int _maxRowInside;

			public Traveller(int minRowInside, int minColInside, int maxRowInside, int maxColIndide, CursorCoords origin, int[,] labyrinth, int rows, int cols)
			{
				_labyrinth = labyrinth;
				_rows = rows;
				_cols = cols;

				_origin = origin;

				_minRowInside = minRowInside;
				_minColInside = minColInside;
				_maxRowInside = maxRowInside;
				_maxColInside = maxColIndide;
			}

			public int Row { get; set; }
			public int Col { get; set; }
			public Direction Dir { get; set; }

			private int _priorRow;
			private int _priorCol;

			public void Draw()
			{
				//var tmpX = Console.CursorLeft;
				//var tmpY = Console.CursorTop;

				var symbol = GetSymborByDir(Dir);

				int x = _origin.X + Col * 4 + 2;
				int y = _origin.Y + Row * 2 + 1;

				Console.SetCursorPosition(x, y);

				Console.Write(symbol);

				//Console.SetCursorPosition(tmpX, tmpY);
			}

			public void ClearPrior()
			{
				int x = _origin.X + _priorCol * 4 + 2;
				int y = _origin.Y + _priorRow * 2 + 1;

				Console.SetCursorPosition(x, y);

				Console.Write(' ');
			}

			public Direction TurnLeft()
			{
				switch (Dir)
				{
					case Direction.Top:
						return Dir = Direction.Left;

					case Direction.Left:
						return Dir = Direction.Bottom;

					case Direction.Bottom:
						return Dir = Direction.Right;

					case Direction.Right:
					default:
						return Dir = Direction.Top;
				}
			}

			public Direction TurnRight()
			{
				switch (Dir)
				{
					case Direction.Top:
						return Dir = Direction.Right;

					case Direction.Right:
						return Dir = Direction.Bottom;

					case Direction.Bottom:
						return Dir = Direction.Left;

					case Direction.Left:
					default:
						return Dir = Direction.Top;
				}
			}

			public bool IsThereWallAtLeftSide()
			{
				switch (Dir)
				{
					case Direction.Top:
						return IsThereWallLeft(_labyrinth, _rows, _cols, Row, Col);

					case Direction.Right:
						return IsThereWallAbove(_labyrinth, _rows, _cols, Row, Col);

					case Direction.Bottom:
						return IsThereWallRight(_labyrinth, _rows, _cols, Row, Col);

					case Direction.Left:
					default:
						return IsThereWallBottom(_labyrinth, _rows, _cols, Row, Col);
				}
			}

			public bool IsOut()
			{
				return Row < _minRowInside || Row > _maxRowInside || Col < _minColInside || Col > _maxColInside;
			}

			public bool CanMoveForward()
			{
				switch (Dir)
				{
					case Direction.Top:
						return !IsThereWallAbove(_labyrinth, _rows, _cols, Row, Col);

					case Direction.Left:
						return !IsThereWallLeft(_labyrinth, _rows, _cols, Row, Col);

					case Direction.Right:
						return !IsThereWallRight(_labyrinth, _rows, _cols, Row, Col);

					case Direction.Bottom:
					default:
						return !IsThereWallBottom(_labyrinth, _rows, _cols, Row, Col);
				}
			}

			public void MoveForward()
			{
				_priorRow = Row;
				_priorCol = Col;

				switch (Dir)
				{
					case Direction.Top:
						--Row;
						break;

					case Direction.Left:
						--Col;
						break;

					case Direction.Right:
						++Col;
						break;

					case Direction.Bottom:
					default:
						++Row;
						break;
				}
			}

			private static char GetSymborByDir(Direction dir)
			{
				switch (dir)
				{
					case Direction.Top:
						return '▲';

					case Direction.Left:
						return '◄';

					case Direction.Right:
						return '►';

					case Direction.Bottom:
					default:
						return '▼';
				}
			}
		}

		static void Main(string[] args)
		{
			Console.CursorVisible = false;
			Console.OutputEncoding = Encoding.UTF8;
			Console.Clear();

			//Thread.Sleep(20000);

			for (int row = 0; row < Rows; ++row)
				for (int col = 0; col < Cols; ++col)
				{
					DrawCell(Origin, Labyrinth, Rows, Cols, row, col);
				}

			Traveller traveller = new Traveller(1, 1, 13, 12, Origin, Labyrinth, Rows, Cols)
			{
				Col = 11,
				Row = 13,
				Dir = Direction.Top
			};

			RedrawTraveler(traveller);

			if (traveller.CanMoveForward())
				traveller.MoveForward();

			RedrawTraveler(traveller);

			while (!traveller.IsOut())
			{
				if (!traveller.IsThereWallAtLeftSide())
				{
					traveller.TurnLeft();
					RedrawTraveler(traveller);
				}

				while (!traveller.CanMoveForward())
				{
					traveller.TurnRight();
					RedrawTraveler(traveller);
				}

				traveller.MoveForward();
				RedrawTraveler(traveller);

			}

			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.SetCursorPosition(20, 2);
			Console.Write("Ура-ура!");

			Console.ReadKey();
		}

		//static void Main(string[] args)
		//{
		//	Console.CursorVisible = false;
		//	Console.OutputEncoding = Encoding.UTF8;
		//	Console.Clear();

		//	Thread.Sleep(20000);

		//	for (int row = 0; row < Rows; ++row)
		//		for (int col = 0; col < Cols; ++col)
		//		{
		//			DrawCell(Origin, Labyrinth, Rows, Cols, row, col);
		//		}

		//	Traveller traveller = new Traveller(1, 1, 13, 12, Origin, Labyrinth, Rows, Cols)
		//	{
		//		Col = 11,
		//		Row = 13,
		//		Dir = Direction.Top
		//	};

		//	RedrawTraveler(traveller);

		//	if (traveller.CanMoveForward())
		//		traveller.MoveForward();

		//	RedrawTraveler(traveller);

		//	while (!traveller.IsOut())
		//	{
		//		if (traveller.IsThereWallAtLeftSide())
		//		{
		//			while (!traveller.CanMoveForward())
		//			{
		//				traveller.TurnRight();
		//				RedrawTraveler(traveller);
		//			}

		//			traveller.MoveForward();
		//			RedrawTraveler(traveller);
		//		}
		//		else
		//		{
		//			traveller.TurnLeft();
		//			RedrawTraveler(traveller);
		//		}

		//	}

		//	Console.ForegroundColor = ConsoleColor.Cyan;
		//	Console.SetCursorPosition(20, 2);
		//	Console.Write("Ура-ура!");

		//	Console.ReadKey();
		//}

		private static void RedrawTraveler(Traveller traveller)
		{
			Thread.Sleep(Delay);

			if (traveller == null)
				return;

			var foreColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;

			traveller.ClearPrior();
			traveller.Draw();

			Console.ForegroundColor = foreColor;
		}

		private static bool IsThereWallAbove(int[,] labyrinth, int rows, int cols, int row, int col)
		{
			if (col < 0 || col >= cols)
				return false;

			if (row <= 0 || row >= rows)
				return false;

			int cellFlags = labyrinth[row, col];
			return cellFlags == 1 || cellFlags == 3;
		}

		private static bool IsThereWallBottom(int[,] labyrinth, int rows, int cols, int row, int col)
		{
			if (col < 0 || col >= cols)
				return false;

			if (row < 0 || row >= rows - 1)
				return false;

			int bottomCellFlags = labyrinth[row + 1, col];
			return bottomCellFlags == 1 || bottomCellFlags == 3;
		}

		private static bool IsThereWallLeft(int[,] labyrinth, int rows, int cols, int row, int col)
		{
			if (row < 0 || row >= rows)
				return false;

			if (col <= 0 || col >= cols)
				return false;

			int cellFlags = labyrinth[row, col];
			return cellFlags == 2 || cellFlags == 3;
		}

		private static bool IsThereWallRight(int[,] labyrinth, int rows, int cols, int row, int col)
		{
			if (row < 0 || row >= rows)
				return false;

			if (col < 0 || col >= cols - 1)
				return false;

			int rightCellFlags = labyrinth[row, col + 1];
			return rightCellFlags == 2 || rightCellFlags == 3;
		}

		private static CursorCoords DrawCell(CursorCoords origin, int[,] labyrinth, int rows, int cols, int row, int col)
		{
			if (row < 0 || col < 0 || row >= rows || col >= cols)
				return origin;

			int cellFlags = labyrinth[row, col];
			bool top = cellFlags == 1 || cellFlags == 3;
			bool left = cellFlags == 2 || cellFlags == 3;

			int x = origin.X + col * 4;
			int y = origin.Y + row * 2;

			Console.SetCursorPosition(x, y);
			char cornerSymbol = GetTopLeftCorner(labyrinth, rows, cols, row, col);
			Console.Write(cornerSymbol);
			if (top)
			{
				Console.SetCursorPosition(x + 1, y);
				Console.Write("━━━");
			}
			if (left)
			{
				Console.SetCursorPosition(x, y + 1);
				Console.Write('┃');
			}

			return new CursorCoords(x + 2, y + 1);
		}

		private static char GetTopLeftCorner(int[,] labyrinth, int rows, int cols, int row, int col)
		{

			if (row < 0 || col < 0 || row >= rows || col >= cols)
				return ' ';

			int cellFlags = labyrinth[row, col];
			int leftCellFlags = col > 0 ? labyrinth[row, col - 1] : 0;
			int topCellFlags = row > 0 ? labyrinth[row - 1, col] : 0;

			bool top = cellFlags == 1 || cellFlags == 3;
			bool left = cellFlags == 2 || cellFlags == 3;
			bool leftOut = leftCellFlags == 1 || leftCellFlags == 3;
			bool topOut = topCellFlags == 2 || topCellFlags == 3;

			if (top && left)
			{
				if (topOut && leftOut)
					return '╋';

				if (topOut && !leftOut)
					return '┣';

				if (!topOut && leftOut)
					return '┳';

				return '┏';
			}

			if (top && !left)
			{
				if (topOut && leftOut)
					return '┻';

				if (topOut && !leftOut)
					return '┗';

				if (!topOut && leftOut)
					return '━';

				return '╺';
			}

			if (!top && left)
			{
				if (topOut && leftOut)
					return '┫';

				if (topOut && !leftOut)
					return '┃';

				if (!topOut && leftOut)
					return '┓';

				return '╻';
			}

			//if (!top && !left)
			{
				if (topOut && leftOut)
					return '┛';

				if (topOut && !leftOut)
					return '╹';

				if (!topOut && leftOut)
					return '╸';

				return ' ';
			}
		}
	}
}
