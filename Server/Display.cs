///////////////////////////////////////////////////////////////////////
// Display.cs - Redirects the contents of the Repository to the      //
//              Console.                                             //
// ver 1.0                                                           //
// Language:    C#, 2008, .Net Framework 4.0                         //
// Platform:    Dell Inspiron 17R 5721, Win 8.1                      //
//              Microsoft Visual Studio 2013 Ultimate                //
// Application: Code Analyzer for CSE681, Project #2, Fall 2014      //
// Author:      Akhil Panchal, MS Computer Science,                  //
//              Syracuse University, (408) 921-0731, ahpancha@syr.edu//
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * ------------------
 * This package displays the Contents of the Repository
 */
/* Required Files:
 *   Parser.cs, Xml.cs
 *   
 * 
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysis
{
    public class Display
    {
        public static void displayOutput(bool xml,int nof,bool relation)
        {
            Repository rep = Repository.getInstance();
            List<Elem> locationtable = rep.locations;
            List<Elem> functiontable = new List<Elem>();
            List<TypeTable> typetable = rep.typetable;
            List<Relationship> relationshiptable = rep.relationshiptable;
            List<PackageDependency> packagetable = rep.packageDeptable;

                //Console.Write("\n\n   Function Analysis:");
                //Console.Write("\n ----------------------------\n");
                //Console.Write("\t\tFunction Name\t    Size\tComplexity\tFilename\n");
                //foreach (Elem e in locationtable)
                //{
                //    if (e.type == "function")
                //    {
                //        Console.Write("\n {0,25},\t {1,5},\t\t{2,5}{3,30}", e.name, e.end - e.begin, e.complexity, e.filename);
                //        if (e.complexity > 5 || (e.end - e.begin) > 50)
                //            functiontable.Add(e);
                //    }
                //}
                Console.WriteLine("\n\n   Type Analysis:");
                Console.Write("----------------------------\n");
                Console.Write("      Type\t\t      Name\t\tNamespace\t\t\tFile Name\n");
                foreach (TypeTable e in typetable)
                {
                    Console.Write("\n{0,10}{1,25}{2,25}{3,30}", e.type, e.name, e.nspace, e.filename);
                    foreach (Relationship r in relationshiptable)
                    {
                        if (r.name1 == e.name)
                            r.filename1 = e.filename;
                        if (r.name2 == e.name)
                            r.filename2 = e.filename;
                    }
                }
                    Console.WriteLine("\n\n");            

         
            if(relation)
            {
                Console.Write("\n\n   Relationship Analysis");
                Console.Write("\n ----------------------------\n");
                Console.Write("     Type 1\t\tName 1\t\t   Relationship\t         Type 2\t    \t\tName 2\n");
                foreach (Relationship rel in relationshiptable)
                {
                    Console.Write("\n{0,10}{1,25}\t{2,15}{3,15}{4,25}{5,25}{6,25}", 
                        rel.type1, rel.name1, rel.relationship, rel.type2, rel.name2,
                        rel.filename1,rel.filename2);                    
                }
                Console.WriteLine();
            }
            
            //Display package dependencies
            
            Console.WriteLine("\n\n Package Dependencies\n");
            foreach (PackageDependency pd in packagetable)
            {
                Console.Write("\n{0,25}",pd.packagename);
                for (int i = 0; i < pd.dependencies.Count(); i++)
                {
                    Console.Write("\t{0,20}\n",pd.dependencies[i]);
                }
                Console.WriteLine();
            }





            //-----------------------------------
            if (xml)
                Xml.createXml();
            Console.Write("\n\n  That's all folks!\n\n");
            Console.WriteLine("Summary of Analysis:\n");
            Console.WriteLine("\tTotal Number of Files analyzed: {0}\n", nof);



        }

//TEST STUB

#if(TEST_DISPLAY)
        static void Main(string[] args)
        {
            bool test = true;
            displayOutput(test);
        }
#endif
    }
}
