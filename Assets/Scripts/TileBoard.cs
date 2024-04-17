using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public GameManager gameManager; //游戏管理器
    public Tile tilePrefab; //tile的预制
    public TileState[] tileStates; //所有的tile状态配置

    private TileGrid grid; //网格
    private List<Tile> tiles = new List<Tile>(); //当前已经创建的Tile
    private bool waiting = false; //是否处于等待操作的状态，用来控制

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
    }


    /// <summary>
    /// 清理board已便进行下一场游戏
    /// </summary>
    public void ClearBoard()
    {
        //清楚所有的Cell 和Tile的链接关系
        foreach (var cell in grid.cells)
        {
            cell.tile = null;
        }

        //销毁所有本局游戏中所创建的Tile
        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
    }

    /// <summary>
    /// 创建tile
    /// </summary>
    public void CreateTile()
    {
        //实例化一个tile
        Tile tile = Instantiate(tilePrefab, grid.transform);
        //tile.SetState(tileStates[0]); //这里总是会生成2，其实可以根据一些规则设置为别的数字
        tile.SetState(tileStates[UnityEngine.Random.Range(0,2)]); //修改为随机生成[0-2)直接的int整数

        //和cell进行关联
        TileCell cell = grid.GetRandomEmptyCell();
        if(cell != null)
        {
            tile.LinkCell(cell);//生成到一个随机的空的单元上
        }
        //将tile添加到动态生成的列表中
        tiles.Add(tile);

    }

    // Update is called once per frame
    private void Update()
    {
        if (!waiting)
        {
            //上箭头或者W
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                Move(Vector2Int.up, 0, 1, 1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Move(Vector2Int.left, 1, 1, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                Move(Vector2Int.down, 0, 1, grid.height - 2, -1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                Move(Vector2Int.right, grid.width - 2, -1, 0, 1);
            }
        }
    }
    /// <summary>
    /// 移动的方向，移动的初始X，Y和步长，用这些信息来决定哪些Cell应该移动
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="startX"></param>
    /// <param name="incrementX"></param>
    /// <param name="startY"></param>
    /// <param name="incrementY"></param>
    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        for (int x = startX; x >= 0 && x < grid.width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);

                //只有被占用的cell才会需要移动
                if (cell.IsOccupied())
                {
                    changed |= MoveTile(cell.tile, direction);

                }
            }
        }
        if (changed)
        {
            StartCoroutine(WaitForChanges());
        }
    }
    /// <summary>
    /// 朝指定的方向移动单元格
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        //如果有相邻的cell
        while (adjacent != null)
        {
            if (adjacent.IsOccupied()) //相邻的Cell里面有Tile
            {
                //如果正好它们两个可以合并，那么就执行合并，本次Tile的移动处理就结束了
                if (CanMerge(tile, adjacent.tile))
                {
                    MergeTiles(tile, adjacent.tile);
                    return true;
                }
                //如果有相邻的tile，而且不能合并，那就直接结束 
                break;
            }

            //执行到这里，说明它相邻的格子为空，那么就把他当前相邻的格子作为一个目标，再去判断相邻格子的相邻格子情况
            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }
        //执行完之后就知道是否有最终的新格子，将tile移动到最终的新格子上去
        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 判定两个单元格是否能合并
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private bool CanMerge(Tile a, Tile b)
    {
        //状态相等，并且b不在动画锁定状态
        return a.tileState == b.tileState && !b.locked;
    }

    /// <summary>
    /// 将A单元格合并到B上
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    private void MergeTiles(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        //找到合并之后的新状态
        int index = Mathf.Clamp(value: (Array.IndexOf(tileStates,b.tileState) + 1), 0, tileStates.Length - 1);
        TileState newState = tileStates[index];

        //设置状态
        b.SetState(newState);

        //添加分数
        gameManager.IncreaseScore(newState.number);
    }

    IEnumerator WaitForChanges()
    {
        //阻止连续操作
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        waiting = false;

        //解除tile的锁定状态
        foreach (var tile in tiles)
        {
            tile.locked = false;
        }

        //创建新的tile
        if (tiles.Count < grid.size)
        {
            CreateTile();
        }

        // 检查游戏是否结束
        if (CheckForGameOver())
        {
            gameManager.GameOver();
        }

    }

    public bool CheckForGameOver()
    {
        //tile数量不够单元格
        if (tiles.Count != grid.size)
        {
            return false;
        }

        //通过循环判定某个单元格的四个方向，任意一个方向可以合并的话，游戏都未结束
        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile))
            {
                return false;
            }
            if (down != null && CanMerge(tile, down.tile))
            {
                return false;
            }
            if (left != null && CanMerge(tile, left.tile))
            {
                return false;
            }
            if (right != null && CanMerge(tile, right.tile))
            {
                return false;
            }
        }
        return true;
    }
}
