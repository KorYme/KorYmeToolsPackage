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
        Project = 2,
        Both = 4,
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SceneModificationAttribute : Attribute
    {
        public ObjectLocation _objectLocation;

        public SceneModificationAttribute(ObjectLocation objectLocation = ObjectLocation.Both)
        {
            _objectLocation = objectLocation;
        }
    }
}