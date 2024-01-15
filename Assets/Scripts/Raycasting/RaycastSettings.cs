using UnityEngine;

namespace VirtualMaze.Assets.Scripts.Raycasting
{
    public class RaycastSettings
    {
        public float DistToScreen { get; private set; }
        public float GazeRadius { get; private set; }
        public float Density { get; private set; }
        public Rect ScreenPixelDims { get; private set; }
        public Rect ScreenCmDims { get; private set; }


        public static readonly float DefaultGazeRadius = 15f;
        public static readonly float DefaultDensity = 1f;
        public static readonly float DefaultDistToScreen = 68f;

        // default for pixels
        public static readonly float DefaultScreenPixelDimX = 1920f;
        public static readonly float DefaultScreenPixelDimY = 1080f;
        
        public static readonly Rect DefaultScreenPixelDims = new Rect(0, 0, DefaultScreenPixelDimX, DefaultScreenPixelDimY);

        // Default values for cm
        public static readonly float DefaultScreenCmDimX = 60f;
        public static readonly float DefaultScreenCmDimY = 40f;
        public static readonly Rect DefaultScreenCmDims = new Rect(0, 0, DefaultScreenCmDimX, DefaultScreenCmDimY);

        // Private constructor
        private RaycastSettings(float distToScreen, float gazeRadius, float density, Rect screenPixelDims, Rect screenCmDims) {
            DistToScreen = distToScreen;
            GazeRadius = gazeRadius;
            Density = density;
            ScreenPixelDims = screenPixelDims;
            ScreenCmDims = screenCmDims;
        }

        public static RaycastSettings FromString(string distToScreen, 
            string gazeRadius, string density, string screenPixelX, 
            string screenPixelY, string screenCmX, string screenCmY) {

            float distToScreenValue = default;
            float gazeRadiusValue = default;
            float densityValue = default;
            Rect screenPixelDimsValue = default;
            Rect screenCmDimsValue = default;
            bool parseSuccess =
                parseFloat(distToScreen, out distToScreenValue, defaultValue: DefaultDistToScreen) &&
                parseFloat(gazeRadius, out gazeRadiusValue, defaultValue: DefaultGazeRadius) &&
                parseFloat(density, out densityValue, defaultValue: DefaultDensity) &&
                parseDimensions(screenPixelX, screenPixelY, out screenPixelDimsValue, defaultValue: DefaultScreenPixelDims) &&
                parseDimensions(screenCmX, screenCmY, out screenCmDimsValue, defaultValue: DefaultScreenCmDims);

            if (!parseSuccess) {
                return null;
            }

            return new RaycastSettings(distToScreenValue, gazeRadiusValue, densityValue, screenPixelDimsValue, screenCmDimsValue);
        }

        public static RaycastSettings FromFloat(float distToScreen, float gazeRadius, float density, float screenPixelX, float screenPixelY, float screenCmX, float screenCmY) {
            Rect screenPixelDimsValue = new Rect(0, 0, screenPixelX, screenPixelY);
            Rect screenCmDimsValue = new Rect(0, 0, screenCmX, screenCmY);

            return new RaycastSettings(distToScreen, gazeRadius, density, screenPixelDimsValue, screenCmDimsValue);
        }

        private static bool parseDimensions(string x, string y, out Rect rect, Rect defaultValue) {
            float xDefault = defaultValue.width;
            float yDefault = defaultValue.height;
            
            float rectX = default;
            float rectY = default;

            bool xSuccess = parseFloat(x, out rectX, xDefault);
            bool ySuccess = parseFloat(y, out rectY, yDefault);

            if (xSuccess && ySuccess) {
                // set rect only if both succeed
                rect = new Rect(0, 0, rectX, rectY);
            } else {
                rect = default;
                // set rect to a default to satisfy compiler
                // Value should be irrelevant if failed anyway.
            }
            

            return xSuccess && ySuccess;
        }
        
        private static bool parseFloat(string stringFloat, out float result, float defaultValue = default) {
            if (string.IsNullOrEmpty(stringFloat)) {
                result = defaultValue;
                return true;
            } else {
                return float.TryParse(stringFloat, out result);
            }
        }
    }
}
