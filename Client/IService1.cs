///////////////////////////////////////////////////////////////////////
// IService1.cs: Communication Interface for Client-Server Communication
// ver 1.0                                                           //
// Language:    C#, 2008, .Net Framework 4.5                         //
// Platform:    Dell Inspiron 17R 5721, Win 8.1                      //
//              Microsoft Visual Studio 2013 Ultimate                //
// Application: Remote Dependency Analyzer for CSE681,               //
//              Project #4, Fall 2014                                //
// Author:      Akhil Panchal, MS Computer Science,                  //
//              Syracuse University, (408) 921-0731, ahpancha@syr.edu//
///////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
* Defines the Communication Service contract for Communication
* between Client and Server.
* 
*
* --------------------
* version 1.0 : 16 Nov 2014
* - first release
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;

namespace CommunicationNamespace
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract(IsOneWay = true)]
        void PostMessage(Message msg);

        //Used only locally so not exposed as service method
        Message GetMessage();
    }

    [DataContract]
    public class Message
    {
        [DataMember]
        public int MessageNumber;
        [DataMember]
        public string Sender;
        [DataMember]
        public string Receiver;
        [DataMember]
        public string MessageID;
        [DataMember]
        public string body;
    }
}
