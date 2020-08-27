using System;
using System.Collections.Generic;
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
        private SerializedProperty _stepsProp;
        private ReorderableList _reorderableList;
        
        private GenericMenu _implementationsMenu;
        
        private void OnEnable()
        {
            _implementations = GetImplementations();
            _implementationsMenu = new GenericMenu();
            foreach (var implementation in _implementations)
            {
                _implementationsMenu.AddItem(
                    new GUIContent(implementation.FullName), 
                    false, 
                    OnAddImplementation,
                    implementation
                );
            }
            
            _stepsProp = serializedObject.FindProperty("_animationSteps");
            
            _reorderableList = new ReorderableList(serializedObject, _stepsProp, true, true, true,true);
            _reorderableList.drawHeaderCallback = DrawHeaderCallback;
            _reorderableList.drawElementCallback = DrawElementCallback;
            _reorderableList.elementHeightCallback = ElementHeightCallback;
            _reorderableList.onAddCallback = list => _implementationsMenu.ShowAsContext();
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
            var current = element.Copy();
            current.Next(true);
            var height = 0f;
            do
            {
                if (SerializedProperty.EqualContents(current, end))
                    break;
                height += EditorGUI.GetPropertyHeight(current, true);
            } while (current.Next(false));

            return height;
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            var element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var end = element.GetEndProperty();
            var current = element.Copy();
            current.Next(true);
            var offset = 0f;
            do
            {
                if (SerializedProperty.EqualContents(current, end))
                    break;
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + offset, rect.width, rect.height - offset),
                    current,
                    true
                );
                offset += EditorGUI.GetPropertyHeight(current, true);
            } while (current.Next(false));
        }

        private static void DrawHeaderCallback(Rect rect) => EditorGUI.LabelField(rect, "Animation Steps");

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private static System.Type[] GetImplementations()
        {
            var interfaceType = typeof(IAnimationStep);
            var types = System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
            return types.Where(type => interfaceType.IsAssignableFrom(type) && !type.IsAbstract).ToArray();
        }
    }

    public static class ExtensionMethods
    {
        public static IEnumerator<SerializedProperty> IterateChildren(this SerializedProperty property)
        {
            var end = property.GetEndProperty();
            var current = property.Copy();
            current.Next(true);
            do
            {
                if (SerializedProperty.EqualContents(current, end))
                    break;
                yield return current.Copy();
            } while (current.Next(false));            
        }
    }
}