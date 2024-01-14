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
                float.TryParse(distToScreen, out distToScreenValue) &&
                float.TryParse(gazeRadius, out gazeRadiusValue) &&
                float.TryParse(density, out densityValue) &&
                parseDimensions(screenPixelX, screenPixelY, out screenPixelDimsValue) &&
                parseDimensions(screenCmX, screenCmY, out screenCmDimsValue);

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

        private static bool parseDimensions(string x, string y, out Rect rect) {
            rect = default;

            if (float.TryParse(x, out float xFloat) && float.TryParse(y, out float yFloat)) {
                rect = new Rect(0, 0, xFloat, yFloat);
                return true;
            }

            return false;
        }
    }
}
