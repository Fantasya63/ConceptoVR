using UnityEditor;
using UnityEngine;
using System;
using Canvas;

public static class CreateStepMenu
{
    [MenuItem("GameObject/Step/Create...", false, 10)]
    private static void ShowMenu(MenuCommand command)
    {
        GenericMenu menu = new GenericMenu();

        foreach (var type in TypeCache.GetTypesDerivedFrom<Step>())
        {
            if (type.IsAbstract)
                continue;

            Type capturedType = type;

            string name = ObjectNames.NicifyVariableName(capturedType.Name);

            menu.AddItem(new GUIContent(name), false, () =>
            {
                CreateStep(capturedType, command);
            });
        }

        menu.ShowAsContext();
    }

    private static void CreateStep(Type stepType, MenuCommand command)
    {
        GameObject go = new GameObject(stepType.Name);
        go.AddComponent(stepType);

        GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);

        Undo.RegisterCreatedObjectUndo(go, "Create Step");
        Selection.activeGameObject = go;
    }
}