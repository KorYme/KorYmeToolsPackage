using System;
using UnityEngine;


namespace KorYmeLibrary.Attributes
{
    public class ButtonAttribute : MethodAttribute
    {
        public string MethodName { get; }
        public ButtonAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}