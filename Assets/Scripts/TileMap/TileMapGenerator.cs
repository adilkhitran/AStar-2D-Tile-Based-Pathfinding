using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace KHiTrAN.PathFinding
{
    public class TileMapGenerator : MonoBehaviour
    {


        public TileType tileType;


        [SerializeField]
        private Tilemap tileMap;

        [SerializeField]
        private Tile[] tiles;

        [SerializeField]
        private RuleTile waterTile;

        [SerializeField]
        private AStar astar;


        [SerializeField]
        private Camera mainCamera;


        [SerializeField]
        private LayerMask layerMask;


        private bool start, goal;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask);

                if (hit.collider != null)
                {
                    Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    Vector3Int clickPos = tileMap.WorldToCell(mouseWorldPos);

                    ChangeTile(clickPos);
                }
            }

        }

        private void ChangeTile(Vector3Int clickPos)
        {

            if (tileType == TileType.WATER)
            {
                tileMap.SetTile(clickPos, waterTile);
            }
            else
            {
                tileMap.SetTile(clickPos, tiles[(int)tileType]);
            }

            if (tileType == TileType.START)
            {
                if (start)
                {
                    tileMap.SetTile(astar.startPos, tiles[(int)TileType.GRASS]);
                }
                start = true;
            }
            else if (tileType == TileType.GOAL)
            {
                if (goal)
                {
                    tileMap.SetTile(astar.goalPos, tiles[(int)TileType.GRASS]);
                }
                goal = true;
            }


            astar.OnChangeTile(tileType, clickPos);
        }

        public void OnClickChangeTileType(TileButton tileBtn)
        {
            tileType = tileBtn.tileType;
        }

        public void ShowHide()
        {
            AStarDebugger.instance.ShowHide();
        }

        public void OnClickRestart()
        {
            start = goal = false;
            astar.Reset();
        }

        public void FindPath(bool step)
        {
            astar.Algorithm(step);
        }
    }
}