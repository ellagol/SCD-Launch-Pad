   M S B A M L   `   `   `  ����J  FInfra.DragDrop, Version=0.1.3.10, Culture=neutral, PublicKeyToken=nullG4clr-namespace:Infra.DragDrop;assembly=Infra.DragDropInfra.DragDrop  5      E AInfra.MVVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:Infra.MVVM;assembly=Infra.MVVM
Infra.MVVM 5      L HContentMgmtModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:ContentMgmtModuleContentMgmtModule 5      P:clr-namespace:ContentMgmtModule;assembly=ContentMgmtModuleContentMgmtModule 5	      M IATSBusinessObjects, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullS<clr-namespace:ATSBusinessObjects;assembly=ATSBusinessObjectsATSBusinessObjects 5
      wNclr-namespace:ATSBusinessObjects.ContentMgmtModels;assembly=ATSBusinessObjects$ATSBusinessObjects.ContentMgmtModels 5      /   )ContentMgmtModule.CMContentManagementView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Cdd4clr-namespace:Infra.DragDrop;assembly=Infra.DragDrop     5      =mvvm,clr-namespace:Infra.MVVM;assembly=Infra.MVVM     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:ContentMgmtModule     5      Icm:clr-namespace:ContentMgmtModule;assembly=ContentMgmtModule     5	      Kbo<clr-namespace:ATSBusinessObjects;assembly=ATSBusinessObjects     5
      _cmtnNclr-namespace:ATSBusinessObjects.ContentMgmtModels;assembly=ATSBusinessObjects     5        � 	Resources  5       �� MergedDictionaries �� Source   BoolToVisibilityConverter  BackgroundC1  BackgroundC2  BackgroundBrush �� 
StartPoint �� EndPoint � Color � Offset �� DataType4  .ContentMgmtModule.CMContentManagementViewModel/  )ContentMgmtModule.CMContentManagementView0  *ContentMgmtModule.CMFolderDetailsViewModel+  %ContentMgmtModule.CMFolderDetailsView1  +ContentMgmtModule.CMContentDetailsViewModel,  &ContentMgmtModule.CMContentDetailsView1  +ContentMgmtModule.CMVersionDetailsViewModel,  &ContentMgmtModule.CMVersionDetailsView,	  &ContentMgmtModule.CMWhereUsedViewModel'
  !ContentMgmtModule.CMWhereUsedView�� 5   
    5      �� 5      $\ Vpack://application:,,,/ContentMgmtModule;component/Resources/Themes/MetroTabStyle.xaml?�6%   %|  &	        &	      &	 4     &	 U     (�� �     5#      *, +)(��     5&      *, +)(�� A    5)      *, +)(�� s    5,      *, +)(�� �    5/      *,	 +)�� 5      6+   ��5      	#FFCFDDEE��64   ��5      	#FFC2D4E8��64   �� 5      $ 0,0.5&�6:   $	 0,1&�6M   .��5      � # �  6   $ 0Q�6F   � 5       # �  6   $ 1Q�6F   5!      �� 5#      "  6   .R�5$        5%      �� 5&      "  6   .R�5'        5(      �� 5)      "  6   .R�5*        5+      �� 5,      "  6   .R�5-        5.      �� 5/      " 	 6   .R�50      
  51      53      .��58      �� -   5         ��Name$ DummyWorkspaceSize��58      
������ $��0q�6C   	 �� Padding$	 0,0,0,0q�6W   ._�59   
   � -   5        
 �Name$
 
VisualRoot��59      #��[ 6#    � ColumnDefinitions 5:      �� 5;      $	��370 �6#   $	��300��6/   $	��800��6>   �� 5<      $
��Auto �6#   �� 5=      $��* �6#   .+�5@      � $��0��6    � RowDefinitions 5A      �� 5B      $��10 �6$   �� 5C      $��35 �6$   �� 5D      $��* �6$   �� 5E      $
��Auto �6$   �� 5F      $
��Auto �6$   .+�5J      �� $��1��6   
����ح�$��1q�6>    �� CornerRadius$ 8��6R   $��1q�6c   
����ح�._�5L      c� 
����ح�$��Stretch=�6:   $��Center=�6Y   $��Center=�6u   .@�5M      ��  � ToolTip$ Refresh��6%   $��Center=�67   ��6R   �� *RefreshCommand+.��5N   "   �� $W��Qpack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/Refresh.png��6(   5O      5P      5R      R� 5U      -   5        $��ContentTreeViewStandard��5U      $��2��6:   $��
0,10,10,10q�6G   $��Stretch=�6[   $��14(�6�   
������ $��0q�6�      Infra.DragDrop.DragDrop   IsDragSource .    IsDropTarget .    UseDefaultDragAdorner . ��6w   �� *	TreeNodes+   DropHandler 5V   c   �� ��5W      �� 5Y       �� 
TargetType" Q�6    .�5Z      �� .  (ContentMgmtModule.CMTreeViewItemBehavior �� Property% �@ IsBroughtIntoViewWhenSelected �� Value . 5\      �� 5_       �@Q�
IsExpanded 6;   �� *
IsExpanded+ �� Mode$ TwoWay=��� 5`       �@Q�
IsSelected 6;   �� *
IsSelected+$ TwoWay=��� 5a       � ��$ DemiBold"�6;   �� 5b       � ��$ 16(�69   �� 5c       � ��
 ��  �6;    �� Triggers 5d      P� 5e   "    P� Property �@Q�
IsSelected P� Value . . �5f   &   ��  � ��$
 Bold"�6C   �� 5g   &    � ��
 �   �6C   5h   #   5j        5m      3  -ContentMgmtModule.CMTreeViewRootNodeViewModel �� ItemsSource �� Orientation � ContextMenu �� Path �� 	Converter=  7ATSBusinessObjects.ContentMgmtModels.TreeNodeObjectType   Folder  m� CommandParameter! m� Command5  /ContentMgmtModule.CMTreeViewFolderNodeViewModel"   Content6  0ContentMgmtModule.CMTreeViewContentNodeViewModel#   ContentVersion6  0ContentMgmtModule.CMTreeViewVersionNodeViewModel��%W	  (��       5r   3   *, +)7   (�� �    5   3   *, +)7   7   7   (�� �    5�   3   *, +)7   7   (�� �    5�   3   *, +)7   �� 5r      "  63    6i   �� *
Children+.R�5s      �� $ 
Horizontal=�6)    5t   "   �� 5u   &   y�62   �� $ IsAddFolderToRoot�8   .@�5v   *   m� $��
New Folder��63   #  Z 6k   ! 6H   �� *NewNodeCommand+6k   5w   '   .+�5y   "   �� $��16��6(   $��16��63   $��Center=�6[   ��6?   �� *
NodeIcon+6[   �� 5z   "   $��Center=�6F   $��3,0,0,0q�6a   $
��Bold"�6r   $��Italic$�6�   $��20(�6�   ��6,   �� *
NodeData+6�   5{      5|      �� 5      "  63    6k   �� *
Children+.R�5�      �� $ 
Horizontal=�6)    5�   "   �� 5�   &   .@�5�   *   m� $��New Content��63   #  Z" 6�   y�6H   �� $ IsAddContent�8   ! 6�   �� *NewNodeCommand+6�   m� 5�   *   $��
New Folder��63   #  Z 6�   y�6G   �� $ IsAddFolder�8  ! 6�   �� *NewNodeCommand+6�   �� 5�   *   m� 5�   *   $��Delete��63   y�6C   �� $ IsDelete�8  ! 6�   �� *DeleteFolderCommand+5�   '   .+�5�   "   �� $��16��6(   $��16��63   $��Center=�6[   ��6?   �� *
NodeIcon+6[   �� 5�   "   $��Center=�6F   $��3,0,0,0q�6a   ��6,   �� *
NodeData+6a   5�      5�      �� 5�      "  63    6l   �� *
Children+.R�5�      �� $ 
Horizontal=�6)    5�   "   �� 5�   &   .@�5�   *   m� $��New Version��63   #  Z# 6�   y�6H   �� $ IsAddVersion�8   ! 6�   �� *NewNodeCommand+6�   �� 5�   *   m� 5�   *   $��Delete��63   y�6C   �� $ IsDelete�8  ! 6�   �� *DeleteContentCommand+5�   '   .+�5�   "   �� $��16��6(   $��16��63   $��Center=�6[   ��6?   �� *
NodeIcon+6[   �� 5�   "   $��Center=�6F   $��3,0,0,0q�6a   ��6,   �� *
NodeData+6a   5�      5�      �� 5�      "  63    6l   �� *
Children+.R�5�      �� $ 
Horizontal=�6)    5�   "   �� 5�   &   .@�5�   *   m� $��Delete��63   y�6C   �� $ IsDelete�8   ! 6�   �� *DeleteVersionCommand+m� 5�   *   $��
Where Used��63   ! 6G   �� *WhereUsedCommand+5�   '   .+�5�   "   �� $��16��6(   $��16��63   $��Center=�6[   ��6?   �� *
NodeIcon+6[   �� 5�   "   $��Center=�6F   $��3,0,0,0q�6a   ��6,   �� *
NodeData+6a   5�      5�      5�      5�      �� 5�      $��3��6     HorizontalSeparatorStyle#���  6)   $��	10,0,10,0q�6\   � 5�      $��4��6    5�      �� 5�      $
��Auto �6+   �� 5�      $��* �6+   �� 5�      $
��Auto �6+   .+�5�      �� $��0��6!   $ 
Horizontal=�61   $
��Left=�6J   .+�5�      �� $��Search��6!   $��70��6W   $��
10,10,0,10q�6b   ��62   �� *CMSearchCommand+6b   �� 5�      $	��120��6h   $��10,10,10,10q�6t   ��6"   �� $ 
SearchText�$ �� UpdateSourceTrigger$$ PropertyChanged=�6t   �� 5�      $
��Next��6!   $��60��6P   $��	0,10,0,10q�6[   ��60   �� *FindCommand+6[   5�      �� 5�      $��2��6!   $ 
Horizontal=�61   $��Right=�6J   .+�5�      �� $��+��6!   $��20��6R   $��
0,10,10,10q�6]   $ Expand All Nodes��6q   ��6-   �� *ExpandAllCommand+6q   �� 5�      $��-��6!   $��20��6T   $��
0,10,10,10q�6_   $ Collapse All Nodes��6s   ��6-   �� *CollapseAllCommand+6s   5�      5�      5�      �� 5�      $��1��6   $��Stretch=�6+   $��6��6G   $��0,0,0,0q�6Q   % �� ResizeBehavior$% PreviousAndNext=�6b   �� 5�      $��2��6   $��10,10,10,10q�6_   ��6-   �� *DetailsViewModel+$ OneWay=�6_   5�      5�      5�      