   M S B A M L   `   `   `  ����J  FInfra.DragDrop, Version=0.1.3.10, Culture=neutral, PublicKeyToken=nullG4clr-namespace:Infra.DragDrop;assembly=Infra.DragDropInfra.DragDrop  5      E AInfra.MVVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:Infra.MVVM;assembly=Infra.MVVM
Infra.MVVM 5      I EExplorerModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:ExplorerModuleExplorerModule 5      M IATSBusinessObjects, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullS<clr-namespace:ATSBusinessObjects;assembly=ATSBusinessObjectsATSBusinessObjects 5      *   $ExplorerModule.ContentManagementView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Cdd4clr-namespace:Infra.DragDrop;assembly=Infra.DragDrop     5      =mvvm,clr-namespace:Infra.MVVM;assembly=Infra.MVVM     5      .localclr-namespace:ExplorerModule     5      Kbo<clr-namespace:ATSBusinessObjects;assembly=ATSBusinessObjects     5        � 	Resources  5
         BackgroundC1  BackgroundC2  BackgroundGradientBrush �� 
StartPoint �� EndPoint � Color � Offset��%�   &	        &	 !     &	 B     ��5   
   	#FFCFDDEE��60   ��5   
   	#FFC2D4E8��60   �� 5   
   $ 0,0.5&�6>   $	 0,1&�6Q   .��5      � # �   6   $ 0Q�6B   � 5      # �  6   $ 1Q�6B   5      5      .��5       �� -   5         ��Name$ DummyWorkspaceSize��5       
������ $��0q�6C    �� Padding$ 0,0,0,0q�6W   $��
20,0,20,20q�6i   ._�5!   
   � -   5         �Name$ 
VisualRoot��5!      #��[ 6#   .+�5#      � $��0��6    � RowDefinitions 5$      �� 5%      $��* �6$   �� 5&      $
��Auto �6$   �� 5'      $
��Auto �6$   .+�5*      R� -   5        $��ContentTreeViewStandard��5*      $��0��6:   $��
0,10,10,10q�6G   $��Stretch=�6[   $��14(�6�   
������ $��0q�6�      Infra.DragDrop.DragDrop	   IsDragSource	 . 
   IsDropTarget
 .     UseDefaultDragAdorner . ��6w   �� *	TreeNodes+   DropHandler 5+   d   �� ��5,      �� 5.       �� 
TargetType" Q�6    .�51      ��  �� Property �@Q�
IsExpanded �� Value 6;   �� *
IsExpanded+ �� Mode$ TwoWay=��� 52       �@Q�
IsSelected 6;   �� *
IsSelected+$ TwoWay=��� 53       � ��$ DemiBold"�6;   �� 54       � ��$ 16(�69   �� 55       � ��
 ��  �6;    �� Triggers 56      P� 57   "    P� Property �@Q�
IsSelected P� Value . . �58   &   ��  � ��$
 Bold"�6C   �� 59   &    � ��
 � ���6C   5:   #   5<        5?       �� DataType0  *ExplorerModule.CMTreeViewRootNodeViewModel �� ItemsSource �� Orientation2  ,ExplorerModule.CMTreeViewFolderNodeViewModel3  -ExplorerModule.CMTreeViewContentNodeViewModel3  -ExplorerModule.CMTreeViewVersionNodeViewModel��%  (��       5D   3   *, +)(�� N    5L   3   *, +)(�� l    5T   3   *, +)(�� �    5^   3   *, +)�� 5D      "  63    6i   �� *
Children+.R�5E      �� $ 
Horizontal=�6)   .+�5F   "   �� $��16��6(   $��16��63   $��Center=�6[   ��6?   �� *
NodeIcon+6[   �� 5G   "   $��Center=�6F   $��3,0,0,0q�6a   $
��Bold"�6r   $��Italic$�6�   $��20(�6�   ��6,   �� *
NodeData+6�   5H      5I      �� 5L      "  63    6k   �� *
Children+.R�5M      �� $ 
Horizontal=�6)   .+�5N   "   �� $��16��6(   $��16��63   $��Center=�6[   ��6?   �� *
NodeIcon+6[   �� 5O   "   $��Center=�6F   $��3,0,0,0q�6a   ��6,   �� *
NodeData+6a   5P      5Q      �� 5T      "  63    6l   �� *
Children+.R�5U      �� $ 
Horizontal=�6)   .+�5W   "   �� $��16��6(   $��16��63   $��Center=�6[   ��6?   �� *
NodeIcon+6[   �� 5X   "   $��Center=�6F   $��3,0,0,0q�6a   ��6,   �� *
NodeData+6a   5Y      5Z      �� 5^      "  63    6l   �� *
Children+.R�5_      �� $ 
Horizontal=�6)   .+�5a   "   �� $��16��6(   $��16��63   $��Center=�6[   ��6?   �� *
NodeIcon+6[   �� 5b   "   $��Center=�6F   $��3,0,0,0q�6a   ��6,   �� *
NodeData+6a   5c      5d      5e      5f      � 5h      $��2��6    � ColumnDefinitions 5i      �� 5j      $
��Auto �6+   �� 5k      $��* �6+   �� 5l      $
��Auto �6+   .+�5o      �� $��0��6!   $ 
Horizontal=�61   $��Right=�6J   .+�5p      �� $��+��6!   $��30��6R   $��10,10,10,10q�6]    � ToolTip$ Expand All Nodes��6r   ��6-   �� *ExpandAllCommand+6r   �� 5q      $��-��6!   $��30��6T   $��
0,10,10,10q�6_   $ Collapse All Nodes��6s   ��6-   �� *CollapseAllCommand+6s   5r      5s      5u      5v      5w      5y      