﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------




using System.Linq;
using System.Xml.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ExplorerModule
{
	namespace Properties
	{
		[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute(), global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0"), global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal sealed partial class Settings : System.Configuration.ApplicationSettingsBase
		{

			private static Settings defaultInstance = (Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings()));
	
		#region My.Settings Auto-Save Functionality
//		#if _MyType == "WindowsForms"
//		Private Shared addedHandler As Boolean
//	
//		Private Shared addedHandlerLockObject As new Object
//	
//		<global::System.Diagnostics.DebuggerNonUserCodeAttribute(), global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)> Private Shared Sub AutoSaveSettings(ByVal sender As System.Object, ByVal e As System.EventArgs)
//			If My.Application.SaveMySettingsOnExit Then
//				My.Settings.Save()
//			End If
//		End Sub
//	#endif
		#endregion
	
			public static Settings Default
			{
				get
				{
	
//		#if _MyType == "WindowsForms"
//				   If Not addedHandler Then
//						SyncLock addedHandlerLockObject
//							If Not addedHandler Then
//								AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
//								addedHandler = true
//							End If
//						End SyncLock
//					End If
//	#endif
					return defaultInstance;
				}
			}
		}
	}


} //end of root namespace