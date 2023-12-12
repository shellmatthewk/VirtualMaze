using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace VirtualMaze.Assets.Scripts.Utils
{
        public static class RayConstants
        {
            // Use this constant to represent an uninitialized or invalid Ray
            public static readonly Ray NullRay = new Ray(Vector3.zero, Vector3.zero);
        }

}