   M S B A M L   `   `   `  ����E  AInfra.MVVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:Infra.MVVM;assembly=Infra.MVVM
Infra.MVVM  5      L HContentMgmtModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:ContentMgmtModuleContentMgmtModule 5      J FInfra.DragDrop, Version=0.1.3.10, Culture=neutral, PublicKeyToken=nullG4clr-namespace:Infra.DragDrop;assembly=Infra.DragDropInfra.DragDrop 5      K*clr-namespace:ContentMgmtModule.ConvertersContentMgmtModule.Converters 5	      ,   &ContentMgmtModule.CMVersionDetailsView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      =mvvm,clr-namespace:Infra.MVVM;assembly=Infra.MVVM     5      1localclr-namespace:ContentMgmtModule     5      P LNavigationPane, Version=1.0.5037.19256, Culture=neutral, PublicKeyToken=nullnpNavigationPane  5      =dd4clr-namespace:Infra.DragDrop;assembly=Infra.DragDrop  5      5conv*clr-namespace:ContentMgmtModule.Converters  5	      :d2http://schemas.microsoft.com/expression/blend/2008  5
      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5        � 	Resources  5   
    �� MergedDictionaries �� Source   BoolToVisibilityConverter;  5ContentMgmtModule.Converters.BoolToFontWidthConverter  BoolToFontWidthA ;ContentMgmtModule.Converters.ItemFileStatusToColorConverter  ItemFileStatusToColor�� 5   
    5      �� 5      $\ Vpack://application:,,,/ContentMgmtModule;component/Resources/Themes/MetroTabStyle.xaml?�6%   �� 5      $c ]pack://application:,,,/ContentMgmtModule;component/Resources/Themes/CustomControlsStyles.xaml?�6%   %W   &	        &	      &	 &     �� 5      6+     5      6,     5      62   5      .��5!      � -   5         �Name$ 
LayoutRoot��5!       � RowDefinitions 5"   
   �� 5#      $��* �6   �� 5$      $
��Auto �6   �� 5%      $
��Auto �6   .+�5(   
   ]� -   5         ]�Name$ ContentArea��5(      $��0��6)   .+�5+      �� $��0��6   ��. $
��Auto=�6@   $
��Auto=�6c   .��5,      ��   MetroTab#���  6R    �� SelectedIndex 6   ��  �� Path$ tabIndex� �� Mode$ TwoWay=�.@�5.      �� $��
Properties��6     MetroTabItem#���  62   
����  �.��5/      �  50      �� 51   "   $��30 �60   �� 52   "   $
��Auto �60   �� 53   "   $��30 �60   �� 54   "   $
��Auto �60   �� 55   "   $��30 �60   �� 56   "   $
��Auto �60   �� 57   "   $��30 �60   �� 58   "   $
��Auto �60   �� 59   "   $��30 �60   �� 5:   "   $
��Auto �60   �� 5;   "   $��30 �60   �� 5<   "   $
��Auto �60   �� 5=   "   $��30 �60   �� 5>   "   $
��Auto �60   �� 5?   "   $��30 �60   �� 5@   "   $��0 �60   �� 5A   "   $
��Auto �60   �� 5B   "   $��30 �60   �� 5C   "   $
��Auto �60   �� 5D   "   $��30 �60   �� 5E   "   $
��Auto �60   �� 5F   "   $��30 �60   �� 5G   "   $
��Auto �60   �� 5H   "   $��30 �60   �� 5I   "   $
��Auto �60   �� 5J   "   $��30 �60   �� 5K   "   $
��Auto �60   	 � ColumnDefinitions	 5M      �� 5N   "   $	��220 �63   �� 5O   "   $��6 �63   �� 5P   "   $
��Auto �63   �� 5Q   "   $
��Auto �63   .+�5U      �� $��1��6$   $��0��61   $��Version Id:��6A     WorkspaceLabelsStyle#���  6W   �� 5V      $��1��6&   $��2��63   {�. $
��Left=�5Z   &   $��Center=�6A   $��Center=�6\   $	��300��6~   ��5V   C   �� $ VersionName�
 �� ValidatesOnDataErrors
 .  �� ValidatesOnExceptions .  �� NotifyOnValidationError .  �� UpdateSourceTrigger$ PropertyChanged=� �� 
IsReadOnly 5Y   7   �� *IsNameReadOnly+5Z   ~   �� 5]      $��3��6$   $��0��61   $��Description:��6A   #���  6X   �� 5^      $��3��6&   $��2��63   $
��Left=�6  $��Center=�6+  $��Center=�6F  $	��300��6h  $	��0,2q�6t  ��6C   �� $ VersionDescription�
 .  .  . $ PropertyChanged=�{�6�   �� *UpdateModeProperty+6t  �� 5a      $��5��6$   $��0��61   $
��ECR:��6A   #���  6P   �� 5b      $��5��6&   $��2��63   $
��Left=�5f   &   $��Center=�6A   $��Center=�6\   $	��300��6~   $	��0,2q�6�   ��5b   C   �� $	 ECR�
 .  .  . $ PropertyChanged=�{�5e   &   �� *UpdateModeProperty+5f   �   �� 5i      $��7��6$   $��0��61   $��
PDM DocID:��6A   #���  6V   �� 5j      $��7��6&   $��2��63   $
��Left=�5o   &   $��Center=�6A   $��Center=�6\   $	��300��6~   $	��0,2q�6�   ��5j   C   �� $ 
DocumentID�
 .  .  . $ PropertyChanged=�{�5n   &   �� *UpdateModeProperty+5o   �   �� 5r      $��9��6$   $��0��61   $��PDM Document Version:��6A   #���  6a   �� 5s      $��9��6&   $��2��63   $
��Left=�5w   &   $��Center=�6A   $��Center=�6\   $	��300��6~   $	��0,2q�6�   ��5s   C   �� $ PDMDocumentVersion�
 .  .  . $ PropertyChanged=�{�5v   &   �� *UpdateModeProperty+5w   �   �� 5z      $��11��6$   $��0��62   $$��Configuration Management Link:��6B   #���  6k   �� 5{      $��11��6&   $��2��64   $
��Left=�5   &   $��Center=�6A   $��Center=�6\   $	��300��6~   $	��0,2q�6�   ��5{   D   �� $! ConfigurationManagementLink�
 .  .  . $ PropertyChanged=�{�5~   &   �� *UpdateModeProperty+5   �   �� 5�      $��13��6$   $��0��62   $��Status:��6B   #���  6T   �� 5�      $��13��6'   $��2��65   $
��Left=�6E   $��Center=�6`   $��Center=�6{   $	��300��6�    �� DisplayMemberPath$	 Key��6p   �� SelectedValuePath$ Value��6�  ��6�   �� *VersionStatusTypesList+{�6�   �� *UpdateModeStatus+ �� SelectedValue 6�   �� *EditVersionStatusType+
 . $ PropertyChanged=�6�  �� 5�      $��16��6$   $��0��62   $��Lock Notes:��6B   #���  6X   �� 5�      $��16��6&   $��2��64   $
��Left=�6  $��Center=�6!  $��Center=�6<  $	��300��6^  $	��0,2q�6j  ��6D   �� $ 	LockNotes�
 .  .  . $ PropertyChanged=�{�6�   �� *UpdateModeEditor+6j  �� 5�      $��16��6&   $��3��64   {�.  $
��Auto��6�   $��10,0,0,0q�6�   ��6D   �� *
EditorName+y�6r   �� $ IsVisibleEditorName�$ OneWay=� �� 	Converter# [  6�   �� 5�      $��18��6$   $��0��62   $��Created On:��6B   #���  6X   �� 5�      $��18��6&   $��2��64    . $
��Left=�6�   $��Center=�6  $��Center=�6*  $	��300��6L  $	��0,2q�6X  ��6D   �� $ CreationDate�
 .  .  . $ PropertyChanged=�6X  �� 5�      $��20��6$   $��0��62   $��Last Update Date:��6B   #���  6^   �� 5�      $��20��6&   $��2��64    . $
��Left=�6�   $��Center=�6  $��Center=�6,  $	��300��6N  $	��0,2q�6Z  ��6D   �� $ LastUpdateTime�
 .  .  . $ PropertyChanged=�6Z  �� 5�      $��22��6$   $��0��62   $��Last Update User:��6B   #���  6^   �� 5�      $��22��6&   $��2��64    . $
��Left=�6�   $��Center=�6  $��Center=�6,  $	��300��6N  $	��0,2q�6Z  ��6D   �� $ LastUpdateUser�
 .  .  . $ PropertyChanged=�6Z  5�      5�      �� 5�      $��Files��6   #���  6-   
���  ��.��5�      �  5�      �� 5�   "   $��30 �60   �� 5�   "   $
��Auto �60   �� 5�   "   $��30 �60   �� 5�   "   $
��Auto �60   �� 5�   "   $��30 �60   �� 5�   "   $
��Auto �60   �� 5�   "   $��30 �60   �� 5�   "   $
��Auto �60   �� 5�   "   $��30 �60   �� 5�   "   $
��Auto �60   �� 5�   "   $��30 �60   �� 5�   "   $
��Auto �60   �� 5�   "   $	��51* �60   �� 5�   "   $��30 �60   �� 5�   "   $
��Auto �60   �� 5�   "   $��30 �60   �� 5�   "   $
��Auto �60   	 5�      �� 5�   "   $	��120 �63   �� 5�   "   $��6 �63   �� 5�   "   $
��Auto �63   �� 5�   "   $
��Auto �63   .+�5�      �� $��0,5,5,0q�6%   $��6��66   $��2��6C   $��6��6S   
�������$	��300��6x   �� 5�      $��1��6$   $��0��61   $��Command Line:��6A   #���  6Y   �� 5�      $��1��6&   $��2��63   {�. $
��Left=�5�   &   $��Center=�6A   $��Center=�5�   &   $	��300��6H   ��5�   C   �� $ CommandLine�
 .  .  . $ PropertyChanged=� 5�   7   �� *IsDataReadOnly+5�   H   �� 5�      $��3��6$   $��0��61   $��path:��6A   #���  6Q   �� 5�      $��3��6&   $��2��63    . $
��Left=�5�   &   $��Center=�6A   $��Center=�6\   $	��300��6~   $	��0,2q�6�   ��5�   C   �� $
 Path�
 .  .  . $ PropertyChanged=�5�   �   �� 5�      -   5        $
��Open��5�   B   $��3��6%   $��3��62   $
��Open��6N   $
��Auto��6�   $��10,0,0,0q�6�   ��6^   �� *OpenPathExecute+6�   �� 5�      $��7��6$   $��0��61   $��  Files and 
Directories:��6A   #���  6j   �� 5�      -   5        $��	btnBrowse��5�   B   $��7��6%   $��3��62   $��Browse��6S   $
��Auto��6�   $��10,0,0,0q�6�   ��6e   �� *BrowseFilesExecute+6�   R� 5�      -   5         R�Name$ Files��5�   '   $��0,5,5,0q�66   $��6��6G   $��2��6T   $��Stretch=�6�   $��Stretch=�6�   $��Stretch=�6�   $	��300��6�   $��6��6�   $	��300��6    Infra.DragDrop.DragDrop   IsDragSource .    IsDropTarget .    UseDefaultDragAdorner . ��6d   �� *SubItemNode+   DropHandler 5�   �   ��   5�   "    �� 
TargetType/  )ContentMgmtModule.CMTreeViewFilesBehavior �� Property �� Value3  -ContentMgmtModule.CMTreeViewFilesItemBehavior �� Color��%  'R�       'Q� ^     (�� �     5�   6   * SystemColors.HighlightBrushKey+)(�� #    5�   6   *$"SystemColors.HighlightTextBrushKey+)(�� B    5�   6   *SystemColors.ControlBrushKey+)�� 5�   &   " R�6,   .�5�   *   ��  �@ IsTreeViewFilesBehavior . 6r   5�   '   �� 5�   &   " Q�6,   .�5�   *   ��  �@Q�
IsSelected 6G   �� *
IsSelected+�� 5�   *   # �@ IsTreeViewFilesItemBehavior . 6z   5�   '   �� 5�   &   $ Transparent��6h   �� 5�   &   $ Black��6l   �� 5�   &   $ White��6f   5�   #    � ContextMenu 5�   &   �� 5�   &    � DataContext 62   �� *PlacementTarget.DataContext+ �� RelativeSource �� *Self+.@�5�   .   m� $��	View File��67    m� Command 6J   �� *ExecuteFileCommand+�� 5�   .   m� 5�   .   $��
Delete all��67    6K   �� *DeleteAllFilesCommand+m� 5�   .   $��Delete selected��67    6P   �� *DeleteSelectedFilesCommand+5�   +   ��5�   &   �� 5�   (     �� ItemsSource  6A   �� *SubItemNode+.R�5�   .   � 	 5�   2   �� 5�   6   $
��Auto �6G   �� 5�   6   $��* �6G   .+�5�   2   �� $��0��68   $��2q�6H   $��32��6S   $��18��6^   $
��Left=�6�   $��Center=�6�   ��6j   �� $
 Icon�6�   �� 5�   .   $��1��6S   $��2q�6c   $��Center=�6o   ��68   �� $
 Name���6�   �� $ Status�# [ ��6�   �� $ 
IsSelected�# [ 5�   /   5�   +   5�   #   �� 5�      $��14��6$   $��0��62   $��File Size (Bytes):��6B   #���  6_   �� 5�      $��14��6&   $��2��64    . $
��Left=�6  $��Center=�6+  $��Center=�6F  $	��300��6h  ��6D   �� $ SelectedItem.FileSize�! �� ElementName$! Files��
 .  .  . $ PropertyChanged=�6h  �� 5�      $��16��6$   $��0��62   $��Modified On:��6B   #���  6Y   �� 5�      $��16��6&   $��2��64    . $
��Left=�6  $��Center=�6-  $��Center=�6H  $	��300��6j  ��6D   �� $ SelectedItem.ModifiedOn�$! Files��
 .  .  . $ PropertyChanged=�6j  5�      5�      �� 5     $��Contents (Versions)��6   #���  6;   
���2�2�.��5     �  5     �� 5  "   $��30 �60   �� 5  "   $
��Auto �60   �� 5  "   $��30 �60   �� 5  "   $
��Auto �60   �� 5  "   $��30 �60   �� 5	  "   $
��Auto �60   �� 5
  "   $��30 �60   �� 5  "   $
��Auto �60   	 5     �� 5  "   $	��120 �63   �� 5  "   $��6 �63   �� 5  "   $
��Auto �63   .+�5     �� $��3��6$   $��0��61   $��Content Version:��6A   #���  6\   �� 5     -   5        " ��Name$" VersionsList��5  K   $��0,5,5,0q�6a   $��2��6r   $��2��6   $��Stretch=�6�   $	��Top=�6  $	��250��65  $	��550��6B  $��4��6N   .  .  . ��. # =� ErrorTemplate# 6'   Z� $ �� SelectedItem$ 6�   �� $ SelectedLinkedVersion���6�   �� $ SubItemVersionLinkNode� 5  a   ��   5  "     ItemContextMenu% �� Mode& �� AncestorType��%a  &	       '��     �� 5  &   .@�5  *   m� $��Open Linked Version��63   
������� 6P   �� $< 6DataContext.OpenLinkedVersionInAnotherWorkspaceCommand� �� $% FindAncestor=�$& ListViewL�6�   m� 5  *   $��
Delete all��63   
������� 6G   �� $* $DataContext.DeleteAllVersionsCommand� �� $% FindAncestor=�$& ListViewL�6�   m� 5  *   $��Delete selected��63   
������� 6L   �� $/ )DataContext.DeleteSelectedVersionsCommand� �� $% FindAncestor=�$& ListViewL�6�   5  '   �� 5  &   " ��6,   .�5  *   ��  � {� . 6F   5  '   5   #   ' J� InputBindings' 5"  "   �� 5#  &   ( �� Gesture$( Delete��61   ) �� Command) 6B   �� $# DeleteSelectedVersionsCommand�6~   * �� View* 5'  "   �� 5(  &   -   5        SubItemVersionLinkNode��+ �� AllowsColumnReorder+ .  .P�5)  *   �� $
��Icon��69   , �� Width$	, 100��6G   - �� CellTemplate- 5*  .   �� 5+  2   .R�5,  6   � .+�5-  :   �� $��32��6Y   $��18��6d   ��6@   �� *Icon+6d   5.  7   5/  3   51  +   �� 52  *   $	, 225��69   $��Content Name��6E   . �� DisplayMemberBinding. 6[   �� *ContentName+�� 53  *   $	, 225��69   $��Version Name��6E   . 6[   �� *Name+54  '   ��57  "   �� 58  &   " ��6,   .�59  *   ��  �@��
IsSelected 6G   �� $ TwoWay=�$ 
IsSelected��� 5:  *   <  6ContentMgmtModule.CMListViewLinkedVersionsItemBehavior, �@ $IsListViewLinkedVersionsItemBehavior . 6�   �� 5<  *      Infra.MVVM.EventToCommand �@ Event$ MouseDoubleClick��6V   �� 5=  *    �@ Command 6X   �� $! VersionsList��$< 6DataContext.OpenLinkedVersionInAnotherWorkspaceCommand��� 5>  *    �@ CommandParameter 6a   �� *,*OpenLinkedVersionInAnotherWorkspaceCommand+�� 5@  *    �@�ContextMenu# [ 6H   5A  '   5C     5D     5E     5F     5G     5H     � 5K  
   $��2��6   y�6   �� $ 
ActionMode�# [  .+�5L     �� $��1��6   / �� Orientation$/ 
Horizontal=�6&   $��Right=�6?   $��0,0,45,0q�6[   .+�5M     �� $��80��68   $��0,2,10,2q�6C   ��6   �� *
ActionName+��6U   �� *SaveCommand+5O     5P     5R     5T     