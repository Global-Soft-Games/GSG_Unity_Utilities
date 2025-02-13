using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GSGUnityUtilities.Runtime
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CreateAssetMenuExAttribute : Attribute
    {
        public string MenuName { get; }
        public string FileName { get; }

        public CreateAssetMenuExAttribute(string menuName, string fileName = null)
        {
            MenuName = menuName;
            FileName = fileName ?? "New " + menuName;
        }
    }
}
