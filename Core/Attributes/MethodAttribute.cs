using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KorYmeLibrary.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class MethodAttribute : PropertyAttribute
    {

    }
}
