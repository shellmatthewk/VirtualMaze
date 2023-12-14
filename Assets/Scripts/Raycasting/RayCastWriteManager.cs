using System;
using System.IO;
using System.Text;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;

namespace VirtualMaze.Assets.Scripts.Raycasting
{
    public abstract class RayCastWriteManager : IDisposable {
        private StreamWriter Writer;

        public static readonly int DEFAULT_BUFFERSIZE = 65536;
        public static readonly bool DEFAULT_APPEND = false;
        public static readonly Encoding DEFUALT_ENCODING = new ASCIIEncoding();

        public static RayCastWriteManager GetCsvManager(string fileLoc, 
            bool append, 
            System.Text.Encoding encoding, 
            int bufferSize) {
            if (!File.Exists(fileLoc)) {
                File.Create(fileLoc);
            }
            StreamWriter writer = new StreamWriter(fileLoc,append,encoding,bufferSize);
            return new CSVWriteManager(writer);
        }

       public static RayCastWriteManager GetCsvManager(string fileLoc){
            return GetCsvManager(fileLoc,
                DEFAULT_APPEND,
                DEFUALT_ENCODING,
                DEFAULT_BUFFERSIZE); 
                // Calls full-length constructor with default params.
            
        }

        public abstract JobHandle ScheduleWrite(RayCastWriteData data);

        public abstract void Write(RayCastWriteData data);

        public abstract void Dispose();


        // private class HdfWriteManager : RayCastWriteManager {

        //     // This is not an error. The library treats HDF files as longs.
        //     private long Hdf5File;
        //     private long status;
        //     protected HdfWriteManager(string fileLoc) {
        //         Hdf5File = H5F.create(fileLoc,H5F.ACC_TRUNC, H5P.DEFAULT, H5P.DEFAULT);
        //         status = H5P.set_chunk() 
        //     }
        // }

        private class CSVWriteManager : RayCastWriteManager {
            public CSVWriteManager(StreamWriter writer) {
                this.Writer = writer;
            }

            public override void Dispose()
            {
                Writer.Dispose();
            }

            public override JobHandle ScheduleWrite(RayCastWriteData data)
            {
                return new WriteJob(Write, data).Schedule();
            }

            public override void Write(RayCastWriteData data)
            {
                Writer.WriteLine(data.DataString);
            }        

        }


        private struct WriteJob : IJob {
            private Action<RayCastWriteData> WriteMethod;
            private NativeArray<RayCastWriteData> RayCastWriteData;
            public WriteJob(Action<RayCastWriteData> writeMethod, RayCastWriteData writeData) {
                this.WriteMethod = writeMethod;
                this.RayCastWriteData = new NativeArray<RayCastWriteData>(1, Allocator.TempJob);
                this.RayCastWriteData[0] = writeData;
            }

            public void Execute()
            {
                WriteMethod(RayCastWriteData[0]);
            }
        }


    }
    
}