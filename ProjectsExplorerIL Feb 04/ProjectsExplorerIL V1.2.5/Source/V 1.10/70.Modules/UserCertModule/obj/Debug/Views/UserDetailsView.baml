   M S B A M L   `   `   `  ����J  FInfra.DragDrop, Version=0.1.3.10, Culture=neutral, PublicKeyToken=nullG4clr-namespace:Infra.DragDrop;assembly=Infra.DragDropInfra.DragDrop  5      E AInfra.MVVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:Infra.MVVM;assembly=Infra.MVVM
Infra.MVVM 5      I EUserCertModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:UserCertModuleUserCertModule 5      $   UserCertModule.UserDetailsView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Cdd4clr-namespace:Infra.DragDrop;assembly=Infra.DragDrop     5      =mvvm,clr-namespace:Infra.MVVM;assembly=Infra.MVVM     5      .localclr-namespace:UserCertModule     5        � 	Resources  5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $Y Spack://application:,,,/ExplorerModule;component/Resources/Themes/MetroTabStyle.xaml?�6%   %    5      .��5      � -   5         �Name$ 
LayoutRoot��5       � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5    
   ]� -   5         ]�Name$ ContentArea��5       $��0��6)   .+�5!      �� $��0��6   ��. $
��Auto=�6@   $
��Auto=�6c   .��5$      �  5%      �� 5&   "   $��30 �60   �� 5'   "   $
��Auto �60   �� 5(   "   $��30 �60   �� 5)   "   $
��Auto �60   �� 5*   "   $��30 �60   �� 5+   "   $
��Auto �60   �� 5,   "   $��30 �60   �� 5-   "   $
��Auto �60   �� 5.   "   $��30 �60   �� 5/   "   $
��Auto �60    � ColumnDefinitions 51      �� 52   "   $	��100 �63   �� 53   "   $
��Auto �63   �� 54   "   $��6 �63   �� 55   "   $��* �63   .+�5<      �� $��1��6   $��1��6)   $��Certificates:��69      WorkspaceLabelsStyle#���   6Q   �� 5=      $��1��6   $��3��6,   $
��Left=�6{   $��Center=�6�   $��Center=�6�   $	��400��6�      Infra.DragDrop.DragDrop   IsDragSource .    IsDropTarget . 	   UseDefaultDragAdorner	 . $��20��6�  
 �� SelectedItem
 6<   ��  �� Path$ SelectedCertificate� �� Mode$ TwoWay=���6�   �� *UserCertificates+$ TwoWay=�   DropHandler 6U  ��   5>       �� 
TargetType �� Property �� Value��%Y   '��       �� 5?   "   " ��6(   .�5@   &   ��  � y�$ 	Collapsed=�6C   5A   #   5B       � ContextMenu 5C      �� 5D       � DataContext 6+   �� *PlacementTarget.DataContext+ �� RelativeSource �� *Self+.@�5E   "   m� $��Remove��6+    m� Command 6<   �� *RemoveCertificateCommand+5F       �� View 5H      �� 5I       �� AllowsColumnReorder .  .P�5J   "   ��  �� Width$	 200��61    �� DisplayMemberBinding 6=   �� *CertificateName+5K      5M      5O      5Q      5R      �� 5U   
   $��1��6     HorizontalSeparatorStyle#���  6!   $��0,0,15,0q�6T   � 5X   
   $��2��6    5Y      �� 5Z      $
��Auto �6#   �� 5[      $��* �6#   �� 5\      $
��Auto �6#   .+�5a      �� $��2��6    # WorkspaceActionsStackPanelStyle#���  6)   .+�5b      �� -   5        $��btnSave��5b      $�� Save ��6(   $��20,0,0,0q�6Y   ��69   �� *SaveCommand+6Y   5c      5e      5g      5k      