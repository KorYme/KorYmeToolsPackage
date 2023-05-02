using System;
using UnityEngine;

namespace KorYmeLibrary.Attributes
{
    public class ForceInterfaceAttribute : PropertyAttribute
    {
        #region FIELDS
        public readonly Type _interfaceType;
        #endregion

        #region CONSTRUCTORS
        public ForceInterfaceAttribute(Type interfaceType)
        {
            this._interfaceType = interfaceType;
        }
        #endregion
    }
}
