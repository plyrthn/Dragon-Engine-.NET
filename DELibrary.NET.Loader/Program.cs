using System;
using System.IO;
using System.Reflection;

namespace OOELibrary.Loader
{
    public class Program
    {
        //The main purpose of this assembly is to have the library *not* be the entrypoint.
        //This way all loaded mods can access the statics from the library instead of having a copy of the lib be automatically loaded as a dependency.
        public static void Main(string[] args)
        {
            string logPath = null;
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string loaderLoc = Assembly.GetExecutingAssembly().Location;
                string loaderDir = string.IsNullOrEmpty(loaderLoc) ? baseDir : Path.GetDirectoryName(loaderLoc);

                // Write diagnostic log next to wherever we think we are
                logPath = Path.Combine(baseDir, "loader_debug.txt");
                File.WriteAllText(logPath,
                    "BaseDirectory: " + baseDir + "\n" +
                    "LoaderLocation: " + loaderLoc + "\n" +
                    "LoaderDir: " + loaderDir + "\n");

                string binPath = Path.Combine(loaderDir, "DELibrary.NET.bin");
                string binPath2 = Path.Combine(baseDir, "DELibrary.NET.bin");
                string dllPath = Path.Combine(baseDir, "mods/DE Library/DELibrary.NET.dll");

                File.AppendAllText(logPath,
                    "Try1: " + binPath + " exists=" + File.Exists(binPath) + "\n" +
                    "Try2: " + binPath2 + " exists=" + File.Exists(binPath2) + "\n" +
                    "Try3: " + dllPath + " exists=" + File.Exists(dllPath) + "\n");

                string libPath = null;
                if (File.Exists(binPath)) libPath = binPath;
                else if (File.Exists(binPath2)) libPath = binPath2;
                else libPath = dllPath;

                File.AppendAllText(logPath, "Using: " + libPath + "\n");

                byte[] asmBytes = File.ReadAllBytes(libPath);
                Assembly asm = Assembly.Load(asmBytes);

                File.AppendAllText(logPath, "Loaded assembly: " + asm.FullName + "\n");

                MethodInfo entry = asm.EntryPoint;
                if (entry != null)
                {
                    File.AppendAllText(logPath, "Invoking entry: " + entry.Name + "\n");
                    entry.Invoke(null, new object[] { new string[] { baseDir } });
                }
                else
                    File.AppendAllText(logPath, "No entry point!\n");
            }
            catch(Exception ex)
            {
                string msg = "Loader Error: " + ex.GetType().Name + " " + ex.Message + "\n" + ex.StackTrace + "\n";
                if (ex.InnerException != null)
                    msg += "Inner: " + ex.InnerException.GetType().Name + " " + ex.InnerException.Message + "\n";
                Console.WriteLine(msg);
                if (logPath != null)
                    try { File.AppendAllText(logPath, msg); } catch { }
            }
        }
    }
}
