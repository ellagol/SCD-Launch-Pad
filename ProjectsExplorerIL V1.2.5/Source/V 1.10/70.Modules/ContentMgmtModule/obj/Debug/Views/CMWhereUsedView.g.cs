﻿#pragma checksum "..\..\..\Views\CMWhereUsedView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "08B47B5B294BA3EA6E5586DE1240EF80"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ContentMgmtModule;
using Infra.DragDrop;
using Infra.MVVM;
using Stema.Controls;
using Stema.Controls.Converters;
using Stema.Windows;
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


namespace ContentMgmtModule {
    
    
    /// <summary>
    /// CMWhereUsedView
    /// </summary>
    public partial class CMWhereUsedView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 53 "..\..\..\Views\CMWhereUsedView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView ContentsList;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\..\Views\CMWhereUsedView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GridView contents;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\..\Views\CMWhereUsedView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView ProjectsList;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\..\Views\CMWhereUsedView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GridView projects;
        
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
            System.Uri resourceLocater = new System.Uri("/ContentMgmtModule;component/views/cmwhereusedview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\CMWhereUsedView.xaml"
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
            this.ContentsList = ((System.Windows.Controls.ListView)(target));
            return;
            case 2:
            this.contents = ((System.Windows.Controls.GridView)(target));
            return;
            case 3:
            this.ProjectsList = ((System.Windows.Controls.ListView)(target));
            return;
            case 4:
            this.projects = ((System.Windows.Controls.GridView)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

