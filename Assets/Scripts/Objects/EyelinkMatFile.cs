public class EyelinkMatFile : AbstractHDF5File {
    public readonly double[,] trial_index;
    public readonly int[,] trial_codes;
    public readonly uint[,] timestamps;
    public readonly float[,] eyePos;

    public readonly double[] fixationStarts;
    public readonly double[] fixationEnds;

    public int Length { get => timestamps.Length; }

    public EyelinkMatFile(string _filename) : base(_filename) {
        using (HDF5Group data = HDFHelper.GetMyDataGroup(this)) {
            trial_index = HDFHelper.GetDataMatrix<double>(data, "trial_timestamps");
            eyePos = HDFHelper.GetDataMatrix<float>(data, "eye_pos");
            timestamps = HDFHelper.GetDataMatrix<uint>(data, "timestamps");
            trial_codes = HDFHelper.GetDataMatrix<int>(data, "trial_codes");
            fixationStarts = HDFHelper.GetDataMatrix<double>(data, "fix_times")[,0];
            fixationEnds = HDFHelper.GetDataMatrix<double>(data, "fix_times")[,1];
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
