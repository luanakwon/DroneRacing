using System;
using System.Runtime.InteropServices;

public class xgb
{
    [DllImport("xgboost.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int XGDMatrixCreateFromMat(float[] data, ulong nrows, ulong ncols, float missing, out IntPtr dmatrix);

    [DllImport("xgboost.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int XGBoosterCreate(IntPtr[] dmats, int len, out IntPtr booster);

    [DllImport("xgboost.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int XGBoosterLoadModel(IntPtr booster, string fname);

    [DllImport("xgboost.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int XGBoosterPredict(IntPtr booster, IntPtr dmat, int optionMask, int ntreeLimit, out IntPtr outResult);
}
