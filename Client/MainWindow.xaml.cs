///////////////////////////////////////////////////////////////////////////////
// MainWindow.xaml.cs:  Event handlers for Client GUI                        //
// ver 1.0                                                                   //
// Language:            C#, 2008, .Net Framework 4.5                         //
// Platform:            Dell Inspiron 17R 5721, Win 8.1                      //
//                      Microsoft Visual Studio 2013 Ultimate                //
// Application:         Remote Dependency Analyzer for CSE681,               //
//                      Project #4, Fall 2014                                //
// Author:              Akhil Panchal, MS Computer Science,                  //
//                      Syracuse University, (408) 921-0731, ahpancha@syr.edu//
///////////////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
* Handles various events from the Client GUI
* by calling the appropriate Event Handlers
*
* 
* --------------------
* version 1.0 : 16 Nov 2014
* - first release
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.ServiceModel;
using System.Threading;

namespace CommunicationNamespace
{
    public class TypeDependency
    {
        public string type1 { get; set; }
        public string name1 { get; set; }
        public string packagename1 { get; set; }
        public string relationship { get; set; }
        public string type2 { get; set; }
        public string name2 { get; set; }
        public string packagename2 { get; set; }
    }

    public class PackageDep
    {
        public string package { get; set; }
        public string dependencies { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> conn_servers = new List<string>();
        XDocument serverList = new XDocument();
        Sender client_sender;
        string source_url = "http://localhost:5000/ClientService";
        string servicename = "NONE";
        string projectname = "NONE";
        string destination_url = "lauda";
        Receiver client_rcvr;
        Message rcvdMsg = new Message();
        int MaxMsgCount = 100;
        Thread rcvThrd = null;
        delegate void NewMessage(Message msg);
        event NewMessage OnNewMessage;
        public MainWindow()
        {
            InitializeComponent();
            //Subscribe to new Message
            OnNewMessage += new NewMessage(OnNewMessageHandler);
            
        }
        private void Window_load(Object sender, RoutedEventArgs e)
        {
            ServerList.Items.Clear();
            ProjectList.Items.Clear();
            Client_url.Text = source_url;

            serverList = XDocument.Load("./Client/ServerInfo.xml");
            var query = from s in serverList.Root.Elements("server")
                        select s.Attribute("name");

            var s_list = query.ToList();
            foreach (string s in s_list)
            {
                ServerList.Items.Add(s);
            }

            //Receiver part of client==============================================
            
            string localPort = "5000";
            string endpoint = source_url;

            try
            {
                client_rcvr = new Receiver();
                client_rcvr.CreateRecvChannel(endpoint);

                // create receive thread which calls rcvBlockingQ.deQ() (see ThreadProc below)
                rcvThrd = new Thread(new ThreadStart(this.ThreadProc));
                rcvThrd.IsBackground = true;
                rcvThrd.Start();
                //ConnectButton.IsEnabled = true;
                //ListenButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Window temp = new Window();
                StringBuilder message = new StringBuilder(ex.Message);
                message.Append("\nport = ");
                message.Append(localPort.ToString());
                temp.Content = message.ToString();
                temp.Height = 100;
                temp.Width = 500;
                temp.Show();
            }
        }
        //----< receive thread processing >------------------------------
        void ThreadProc()
        {
            while (true)
            {
                // get message out of receive queue - will block if queue is empty
                rcvdMsg = client_rcvr.GetMessage();

                // call window functions on UI thread
                this.Dispatcher.BeginInvoke(
                  System.Windows.Threading.DispatcherPriority.Normal,
                  OnNewMessage,
                  rcvdMsg);
            }
        }
        void OnNewMessageHandler(Message msg)
        {
            //Code for displaying received information on UI
            switch(msg.MessageNumber)
            {
                case 1: ProjectList.Items.Clear();
                        string[] projectlist = msg.body.Split(' ');
                        foreach(string s in projectlist)
                        {
                            ProjectList.Items.Add(s);
                        }
                        break;
                case 2: DependencyInfo.ItemsSource = null;
                        ShowTypeDependencies(msg);
                        break;
                case 3: PackageDependenciesGrid.ItemsSource = null;
                        ShowPackages(msg);
                        break;
                case 4: DependencyInfo.ItemsSource = null;
                        ShowTypeDependencies(msg);
                        PackageDependenciesGrid.ItemsSource = null;
                        ShowPackages(msg);
                        break;
                case 6: //create xml
                        Createxml(msg);
                        break;

            }
            if (ProjectList.Items.Count > MaxMsgCount)
                ProjectList.Items.RemoveAt(ProjectList.Items.Count - 1);
        }

        //Create XML on Client from the response received
        
        void Createxml(Message msg)
        {
            System.IO.File.WriteAllText("../../Analysis.xml", msg.body);
            XmlDoc.Text = System.IO.File.ReadAllText("../../Analysis.xml");
        }

        //Show package dependencies on client
        void ShowPackages(Message msg)
        {
            List<PackageDep> packdep = new List<PackageDep>();
            string[] packInfo = new string[2000];
            packInfo = msg.body.Split(' ');
            int ind_count = 0;
            int j = 0;
            for (int i = 0; i <= packInfo.Count() && packInfo[i] != ""; i = i + j)
            {
                //PackageDep pd = new PackageDep();
                ind_count = Convert.ToInt32(packInfo[i]);
                i++;
                for (j = 0; j < ind_count; j++)
                {
                    PackageDep pd = new PackageDep();

                    pd.package = packInfo[i];
                    i++;
                    pd.dependencies = packInfo[i + j];
                    packdep.Add(pd);
                }
            }
            PackageDependenciesGrid.ItemsSource = packdep;
        }

        //Show type dependencies on client
        void ShowTypeDependencies(Message msg)
        {
            List<TypeDependency> typerels = new List<TypeDependency>();
            string[] depInfo = new string[2000];
            depInfo = msg.body.Split(' ');
            for (int i = 0; i <=depInfo.Count() && depInfo[i]!=""; i=i+7)
            {
                TypeDependency r = new TypeDependency();
                r.type1 = depInfo[i];
                r.name1 = depInfo[i+1];
                r.packagename1 = depInfo[i + 2];
                r.relationship = depInfo[i + 3];
                r.type2 = depInfo[i + 4];
                r.name2 = depInfo[i + 5];
                r.packagename2 = depInfo[i + 6];
                typerels.Add(r);
            }
            DependencyInfo.Items.Clear();
            DependencyInfo.ItemsSource = typerels;
        }
        private void ServerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Save server selection here.
            if (ServerList.SelectedItem!=null)
            servicename = ServerList.SelectedItem.ToString();
        }
        private void ProjectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {   //Save project selection here
            if (ProjectList.SelectedItem!=null)
            projectname = ProjectList.SelectedItem.ToString();
        }

        //Connect to server button handler
        
        private void ButtonConnectServer(object sender, RoutedEventArgs e)
        {
            if(servicename=="NONE")
            {
                string messageBoxText = "You have not selected a Server. Please make sure you select one!";
                MessageBoxResult button = MessageBox.Show(messageBoxText);
                return;                
            }
            //search servers.xml for the url of the selected server
            var query = from s in serverList.Descendants("server")
                        select s;
            foreach(XElement xe in query )
            {
                if (xe.Attribute("name").Value == servicename)
                    destination_url = xe.Attribute("url").Value;
            }
            Message msg = new Message();
            msg.MessageNumber = 1;
            msg.MessageID = "Send Projects";
            msg.Receiver = destination_url;
            msg.Sender = source_url;
            client_sender = new Sender(destination_url);
            client_sender.PostMessage(msg);
        }

        //Find Dependencies Button handler
        private void ButtonFindDependencies(object sender, RoutedEventArgs e)
        {
            if (servicename == "NONE")
            {
                string messageBoxText = "You have not selected a Server. Please make sure you select one!";
                MessageBoxResult button = MessageBox.Show(messageBoxText);
                return;
            }
            if (projectname == "NONE")
            {
                string messageBoxText = "You have not selected a Project. Please make sure you select one!";
                MessageBoxResult button = MessageBox.Show(messageBoxText);
                return;
            }
            Message msg = new Message();
            if (TypeDependencies.IsChecked.Value)// && !PackageDependencies.IsChecked.Value)
            {
                msg.MessageNumber = 2;
                sendrequest(msg);
            }                
            if (!TypeDependencies.IsChecked.Value)// && PackageDependencies.IsChecked.Value)
            {
                msg.MessageNumber = 3;
                sendrequest(msg);
            }
            if (TypeDependencies.IsChecked.Value && PackageDependencies.IsChecked.Value)
            {
                msg.MessageNumber = 2;
                sendrequest(msg);
                msg.MessageNumber = 3;
                sendrequest(msg);
            }
            if(CreateXML.IsChecked.Value)
            {
                msg.MessageNumber=6;
                sendrequest(msg);
            }
        }

        //sends request to the corresponding Server.
        void sendrequest(Message msg)
        {
            msg.MessageID = "Send Dependencies";
            msg.Receiver = destination_url;
            msg.Sender = source_url;
            msg.body = projectname;
            client_sender = new Sender(destination_url);
            client_sender.PostMessage(msg);
        }
        private void Window_Close(object sender, RoutedEventArgs e)
        {
            Message msg = new Message();
            msg.MessageID = "Quit";
            msg.Receiver = destination_url;
            msg.Sender = source_url;
            msg.body = "None";
            client_sender = new Sender(destination_url);
            client_sender.PostMessage(msg);
        }        
        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            servicename = ServerList.SelectedItem.ToString();
        }
        private void Client_url_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(Client_url.Text!="")
                source_url = Client_url.Text;
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Message msg = new Message();
            msg.MessageNumber = 7;
            client_sender = new Sender(destination_url);
            client_sender.PostMessage(msg);
            client_sender.Close();
            client_rcvr.Close();
        }
    }
}
