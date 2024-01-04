using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SkiResortInAustria
{
    enum Direction
    {
        North,
        South,
        East,
        West
    }

    public class Point
    {
        public int x { get; set; }
        public int y { get; set; }
        public Point(int p1, int p2)
        {
            x = p1;
            y = p2;
        }
    }

    public class Node
    {
        public int Id { get; set; }
        public Point Loc { get; set; }
        public int NodeValue { get; set; }
        public int ParentID { get; set; }
        public int Level { get; set; }
        public Node(int id, Point loc, int nodeValue, int parentId, int level)
        {
            Id = id;
            Loc = loc;
            NodeValue = nodeValue;
            ParentID = parentId;
            Level = level;
        }
    }

    class Program
    {
        static List<Node> allPresentNodes = new List<Node>();
        static int[][] points;
        static int runningValueID = 1;
        static int runningLevel = 0;

        static int[][] ReadFile()
        {
            var filePath = "map.txt";
            return File.ReadLines(filePath).Skip(1).Select(x => x.Split(' ').Select(y => int.Parse(y)).ToArray()).ToArray();
        }

        static void Main(string[] args)
        {
            /* this program will traverse through each eligible node and build a treelist level by level
                4 4
                4 8 7 3 
                2 5 9 3 
                6 3 2 5 
                4 4 1 6  
             * level 0 is the base node, leveling up will get the next eligible node with parent as the level 0 id
             * only node with lower value is added to the tree, thus eliminating the chances of traversing back 
             */
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                points = ReadFile();
                for (int i = 0; i < points.Length; i++)
                {
                    for (int j = 0; j < points[i].Length; j++)
                    {
                        AddBaseNodeLevel(new Point(i, j), points[i][j]);
                    }
                }
                //loop until there is no nodes to traverse 
                while (AddSubNode() > 0) { }

                var longestPath = allPresentNodes.OrderByDescending(x => x.Level).FirstOrDefault().Level;
                var steepestPathNodes = new List<int[]>();
                allPresentNodes.Where(y => y.Level == longestPath).OrderByDescending(x => x.Id).ToList().ForEach(y => steepestPathNodes.Add(GetTreeNodeValues(y)));
                var steepestPath = steepestPathNodes.OrderByDescending(x => x.Max() - x.Min()).FirstOrDefault();
                Console.WriteLine(" ");
                Console.WriteLine("Answer : ");
                Console.WriteLine(" ");
                Console.WriteLine("Longest path length is : {0}  ", longestPath + 1, steepestPath.Max() - steepestPath.Min());
                Console.WriteLine("The steepest drop is : {1} ", longestPath + 1, steepestPath.Max() - steepestPath.Min());
                Console.WriteLine("The path is :");
                steepestPath.ToList().ForEach(x => Console.WriteLine(x));
                watch.Stop();
                Console.WriteLine(" ");
                Console.WriteLine("The time taken to execute : {0} ms", watch.ElapsedMilliseconds);
                Console.WriteLine(" ");
                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sorry, something gone wrong! {0}", ex.Message);
                Console.ReadLine();
            }
        }
        
        static int[] GetTreeNodeValues(Node node)
        {
            var nodePath = new List<int>() { node.NodeValue };
            Node parentNode = null;
            do
            {
                parentNode = allPresentNodes.Where(x => x.Id == node.ParentID).SingleOrDefault();
                if (parentNode != null)
                    nodePath.Add(parentNode.NodeValue);
                node = parentNode;
            } while (parentNode != null);
            return nodePath.ToArray();
        }

        static void AddBaseNodeLevel(Point node, int nodeValue)
        {
            allPresentNodes.Add(new Node(runningValueID++, node, nodeValue, 0, 0));
        }

        static int AddSubNode()
        {
            int count = 0;
            allPresentNodes.Where(x => x.Level == runningLevel).ToArray().ToList().ForEach(x =>
            {
                count += AddEligibleNeighbour(Direction.North, x) + AddEligibleNeighbour(Direction.South, x) + AddEligibleNeighbour(Direction.East, x) + AddEligibleNeighbour(Direction.West, x);
            });
            runningLevel++;
            return count;
        }

        static int AddEligibleNeighbour(Direction dir, Node node)
        {
            Point neighbour = FindNeighbour(dir, node.Loc);
            if (neighbour != null && points[neighbour.x][neighbour.y] < node.NodeValue)
            {
                allPresentNodes.Add(new Node(runningValueID++, neighbour, points[neighbour.x][neighbour.y], node.Id, runningLevel + 1));
                return 1;
            }
            return 0;
        }

        static Point FindNeighbour(Direction dir, Point node)
        {
            Point nextLoc = null;
            switch (dir)
            {
                case Direction.North:
                    nextLoc = node.x - 1 >= 0 ? new Point(node.x - 1, node.y) : null;
                    break;
                case Direction.South:
                    nextLoc = node.x + 1 < points.GetLength(0) ? new Point(node.x + 1, node.y) : null;
                    break;
                case Direction.East:
                    nextLoc = node.y + 1 < points.GetLength(0) ? new Point(node.x, node.y + 1) : null;
                    break;
                case Direction.West:
                    nextLoc = node.y - 1 >= 0 ? new Point(node.x, node.y - 1) : null;
                    break;
                default:
                    break;
            }
            return nextLoc;

        }


    }
}

