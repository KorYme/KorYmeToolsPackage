using UnityEditor;
using UnityEngine;

namespace KorYmeLibrary.Attributes
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfPropertyDrawer : PropertyDrawer
    {
        private ShowIfAttribute drawIfAttribute;
        SerializedProperty comparedField;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            drawIfAttribute = attribute as ShowIfAttribute;
            if ((ShowMe(property) && drawIfAttribute.trueCaseDisablingType == DisablingType.DontDraw)
                || (!ShowMe(property) && drawIfAttribute.falseCaseDisablingType == DisablingType.DontDraw))
            {
                return 0f;
            }
            return base.GetPropertyHeight(property,label);
        }

        public bool ShowMe(SerializedProperty property)
        {
            drawIfAttribute = attribute as ShowIfAttribute;
            if (drawIfAttribute.simpleBoolean) return true;
            string path = property.propertyPath.Contains(".") ? 
                System.IO.Path.ChangeExtension(property.propertyPath, drawIfAttribute.comparedPropertyName) : 
                drawIfAttribute.comparedPropertyName;
            comparedField = property.serializedObject.FindProperty(path);
            if (comparedField == null)
            {
                Debug.LogError("Cannot find property with name: " + path);
                return false;
            }
            if (drawIfAttribute.comparedValues.Length == 0 && comparedField.type == "bool")
            {
                return comparedField.boolValue;
            }
            foreach (object item in drawIfAttribute.comparedValues)
            {
                switch (comparedField.type)
                {
                    case "bool":
                        if (comparedField.boolValue.Equals(item)) return true;
                        break; 
                    case "Enum":
                        if (comparedField.enumValueIndex.Equals((int)item)) return true;
                        break;
                    case "int":
                    case "long":
                    case "byte":
                    case "short":
                    case "uint":
                        if (CompareInt(drawIfAttribute.comparisonType, item)) return true;
                        break;
                    case "float":
                    case "double":
                        return CompareFloat(drawIfAttribute.comparisonType, item);
                    default:
                        Debug.Log("The type " + comparedField.type + " of this field is not supported");
                        break;
                }
            }
            return false;
        }

        private bool CompareInt(ComparisonType comparisonType, object item)
        {
            return comparisonType switch
            {
                ComparisonType.Equals => comparedField.intValue.Equals(item),
                ComparisonType.NotEqual => !comparedField.intValue.Equals(item),
                ComparisonType.GreaterThan => comparedField.intValue > (int)item,
                ComparisonType.SmallerThan => comparedField.intValue < (int)item,
                ComparisonType.SmallerOrEqual => comparedField.intValue <= (int)item,
                ComparisonType.GreaterOrEqual => comparedField.intValue >= (int)item,
                _ => false,
            };
        }

        private bool CompareFloat(ComparisonType comparisonType, object item)
        {
            return comparisonType switch
            {
                ComparisonType.Equals => comparedField.floatValue.Equals(item),
                ComparisonType.NotEqual => !comparedField.floatValue.Equals(item),
                ComparisonType.GreaterThan => comparedField.floatValue > (float)item,
                ComparisonType.SmallerThan => comparedField.floatValue < (float)item,
                ComparisonType.SmallerOrEqual => comparedField.floatValue <= (float)item,
                ComparisonType.GreaterOrEqual => comparedField.floatValue >= (float)item,
                _ => false,
            };
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            drawIfAttribute = attribute as ShowIfAttribute;
            if (ShowMe(property))
            {
                switch (drawIfAttribute.trueCaseDisablingType)
                {
                    case DisablingType.Draw:
                        EditorGUI.PropertyField(position, property, label);
                        break;
                    case DisablingType.ReadOnly:
                        GUI.enabled = false;
                        EditorGUI.PropertyField(position, property, label);
                        GUI.enabled = true;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (drawIfAttribute.falseCaseDisablingType)
                {
                    case DisablingType.Draw:
                        EditorGUI.PropertyField(position, property, label);
                        break;
                    case DisablingType.ReadOnly:
                        GUI.enabled = false;
                        EditorGUI.PropertyField(position, property, label);
                        GUI.enabled = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

