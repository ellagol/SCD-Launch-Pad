   M S B A M L   `   `   `  ����J  FInfra.DragDrop, Version=0.1.3.10, Culture=neutral, PublicKeyToken=nullG4clr-namespace:Infra.DragDrop;assembly=Infra.DragDropInfra.DragDrop  5      E AInfra.MVVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:Infra.MVVM;assembly=Infra.MVVM
Infra.MVVM 5      I EUserCertModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:UserCertModuleUserCertModule 5      ,   &UserCertModule.CertificatesDetailsView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Cdd4clr-namespace:Infra.DragDrop;assembly=Infra.DragDrop     5      =mvvm,clr-namespace:Infra.MVVM;assembly=Infra.MVVM     5      .localclr-namespace:UserCertModule     5        � 	Resources  5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $Y Spack://application:,,,/ExplorerModule;component/Resources/Themes/MetroTabStyle.xaml?�6%   %    5      .��5      � -   5         �Name$ 
LayoutRoot��5       � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   ]� -   5         ]�Name$ ContentArea��5      $��0��6)   .+�5      �� $��0��6   ��. $
��Auto=�6@   $
��Auto=�6c   .��5"      �  5#      �� 5$      $��20 �6$   �� 5%      $
��Auto �6$   �� 5&      $��20 �6$   �� 5'      $
��Auto �6$   �� 5(      $��20 �6$   �� 5)      $
��Auto �6$   �� 5*      $��20 �6$   �� 5+      $
��Auto �6$   �� 5,      $��20 �6$   �� 5-      $
��Auto �6$   �� 5.      $��20 �6$   �� 5/      $
��Auto �6$   �� 50      $��20 �6$   �� 51      $
��Auto �6$   �� 52      $��20 �6$   �� 53      $
��Auto �6$   �� 54      $��20 �6$   �� 55      $
��Auto �6$    � ColumnDefinitions 58      �� 59      $	��100 �6'   �� 5:      $
��Auto �6'   �� 5;      $��6 �6'   �� 5<      $��* �6'   .+�5B      �� $��1��6   $��1��6%   $	��Id:��65      WorkspaceLabelsStyle#���   6C   �� 5C      $��1��6   $��3��6+   $
��Left=�6�   $��Center=�6�   $��Center=�6  $	��400��6(  ��6<   ��  �� Path$ Id� �� ValidatesOnDataErrors . 	 �� ValidatesOnExceptions	 . 
 �� NotifyOnValidationError
 .  �� UpdateSourceTrigger$ PropertyChanged=� �� 
IsReadOnly 64  �� $ LockId� �� Mode$ TwoWay=��� 5G      $��3��6   $��1��6%   $��Name:��65   #���   6E   �� 5H      $��3��6   $��3��6'   $
��Left=�6�   $��Center=�6�   $��Center=�6  $	��400��6&  ��68   �� $
 Name� . 	 . 
 . $ PropertyChanged=�6&  �� 5L      $��5��6   $��1��6%   $��Description:��65   #���   6L   �� 5M      $��5��6   $��3��6+   $
��Left=�6�   $��Center=�6�   $��Center=�6  $	��400��61  ��6<   �� $ Description� . 	 . 
 . $ PropertyChanged=�61  �� 5P      $��7��6   $��1��6%   $��Status:��65   #���   6G   �� 5Q      $��7��6    $��3��6-   $
��Left=�6�   $��Center=�6�   $��Center=�6�   $	��300��6�   ��6=   �� $ Status� �� SelectedValue 6a   �� $ SelectedStatus�$ TwoWay=�6�   �� 5U      $��9��6   $��1��6)   $��Created On:��69   #���   6O   y�6�   �� $ 
ViewFields�$ TwoWay=��� 5V      $��9��6   $��3��6+   $
��Left=�6�   $��Center=�6�   $��Center=�6  $	��400��61   . ��6;   �� $ CreationDate� . 	 . 
 . $ PropertyChanged=�y�6O  �� $ 
ViewFields�$ TwoWay=��� 5Y      $��11��6   $��1��6*   $��Last Update User��6:   #���   6U   y�6�   �� $ 
ViewFields�$ TwoWay=��� 5Z      $��11��6   $��3��6,   $
��Left=�6�   $��Center=�6�   $��Center=�6  $	��400��64   . ��6<   �� $ LastUpdateUser� . 	 . 
 . $ PropertyChanged=�y�6T  �� $ 
ViewFields�$ TwoWay=��� 5]      $��13��6   $��1��6*   $��Modified On:��6:   #���   6Q   y�6�   �� $ 
ViewFields�$ TwoWay=��� 5^      $��13��6   $��3��6,   $
��Left=�6�   $��Center=�6�   $��Center=�6  $	��400��64   . ��6<   �� $ LastUpdateTime� . 	 . 
 . $ PropertyChanged=�y�6R  �� $ 
ViewFields�$ TwoWay=��� 5a      $��1��6L   $��Certified Users:��6\   #���   6w   ��6   �� $ UserRow�$ TwoWay=�6w   �� 5b      $��3��6P   $
��Left=�6�   $��Center=�6�   $��Center=�6�   $	��400��6�      Infra.DragDrop.DragDrop   IsDragSource .    IsDropTarget .    UseDefaultDragAdorner . $��20��6�  ��6   �� $ UserRow�$ TwoWay=� �� SelectedItem 6`   �� $ SelectedUser�$ TwoWay=���6�   �� *Users+$ TwoWay=�   DropHandler 6g  ��   5c       �� 
TargetType �� Property �� Value��%Y   '��       �� 5d      " ��6$   .�5e   "   ��  � y�$ 	Collapsed=�6?   5f      5g       � ContextMenu 5h      �� 5i      .@�5j   "   m� $��Remove��6+    m� Command 6<   �� *RemoveUserCommand+5k       �� View 5m      �� 5n       �� AllowsColumnReorder .  .P�5o   "   ��  �� Width$	 200��61    �� DisplayMemberBinding 6=   �� *
UserName+5p      5r      5u      5w      5x      �� 5{   
   $��1��6     HorizontalSeparatorStyle#���  6!   $��0,0,15,0q�6T   � 5~   
   $��2��6    5      �� 5�      $
��Auto �6#   �� 5�      $��* �6#   �� 5�      $
��Auto �6#   .+�5�      �� $��2��6    # WorkspaceActionsStackPanelStyle#���  6)   .+�5�      �� -   5        $��btnSave��5�      $�� Save ��6(   $��20,0,0,0q�6Y   ��69   �� *SaveCommand+6Y   5�      5�      5�      5�      