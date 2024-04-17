using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile State")]
public class TileState : ScriptableObject
{
    public Color backgroundColor; //背景颜色
    public Color textColor; //字体颜色
    public int number; //数字

}
