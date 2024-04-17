using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinates; //当前Cell格子的坐标
    public Tile tile; //关联的Tile

    /// <summary>
    /// 当前Cell是否被占用
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return tile == null;
    }

    /// <summary>
    /// 当前格子是否被占用
    /// </summary>
    /// <returns></returns>
    public bool IsOccupied()
    {
        return tile != null;
    }
}
