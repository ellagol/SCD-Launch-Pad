   M S B A M L   `   `   `  ����J  FInfra.DragDrop, Version=0.1.3.10, Culture=neutral, PublicKeyToken=nullG4clr-namespace:Infra.DragDrop;assembly=Infra.DragDropInfra.DragDrop  5      E AInfra.MVVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:Infra.MVVM;assembly=Infra.MVVM
Infra.MVVM 5      I EExplorerModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:ExplorerModuleExplorerModule 5      M IATSBusinessObjects, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullS<clr-namespace:ATSBusinessObjects;assembly=ATSBusinessObjectsATSBusinessObjects 5      1   +ExplorerModule.ContentManagementReplaceView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Cdd4clr-namespace:Infra.DragDrop;assembly=Infra.DragDrop     5      =mvvm,clr-namespace:Infra.MVVM;assembly=Infra.MVVM     5      .localclr-namespace:ExplorerModule     5      Kbo<clr-namespace:ATSBusinessObjects;assembly=ATSBusinessObjects     5        � 	Resources  5
         BackgroundC1  BackgroundC2  BackgroundGradientBrush �� 
StartPoint �� EndPoint � Color � Offset��%�   &	        &	 !     &	 B     ��5   
   	#FFCFDDEE��60   ��5   
   	#FFC2D4E8��60   �� 5   
   $ 0,0.5&�6>   $	 0,1&�6Q   .��5      � # �   6   $ 0Q�6B   � 5      # �  6   $ 1Q�6B   5      5      .��5      �� -   5         ��Name$ DummyWorkspaceSize��5      
������ $��0q�6C    �� Padding$ 0,0,0,0q�6W   $��
20,0,20,20q�6i   ._�5   
   � -   5         �Name$ 
VisualRoot��5      #��[ 6#   .+�5      � $��0��6    � RowDefinitions 5      �� 5      $��* �6$   �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   .+�5      R� -   5        $��ContentTreeViewStandard��5      $��0��6:   $��
0,10,10,10q�6G   $��Stretch=�6[   $��14(�6�   
������ $��0q�6�      Infra.DragDrop.DragDrop	   IsDragSource	 . 
   IsDropTarget
 .     UseDefaultDragAdorner . ��6w   �� *	TreeNodes+   DropHandler 5    d   �� ��5!      �� 5#       �� 
TargetType" Q�6    .�5&      ��  �� Property �@Q�
IsExpanded �� Value 6;   �� *
IsExpanded+ �� Mode$ TwoWay=��� 5'       �@Q�
IsSelected 6;   �� *
IsSelected+$ TwoWay=��� 5(       � ��$ DemiBold"�6;   �� 5)       � ��$ 16(�69   �� 5*       � ��
 ��  �6;    �� Triggers 5+      P� 5,   "    P� Property �@Q�
IsSelected P� Value . . �5-   &   ��  � ��$
 Bold"�6C   �� 5.   &    � ��
 � ���6C   5/   #   51        54       �� DataType0  *ExplorerModule.CMTreeViewRootNodeViewModel �� ItemsSource �� Orientation2  ,ExplorerModule.CMTreeViewFolderNodeViewModel3  -ExplorerModule.CMTreeViewContentNodeViewModel3  -ExplorerModule.CMTreeViewVersionNodeViewModel��%  (��       59   3   *, +)(�� N    5A   3   *, +)(�� l    5I   3   *, +)(�� �    5S   3   *, +)�� 59      "  63    6i   �� *
Children+.R�5:      �� $ 
Horizontal=�6)   .+�5;   "   �� $��16��6(   $��16��63   $��Center=�6[   ��6?   �� *
NodeIcon+6[   �� 5<   "   $��Center=�6F   $��3,0,0,0q�6a   $
��Bold"�6r   $��Italic$�6�   $��20(�6�   ��6,   �� *
NodeData+6�   5=      5>      �� 5A      "  63    6k   �� *
Children+.R�5B      �� $ 
Horizontal=�6)   .+�5C   "   �� $��16��6(   $��16��63   $��Center=�6[   ��6?   �� *
NodeIcon+6[   �� 5D   "   $��Center=�6F   $��3,0,0,0q�6a   ��6,   �� *
NodeData+6a   5E      5F      �� 5I      "  63    6l   �� *
Children+.R�5J      �� $ 
Horizontal=�6)   .+�5L   "   �� $��16��6(   $��16��63   $��Center=�6[   ��6?   �� *
NodeIcon+6[   �� 5M   "   $��Center=�6F   $��3,0,0,0q�6a   ��6,   �� *
NodeData+6a   5N      5O      �� 5S      "  63    6l   �� *
Children+.R�5T      �� $ 
Horizontal=�6)   .+�5V   "   �� $��16��6(   $��16��63   $��Center=�6[   ��6?   �� *
NodeIcon+6[   �� 5W   "   $��Center=�6F   $��3,0,0,0q�6a   ��6,   �� *
NodeData+6a   5X      5Y      5Z      5[      � 5]      $��2��6    � ColumnDefinitions 5^      �� 5_      $
��Auto �6+   �� 5`      $��* �6+   �� 5a      $
��Auto �6+   .+�5c      �� $��0��6!   $ 
Horizontal=�61   $��Right=�6J   .+�5d      �� $��+��6!   $��30��6R   $��10,10,10,10q�6]    � ToolTip$ Expand All Nodes��6r   ��6-   �� *ExpandAllCommand+6r   �� 5e      $��-��6!   $��30��6T   $��
0,10,10,10q�6_   $ Collapse All Nodes��6s   ��6-   �� *CollapseAllCommand+6s   5f      �� 5g      $��1��6!   $ 
Horizontal=�61   $��Right=�6J   .+�5h      �� $��
0,10,10,10q�6M   $��35��6a   ��6!   �� *FilterByExistingCommand+ 5i   "   �� *ButtonToolTip+.��5j      �� $��20��5k   $   $��20��60   $��Center=�6;   $��Center=�6X   ��5j   $   �� *ButtonIconPath+5k   X   5l      �� 5m      $��0,10,120,10q�6N   $��35��6c   ��6!   �� *FilterByTemplatesCommand+ 5n   "   �� *TButtonToolTip+.��5o      �� $��20��5p   $   $��20��60   $��Center=�6;   $��Center=�6X   ��5o   $   �� *TButtonIconPath+5p   X   5q      5r      5s      5u      5v      5w      5y      