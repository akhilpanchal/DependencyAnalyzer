///////////////////////////////////////////////////////////////////////
// FileMgr.cs - Navigates the Filesystem to discover the files of    //
//              the mentioned patterns in the given path.            //
// ver 1.0                                                           //
// Language:    C#, 2008, .Net Framework 4.5                         //
// Platform:    Dell Inspiron 17R 5721, Win 8.1,                     //
//              Microsoft Visual Studio 2013 Ultimate                //
// Application: Code Analyzer for CSE681, Project #2, Fall 2014      //
// Author:      Akhil Panchal, MS Computer Science,                  //
//              Syracuse University, (408) 921-0731, ahpancha@syr.edu//
///////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
* Discovers the file handles of the files of the mentioned pattern in the 
* Command Line Arguments and returns a set of input files.
* 
* Required Files:
* None 
*   
* Maintenance History:
* --------------------
* version 1.0 : 08 Oct 2014
* - first release
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CodeAnalysis
{
    public class FileMgr
    {
        private List<string> files = new List<string>();
        private List<string> patterns = new List<string>();        
        public void findFiles(string path,bool recurse) 
        {
            if(Directory.Exists(path))
            {
              if (patterns.Count == 0)
                    addPattern("*.*");
                foreach (string pattern in patterns)
                {
                    string[] newFiles = Directory.GetFiles(path,pattern);
                    for (int i = 0; i < newFiles.Length; ++i)
                        newFiles[i] = Path.GetFullPath(newFiles[i]);
                    files.AddRange(newFiles);
                }
                if (recurse)
                {
                    string[] subdirs = Directory.GetDirectories(path);
                    foreach (string dir in subdirs)
                         findFiles(dir,recurse);
                }
            }
            else
            {
                Console.WriteLine("Path Does not Exist.");
                return;
            }

        }
        public void addPattern(string pattern)
        {
            patterns.Add(pattern);
        }

        public List<string> getFiles()
        {
            return files;
        }

#if(TEST_FILEMGR)
        static void Main(string[] args)
        {
            Console.Write("\n Testing FileMgr Class");
            Console.Write("\n ======================\n");
            FileMgr fm = new FileMgr();
            fm.addPattern("*.cs");
            fm.findFiles("../../",true);
            List<string> files = fm.getFiles();
            foreach (string file in files)
                Console.Write("\n {0}", file);
            Console.Write("\n\n");
        }
#endif
    }
}
