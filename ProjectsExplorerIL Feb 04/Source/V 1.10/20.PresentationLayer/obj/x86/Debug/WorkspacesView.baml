   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5      @ <ATSVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null,"clr-namespace:ATSVM;assembly=ATSVMATSVM 5      E AInfra.MVVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:Infra.MVVM;assembly=Infra.MVVM
Infra.MVVM 5      	 ATSUI   ATSUI.WorkspacesView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35T PRibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=nullW SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5      1vm"clr-namespace:ATSVM;assembly=ATSVM     5      =mvvm,clr-namespace:Infra.MVVM;assembly=Infra.MVVM     5         WorkspaceStyle#���   5      .��5
      � $��15,0,0,0q�6     � RowDefinitions  5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��Open Workspaces��6     WorkspaceTitleStyle#���  67   �� 5   
   $��1��6     HorizontalSeparatorStyle#���  6!   $��0,0,15,0q�6T   � 5   
   $��2��6   .+�5      �� -   5         ��Name$ WorkspacesList��5      $��
0,20,15,20q�6�   $��Disabled=�5      ��5   .   �� *
Workspaces+ �� SelectedItem 6Q   �� *SelectedWorkspace+ �� Mode$ TwoWay=���5      �� 5      .R�5      �  � 
ItemHeight$	 286��6$    � 	ItemWidth$	 682��65    � Orientation$ 
Horizontal=�6E   5       ��5"      �� 5#       �� BasedOn# [B��6    �� 
TargetType" ��6L   .�5$      ��   Infra.MVVM.EventToCommand	 �� Property	 �@ Event
 �� Value$
 MouseDoubleClick��6F   �� 5%      	 �@ Command
 6H   ��  �� ElementName$ WorkspacesList�� �� Path$$ DataContext.DoubleClickCommand��� 5&      	 �@ CommandParameter
 6Q   �� 5'      ��5)      �� 5*      .R�5+      �� $��20q�6%   $��Center=�61   .+�5,      (� .��5-   "   �  �Name$ backgroundGrid��6'   .+�5.   &   ��  ��Name$
 Rect��60     WorkspacesBackgroundBrush#���  6>   �� 5/   &   $��Stretch=�6E   $��Stretch=�6c   ��6,   �� *Thumb+6c   50   #   51      52      53      56      57      �� 5:   
   $��3��6   #���  6!   $��0,0,15,0q�6T   �� 5=   
   $��4��6    # WorkspaceActionsStackPanelStyle#���  6"   .+�5>      �� -   5        $��btnGo��5>      $
�� Go ��6"   ��61   �� *	GoCommand+5?      5@      5A      