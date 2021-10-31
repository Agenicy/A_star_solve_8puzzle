using System;
using System.Collections.Generic;

namespace A_Star_8_Puzzle
{
	struct Position
	{
		public int x, y;
		public Position(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
		public static Position operator +(Position a, Position b) => new Position(b.x + a.x, b.y + a.y);
	}

	/// <summary>
	/// Board Data, 1 to max as block, 0 as hole.
	/// </summary>
	struct Board
	{
		public short[,] board;
		public readonly static short[,] board_Solved = new short[,] { { 1, 2, 3 }, { 8, 0, 4 }, { 7, 6, 5 } };
		public int manhattanDistance;
		Position Position0;
		public Board(short[,] board)
		{
			this.board = board.Clone() as short[,];

			Position0 = new Position(-1, -1);
			manhattanDistance = -1;
			CountManhattanDistance();
		}
		public void Swap(Position posA, Position posB)
		{
			(board[posA.x, posA.y], board[posB.x, posB.y]) = (board[posB.x, posB.y], board[posA.x, posA.y]);
		}

		public override string ToString()
		{
			string ret = "";
			for (short x = 0; x < board.GetLength(0); x++)
				for (short y = 0; y < board.GetLength(1); y++)
					ret += $"{board[x, y]}";
			return ret;
		}


		/// <summary>
		/// 計算並更新 Manhattan Distance
		/// </summary>
		public void CountManhattanDistance()
		{
			int total = 0;
			for (short i = 0; i < board_Solved.GetLength(0); i++)
				for (short j = 0; j < board_Solved.GetLength(1); j++)
					for (short x = 0; x < board.GetLength(0); x++)
						for (short y = 0; y < board.GetLength(1); y++)
						{
							if (board[x, y] == 0)
								Position0 = new Position(x, y);
							if (board[x, y] == board_Solved[i, j])
								total += Math.Abs(x - i) + Math.Abs(j - y);
						}
			manhattanDistance = total;
		}

		/// <summary>
		/// 根據空洞位置展開所有可選移動方式
		/// </summary>
		/// <returns>所有可能變化盤面</returns>
		public List<Board> Expand()
		{
			List<Board> ret = new List<Board>();

			var center = Position0;
			foreach (var bias in new[] { new Position(-1, 0), new Position(1, 0), new Position(0, -1), new Position(0, 1) })
			{
				Position selected = bias + center;
				if (InRange(selected))
				{
					Board copy = new Board(this.board);

					copy.Swap(selected, center);

					ret.Add(copy);
				}
			}
			return ret;
		}

		/// <summary>
		/// 檢測指定座標是否可用
		/// </summary>
		bool InRange(Position pos)
		{
			return 0 <= pos.x && pos.x < board_Solved.GetLength(0) &&
				0 <= pos.y && pos.y < board_Solved.GetLength(1);
		}
	}

	class BoardNode : Node
	{
		Board board;
		List<Node> expand;
		BoardNode parentBoardNode;

		public BoardNode(Board board)
		{
			this.board = board;
			this.board.CountManhattanDistance();
		}

		public override string ToString()
		{
			return board.ToString();
		}

		public override float G_Cost { get => g_cost; set => g_cost = value; }
		float g_cost = 0;
		public override float H_Cost { get => h_cost; set => h_cost = value; }
		float h_cost = 0;

		public override float PassingCost(Node b) => 1;

		public override Node parent { get => parentBoardNode; set => parentBoardNode = value as BoardNode; }

		public override int state => ToString().GetHashCode();

		public override List<Node> Expand()
		{
			expand = new List<Node>();
			foreach (var b in board.Expand())
				expand.Add(new BoardNode(b));
			return expand;
		}

		public override int ValueFunc() => board.manhattanDistance;
	}

	#region Debug.Log
	class Debug
	{
		public static void Log(object obj)
		{
			Console.WriteLine(obj.ToString());
		}
	}
	#endregion

	class Program
	{
		static void Main(string[] args)
		{
			Board board = new Board(new short[,] { { 2, 3, 4 }, { 8, 0, 5 }, { 1, 7, 6 } });
			//Board board = new Board(new short[,] { { 1, 2, 3 }, { 8, 6, 4 }, { 7, 0, 5 } });

			BoardNode root = new BoardNode(board);
			BoardNode target = new BoardNode(new Board(Board.board_Solved));
			AStar aStar = new AStar();

			List<Node> path = aStar.FindPath(root, target);
			if (path is null)
				Debug.Log("No Path");

			for (int i = 0; i < path.Count; i++)
			{
				string state = path[i].ToString();
				Debug.Log($"Step {i}:");
				Debug.Log(" " + state.Substring(0, 3));
				Debug.Log(" " + state.Substring(3, 3));
				Debug.Log(" " + state.Substring(6, 3));
				Debug.Log($"G:{path[i].G_Cost}, H:{path[i].H_Cost}");
				Debug.Log("");
			}
		}
	}
}
