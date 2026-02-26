#if UNITY_EDITOR

using Seagull.Interior_01.Utility; // Make sure this matches where YureiManagerBRP is
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Seagull.Interior_01.Utility.Inspector
{

    // FIX: Target the actual script you want to inspect!
    [CustomEditor(typeof(YureiManagerBRP), true)]
    public class YureiInspector : Editor
    {

        // Note: You don't need OnEnable() to create a SerializedObject. 
        // The base 'Editor' class already provides 'serializedObject' automatically.

        public override void OnInspectorGUI()
        {
            // FIX: Pull the latest data from the real object into the serialized object
            serializedObject.Update();

            Type type = target.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            EditorGUI.BeginChangeCheck();

            foreach (var field in fields)
            {
                // 1. Draw Buttons for YureiButtonAttribute
                if (field.FieldType == typeof(UnityEvent) || field.FieldType == typeof(Action))
                {
                    YureiButtonAttribute attr = field.GetCustomAttribute<YureiButtonAttribute>();
                    if (attr != null)
                    {
                        string name = attr.text ?? field.Name;

                        if (field.FieldType == typeof(UnityEvent) && !Application.isPlaying)
                            name = $"{name} (Requires starting the game)";

                        if (GUILayout.Button(name))
                        {
                            if (field.FieldType == typeof(UnityEvent))
                            {
                                ((UnityEvent)field.GetValue(target))?.Invoke();
                            }
                            else
                            {
                                try
                                {
                                    ((Action)field.GetValue(target))?.Invoke();
                                }
                                catch (Exception)
                                {
                                    MethodInfo startMethod = type.GetMethod("Reset", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                                    if (startMethod != null && startMethod.GetParameters().Length == 0)
                                    {
                                        startMethod.Invoke(target, null);
                                        ((Action)field.GetValue(target))?.Invoke();
                                    }
                                }
                            }
                        }
                    }
                }

                // 2. Draw standard properties IF they don't have IgnoreInInspector
                if (field.GetCustomAttribute<HideInInspector>() == null)
                {
                    SerializedProperty prop = serializedObject.FindProperty(field.Name);

                    // FIX: Action and other non-serialized types return a null SerializedProperty!
                    // We must check if prop is null before drawing to avoid breaking the inspector.
                    if (prop != null)
                    {
                        EditorGUILayout.PropertyField(prop, true);
                    }
                }
            }

            // FIX: Apply any changes made in the inspector back to the actual object
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

#endif