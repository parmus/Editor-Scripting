using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace Editor
{
    [CustomEditor(typeof(AnimationController))]
    public class AnimationControllerEditor : UnityEditor.Editor
    {
        private Type[] _implementations;
        private int _implementationIndex;
        private SerializedProperty _stepsProp;
        private ReorderableList _reorderableList;
        
        private void OnEnable()
        {
            _implementations = GetImplementations();
            _implementationIndex = 0;
            
            _stepsProp = serializedObject.FindProperty("_animationSteps");
            
            _reorderableList = new ReorderableList(serializedObject, _stepsProp, true, true, false,true);
            _reorderableList.drawHeaderCallback = DrawHeaderCallback;
            _reorderableList.drawElementCallback = DrawElementCallback;
            _reorderableList.elementHeightCallback = ElementHeightCallback;
        }

        private float ElementHeightCallback(int index)
        {
            SerializedProperty element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element, true);
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            SerializedProperty element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, true);

        }

        private static void DrawHeaderCallback(Rect rect) => EditorGUI.LabelField(rect, "Animation Steps");

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _reorderableList.DoLayoutList();

            _implementationIndex = EditorGUILayout.Popup(
                new GUIContent("Implementation"),
                _implementationIndex,
                _implementations.Select(type => type.FullName).ToArray());

            if (GUILayout.Button("Add"))
            {
                var prop = serializedObject.FindProperty("_animationSteps");
                prop.InsertArrayElementAtIndex(prop.arraySize);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                var elem = prop.GetArrayElementAtIndex(prop.arraySize-1);
                elem.managedReferenceValue = Activator.CreateInstance(_implementations[_implementationIndex]);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private static System.Type[] GetImplementations()
        {
            var interfaceType = typeof(IAnimationStep);
            var types = System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
            return types.Where(type => interfaceType.IsAssignableFrom(type) && !type.IsAbstract).ToArray();
        }
    }
}