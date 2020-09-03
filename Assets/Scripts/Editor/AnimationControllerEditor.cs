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
        private SerializedProperty _stepsProp;
        private ReorderableList _reorderableList;
        
        private GenericMenu _implementationsMenu;
        
        private void OnEnable()
        {
            _implementationsMenu = new GenericMenu();
            foreach (var implementation in GetImplementations())
            {
                _implementationsMenu.AddItem(
                    new GUIContent(implementation.FullName), 
                    false, 
                    OnAddImplementation,
                    implementation
                );
            }
            
            _stepsProp = serializedObject.FindProperty("_animationSteps");

            _reorderableList = new ReorderableList(serializedObject, _stepsProp, true, true, true, true)
            {
                drawHeaderCallback = DrawHeaderCallback,
                drawElementCallback = DrawElementCallback,
                elementHeightCallback = ElementHeightCallback,
                onAddDropdownCallback = (rect, list) => _implementationsMenu.ShowAsContext()
            };
        }


        private void OnAddImplementation(object o)
        {
            var implementation = (Type) o;
            var nextIdx = _stepsProp.arraySize;
            _stepsProp.InsertArrayElementAtIndex(nextIdx);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            var elem = _stepsProp.GetArrayElementAtIndex(nextIdx);
            elem.managedReferenceValue = Activator.CreateInstance(implementation);
            serializedObject.ApplyModifiedProperties();
        }

        private float ElementHeightCallback(int index)
        {
            var element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var end = element.GetEndProperty();
            var height = EditorGUIUtility.standardVerticalSpacing;

            element.Next(true);
            do {
                if (SerializedProperty.EqualContents(element, end)) break;
                height += EditorGUI.GetPropertyHeight(element, true) + EditorGUIUtility.standardVerticalSpacing;
            } while (element.Next(false));
            
            return height;   
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            var element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var end = element.GetEndProperty();
            var offset = EditorGUIUtility.standardVerticalSpacing;

            element.Next(true);
            do {
                if (SerializedProperty.EqualContents(element, end)) break;
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + offset, rect.width, rect.height - offset),
                    element, true
                );
                offset += EditorGUI.GetPropertyHeight(element, true) + EditorGUIUtility.standardVerticalSpacing;
            } while (element.Next(false));
        }

        private static void DrawHeaderCallback(Rect rect) => EditorGUI.LabelField(rect, "Animation Steps");


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private static Type[] GetImplementations()
        {
            var interfaceType = typeof(IAnimationStep);
            var types = System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
            return types.Where(type => interfaceType.IsAssignableFrom(type) && !type.IsAbstract).ToArray();
        }
    }
}