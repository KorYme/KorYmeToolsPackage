using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace KorYmeLibrary.Attributes
{
    public abstract class Button
    {
        protected string _methodName;
        public string MethodName
        { 
            get => _methodName;
            set => _methodName = value;
        }

        protected MethodInfo _methodInfo;
        public MethodInfo MethodInfo
        {
            get => _methodInfo;
            set => _methodInfo = value;
        }

        protected Button(string methodName)
        {

        }
    }
}
