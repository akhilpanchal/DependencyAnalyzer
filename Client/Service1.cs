///////////////////////////////////////////////////////////////////////
// Service1.cs: Communication Service Definition for Client-Server   //
// ver 1.0      Communication                                        //
// Language:    C#, 2008, .Net Framework 4.5                         //
// Platform:    Dell Inspiron 17R 5721, Win 8.1                      //
//              Microsoft Visual Studio 2013 Ultimate                //
// Application: Remote Dependency Analyzer for CSE681,               //
//              Project #4, Fall 2014                                //
// Author:      Akhil Panchal, MS Computer Science,                  //
//              Syracuse University, (408) 921-0731, ahpancha@syr.edu
// Source:      Jim Fawcett, CST 4-187, Syracuse University          //
//              (315) 443-3948, jfawcett@twcny.rr.com                //
///////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
* Defines the Communication Service contract definition for Communication
* between Client and Server.
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
using System.Threading;
using System.Windows;


namespace CommunicationNamespace
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Receiver : IService1
    {
        static BlockingQueue<Message> rcvBlockingQ = null;
        ServiceHost service = null;

        public Receiver()
        {
            if (rcvBlockingQ == null)
                rcvBlockingQ = new BlockingQueue<Message>();
        }

        public void Close()
        {
            service.Close();
        }

        //  Create ServiceHost for Communication service
        public void CreateRecvChannel(string address)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            Uri baseAddress = new Uri(address);
            service = new ServiceHost(typeof(Receiver), baseAddress);
            service.AddServiceEndpoint(typeof(IService1), binding, baseAddress);
            service.Open();
        }

        // Implement service method to receive messages from other Peers

        public void PostMessage(Message msg)
        {
            rcvBlockingQ.enQ(msg);
        }

        // Implement service method to extract messages from other Peers.
        // This will often block on empty queue, so user should provide
        // read thread.

        public Message GetMessage()
        {
            return rcvBlockingQ.deQ();
        }
    }
    ///////////////////////////////////////////////////
    // client of another Peer's Communication service
    public class Sender
    {
        IService1 channel;
        string lastError = "";
        BlockingQueue<Message> sndBlockingQ = null;
        Thread sndThrd = null;
        int tryCount = 0, MaxCount = 10;

        // Processing for sndThrd to pull msgs out of sndBlockingQ
        // and post them to another Peer's Communication service

        void ThreadProc()
        {
            while (true)
            {
                try
                {
                    Message msg = new Message();
                    msg = sndBlockingQ.deQ();
                    channel.PostMessage(msg);
                    if (msg.MessageID == "quit" || msg.MessageNumber == 7)
                        break;
                }
                catch (Exception e)
                {
                    string messageBoxText = "An error was encountered!\nDetails:\n\n"+ e;
                    MessageBoxResult button = MessageBox.Show(messageBoxText);
                    //Console.WriteLine("An error was encountered: {0}.", e);
                }
            }
        }

        // Create Communication channel proxy, sndBlockingQ, and
        // start sndThrd to send messages that client enqueues

        public Sender(string url)
        {
            sndBlockingQ = new BlockingQueue<Message>();
            while (true)
            {
                try
                {
                    CreateSendChannel(url);
                    tryCount = 0;
                    break;
                }
                catch (Exception ex)
                {
                    if (++tryCount < MaxCount)
                        Thread.Sleep(100);
                    else
                    {
                        lastError = ex.Message;
                        break;
                    }
                }
            }
            sndThrd = new Thread(ThreadProc);
            sndThrd.IsBackground = true;
            sndThrd.Start();
        }

        // Create proxy to another Peer's Communicator

        public void CreateSendChannel(string address)
        {
            EndpointAddress baseAddress = new EndpointAddress(address);
            BasicHttpBinding binding = new BasicHttpBinding();
            ChannelFactory<IService1> factory = new ChannelFactory<IService1>(binding, address);
            channel = factory.CreateChannel();
        }

        // Sender posts message to another Peer's queue using
        // Communication service hosted by receipient via sndThrd

        public void PostMessage(Message msg)
        {
            sndBlockingQ.enQ(msg);
        }

        public string GetLastError()
        {
            string temp = lastError;
            lastError = "";
            return temp;
        }

        public void Close()
        {
            ChannelFactory<IService1> temp = (ChannelFactory<IService1>)channel;
            temp.Close();
        }
    }
}
