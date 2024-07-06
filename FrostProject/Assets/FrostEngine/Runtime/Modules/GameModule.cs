using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrostEngine
{
    public class GameModule: MonoBehaviour
    {
        private static readonly Dictionary<Type, Module> _moduleMaps = new Dictionary<Type, Module>(ModuleImpSystem.DesignModuleCount);

    }
}