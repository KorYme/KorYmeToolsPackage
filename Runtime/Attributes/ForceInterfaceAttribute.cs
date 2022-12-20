using System;
using UnityEngine;

namespace ToolLibrary
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
