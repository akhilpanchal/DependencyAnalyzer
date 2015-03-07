﻿#pragma checksum "..\..\Client_GUI.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7399B4D75902DBBA225BD5C682D60CD9"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace CommunicationNamespace {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\Client_GUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox ServerList;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\Client_GUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox TypeDependencies;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\Client_GUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox PackageDependencies;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\Client_GUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonDependencies;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\Client_GUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CreateXML;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\Client_GUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox ProjectList;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\Client_GUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid DependencyInfo;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\Client_GUI.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid PackageDependenciesGrid;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Client_GUI;component/client_gui.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Client_GUI.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\Client_GUI.xaml"
            ((CommunicationNamespace.MainWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_load);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ServerList = ((System.Windows.Controls.ListBox)(target));
            
            #line 13 "..\..\Client_GUI.xaml"
            this.ServerList.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ServerList_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 15 "..\..\Client_GUI.xaml"
            ((System.Windows.Controls.ListBoxItem)(target)).Selected += new System.Windows.RoutedEventHandler(this.ListBoxItem_Selected);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 16 "..\..\Client_GUI.xaml"
            ((System.Windows.Controls.ListBoxItem)(target)).Selected += new System.Windows.RoutedEventHandler(this.ListBoxItem_Selected_1);
            
            #line default
            #line hidden
            return;
            case 5:
            this.TypeDependencies = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 6:
            this.PackageDependencies = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 7:
            this.ButtonDependencies = ((System.Windows.Controls.Button)(target));
            
            #line 20 "..\..\Client_GUI.xaml"
            this.ButtonDependencies.Click += new System.Windows.RoutedEventHandler(this.ButtonFindDependencies);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 21 "..\..\Client_GUI.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ButtonConnectServer);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 22 "..\..\Client_GUI.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.CreateXML = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 11:
            this.ProjectList = ((System.Windows.Controls.ListBox)(target));
            
            #line 25 "..\..\Client_GUI.xaml"
            this.ProjectList.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ProjectList_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 12:
            
            #line 36 "..\..\Client_GUI.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 13:
            
            #line 37 "..\..\Client_GUI.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_1);
            
            #line default
            #line hidden
            return;
            case 14:
            this.DependencyInfo = ((System.Windows.Controls.DataGrid)(target));
            
            #line 41 "..\..\Client_GUI.xaml"
            this.DependencyInfo.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.DependencyInfo_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 15:
            this.PackageDependenciesGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
