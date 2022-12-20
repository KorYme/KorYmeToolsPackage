using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ToolLibrary
{
    public enum ObjectLocation
    {
        Scene = 1,
        PrefabsFolder = 2,
        ResourcesFolder = 4,
        Other = 8,
        All = 16,
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SceneModificationAttribute : Attribute
    {
        public ObjectLocation _objectLocation;

        public SceneModificationAttribute(ObjectLocation objectLocation = ObjectLocation.All)
        {
            _objectLocation = objectLocation;
        }
    }
}