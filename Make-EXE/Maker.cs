using System;
using System.IO;
using System.Diagnostics;
[assembly: System.Reflection.AssemblyVersion("0.0.0.0")]
[assembly: System.Reflection.AssemblyFileVersion("0.0.0.0")]
[assembly: System.Reflection.AssemblyProduct("")]
[assembly: System.Reflection.AssemblyCompany("")]
[assembly: System.Reflection.AssemblyDescription("")]
[assembly: System.Reflection.AssemblyCopyright("")]
namespace Make_EXE
{
    class Program
    {
        static bool redirect = false;
        static void Main(string[] args)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();
            //Console.Title = resources[0];
            //Console.WriteLine("Extracting resource files...");
            var basePath = System.IO.Path.GetTempPath();
            if (System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem)
            {
                basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            }
            var workingDir = "";
            var count = 0;
            while (workingDir == "")
            {
                try
                {
                    if (Directory.Exists(Path.Combine(basePath, "Make-EXE" + count)))
                    {
                        Directory.Delete(Path.Combine(basePath, "Make-EXE" + count), true);
                    }
                    else
                    {
                        workingDir = Path.Combine(basePath, "Make-EXE" + count + "\\");
                        Directory.CreateDirectory(workingDir);
                    }
                }
                catch
                {
                    count++;
                }
            }
            count = 1;
            foreach (var resource in resources)
            {
               // Console.WriteLine("Extracting file " + count + " of " + resources.Length + "...");
                using (var rs = assembly.GetManifestResourceStream(resource))
                {
                    using (var fs = new FileStream(workingDir + resource, FileMode.Create))
                    {
                        rs.CopyTo(fs);
                        fs.Flush();
                        fs.Close();
                    }
                }
                count++;
            }

            var arguments = "";
            foreach (var arg in args)
            {
                arguments = arguments + " \"" + arg + "\"";
            }
            //Console.WriteLine("Starting up...");

            var psi = new ProcessStartInfo();
            psi.UseShellExecute = false;
            psi.WorkingDirectory = workingDir;

            if (Path.GetExtension(resources[0]).ToLower() == ".ps1")
            {
                psi.FileName = "powershell.exe";
                psi.Arguments = "-executionpolicy bypass -WindowStyle hidden -file \"" + resources[0] + "\"" + arguments;
            }
            else if (Path.GetExtension(resources[0]).ToLower() == ".bat")
            {
                psi.FileName = "cmd.exe";
                psi.Arguments = "/c \"\"" + resources[0] + "\"" + arguments + "\"";
            }

            if (redirect)
            {
                var proc = new Process();
                proc.EnableRaisingEvents = true;
                proc.StartInfo = psi;
                psi.RedirectStandardOutput = true;
                proc.Start();
                proc.WaitForExit();
                //Console.WriteLine(proc.StandardOutput.ReadToEnd());
            }
            else
            {
                System.Diagnostics.Process.Start(psi);
            }
        }
    }
}