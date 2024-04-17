using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] rows { get; private set; }  //当前的grid统辖的row有哪些

    public TileCell[] cells { get; private set; } //网格内所有的cell单元格

    public int size => cells.Length; //获取当前网格中单元格的数量

    public int height => rows.Length; //获取当前网格的高（行数）

    public int width => size / height; //网格的宽

    private void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
    }

    private void Start()
    {
        for(int i = 0; i < rows.Length; i++)
        {
            for(int j = 0; j < rows[i].cells.Length; j++)
            {
                rows[i].cells[j].coordinates = new Vector2Int(j, i);
            }
        }
    }

    /// <summary>
    /// 根据坐标返回单元格
    /// </summary>
    /// <param name="coord"></param>
    /// <returns></returns>
    public TileCell GetCell(Vector2Int coord)
    {
        return GetCell(coord.x,coord.y);
    }

    /// <summary>
    /// 根据坐标返回单元格
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public TileCell GetCell(int x,int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return rows[y].cells[x];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 获取一个单元格，在某个方向上的邻居
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }

    /// <summary>
    /// 提供一个随机的空单元格
    /// </summary>
    /// <returns></returns>
    public TileCell GetRandomEmptyCell()
    {
        List<TileCell> emptyCells = new List<TileCell>();
        foreach(var cell in cells) 
        {
            if (cell.IsEmpty())
            {
                emptyCells.Add(cell);
            }
        }

        //如果有空的单元格
        if(emptyCells.Count > 0)
        {
            return emptyCells[Random.Range(0,emptyCells.Count)];
        }

        return null;
    }








}
