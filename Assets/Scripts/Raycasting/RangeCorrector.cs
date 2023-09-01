using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace RangeCorrector{
    public class RangeCorrector {

        private Tuple<double,double,double,double>  originalRange;
        //x-start, x-end, y-start, y-end
        private Tuple<double,double,double,double> newRange; // Target range
        //x-start, x-end, y-start, y-end

        private static readonly Lazy<RangeCorrector> HD_TO_VIEWPORT_INTERNAL = 
        new Lazy<RangeCorrector>(() => new RangeCorrector(
            Tuple.Create(0.0,0.0,1920.0,1080.0),
            Tuple.Create(0.0,0.0,1.0,1.0))
            );
        
        public static RangeCorrector HD_TO_VIEWPORT {get {return HD_TO_VIEWPORT_INTERNAL.Value;}}
        public RangeCorrector(Tuple<double,double,double,double> originalRange,
                                Tuple<double,double,double,double> newRange) {
            
            this.originalRange = originalRange;
            this.newRange = newRange;
        }



        public Vector2 correctVector(Vector2 value) {
            
            // perform intervening calculations in double to try to mitigate errors
            double normalisedX = ((value.x - originalRange.Item1) / 
            (originalRange.Item2 - originalRange.Item1)); 
            double normalisedY = ((value.y - originalRange.Item3) / 
            (originalRange.Item4 - originalRange.Item3));

            double correctedX = (normalisedX * (newRange.Item2 - newRange.Item1)) + newRange.Item1;
            double correctedY = (normalisedY * (newRange.Item4 - newRange.Item3)) + newRange.Item3;

            return new Vector2((float) correctedX, (float) correctedY);
            // lose precision but vector2 is in float so no choice
        }

    }
}
