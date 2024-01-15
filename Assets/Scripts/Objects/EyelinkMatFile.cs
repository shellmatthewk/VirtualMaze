using System;
using VirtualMaze.Assets.Scripts.Utils;

public class EyelinkMatFile : AbstractHDF5File {
    public readonly double[,] trial_index;
    public readonly int[,] trial_codes;
    public readonly uint[,] timestamps;
    public readonly float[,] eyePos;

    public readonly double[] fixationStarts;
    public readonly double[] fixationEnds;
    private static readonly int FIXATION_START_COL = 0;
    private static readonly int FIXATION_END_COL = 1;

    public int Length { get => timestamps.Length; }

    public EyelinkMatFile(string _filename) : base(_filename) {
        using (HDF5Group data = HDFHelper.GetMyDataGroup(this)) {
            trial_index = HDFHelper.GetDataMatrix<double>(data, "trial_timestamps");
            eyePos = HDFHelper.GetDataMatrix<float>(data, "eye_pos");
            timestamps = HDFHelper.GetDataMatrix<uint>(data, "timestamps");
            trial_codes = HDFHelper.GetDataMatrix<int>(data, "trial_codes");

            double[,] fixationTimes = HDFHelper.GetDataMatrix<double>(data, "fix_times");
            fixationStarts = TwoDArrayUtils.GetColumn(fixationTimes,FIXATION_START_COL);
            fixationEnds = TwoDArrayUtils.GetColumn(fixationTimes,FIXATION_END_COL);
        }
        //Hdf5Group grp = Groups["el"].Groups["data"];
        //trial_index = (double[,])grp.Datasets["trial_timestamps"].GetData();
        ////indices = (double[,])grp.Datasets["indices"].GetData();
        //eyePos = (float[,])grp.Datasets["eye_pos"].GetData();
        //timestamps = (uint[,])grp.Datasets["timestamps"].GetData();

        //Array a = grp.Datasets["trial_codes"].GetData();

        //Debug.Log(a);

        //trial_codes = (int[,])grp.Datasets["trial_codes"].GetData();
        
    }



}
