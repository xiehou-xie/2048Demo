using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [HideInInspector]
    public TileCell cell; //所关联的单元格

    [HideInInspector]
    public TileState tileState; //状态，用于处理数字和颜色

    [HideInInspector]
    public bool locked; //是否被锁定()用于单元格发生变化之后，进行移动效果的展示)

    public Image imgBackground; //背景组件

    public TextMeshProUGUI txtNumber;//文字组件
    
    /// <summary>
    /// 设置状态
    /// </summary>
    /// <param name="state"></param>
    public void SetState(TileState state)
    {
        this.tileState = state;

        imgBackground.color = state.backgroundColor;
        txtNumber.color = state.textColor;
        txtNumber.text = state.number.ToString();
    }

    /// <summary>
    /// 关联cell,并设置坐标
    /// </summary>
    /// <param name="cell"></param>
    public void LinkCell(TileCell cell)
    {
        //先解除已有的绑定
        if(this.cell != null)
        {
            this.cell.tile = null;

        }
        //关联现有的cell
        this.cell = cell;
        this.cell.tile = this;

        //设置到对应的位置
        transform.position = cell.transform.position;
    }

    /// <summary>
    /// 将tile移动到某个cell的位置上
    /// </summary>
    /// <param name="cell"></param>
    public void MoveTo(TileCell cell)
    {
        if(this.cell != null)
        {
            this.cell.tile = null;
        }
        this.cell = cell;
        this.cell.tile = this;

        //用协程做一个简单的动画效果
        StartCoroutine(routine:MoveAnimate(cell.transform.position, false));
    }

    /// <summary>
    /// 将Tile合并到某个cell的位置上
    /// </summary>
    /// <param name="cell"></param>
    public void Merge(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }
        this.cell = cell;
        cell.tile.locked = true;

        //和移动的区别是最后一个参数，控制是否是合并过去的
        StartCoroutine(routine: MoveAnimate(cell.transform.position, true));
    }

    private IEnumerator MoveAnimate(Vector3 to, bool merging)
    {
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 from = transform.position;

        //在持续的时间内，用插值的方式将位置移动过去
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;

        //如果是合并过去的，完成之后需要把自己删除
        if (merging)
        {
            Destroy(gameObject);
        }
    }
}
