///////////////////////////////////////////////////////////////////////////////////
// CommunicationHandler.cs -Initializes the Dependency Analyzer Server           //
// ver 1.0                                                                       //
// Language:                C#, 2008, .Net Framework 4.5                         //
// Platform:                Dell Inspiron 17R 5721, Win 8.1                      //
//                          Microsoft Visual Studio 2013 Ultimate                //
// Application:             Dependency Analyzer for CSE681, Project #4, Fall 2014//
// Author:                  Akhil Panchal, MS Computer Science,                  //
//                          Syracuse University, (408) 921-0731, ahpancha@syr.edu//
///////////////////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
* Handles the communication requests received by the Server from various clients
* and generates appropriates responses to be sent back to the respective clients.
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
using System.ServiceModel;
using System.IO;
using CodeAnalysis;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;

namespace CommunicationNamespace
{
    class CommunicationHandler
    {
        Receiver server_rcvr;
        Sender server_sender;
        public string serveraddress { get; set; }
        string projectspath = "./Server/Projects";

        Message MakeDeepCopy(Message m1)
        {
            Message copy_rcvd_message = new Message();
            copy_rcvd_message.MessageID = m1.MessageID;
            copy_rcvd_message.MessageNumber = m1.MessageNumber;
            copy_rcvd_message.Receiver = m1.Receiver;
            copy_rcvd_message.Sender = m1.Sender;
            copy_rcvd_message.body = m1.body;
            return copy_rcvd_message;
        }

        //Pulls out messages from the receiving queue
        public void RcvThread(string hosted_url)
        {
            server_rcvr = new Receiver();
            server_rcvr.CreateRecvChannel(hosted_url);
            while (true)
            {
                Message rcvd_msg = server_rcvr.GetMessage();
                Message copy_rcvd_message=MakeDeepCopy(rcvd_msg);
                Console.WriteLine(copy_rcvd_message.MessageID);
                Respond(rcvd_msg);
                if (rcvd_msg.MessageNumber == 7)
                    return;
            }
        }
        //Merges Type Table from other Servers for finding outgoing dependencies
        void MergeTypeTable(Message msg)
        {
            Repository repo = Repository.getInstance();
            List<TypeTable> receivedtypes = new List<TypeTable>();


            //storing received type information received from other server
            string[] rcvdtypes = new string[2000];
            rcvdtypes = msg.body.Split(' ');
            bool flag=false;
            for (int i = 0; i < rcvdtypes.Count() && rcvdtypes[i] != ""; i = i + 4)
            {
                foreach (TypeTable t in repo.typetable)
                {
                    if (t.type == rcvdtypes[i] && t.name == rcvdtypes[i + 1])
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag == true)
                    continue;

                else
                {
                    //add type in table
                    TypeTable t1 = new TypeTable();
                    t1.type = rcvdtypes[i];
                    t1.name = rcvdtypes[i + 1];
                    t1.nspace = rcvdtypes[i + 2];
                    t1.filename = rcvdtypes[i + 3];
                    receivedtypes.Add(t1);
                }
            }

            foreach (TypeTable t in receivedtypes)
            {
                repo.typetable.Add(t);
            }
        }

        //Send Response information to the received request
        public void Respond(Message msg)
        {
            Message response = new Message();
            response.MessageID = "Bol Bhadwe";
            response.Receiver = msg.Sender;
            response.Sender = msg.Receiver;
            response.body = msg.body;
            if (msg.MessageNumber == 5)
            {
                MergeTypeTable(msg);
                return;
            }
            else
            {
                switch (msg.MessageNumber)
                {
                    case 1: //send project list
                        string[] projects = new string[10];
                        string path = projectspath;
                        int count = 0;
                        List<string> projectlist = new List<string>();
                        foreach (string s in Directory.GetDirectories(path))
                        {
                            projects[count] = (s.Remove(0, path.Length + 1));
                            count++;
                        }
                        response.body = string.Join(" ", projects);
                        response.MessageNumber = 1;
                        break;

                    case 2: //send type relationship information
                            response.MessageNumber = 2;
                            TypeSendDependencyInfo(response);
                            break;
                    case 3: //send package dependency information
                            response.MessageNumber = 3;
                            SendPackageDependency(response);
                            break;
                    case 6: //send xml
                            response.MessageNumber = 6;
                            SendXml(response);
                            break;
                }
            }
            Console.WriteLine("Sending response to: {0}",msg.Sender);
            server_sender = new Sender(msg.Sender);
            server_sender.PostMessage(response);
        }

        //Send Dependency in XML format
       void SendXml(Message response)
        {
            List<string> pattern = new List<string>();
            pattern.Add("*.cs");
            string path = projectspath +"/" + response.body;
            string[] files = Analyzer.getFiles(path, pattern, true);    //get input fileset
            Analyzer.doAnalysis(files, true, true, response.MessageNumber);  //Perform analysis on input file set
            //Display.displayOutput(true, Analyzer.get_no_of_files(), true);

            string contents = File.ReadAllText("Analysis.xml");
            response.body = contents;
        }

        //Send Type table to other Servers
        public void SendTypeTable(string sendadd)
        {

            List<string> pattern = new List<string>();
            pattern.Add("*.cs");
            Message response = new Message();
            response.MessageNumber = 5;
            response.MessageID = "Merge TypeTable";
            response.Receiver = sendadd;
            response.Sender = serveraddress;
            
            List<TypeTable> ttable = new List<TypeTable>();
            string[] files = Analyzer.getFiles(projectspath, pattern, true);    //get input fileset
            Analyzer.doAnalysis(files, true, true,5);  //Perform analysis on input file set
            //Display.displayOutput(true, Analyzer.get_no_of_files(), true);
            Repository repo = Repository.getInstance();
            ttable = repo.typetable;
            string[] typeinfo = new string[1000];
            int count = 0;
            foreach (TypeTable r in ttable)
            {
                typeinfo[count] = r.type;
                typeinfo[count + 1] = r.name;
                typeinfo[count + 2] = r.nspace;
                typeinfo[count + 3] = r.filename;
                count = count + 4;
            }
            response.body = string.Join(" ", typeinfo);

            server_sender = new Sender(response.Receiver);
            server_sender.PostMessage(response);
            
        }

        //Send package Dependencies to the client
        void SendPackageDependency(Message response)
        {
            List<string> pattern = new List<string>();
            pattern.Add("*.cs");
            string[] files = Analyzer.getFiles(projectspath+"/"+response.body, pattern, true);    //get input fileset
            Analyzer.doAnalysis(files, true, true, response.MessageNumber);  //Perform analysis on input file set
            
            Repository repo = Repository.getInstance();
            List<PackageDependency> packtable = repo.packageDeptable;
            string[] packinfo = new string[1000];
            int count = 0;
            foreach(PackageDependency pd in packtable)
            {
                packinfo[count] = pd.count.ToString();
                count++;
                packinfo[count] = pd.packagename;
                count++;
                for(int i=0;i<pd.count;i++)
                {
                    packinfo[count+i]=pd.dependencies[i];
                }
                count = count + pd.count;
            }
            response.body = string.Join(" ", packinfo);
        }

        //Send Type dependencies to client
        void TypeSendDependencyInfo(Message response)
        {
            List<string> pattern = new List<string>();
            pattern.Add("*.cs");
            string path = projectspath +"/" + response.body;
            string[] files = Analyzer.getFiles(path,pattern,true);    //get input fileset
            Analyzer.doAnalysis(files, true,true,response.MessageNumber);  //Perform analysis on input file set
            //Display.displayOutput(true, Analyzer.get_no_of_files(), true);
            Repository repo = Repository.getInstance();
            
            //Transform relationsship table to string body
            List<Relationship> relationshiptable = repo.relationshiptable;
            string[] relinfo = new string[1000];
            int count = 0;
            foreach(Relationship r in relationshiptable)
            {
                relinfo[count] = r.type1;
                relinfo[count + 1] = r.name1;
                relinfo[count + 2] = r.filename1;
                relinfo[count + 3] = r.relationship;
                relinfo[count + 4] = r.type2;
                relinfo[count + 5] = r.name2;
                relinfo[count + 6] = r.filename2;
                count = count + 7;
            }
            response.body = string.Join(" ", relinfo);
        }

//Test Stub
#if(TEST_PROGRAM)
        static void Main(string[] args)
        {           
            Program prg = new Program();
            prg.serveraddress = args[0];
            Console.WriteLine("Service Hosted at url: {0}", prg.serveraddress);
            Console.WriteLine("========================================================");
            //Merge Type Table
            prg.SendTypeTable("http://localhost:4001/ServerService");
            Task task = new Task(() => prg.RcvThread(args[0]));
            task.Start();
            Console.ReadLine();
        }
#endif
    }
}
