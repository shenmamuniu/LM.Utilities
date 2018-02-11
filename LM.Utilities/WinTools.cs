using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LM.Utilities
{
    /// <summary>
    /// It'a about interactive with local system.
    /// </summary>
    public class WinTools
    {
        /// <summary>
        /// Exccute commands in cmd
        /// </summary>
        /// <param name="inputAction">like p=>{p("");p("");}</param>
        public static void ExecBatCommand(Action<Action<string>> inputAction,DataReceivedEventHandler dataReceivedEventHandler, DataReceivedEventHandler errEventHandler) {
            Process pro = null;
            StreamWriter sIn = null;
            StreamReader sOut = null;

            try
            {
                pro = new Process();
                pro.StartInfo.FileName = "cmd.exe";
                pro.StartInfo.UseShellExecute = false;
                pro.StartInfo.CreateNoWindow = true;
                pro.StartInfo.RedirectStandardInput = true;
                pro.StartInfo.RedirectStandardOutput = true;
                pro.StartInfo.RedirectStandardError = true;

                pro.OutputDataReceived += dataReceivedEventHandler;
                pro.ErrorDataReceived += errEventHandler;

                pro.Start();
                sIn = pro.StandardInput;
                sIn.AutoFlush = true;

                pro.BeginOutputReadLine();
                inputAction(value => sIn.WriteLine(value));

                pro.WaitForExit();
            }
            finally
            {
                if (pro != null && !pro.HasExited)
                    pro.Kill();

                if (sIn != null)
                    sIn.Close();
                if (sOut != null)
                    sOut.Close();
                if (pro != null)
                    pro.Close();
            }
        }
        /// <summary>
        /// get all software installed
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllApps() {
            List<string> appList = new List<string>();
            string tempType = null;
            object displayName = null, uninstallString = null, releaseType = null;
            RegistryKey currentKey = null;
            RegistryKey pregkey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
            try
            {                
                foreach (string item in pregkey.GetSubKeyNames())
                {
                    currentKey = pregkey.OpenSubKey(item);
                    displayName = currentKey.GetValue("DisplayName");
                    uninstallString = currentKey.GetValue("UninstallString");
                    releaseType = currentKey.GetValue("ReleaseType");                    
                    bool isSecurityUpdate = false;
                    if (releaseType != null)
                    {
                        tempType = releaseType.ToString();
                        if (tempType == "Security Update" || tempType == "Update")
                            isSecurityUpdate = true;
                    }

                    if (!isSecurityUpdate && displayName != null && uninstallString != null)
                    {
                        //appList.Add(softNum.ToString() + "," + displayName.ToString() + "," + uninstallString.ToString());
                        appList.Add(displayName.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message.ToString(), "Exception");
            }
            finally
            {
                pregkey.Close();
            }
            return appList;
        }
        #region check app started.
        /// <summary>
        /// get windows intptr from memory.if intptr equals IntPtr.Zero,it shows this app hasn't started yet,or this app has started.
        /// http://blog.csdn.net/lightspear/article/details/50831949
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IntPtr GetMemory(string key)
        {

            try
            {
                using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(key))
                {
                    using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                    {
                        IntPtr handler = IntPtr.Zero;
                        accessor.Read(0, out handler);
                        return handler;

                    }
                }
            }
            catch (FileNotFoundException)
            {
                return IntPtr.Zero;
            }
        }
        /// <summary>
        /// write window intptr into memory
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public static void WriteMemory(string key, IntPtr handler)
        {
            MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen(key, 1024000);
            using (MemoryMappedViewStream stream = mmf.CreateViewStream()) 
            {
                using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                {
                    accessor.Write(0, ref handler);
                }
            }
        } 
        #endregion
    }
}
