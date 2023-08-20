using System;
using System.Runtime.CompilerServices;
using Python.Runtime;

public class PSetup
{

    //[ModuleInitializer()]
    public static void Setup()
    {
        if (PythonEngine.IsInitialized)
        {
            return;
        }

        string dllPath = @"D:\Python3114\python311.dll";
        //string pythonHomePath = @"D:\other\ooba\installer_files\env";
        //// 对应python内的重要路径
        //string[] py_paths = {"DLLs", "lib", "lib/site-packages", "lib/site-packages/win32"
        //        , "lib/site-packages/win32/lib", "lib/site-packages/Pythonwin" };
        //string pySearchPath = $"{pythonHomePath};";
        //foreach (string p in py_paths)
        //{
        //    pySearchPath += $"{pythonHomePath}/{p};";
        //}

        //// 此处解决BadPythonDllException报错
        Runtime.PythonDLL = dllPath;
        ////Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", dllPath);
        //// 配置python环境搜索路径解决PythonEngine.Initialize() 崩溃
        //PythonEngine.PythonHome = pythonHomePath;
        //PythonEngine.PythonPath = pySearchPath;
        PythonEngine.Initialize();

    }

    //using (Py.GIL())
    //{
    //    dynamic np = Py.Import("numpy");
    //    PyObject ar = np.array(new int[] { 1, 2, 3, 4 });
    //    Console.WriteLine(ar);

    //}
}