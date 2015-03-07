///////////////////////////////////////////////////////////////////////
// Executive.cs - Initializes the Dependency Analyzer Server         //
//                                                                   //
// ver 1.0                                                           //
// Language:    C#, 2008, .Net Framework 4.5                         //
// Platform:    Dell Inspiron 17R 5721, Win 8.1                      //
//              Microsoft Visual Studio 2013 Ultimate                //
// Application: Dependency Analyzer for CSE681, Project #4, Fall 2014//
// Author:      Akhil Panchal, MS Computer Science,                  //
//              Syracuse University, (408) 921-0731, ahpancha@syr.edu//
///////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
* Simply initializes the Dependency Analyzer Server application.
* Initiates the server to accept requests from the clients
* 
*   
* Maintenance History:
* --------------------
* version 1.0 : 16 Nov 2014
* - first release
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNamespace
{
    class Executive
    {
        static void Main(string[] args)
        {
            CommunicationHandler prg = new CommunicationHandler();
            prg.serveraddress = args[0];
            Console.WriteLine("Service Hosted at url: {0}", prg.serveraddress);
            Console.WriteLine("========================================================");
            //Merge Type Table
            try
            {
                prg.SendTypeTable("http://localhost:4002/ServerService2");

            }
            catch (Exception e)
            {
                Console.WriteLine("No other Server detected!\n\n{0}", e.ToString());
            }
            Task task = new Task(() => prg.RcvThread(args[0]));
            task.Start();
            Console.ReadLine();
        }
    }
}
