using System;
using UnityEngine;

namespace KorYmeLibrary.Attributes
{
    public class ForceInterfaceAttribute : PropertyAttribute
    {
        public readonly Type interfaceType;

        public ForceInterfaceAttribute(Type interfaceType)
        {
            this.interfaceType = interfaceType;
        }
    }
}
