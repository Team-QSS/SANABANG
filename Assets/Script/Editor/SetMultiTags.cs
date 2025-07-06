using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

[InitializeOnLoad]
public static class AutoComponentInjector
{
    static AutoComponentInjector()
    {
        EditorApplication.hierarchyChanged += CheckAndInject;
    }

    static void CheckAndInject()
    {
        var gameObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (var go in gameObjects)
        {
            if (go.GetComponent<CustomTagger>() == null)
            {
                Undo.AddComponent<CustomTagger>(go);
            }
        }
    }
}

