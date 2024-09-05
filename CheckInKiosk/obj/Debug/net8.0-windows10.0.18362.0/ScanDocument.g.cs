﻿#pragma checksum "..\..\..\ScanDocument.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "ABEFA7AF0E1EB15A24C8933FCC71E45376D9E09B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
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
using System.Windows.Controls.Ribbon;
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


namespace CheckInKiosk {
    
    
    /// <summary>
    /// ScanDocument
    /// </summary>
    public partial class ScanDocument : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 104 "..\..\..\ScanDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel MainStackPanel;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\..\ScanDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox DocumentTypeComboBox;
        
        #line default
        #line hidden
        
        
        #line 123 "..\..\..\ScanDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox AdditionalInfoTextBox;
        
        #line default
        #line hidden
        
        
        #line 125 "..\..\..\ScanDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel ImageUploadPanel;
        
        #line default
        #line hidden
        
        
        #line 128 "..\..\..\ScanDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image UploadedImage;
        
        #line default
        #line hidden
        
        
        #line 141 "..\..\..\ScanDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock VerificationMessage;
        
        #line default
        #line hidden
        
        
        #line 142 "..\..\..\ScanDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ErrorMessage;
        
        #line default
        #line hidden
        
        
        #line 146 "..\..\..\ScanDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid LoadingOverlay;
        
        #line default
        #line hidden
        
        
        #line 154 "..\..\..\ScanDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel ManualCheckInPanel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CheckInKiosk;component/scandocument.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\ScanDocument.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.MainStackPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 2:
            this.DocumentTypeComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 117 "..\..\..\ScanDocument.xaml"
            this.DocumentTypeComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.OnDocumentTypeSelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.AdditionalInfoTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.ImageUploadPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 5:
            
            #line 127 "..\..\..\ScanDocument.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OnChooseImageClick);
            
            #line default
            #line hidden
            return;
            case 6:
            this.UploadedImage = ((System.Windows.Controls.Image)(target));
            return;
            case 7:
            
            #line 138 "..\..\..\ScanDocument.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OnSubmitClick);
            
            #line default
            #line hidden
            return;
            case 8:
            this.VerificationMessage = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.ErrorMessage = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.LoadingOverlay = ((System.Windows.Controls.Grid)(target));
            return;
            case 11:
            this.ManualCheckInPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 12:
            
            #line 158 "..\..\..\ScanDocument.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OnOkayClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

