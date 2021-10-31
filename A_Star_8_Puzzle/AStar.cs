using System;
using System.Collections.Generic;
using System.Text;

namespace A_Star_8_Puzzle
{
	public abstract class Node
	{
		public abstract float G_Cost { get; set; }
		public abstract float H_Cost { get; set; }

		public abstract Node parent { get; set; }

		/// <summary>
		/// function that present the loss to the goal
		/// </summary>
		public abstract int ValueFunc();
		public abstract List<Node> Expand();

		public abstract int state { get; }

		/// <summary>
		/// 從 Node 移動到另一個 Node 實際經過的距離
		/// </summary>
		public abstract float PassingCost(Node b);

		public override bool Equals(object obj)
		{
			return string.Compare(ToString(), obj.ToString()) == 0;
		}
	}

	public class AStar
	{
		float bestG = 0;
		Node bestNode = null;

		public List<Node> FindPath(Node start, Node goal)
		{
			List<Node> list_Searched;
			List<Node> list_Unsearch;
			start.G_Cost = 0.0f;
			start.H_Cost = start.ValueFunc();

			Node node_Searching = default(Node); // next search

			while (bestNode is null)
			{
				list_Searched = new List<Node>();
				list_Unsearch = new List<Node>();
				list_Unsearch.Add(start);

				++bestG;
				while(list_Unsearch.Count > 0)
				{
					node_Searching = list_Unsearch[0];

					//Push the current node to the closed list  
					list_Searched.Add(node_Searching);
					//and remove it from openList  
					list_Unsearch.Remove(node_Searching);

					//Check if the current node is the goal node  
					if (node_Searching.state == goal.state)
					{
						if (node_Searching.G_Cost < bestG)
						{
							bestNode = node_Searching;
							bestG = bestNode.G_Cost;
						}
					}

					//Create an ArrayList to store the neighboring nodes  
					foreach (var neighbourNode in node_Searching.Expand())
					{
						if (!list_Searched.Contains(neighbourNode))
						{
							if (!list_Unsearch.Contains(neighbourNode))
							{
								//G  
								float cost = node_Searching.PassingCost(neighbourNode);
								float totalCost = node_Searching.G_Cost + cost;

								//H  
								float neighbourNodeEstCost = neighbourNode.ValueFunc();

								neighbourNode.G_Cost = totalCost;
								neighbourNode.parent = node_Searching;
								neighbourNode.H_Cost = neighbourNodeEstCost;

								if (neighbourNode.G_Cost < bestG)
									list_Unsearch.Add(neighbourNode);

							}
							else
							{
								float cost = node_Searching.PassingCost(neighbourNode);
								float totalCost = node_Searching.G_Cost + cost;

								if (neighbourNode.G_Cost > totalCost)
								{
									neighbourNode.G_Cost = totalCost;
									neighbourNode.parent = node_Searching;
								}
							}
						}
					}

					list_Unsearch.Sort((a, b) => a.ValueFunc().CompareTo(b.ValueFunc()));
				}

			}
			if (bestNode?.state != goal.state)
			{
				Debug.Log("Goal Not Found");
				return null;
			}

			return CalculatePath(bestNode);
		}


		private static List<Node> CalculatePath(Node node)
		{
			List<Node> list = new List<Node>();
			Node? now = node;
			while (now != null)
			{
				list.Add(now);
				now = now.parent;
			}

			list.Reverse();
			return list;
		}
	}
}
