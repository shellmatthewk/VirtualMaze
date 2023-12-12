using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualMaze.Assets.Scripts.Raycasting
{
    public struct AreaCastJob : IJobParalellFor {
        private NativeArray<Ray> rays;
        private NativeArray<RaycastHit> results;
        private NativeArray<RaycastCommand> commands;
        private AreaCastJob(NativeArray<Ray> rays,
            NativeArray<RaycastHit> results, 
            NativeArray<RaycastCommand> commands) {
            this.rays = rays;
            this.results = results;
            this.commands = commands;

        }


        public void Execute() {
            
        }

        public JobHandle ConsumeRaysAndScheduleCast(NativeArray<Ray> raysToCast,
                out NativeArray<RaycastHit> results) {
            // Note to anyone looking at this later, NativeArray-s are essentially wrappers around pointers
            // So if you're trying to optimise, trying to manipulate raysToCast to be passed by reference
            // Should not improve performance much if at all
        
            

        }
        
    }
}