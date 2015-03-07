///////////////////////////////////////////////////////////////////////
// Executive.cs - Initializes the Code Analyzer Application.         //
//                                                                   //
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
* Simply initializes the Code Analyzer application.
* 
* Required Files:
* Analyzer.cs, CommandLineParser.cs
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
    class Executive
    {       
        static void Main(string[] args)
        {
            Repository repo = Repository.getInstance();
            CmdArgs c = new CmdArgs();
            c = CommandLineParser.parseCommandLine(args);   //parse command line arguments
            string[] files = Analyzer.getFiles(c.path, c.patterns, c.recursive);    //get input fileset
            Analyzer.doAnalysis(files, c.xml,c.relationships);  //Perform analysis on input file set
        }
    }
}
