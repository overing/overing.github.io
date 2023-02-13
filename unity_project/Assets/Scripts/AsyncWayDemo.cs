using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 展示 同步, Coroutine 非同步, Task 非同步 的範本
/// </summary>
public sealed class AsyncWayDemo : MonoBehaviour
{
    bool _guiInitialized;

    const float BeginX = -5, EndX = 5, StepX = .05f;

    void OnGUI()
    {
        if (!_guiInitialized)
        {
            var height = Screen.height;
            GUI.skin.label.fontSize = height / 18;
            GUI.skin.toggle.fontSize = height / 20;
            GUI.skin.button.fontSize = height / 20;
            _guiInitialized = true;
        }


        GUILayout.BeginVertical(GUILayout.Width(Screen.width), GUILayout.Height(Screen.width)); // 開始 layout 垂直區域

        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true)); // 開始 layout 水平區域 (因為要把三個按鈕擺成橫的)

        GUILayout.Label(nameof(AsyncWayDemo));

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("source code"))
            Application.OpenURL("https://gist.github.com/overing/ee332c16f6c7282d0887b39b8cf1f178");

        GUILayout.EndHorizontal(); // 結束 layout 水平區域

        if (GUILayout.Button("Clear", GUILayout.ExpandWidth(true))) // layout 按鈕 顯示文字 "Clear" 如果被按下
            Array.ForEach(GameObject.FindGameObjectsWithTag(BallTag), Destroy); // 找出所有 tag 是 "Respawn" 的物件把它們註銷掉

        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true)); // 開始 layout 水平區域 (因為要把三個按鈕擺成橫的)

        if (GUILayout.Button("Sync")) // layout 按鈕 顯示文字 "Sync" 如果被按下
            RunSync(); // 執行 RunSync

        if (GUILayout.Button("CoroutineAsync")) // layout  按鈕 顯示文字 "CoroutineAsync" 如果被按下
            StartCoroutine(CoroutineAsync()); // 開始一個協程 CoroutineAsync

        if (GUILayout.Button("TaskAsync")) // layout  按鈕 顯示文字 "TaskAsync" 如果被按下
            RunTaskAsync(); // 執行 RunTaskAsync

        GUILayout.EndHorizontal(); // 結束 layout 水平區域

        GUILayout.EndVertical();
    }

    const string BallTag = "Respawn";

    /// <summary>
    /// 註銷所有球體, 重新產生一個新球體
    /// </summary>
    /// <returns>重新產生的球體</returns>
    GameObject RenewBall()
    {
        Array.ForEach(GameObject.FindGameObjectsWithTag(BallTag), Destroy); // 找出所有 tag 是 "Respawn" 的物件把它們註銷掉

        var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere); // 產生一顆球
        ball.tag = BallTag; // 將球的 tag 指定為 "Respawn"
        ball.transform.parent = transform;
        var renderer = ball.GetComponent<Renderer>();
        renderer.material.shader = Shader.Find("Legacy Shaders/Diffuse");
        return ball;
    }

    /// <summary>
    /// 用同步的方式產生一顆球然後將它從 (-5, 0, 0) 的位置每次往右移動 0.005 的方式直到位置超過 (5, 0, 0) 為止
    /// </summary>
    void RunSync()
    {
        var ball = RenewBall();
        for (float x = BeginX; x <= EndX; x += StepX)
        {
            if (ball == null) // 如果球為 null 表示可能被別的按鈕的執行註銷了
                break; // 就離開迴圈
            ball.transform.position = new Vector3(x, 0, 0); // 改變球的位置
        }
    }

    /// <summary>
    /// 用 Coroutine 非同步的方式產生一顆球然後將它從 (-5, 0, 0) 的位置每次往右移動 0.005 的方式直到位置超過 (5, 0, 0) 為止
    /// </summary>
    IEnumerator CoroutineAsync()
    {
        var ball = RenewBall();
        for (float x = BeginX; x <= EndX; x += StepX)
        {
            yield return null; // 返回一個中斷 暫停現在的方法 讓引擎去做別的事 (這包含更新畫面; 畫出球的新位置)
            if (ball == null) // 如果球為 null 表示可能被別的按鈕的執行註銷了
                break; // 就離開迴圈
            ball.transform.position = new Vector3(x, 0, 0); // 改變球的位置
        }
        yield return null; // 返回一個中斷 暫停現在的方法 讓引擎去做別的事 (這包含更新畫面; 畫出球的新位置)
    }

    /// <summary>
    /// 用 Task 非同步的方式產生一顆球然後將它從 (-5, 0, 0) 的位置每次往右移動 0.005 的方式直到位置超過 (5, 0, 0) 為止
    /// </summary>
    async void RunTaskAsync()
    {
        var ball = RenewBall();
        for (float x = BeginX; x <= EndX; x += StepX)
        {
            await Task.Yield(); // 等待一個中斷 暫停現在的方法 讓引擎去做別的事 (這包含更新畫面; 畫出球的新位置)
            if (ball == null) // 如果球為 null 表示可能被別的按鈕的執行註銷了
                break; // 就離開迴圈
            ball.transform.position = new Vector3(x, 0, 0); // 改變球的位置
        }
        await Task.Yield(); // 等待一個中斷 暫停現在的方法 讓引擎去做別的事 (這包含更新畫面; 畫出球的新位置)
    }
}