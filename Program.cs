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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watcher
{
    class Program
    {
        internal static Dictionary<string, bool> Options;

        static void PrintHelp()
        {
            Console.WriteLine("Command List:");
            Console.WriteLine("help: Print this");
            Console.WriteLine("set: Set OS path");
            Console.WriteLine("clean: Cleans the obj folder, and rebuilds the structure");
            Console.WriteLine("now: Create the makefile now");
            Console.WriteLine("start: Beging continuous creation");
            Console.WriteLine("stop: Stop continuous creation");

            Console.WriteLine("enable [options]: Enables options ('help options' for list of options)");
            Console.WriteLine("disable [options]: Disable options");

            Console.WriteLine("exit: Exit");
        }
        static void PrintOptions()
        {
            Console.WriteLine("Options list:");
            Console.WriteLine("temp: Use the temp folder for build/dependencies");
            Console.WriteLine("ggdb: Generate debug information");
        }
        static void Main(string[] args)
        {
            Options = new Dictionary<string, bool>();
            Options.Add("temp", false);
            Options.Add("ggdb", false);

            string osPath = "C:\\OS\\";
            Watch watch = new Watch();

            Console.WriteLine("**** Hydra Makefile Generator ****");
            PrintHelp();
            while (true)
            {
                Console.Write(">");
                string input = Console.ReadLine();
                input = input.ToLower();
                string[] splitInput=input.Split(' ');
                switch (splitInput[0])
                {
                    default:
                        Console.WriteLine("Command not found");
                        break;
                    case "enable":
                        for(int i=1;i<splitInput.Length;i++)
                        {
                            string option=splitInput[i];
                            if(Options.ContainsKey(option))
                            {
                                Options[option] = true;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Error: option {0} not found");
                                Console.ResetColor();
                            }
                        }
                        Console.WriteLine("Done.");
                        break;
                    case "disable":
                        for (int i = 1; i < splitInput.Length; i++)
                        {
                            string option = splitInput[i];
                            if (Options.ContainsKey(option))
                            {
                                Options[option] = false;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Error: option {0} not found");
                                Console.ResetColor();
                            }
                        }
                        Console.WriteLine("Done.");
                        break;
                    case "now":
                        {
                            Makefile makefile = new Makefile();
                            makefile.Create(osPath);
                        }
                        break;
                    case "clean":
                        {
                            Directory.Delete(osPath + "obj", true);
                            Directory.CreateDirectory(osPath + "obj");
                            Makefile makefile = new Makefile();
                            makefile.Create(osPath);
                        }
                        break;
                    case "exit":
                        return;
                    case "help":
                        if (splitInput.Length == 1)
                        {
                            PrintHelp();
                        }
                        else if(splitInput[1]=="options")
                        {
                            PrintOptions();
                        }
                        else
                        {
                            Console.WriteLine("Help not found");
                        }
                        break;
                    case "set":
                        input = input.Remove(0, 4);
                        //Input is the new path
                        try
                        {
                            DirectoryInfo info = new DirectoryInfo(input);
                            if (!info.Exists)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Error: Path doesn't exist or invalid");
                                Console.ResetColor();
                            }
                            //Format check
                            if (input.Last() != '\\' && input.Last() != '/')
                            {
                                input += '\\';
                            }
                            osPath = input;
                            Console.WriteLine("Done.");
                        }
                        catch
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error: Path doesn't exist or invalid");
                            Console.ResetColor();
                        }
                        break;
                    case "start":
                        watch.Start(osPath);
                        break;
                    case "stop":
                        watch.Stop();
                        break;
                }
            }

        }
    }
}
