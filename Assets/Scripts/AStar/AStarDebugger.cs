using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace KHiTrAN.PathFinding
{
    public class AStarDebugger : MonoBehaviour
    {


        [SerializeField]
        private Grid grid;

        [SerializeField]
        private Tile tile;


        [SerializeField]
        private Tilemap tileMap;

        [SerializeField]
        private Color openColor, closeColor, pathColor, currentColor, startColor, goalColor;

        [SerializeField]
        private Canvas canvas;

        [SerializeField]
        private GameObject debugTextPrefab;

        private List<GameObject> debugObjects = new List<GameObject>();

        public static AStarDebugger instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);
        }

        public void CreateTiles(HashSet<Node> openList, HashSet<Node> closeList, Dictionary<Vector3Int, Node> allNodes, Vector3Int start, Vector3Int goal, Stack<Vector3Int> path = null)
        {


            foreach (var obj in debugObjects)
            {
                Destroy(obj);
            }


            foreach (Node node in openList)
            {
                ColorTile(node.Position, openColor);
            }
            foreach (Node node in closeList)
            {
                ColorTile(node.Position, closeColor);
            }


            if (path != null)
            {

                foreach (var pos in path)
                {
                    if (pos != start && pos != goal)
                    {
                        ColorTile(pos, pathColor);
                    }
                }
            }

            ColorTile(start, startColor);
            ColorTile(goal, goalColor);

            foreach (KeyValuePair<Vector3Int, Node> node in allNodes)
            {
                if (node.Value.Parent != null)
                {
                    GameObject go = Instantiate(debugTextPrefab, canvas.transform);
                    go.transform.position = grid.CellToWorld(node.Key);
                    go.transform.GetChild(0).right = node.Value.Parent.Position - node.Key;
                    debugObjects.Add(go);
                    UpdateText(node.Value, go.GetComponent<DebugText>());
                }
            }
        }

        private void UpdateText(Node node, DebugText debug)
        {

            debug.P.text = $"P:{node.Position.x},{node.Position.y}";
            debug.F.text = $"F:{node.F}";
            debug.G.text = $"G:{node.G}";
            debug.H.text = $"H:{node.H}";
        }

        public void ColorTile(Vector3Int position, Color color)
        {
            tileMap.SetTile(position, tile);
            tileMap.SetTileFlags(position, TileFlags.None);
            tileMap.SetColor(position, color);
        }

        public void ShowHide()
        {
            canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
            Color c = tileMap.color;
            c.a = c.a == 0 ? 1 : 0;
            tileMap.color = c;
        }

        public void OnClickRestart(Dictionary<Vector3Int, Node> allNodes)
        {
            foreach (GameObject go in debugObjects)
            {
                Destroy(go);
            }

            foreach (Vector3Int pos in allNodes.Keys)
            {
                tileMap.SetTile(pos, null);
            }

            debugObjects.Clear();
        }
    }
}