using System.Security.AccessControl;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace VirtualMaze.Assets.Scripts.Raycasting{
    public class RangeCorrector {

        private Rect  originalRange;
        //x-start, x-end, y-start, y-end
        private Rect newRange; // Target range
        //x-start, x-end, y-start, y-end

        private static readonly Lazy<RangeCorrector> HD_TO_VIEWPORT_INTERNAL = 
        new Lazy<RangeCorrector>(() => new RangeCorrector(
            new Rect(0,0,1920,1080),
            new Rect(0,0,1,1))
            );
        
        
        

        public static RangeCorrector HD_TO_VIEWPORT {get {return HD_TO_VIEWPORT_INTERNAL.Value;}}
        
        public RangeCorrector(Rect orig, Rect newRect) {
            this.originalRange = orig;
            this.newRange = newRect;
        }
        



        public Vector2 correctVector(Vector2 value) {
            value.x = Mathf.Clamp(value.x,originalRange.xMin,originalRange.xMax);
            value.y = Mathf.Clamp(value.y,originalRange.yMin,originalRange.yMax);
            return Rect.NormalizedToPoint(this.newRange,Rect.PointToNormalized(this.originalRange,value));
        }

    }
}
