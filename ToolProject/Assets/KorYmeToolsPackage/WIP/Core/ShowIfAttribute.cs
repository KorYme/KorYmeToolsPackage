using System;
using UnityEngine;

namespace KorYmeLibrary.Attributes
{
    /// <summary>
    /// Has to be a serialized field
    /// Don't draw the field if the condition is not filled
    /// Else choose if it has to be readonly or not drawn
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string comparedPropertyName;
        public object[] comparedValues;
        public bool simpleBoolean = false;
        public ComparisonType comparisonType;
        public DisablingType trueCaseDisablingType;
        public DisablingType falseCaseDisablingType;

        public ShowIfAttribute(string comparedPropertyName)
        {
            this.comparedPropertyName = comparedPropertyName;
            this.comparedValues = new object[] { };
            this.comparisonType = ComparisonType.Equals;
            this.trueCaseDisablingType = DisablingType.Draw;
            this.falseCaseDisablingType = DisablingType.DontDraw;
            simpleBoolean = false;
        }

        public ShowIfAttribute(string comparedPropertyName, object comparedValue) 
        {
            this.comparedPropertyName = comparedPropertyName;
            this.comparedValues = new object[] { comparedValue };
            this.comparisonType = ComparisonType.Equals;
            this.trueCaseDisablingType = DisablingType.Draw;
            this.falseCaseDisablingType = DisablingType.DontDraw;
            simpleBoolean = false;
        }

        public ShowIfAttribute(string comparedPropertyName, object comparedValue, DisablingType falseCaseDisablingType = DisablingType.Draw,
            DisablingType trueCaseDisablingType = DisablingType.DontDraw, ComparisonType comparisonType = ComparisonType.Equals)
        {
            this.comparedPropertyName = comparedPropertyName;
            this.comparedValues = new object[] {comparedValue};
            this.comparisonType = comparisonType;
            this.trueCaseDisablingType = trueCaseDisablingType;
            this.falseCaseDisablingType = falseCaseDisablingType;
            simpleBoolean = false;
        }
    
        public ShowIfAttribute(string comparedPropertyName, object[] comparedValue, ComparisonType comparisonType = ComparisonType.Equals,
            DisablingType trueCaseDisablingType = DisablingType.Draw, DisablingType falseCaseDisablingType = DisablingType.DontDraw)
        {
            this.comparedPropertyName = comparedPropertyName;
            this.comparedValues = comparedValue;
            this.comparisonType = comparisonType;
            this.trueCaseDisablingType = trueCaseDisablingType;
            this.falseCaseDisablingType = falseCaseDisablingType;
            simpleBoolean = false;
        }

        public ShowIfAttribute(bool simpleBoolean)
        {
            this.comparedPropertyName = null;
            this.comparedValues = null;
            this.comparisonType = ComparisonType.Equals;
            this.simpleBoolean = simpleBoolean;
            this.trueCaseDisablingType = DisablingType.Draw;
            this.falseCaseDisablingType = DisablingType.DontDraw;
        }

        public ShowIfAttribute(bool simpleBoolean,
        DisablingType trueCaseDisablingType = DisablingType.Draw, DisablingType falseCaseDisablingType = DisablingType.DontDraw)
        {
            this.comparedPropertyName = null;
            this.comparedValues = null;
            this.comparisonType = ComparisonType.Equals;
            this.simpleBoolean = simpleBoolean;
            this.trueCaseDisablingType = trueCaseDisablingType;
            this.falseCaseDisablingType = falseCaseDisablingType;
        }
    }

    public enum ComparisonType
    {
        Equals = 1,
        NotEqual = 2,
        GreaterThan = 3,
        SmallerThan = 4,
        SmallerOrEqual = 5,
        GreaterOrEqual = 6,
    }

    public enum DisablingType
    {
        Draw = 1,
        ReadOnly = 2,
        DontDraw = 3,
    }
}