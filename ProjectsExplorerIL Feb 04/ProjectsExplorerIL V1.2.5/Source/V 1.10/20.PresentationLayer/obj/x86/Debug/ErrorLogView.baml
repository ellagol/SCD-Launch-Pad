   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5      @ <ATSVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null,"clr-namespace:ATSVM;assembly=ATSVMATSVM 5      @ <ATSUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:ATSUI.CustomControlsATSUI.CustomControls 5      E AInfra.MVVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:Infra.MVVM;assembly=Infra.MVVM
Infra.MVVM 5         ATSUI.ErrorLogView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35T PRibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=nullW SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5      1vm"clr-namespace:ATSVM;assembly=ATSVM     5      3ctls"clr-namespace:ATSUI.CustomControls     5      =mvvm,clr-namespace:Infra.MVVM;assembly=Infra.MVVM     5         WorkspaceStyle#���   5        � 	Resources  5       �� 
TargetType  LogViewerStyle �� Property �� Value1  +ATSUI.CustomControls.ScrollViewerExtensions   AlwaysScrollToEnd K� IsItemsHost �� DataType  ATSVM.LogEntry � IsSharedSizeScope �� 
IsReadOnly$	 �� HorizontalScrollBarVisibility"
 �� VerticalScrollBarVisibility  BackgroundC1  BackgroundC2  BackgroundBrush �� 
StartPoint �� EndPoint � Color � Offset��%�  &	       (��      5(      *, +)&	 �    &	     &	 ,    �� 5   
   " ��6   .�5      ��  � �� 5      �� 5      .R�5      �� $
��Auto=�6'   $
��Auto=�6L   ��.  . .��5      �� 5      5      5      �� 5       � �� 5      �� 5      .R�5      $�  . 61   5      5      5      �� 5(   
   "  6   .R�5)      �  . .+�5+      ��  . $	 Visible=�6E   $
 Visible=�6m   $	��500��6�   $
��1100��6�   ��6,   �� *	Message+.��6�   5,      5-      5.      ��59   
   	#FFCFDDEE��60   ��5:   
   	#FFC2D4E8��60   �� 5<   
   $ 0,0.5&�66   $	 0,1&�6I   .��5=      � # �  6   $ 0Q�6B   � 5>      # �  6   $ 1Q�6B   5?      5A      .��5C      � $��15,0,0,0q�6    � RowDefinitions 5D   
   �� 5E      $
��Auto �6   �� 5F      $
��Auto �6   �� 5G      $��* �6   �� 5H      $
��Auto �6   �� 5I      $
��Auto �6   .+�5M   
   �� $��0��6   $!��Data Access Layer Error Log��6     WorkspaceTitleStyle#���  6C   �� 5P   
   $��1��6     HorizontalSeparatorStyle#���  6!   $��0,0,15,0q�6T   � 5S   
   $��2��6   .+�5T      ]� #��[ 6   $��	0,0,15,15q�6F   .+�5U      �� ��6   ��  �� Path$ 
LogEntries���5V      �� 5W      .R�5X      �� $
��Auto=�6+   $
��Auto=�6P   ��.  . .��5Y   "   �� 5Z      5[      ��5]      �� 5^      .R�5_      $�  . 65   5`      5b      5c      5d      5m      5n      