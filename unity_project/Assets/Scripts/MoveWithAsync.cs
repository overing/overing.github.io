using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 展示用 async / await 來做移動物件證明 async 也可以持續在主線上執行; (WebGL passed)
/// </summary>
public sealed class MoveWithAsync : MonoBehaviour
{
    bool _guiInitialized;

    string _state;

    [SerializeField]
    [Range(0, 10)]
    float _speed = 6;

    async Task Start()
    {
        _state = string.Format("Create ball, Thread#{0}", Environment.CurrentManagedThreadId);

        var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.transform.parent = transform;
        var renderer = ball.GetComponent<Renderer>();
        renderer.material.shader = Shader.Find("Legacy Shaders/Diffuse");
        await Task.Yield();

        while (true)
        {
            var targetPosition = (Vector3)UnityEngine.Random.insideUnitCircle * 3;
            var startPos = ball.transform.position;
            float startTime = Time.time;
            float journeyLength = Vector3.Distance(startPos, targetPosition);
            renderer.material.color = Color.blue;
            while (transform.position != targetPosition)
            {
                _state = string.Format("Move ball, Thread#{0}", Environment.CurrentManagedThreadId);

                float distCovered = (Time.time - startTime) * _speed;
                float fractionOfJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(startPos, targetPosition, fractionOfJourney);
                await Task.Yield();
            }

            renderer.material.color = Color.gray;
            ball.transform.position = targetPosition;

            _state = string.Format("Delay 2 sec, Thread#{0}", Environment.CurrentManagedThreadId);

            // await Task.Delay(TimeSpan.FromSeconds(2)); // webgl not applicable
            var expire = Time.time + 2;
            while (Time.time < expire)
                await Task.Yield();
        }
    }

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

        GUILayout.BeginVertical(GUILayout.Width(Screen.width), GUILayout.Height(Screen.width));

        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

        GUILayout.Label(nameof(MoveWithAsync));

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("source code"))
            Application.OpenURL("https://gist.github.com/overing/583c9a710fccdd478c33737b413bd4cb");

        GUILayout.EndHorizontal();

        GUILayout.Label(_state);

        GUILayout.EndVertical();
    }
}
