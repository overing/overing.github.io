using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class Menu : MonoBehaviour
{
    bool _guiInitialized;

    GameObject _demoItem;

    Vector3 _scrollPosition;

    static readonly IDictionary<string, Type> DemoTypes;

    static Menu()
        => DemoTypes = new SortedList<string, Type>(typeof(Menu).Assembly.ExportedTypes
            .Where(typeof(MonoBehaviour).IsAssignableFrom)
            .Where(t => t != typeof(Menu))
            .ToDictionary(t => t.Name));

    [RuntimeInitializeOnLoadMethod]
    static void InitializeOnLoad() => DontDestroyOnLoad(new GameObject(nameof(Menu), typeof(Menu)));

    void Start()
    {
        try
        {
            var url = Application.absoluteURL;
            if (!string.IsNullOrWhiteSpace(url))
            {
                var query = new UriBuilder(url).Query;
                if (!string.IsNullOrWhiteSpace(query))
                {
                    var argSp = new[] { '=' };
                    var args = query.Split(new[] { '?', '&' }, StringSplitOptions.RemoveEmptyEntries);
                    var mapping = new Dictionary<string, string>(capacity: args.Length, StringComparer.OrdinalIgnoreCase);
                    foreach (var arg in args)
                    {
                        if (string.IsNullOrWhiteSpace(arg))
                            continue;
                        var pair = arg.Split(argSp, StringSplitOptions.RemoveEmptyEntries);
                        mapping[pair[0]] = pair.Length > 1 ? pair[1] : string.Empty;
                    }
                    if (mapping.TryGetValue("demo", out var name) && DemoTypes.TryGetValue(name, out var type))
                        _demoItem = new GameObject(name, type);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
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

        if (_demoItem == null)
        {
            GUILayout.Label("Demos");

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true));

            foreach (var pair in DemoTypes)
                if (GUILayout.Button(pair.Key))
                    _demoItem = new GameObject(pair.Key, pair.Value);

            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Space(Screen.height - GUI.skin.button.lineHeight);

            if (GUILayout.Button("Menu"))
                Destroy(_demoItem);
        }

        GUILayout.EndVertical();
    }
}
