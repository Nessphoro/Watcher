/*
Copyright (c) 2013, Pavlo Malynin
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. All advertising materials mentioning features or use of this software
   must display the following acknowledgement:
   This product includes software developed by the <organization>.
4. Neither the name of the <organization> nor the
   names of its contributors may be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY Pavlo Malynin ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Watcher
{
    class Watch
    {
        FileSystemWatcher fsw;
        string OSPath;
        HashSet<string> ExtensionsToListen = new HashSet<string>();
        public Watch()
        {
            ExtensionsToListen.Add(".cpp");
            ExtensionsToListen.Add(".c");
            ExtensionsToListen.Add(".asm");
            fsw = new FileSystemWatcher();
            fsw.Created += fsw_Created;
            fsw.Deleted += fsw_Deleted;
            fsw.Renamed += fsw_Renamed;
            fsw.IncludeSubdirectories = true;
            fsw.NotifyFilter = NotifyFilters.FileName|NotifyFilters.DirectoryName;
        }

        void fsw_Renamed(object sender, RenamedEventArgs e)
        {
            try
            {
                FileInfo info = new FileInfo(e.OldFullPath);
                FileInfo newInfo=new FileInfo(e.FullPath);
                if(info.Extension=="")
                {
                
                }
                else if(!ExtensionsToListen.Contains(info.Extension) && !ExtensionsToListen.Contains(newInfo.Extension))
                {
                    //Check if it was a folder or a file. If a file then check its extension to make sure we're not renaming/deleting .txt or something
                    return;
                }
                string transformed = Makefile.TransformSourcePath(OSPath, info).Replace('/', '\\');
                string obj_folder;
                if (Program.Options["temp"])
                {
                    obj_folder = "/tmp/";
                }
                else
                {
                    obj_folder = "obj/";
                }
                File.Delete(OSPath + obj_folder + transformed + (info.Extension != "" ? ".o" : ""));
                //Delete the old object
            }
            catch
            {
    
            }
            Makefile make = new Makefile();
            make.Create(OSPath);
        }

        void fsw_Deleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                FileInfo info = new FileInfo(e.FullPath);
                if (info.Extension == "")
                {

                }
                else if (!ExtensionsToListen.Contains(info.Extension))
                {
                    //Check if it was a folder or a file. If a file then check its extension to make sure we're not renaming/deleting .txt or something
                    return;
                }
                string transformed = Makefile.TransformSourcePath(OSPath, info).Replace('/', '\\');
                string obj_folder;
                if (Program.Options["temp"])
                {
                    obj_folder = "/tmp/";
                }
                else
                {
                    obj_folder = "obj/";
                }
                File.Delete(OSPath + obj_folder + transformed + (info.Extension != "" ? ".o" : ""));
                //Delete the old object
            }
            catch
            {
                
            }
            //Delete the old object
            Makefile make = new Makefile();
            make.Create(OSPath);
        }

        void fsw_Created(object sender, FileSystemEventArgs e)
        {
            FileInfo info = new FileInfo(e.FullPath);
            if (!ExtensionsToListen.Contains(info.Extension))
            {
                return;
            }
            Makefile make = new Makefile();
            make.Create(OSPath);
        }

        public void Start(string path)
        {
            OSPath = path;
            DirectoryInfo info = new DirectoryInfo(path);
            fsw.Path = info.GetDirectories("source")[0].FullName;
            
            
            fsw.EnableRaisingEvents = true;
            Console.WriteLine("Started");
        }

        public void Stop()
        {
            fsw.EnableRaisingEvents = false;
            Console.WriteLine("Stoped");
        }
    }
}
