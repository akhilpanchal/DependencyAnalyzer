///////////////////////////////////////////////////////////////////////
// CommandLineParser.cs - Parses the Command Line and gives          // 
//                        information back to the Analyzer.          //
// ver 1.0                                                           //
// Language:    C#, 2008, .Net Framework 4.5                         //
// Platform:    Dell Inspiron 17R 5721, Win 8.1                      //
//              Microsoft Visual Studio 2013 Ultimate                //
// Application: Code Analyzer for CSE681, Project #2, Fall 2014      //
// Author:      Akhil Panchal, MS Computer Science,
//              Syracuse University, (408) 921-0731, ahpancha@syr.edu//
///////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
* Parses the command line arguments and returns the structure of 
* arguments to the Analyzer package.
* 
* Required Files:
* None
* 
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

namespace CodeAnalysis
{
    public struct CmdArgs
    {
        public string path { get; set; }
        public List<string> patterns { get; set; }
        public bool relationships { get; set; }
        public bool recursive { get; set; }
        public bool xml { get; set; }
    }
    public class CommandLineParser
    {
        //parses command line
        public static CmdArgs parseCommandLine(string[] args)
        {
            CmdArgs c = new CmdArgs();
            c.relationships = c.xml = c.recursive = false;
            c.path = "";
            int index=0, count = args.Length;
            c.path = args[0];
            int i = 0;
            c.patterns = new List<string>();

            for (i = index; i < count; i++)
            {
                if (args[i] == "/x" || args[i] == "/X")
                    c.xml = true;
                if (args[i] == "/r" || args[i] == "/R")
                    c.relationships = true;
                if (args[i] == "/s" || args[i] == "/S")
                    c.recursive = true;
                if (args[i][0] == '*')
                    c.patterns.Add(args[i]);
            }            
            return c;
        }

//TEST STUB
#if(TEST_CMDPARSER)
        static void Main(string[] args)
        {
            CmdArgs c1 = new CmdArgs();
            Console.WriteLine(args);
            c1 = CommandLineParser.parseCommandLine(args);
         
            Console.WriteLine(c1.path);

            foreach (string s in c1.patterns)
                Console.WriteLine(s);

            Console.WriteLine(c1.recursive);
            Console.WriteLine(c1.relationships);
            Console.WriteLine(c1.xml);
        }
#endif
    }
}
