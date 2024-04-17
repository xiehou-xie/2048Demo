using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public CanvasGroup gameOver; //游戏结束面板
    public TextMeshProUGUI txtScore; //当前游戏得分
    public TextMeshProUGUI bestScore; //最高游戏得分

    public TileBoard board; //游戏核心逻辑
    private int score = 0; //游戏得分

    // Start is called before the first frame update
    void Start()
    {
        NewGame();
    }

    /// <summary>
    /// 开始新游戏
    /// </summary>
    public void NewGame()
    {
        //还原分数
        SetSource(0);

        //加载最高分数
        bestScore.text = LoadHiscore().ToString();

        //隐藏游戏结束面板
        gameOver.alpha = 0f;
        gameOver.interactable = false;

        //初始化核心逻辑
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }
    public void ExitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
    /// <summary>
    /// 分数增长
    /// </summary>
    /// <param name="points"></param>
    public void IncreaseScore(int points)
    {
        SetSource(score + points);
    }

    /// <summary>
    /// 设置分数
    /// </summary>
    /// <param name="score"></param>
    private void SetSource(int score)
    {
        this.score = score;
        txtScore.text = score.ToString();

        //保存分数
        SaveHiscore();
    }

    /// <summary>
    /// 如果已经超过历史分数，那么就将当前分数设置为最高分数
    /// </summary>
    /// <returns></returns>
    private void SaveHiscore()
    {
        int hiscore = LoadHiscore();
        if (score > hiscore)
        {
            PlayerPrefs.SetInt("hiscore", score);
        }
    }

    /// <summary>
    /// 加载最高分数
    /// </summary>
    /// <returns></returns>
    private int LoadHiscore()
    {
        return PlayerPrefs.GetInt("hiscore", 0);
    }
    /// <summary>
    /// 游戏结束，显示面板
    /// </summary>
    public void GameOver()
    {
        board.enabled = false;
        gameOver.interactable = true;
        PlayerPrefs.Save();

        StartCoroutine(routine: Fade(gameOver, 1f, 1f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
