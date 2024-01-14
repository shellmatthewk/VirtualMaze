using System.Transactions;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eyelink.Structs;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using VirtualMaze.Assets.Scripts.Utils;
using UnityEditor;
using System.Runtime.CompilerServices;
using VirtualMaze.Assets.Scripts.Utils;

namespace VirtualMaze.Assets.Scripts.Raycasting {

    // TODO : Implement an async that manages its own memory and IO and async processes then writes 
    // Idea : Job that schedules jobs for raycasting in a fixation, then schedules a job to write to file
    // need to expose job method in a recorder class
    
    public class AreaRaycastManager
    {


        

        float angularRadius;
        float angularDensity;
        float distToScreen;
        Rect screenDims;
        Rect pixelDims;
        public static float SCENE_MAX_DIST = 7.5f;
        // based of tan(30) * 25 units, rounded up to give a buffer.

        public AreaRaycastManager(
            float angularRadius, float angularDensity, float distToScreen, 
                Rect screenDims, Rect pixelDims) {


            this.angularRadius = angularRadius;
            this.angularDensity = angularDensity;
            this.distToScreen = distToScreen;
            this.screenDims = screenDims;
            this.pixelDims = pixelDims;

        }

        public async Task<RaycastHit[]> ScheduleAreaCasting(Fsample sampleToCast, Camera viewport, JobHandle dependency = default) {
            if (sampleToCast.RightGaze.isNaN()) {
                return new RaycastHit[] {};
                // return empty if the sample gaze is nan (has any nan component)
            }
            List<Fsample> toCast = generateFromAngle(sampleToCast);
            List<Ray> rays = ConvertToRays(toCast, viewport);

            NativeArray<RaycastCommand> raycastCommands = 
                new NativeArray<RaycastCommand>(rays.Count, Allocator.TempJob);
            NativeArray<RaycastHit> resultsAsNative = new NativeArray<RaycastHit>(rays.Count, Allocator.TempJob);
            
            for (int i = 0; i < rays.Count; i++) {
                Ray ray = rays[i];
                if (RayConstants.IsAbsoluteEqual(ray, RayConstants.NullRay)) {
                    continue;
                     // will leave the raycast command as null, no raycasting will be performed here.
                }
                raycastCommands[i] = new RaycastCommand(ray.origin, ray.direction, layerMask : BinWallManager.ignoreBinningLayer);
            }
            
            JobHandle raycastHandle = RaycastCommand.ScheduleBatch(
                raycastCommands, resultsAsNative, 1, dependency);
            
            raycastHandle.Complete();
            UnityEngine.Debug.Log($"Completed areacast for {sampleToCast}");
            return resultsAsNative.ToArray();
        }

        public void ScheduleHitWriteAndDispose(
            uint time,
            Task<RaycastHit[]> resultsTask,
            Vector3 subjectLoc,
            Vector2 rawGaze,
            RayCastWriteManager writeManager) {

            // Ensure resultsTask is completed synchronously
            RaycastHit[] results = resultsTask.Result;

            void WriteTask(RaycastHit hit) {
                string objName = RelativeHitLocFinder.getChainedName(hit.transform.gameObject);
                Vector3 hitLoc = hit.point;
                RayCastWriteData toWrite = new RayCastWriteDataBuilder()
                    .WithTime(time)
                    .WithSubjectLoc(subjectLoc)
                    .WithRawGaze(rawGaze)
                    .WithHitObjLocation(hitLoc)
                    .WithObjName(objName)
                    .Build();

                writeManager.Write(toWrite);
            }

            void NullWriteTask() {
                string objName = "NaN(Null RaycastHit)";
                Vector3 hitLoc = new Vector3(0, 0, 0);
                RayCastWriteData toWrite = new RayCastWriteDataBuilder()
                    .WithTime(time)
                    .WithSubjectLoc(subjectLoc)
                    .WithRawGaze(rawGaze)
                    .WithHitObjLocation(hitLoc)
                    .WithObjName(objName)
                    .Build();

                writeManager.Write(toWrite);

            }

            if (results.Length == 0) {
                UnityEngine.Debug.Log($"Wrote NaN result");
                string objName = "NaN(NaN Gaze)";
                Vector3 hitLoc = new Vector3(0, 0, 0);
                RayCastWriteData toWrite = new RayCastWriteDataBuilder()
                    .WithTime(time)
                    .WithSubjectLoc(subjectLoc)
                    .WithRawGaze(rawGaze)
                    .WithHitObjLocation(hitLoc)
                    .WithObjName(objName)
                    .Build();

                writeManager.Write(toWrite);

            }

            UnityEngine.Debug.Log($"Trying to write {results.Length} many non-NaN results");
            foreach (RaycastHit hit in results) {
                if (hit.transform == null) {
                    UnityEngine.Debug.Log($"null raycasthit");
                    NullWriteTask();
                }
                WriteTask(hit);
            }

            UnityEngine.Debug.Log($"Wrote {results.Length} many non-NaN results");
        }


        private List<Ray> ConvertToRays(List<Fsample> toConvert, Camera viewport) {
            // Pre-allocate the memory because we know the exact length of the output list
            List<Ray> outList = new List<Ray>(toConvert.Count);
            foreach (Fsample fsample in toConvert) {
                if (ScreenSaver.IsInScreenBounds(fsample.RightGaze)) {
                    Vector2 viewportGaze = RangeCorrector.HD_TO_VIEWPORT.
                        correctVector(fsample.RightGaze);
                    outList.Add(viewport.ViewportPointToRay(viewportGaze));
                } else {
                    outList.Add(RayConstants.NullRay);
                }
            }
            return outList;
        }
        private List<Fsample> generateFromAngle(Fsample sample) {
                float stepSize = 1.0f / this.angularDensity;
                List<Fsample> outList = new List<Fsample>();
                RangeCorrector pixelToCm = new RangeCorrector(pixelDims, screenDims);
                RangeCorrector CmToPixel = new RangeCorrector(screenDims, pixelDims);
                Vector2 CmOffset = pixelToCm.correctVector(sample.rawRightGaze);
                float xRadianLoc = Mathf.Atan2(CmOffset.x,distToScreen);
                float yRadianLoc = Mathf.Atan2(CmOffset.y,distToScreen);

                // iterate through all possible angular offsets ...
                for (float i = -angularRadius; i <= angularRadius; i += stepSize) {
                    for (float j = -angularRadius; j <= angularRadius; j += stepSize) {
                        if (i * i + j * j < angularRadius * angularRadius) {
                            // but accept only those that fall within the radius

                            // Generate new Fsample based on angular offset
                            float xRadianNew = xRadianLoc + Mathf.Deg2Rad * i;
                            float yRadianNew = yRadianLoc + Mathf.Deg2Rad * j;
                            
                            float xCmNew = Mathf.Tan(xRadianNew) * distToScreen;
                            float yCmNew = Mathf.Tan(yRadianNew) * distToScreen;

                            Vector2 vectorCmNew = new Vector2(xCmNew, yCmNew);
                            // the new location of the gaze in cm coordinates
                            if (!screenDims.Contains(vectorCmNew)) {
                                continue; // sanity check for inside location
                            }
                            Vector2 vectorPixelNew  = CmToPixel.correctVector(vectorCmNew);

                            
                            Vector2 vectorPixelRounded = roundToQuarterPixel(vectorPixelNew);

                            Fsample temp = new Fsample(sample.time,
                                vectorPixelRounded.x,
                                vectorPixelRounded.y,
                                sample.dataType);
                            outList.Add(temp);

                            
                        }
                    }
                }
                return outList;
        }
        private Vector2 roundToQuarterPixel(Vector2 gazeXY) {
            float rawX = gazeXY.x;
            float rawY = gazeXY.y;

            float roundedX = Mathf.Round(rawX * 4) / 4;
            float roundedY = Mathf.Round(rawY * 4) / 4; 
            // multiplying by four, rounding to whole number, and dividing by 4 rounds to .25

            return new Vector2(roundedX, roundedY);
        }


    }

}
