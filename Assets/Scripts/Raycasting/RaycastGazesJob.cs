using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eyelink.Structs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;


namespace VirtualMaze.Assets.Scripts.Raycasting
{
class RaycastGazesJob : IDisposable {
        protected int numSamples = 0;
        protected List<Fsample> binSamples;
        protected NativeArray<RaycastHit> results;
        protected NativeArray<RaycastCommand> cmds;
        public JobHandle h;

        public RaycastGazesJob(JobHandle h, int numSamples, List<Fsample> binSamples, NativeArray<RaycastHit> results, NativeArray<RaycastCommand> cmds) {
            this.numSamples = numSamples;
            this.binSamples = binSamples;
            this.results = results;
            this.cmds = cmds;
            this.h = h;
        }
        public RaycastGazesJob(RaycastGazesJob raycastGazesJob) {
            if (raycastGazesJob != null) {
                this.numSamples = raycastGazesJob.numSamples;
                this.binSamples = raycastGazesJob.binSamples;
                this.results = raycastGazesJob.results;
                this.cmds = raycastGazesJob.cmds;
                this.h = raycastGazesJob.h;
            } else {
                this.numSamples = 0;
                this.binSamples = new List<Fsample>();
                this.results = new NativeArray<RaycastHit>();
                this.cmds = new NativeArray<RaycastCommand>();
                this.h = default;
            }
        }

        public virtual void Process(AllFloatData currData, RayCastRecorder recorder, Transform robot, bool isLastSampleInFrame, GazePointPool gazePointPool, bool displayGazes, RectTransform GazeCanvas, Camera viewport) {
            int lastGazeIndex = numSamples - 2;
            Image img = null;
            //Debug.Log($"Writing datapoints from {binSamples[0].time} to {binSamples[binSamples.Count-1].time}");
            for (int i = 0; i < numSamples; i++) {
                if (i == lastGazeIndex && currData is MessageEvent) {
                    recorder.FlagEvent(((MessageEvent)currData).message);
                }

                Fsample fsample = binSamples[i];
                RaycastHit raycastHit = results[i];
                if (raycastHit.collider != null) {
                    Transform hitObj = raycastHit.transform;
                    recorder.WriteSample(
                            type: fsample.dataType,
                            time: fsample.time,
                            objName: RelativeHitLocFinder.getChainedName(raycastHit.transform.gameObject),
                            centerOffset: RelativeHitLocFinder.getRelativeHit(raycastHit),
                            hitObjLocation: hitObj.position,
                            pointHitLocation: raycastHit.point,
                            rawGaze: fsample.rawRightGaze,
                            subjectLoc: robot.position,
                            subjectRotation: robot.rotation.eulerAngles.y,
                            isLastSampleInFrame: i == lastGazeIndex && isLastSampleInFrame);
                }
                else {
                    recorder.IgnoreSample(
                            type: fsample.dataType,
                            time: fsample.time,
                            rawGaze: fsample.rawRightGaze,
                            subjectLoc: robot.position,
                            subjectRotation: robot.rotation.eulerAngles.y,
                            isLastSampleInFrame: i == lastGazeIndex && isLastSampleInFrame
                        );
                }
                if (displayGazes) {
                    img = gazePointPool.AddGazePoint(GazeCanvas, viewport, fsample.RightGaze);
                }
            }
            if (img != null) {
                img.color = Color.red;
            }
        }
        public virtual void Dispose() {
            results.Dispose();
            cmds.Dispose();
        }
    }
}