using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRow : MonoBehaviour
{
    public TileCell[] cells; //当前行所管理的单元格
    // Start is called before the first frame update
    private void Awake()
    {
        //通过获取子节点中所有带有TileCell的对象
        cells = GetComponentsInChildren<TileCell>();
    }
}
