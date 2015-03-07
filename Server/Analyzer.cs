///////////////////////////////////////////////////////////////////////
// Analyzer.cs - Analyzes input Code files in 2 passes               //
// ver 1.0                                                           //
// Language:    C#, 2008, .Net Framework 4.5                         //
// Platform:    Dell Inspiron 17R 5721, Win 8.1                      //
//              Microsoft Visual Studio 2013 Ultimate                //
// Application: Code Analyzer for CSE681, Project #2, Fall 2014      //
// Author:      Akhil Panchal, MS Computer Science,                  //
//              Syracuse University, (408) 921-0731, ahpancha@syr.edu//
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * Analyzes Input Code files in 2 passes.
 * Pass1 - For detecting the defined types
 * Pass2 - For detecting the relationships between detected types 
 *  
 */
/* Required Files:
 * IRuleAndAction.cs, RulesAndActions.cs, Parser.cs, ScopeStack.cs,
 * Semi.cs, Toker.cs, FileMgr.cs, Display.cs
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

    public class Analyzer
    {
        private static int no_of_files = 0;
        public static int get_no_of_files()
        {
            return no_of_files;
        }
        static public string[] getFiles(string path, List<string> patterns, bool recurse)
        {
            FileMgr fm = new FileMgr();
            foreach (string p in patterns)
                fm.addPattern(p);
            fm.findFiles(path, recurse);
            return fm.getFiles().ToArray();
        }

        //Pass 1 : Detects defined types, functions an calculates size and complexity of the detected functions
        static void pass1(string[] files)
        {
            CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
            BuildCodeAnalyzer builder = new BuildCodeAnalyzer(semi);
            Parser parser = builder.buildpass1();
            Repository repo = Repository.getInstance();
            List<Elem> locationtable = repo.locations;
            List<TypeTable> typetable = repo.typetable;
            List<Relationship> relationshiptable = repo.relationshiptable;
            int cnt = 0, cnt2 = 0;

            foreach (object file in files)
            {
                string asd = file.ToString();
                if (Path.GetExtension(asd) == ".suo" || Path.GetExtension(asd) == ".exe" || Path.GetExtension(asd) == ".cache" || Path.GetExtension(asd) == ".dll" || Path.GetExtension(asd) == ".pdb" || Path.GetExtension(asd) == ".csproj" || Path.GetExtension(asd) == ".doc" || Path.GetExtension(asd) == ".pdf" || Path.GetExtension(asd) == ".vsd" || Path.GetExtension(asd) == ".dat")
                    continue;
                //Console.Write("\n  Processing file {0}\n", file as string);
                no_of_files++;
                semi.displayNewLines = false;
                if (!semi.open(file as string))
                {
                    Console.Write("\n  Can't open {0}\n\n", file);
                    return;
                }
                try
                {
                    while (semi.getSemi())
                        parser.parse(semi);
                }
                catch (Exception ex)
                {
                    Console.Write("\n\n  {0}\n", ex.Message);
                }
                for (int i = cnt; i < locationtable.Count; i++)
                    locationtable.ElementAt(i).filename = Path.GetFileName(file.ToString());
                for (int i = cnt2; i < typetable.Count; i++)
                    typetable.ElementAt(i).filename = Path.GetFileName(file.ToString());
                cnt = locationtable.Count;
                cnt2 = typetable.Count;
                semi.close();
            }
        }

        //detects relationships between defined types.
        static void pass2(string[] files, bool relation)
        {
            if (relation)
            {
                Repository repo = Repository.getInstance();

                CSsemi.CSemiExp semi2 = new CSsemi.CSemiExp();
                BuildCodeAnalyzer2 builder2 = new BuildCodeAnalyzer2(semi2);
                Parser parser2 = builder2.buildpass2();

                foreach (object file in files)
                {
                    semi2.displayNewLines = false;
                    string asd = file.ToString();
                    if (Path.GetExtension(asd) == ".suo" || Path.GetExtension(asd) == ".exe" || Path.GetExtension(asd) == ".cache" || Path.GetExtension(asd) == ".dll" || Path.GetExtension(asd) == ".pdb" || Path.GetExtension(asd) == ".csproj" || Path.GetExtension(asd) == ".doc" || Path.GetExtension(asd) == ".pdf" || Path.GetExtension(asd) == ".vsd" || Path.GetExtension(asd) == ".dat")
                        continue;
                    if (!semi2.open(file as string))
                    {
                        Console.Write("\n  Can't open {0}\n\n", file);
                        return;
                    }
                    try
                    {
                        while (semi2.getSemi())
                            parser2.parse(semi2);
                    }
                    catch (Exception ex)
                    {
                        Console.Write("\n\n  {0}\n", ex.Message);
                    }
                    semi2.close();
                }


                foreach (TypeTable e in repo.typetable)
                {
                    foreach (Relationship r in repo.relationshiptable)
                    {
                        if (r.name1 == e.name)
                            r.filename1 = e.filename;
                        if (r.name2 == e.name)
                            r.filename2 = e.filename;
                    }
                }
            }
            Xml.createXml();
        }

        static void pass3()
        {
            //Find package dependencies
            Repository repo = Repository.getInstance();
            List<Relationship> reltab = repo.relationshiptable;
            List<PackageDependency> packdeptab = new List<PackageDependency>();
            string package;
            int individual_count = 0, message_count = 0;
            bool flag = false;
            foreach (Relationship r1 in reltab)
            {
                //Check if packagedep size is 0
                if (packdeptab.Count != 0)
                {
                    flag = false;
                    //see if r1 dependencies have been calculated
                    foreach (PackageDependency p in packdeptab)
                    {
                        if (p.packagename == r1.filename1)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag == true)
                    {
                        continue;
                    }
                }
                package = r1.filename1;
                individual_count = 0;
                string[] deppackages = new string[20];
                Array.Clear(deppackages, 0, deppackages.Length);
                foreach (Relationship r2 in reltab)
                {
                    if (r1.filename1 == r2.filename1 && r1.filename1 != r2.filename2)
                    {
                        deppackages[individual_count] = r2.filename2;
                        individual_count++;
                        message_count++;
                    }
                }
                if (individual_count == 0)
                    continue;
                else
                {
                    PackageDependency pd1 = new PackageDependency();
                    pd1.count = individual_count;
                    pd1.packagename = package;
                    pd1.dependencies = deppackages;
                    packdeptab.Add(pd1);
                }
            }


            //Copy to Main repository
            foreach (PackageDependency pd in packdeptab)
            {
                repo.packageDeptable.Add(pd);
            }

        }

        public static void doAnalysis(string[] files, bool xml, bool relation, int selectpass)
        {
            //PASS 1------------------------------------------------------------------------
            if (selectpass != 2)
                pass1(files);
            //PASS 2------------------------------------------------------------------------
            if (selectpass != 5)
            {
                pass2(files, relation);
                //Display.displayOutput(xml,no_of_files,relation);
                pass3();
            }
        }

#if(TEST_ANALYZER)
        static void Main(string[] args)
        {
            int count=args.Length;
            string path =args[0];
            List<string> optparameters = new List<string>();
            //int index=0,i=0;
            List<string> patterns = new List<string>();
           /* for(i=0;i<count;i++)
            {
                if (args[i][0] == '/')
                {
                    index = i;
                    break;
                }
                    
            }*/
           // for (i = 1; i < index ;i++)
            //{
                patterns.Add("*.cs");
            //}
           /* for(i=i+1;i<count;i++)
            {
                optparameters.Add(args[i]);
            }*/
                
            string[] files = Analyzer.getFiles(path,patterns);
            doAnalysis(files);
        }
#endif
    }
}