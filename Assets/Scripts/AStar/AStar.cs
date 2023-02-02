using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

namespace KHiTrAN.PathFinding
{
    public class AStar : MonoBehaviour
    {

        [SerializeField]
        public bool canGoDiagonal;

        [SerializeField]
        private Tilemap tileMap;
        [SerializeField]
        private Tile pathTile, defaultTile;

        public Vector3Int startPos, goalPos;

        private HashSet<Node> openList, closeList;
        private HashSet<Vector3Int> changeTiles;

        private Node currentNode;

        private Stack<Vector3Int> path;



        private Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();

        private List<Vector3Int> waterTiles = new List<Vector3Int>();



        private void Start()
        {
            changeTiles = new HashSet<Vector3Int>();
        }

        private void Init()
        {

            openList = new HashSet<Node>();
            closeList = new HashSet<Node>();

            currentNode = GetNode(startPos);

            openList.Add(currentNode);
        }


        public void Algorithm(bool step)
        {
            if (currentNode == null)
            {
                Init();
            }

            while (openList.Count > 0 && path == null)
            //for (int i = 0; i < 3; i++)
            {

                List<Node> neighbours = FindNeighbours(currentNode.Position);

                ExamineNeighbours(neighbours, currentNode);

                UpdateCurrentTile(ref currentNode);

                path = GeneratePath(currentNode);

                if (step)
                {
                    break;
                }
            }


            if (path != null)
            {
                foreach (Vector3Int position in path)
                {
                    if (position != goalPos)
                    {
                        tileMap.SetTile(position, pathTile);
                    }
                }
            }

            AStarDebugger.instance.CreateTiles(openList, closeList, allNodes, startPos, goalPos, path);
        }


        private List<Node> FindNeighbours(Vector3Int parentPosition)
        {

            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector3Int neighbourPos = parentPosition - new Vector3Int(x, y, 0);

                    bool isWalkableArea = false;
                    bool isValidNeighbour = y != 0 || x != 0;

                    if (!canGoDiagonal)
                    {
                        isValidNeighbour = (y == 0 && x != 0) || (x == 0 && y != 0);
                    }


                    SingleTile tile = tileMap.GetTile(neighbourPos) as SingleTile;

                    //isWalkableArea = tile != null;
                    isWalkableArea = !waterTiles.Contains(neighbourPos) && tileMap.GetTile(neighbourPos);


                    if (isValidNeighbour && isWalkableArea && neighbourPos != startPos)
                    {
                        Node neighbour = GetNode(neighbourPos);
                        neighbours.Add(neighbour);
                    }
                }
            }
            return neighbours;
        }


        private void ExamineNeighbours(List<Node> neighbours, Node current)
        {

            for (int i = 0; i < neighbours.Count; i++)
            {

                Node neighbour = neighbours[i];


                if (!isConnectedDiagonally(current, neighbour))
                    continue;


                int gScore = DetermineGScore(neighbours[i].Position, current.Position);
                if (openList.Contains(neighbour))
                {
                    if (currentNode.G + gScore < neighbour.G)
                    {
                        CalculateValue(current, neighbour, gScore);
                    }
                }
                else if (!closeList.Contains(neighbour))
                {
                    CalculateValue(current, neighbour, gScore);
                    openList.Add(neighbour);
                }
            }
        }

        private void CalculateValue(Node parent, Node neighbour, int cost)
        {
            neighbour.Parent = parent;
            neighbour.G = parent.G + cost;

            neighbour.H = (Mathf.Abs(neighbour.Position.x - goalPos.x) + Mathf.Abs(neighbour.Position.y - goalPos.y)) * 10;
            neighbour.F = neighbour.G + neighbour.H;
        }


        private int DetermineGScore(Vector3Int neighbor, Vector3Int current)
        {
            int gscore = 0;

            int x = current.x - neighbor.x;
            int y = current.y - neighbor.y;


            if (Mathf.Abs(x - y) % 2 == 1)
            {
                gscore = 10;
            }
            else
                gscore = 14;

            return gscore;
        }


        private void UpdateCurrentTile(ref Node current)
        {
            openList.Remove(current);
            closeList.Add(current);

            if (openList.Count > 0)
            {
                current = openList.OrderBy(x => x.F).First();
            }
        }

        private Node GetNode(Vector3Int postion)
        {

            if (allNodes.ContainsKey(postion))
            {
                return allNodes[postion];
            }
            else
            {
                Node node = new Node(postion);
                allNodes.Add(postion, node);
                return node;
            }
        }

        public void OnChangeTile(TileType tileType, Vector3Int clickPos)
        {
            if (tileType == TileType.START)
            {
                startPos = clickPos;
            }
            else if (tileType == TileType.GOAL) { 
                goalPos = clickPos;
            }
            if (tileType == TileType.WATER)
                waterTiles.Add(clickPos);

            changeTiles.Add(clickPos);


        }


        private bool isConnectedDiagonally(Node current, Node neighbour)
        {
            Vector3Int direction = current.Position - neighbour.Position;

            Vector3Int first = new Vector3Int(current.Position.x - direction.x, current.Position.y, current.Position.z);
            Vector3Int second = new Vector3Int(current.Position.x, current.Position.y - direction.y, current.Position.z);

            if (waterTiles.Contains(first) || waterTiles.Contains(second))
                return false;

            return true;
        }


        private Stack<Vector3Int> GeneratePath(Node current)
        {
            if (current.Position == goalPos)
            {
                Stack<Vector3Int> finalPath = new Stack<Vector3Int>();

                while (current.Position != startPos)
                {
                    finalPath.Push(current.Position);

                    current = current.Parent;
                }
                return finalPath;
            }
            return null;
        }

        public void Reset()
        {

            AStarDebugger.instance.OnClickRestart(allNodes);

            foreach (Vector3Int pos in changeTiles)
            {
                tileMap.SetTile(pos, defaultTile);
            }

            foreach (Vector3Int pos in path)
            {
                tileMap.SetTile(pos, defaultTile);
            }

            tileMap.SetTile(startPos,defaultTile);
            tileMap.SetTile(goalPos,defaultTile);

            allNodes.Clear();
            path = null;
            waterTiles.Clear();
            currentNode = null;
        }
    }
}