using System;
using System.Runtime.InteropServices;

public static class ComInterop
{
    [DllImport("oleaut32.dll", PreserveSig = false)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public static extern object GetActiveObject([MarshalAs(UnmanagedType.BStr)] string progID);
}
