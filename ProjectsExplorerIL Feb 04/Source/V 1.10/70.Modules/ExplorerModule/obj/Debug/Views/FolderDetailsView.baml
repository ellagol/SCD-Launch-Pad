   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5      E AInfra.MVVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:Infra.MVVM;assembly=Infra.MVVM
Infra.MVVM 5      F BNotesModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null>.clr-namespace:NotesModule;assembly=NotesModuleNotesModule 5      I EExplorerModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:ExplorerModuleExplorerModule 5      J FInfra.DragDrop, Version=0.1.3.10, Culture=neutral, PublicKeyToken=nullG4clr-namespace:Infra.DragDrop;assembly=Infra.DragDropInfra.DragDrop 5	      &    ExplorerModule.FolderDetailsView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5      =mvvm,clr-namespace:Infra.MVVM;assembly=Infra.MVVM     5      @notes.clr-namespace:NotesModule;assembly=NotesModule     5      .localclr-namespace:ExplorerModule     5      P	 LNavigationPane, Version=1.0.5037.19256, Culture=neutral, PublicKeyToken=nullnpNavigationPane 	 5      =dd4clr-namespace:Infra.DragDrop;assembly=Infra.DragDrop 	 5	        � 	Resources  5       �� MergedDictionaries �� Source   ContentListViewStyle �� 
TargetType �� Property �� Value  SubFolderTileTemplate �� DataType.  (ExplorerModule.TreeViewNodeViewModelBase �� Padding � ColumnDefinitions  CertificatesListViewStyle�� 5   
    5      �� 5      $Y Spack://application:,,,/ExplorerModule;component/Resources/Themes/MetroTabStyle.xaml?�6%   %l  &	        &	 �    &	 �    �� 5      " ��61   .�5      ��  � ��
 ���� 6/   �� 5       � ��$
 Left=�68   �� 5       � ��$
 Left=�6?   �� 5       � ��
 ���� 60   �� 5       � ��$ 0q�64   �� 5       � �� .  6B   �� 5       �@��AlternationCount$ 0��65   �� 5       � �� 5      �� 5      " ��6    .�5      ��  � ��
 ���� 6;   5      5!      5"      �� 5$      "  69   .R�5%      �� $��Stretch=�6   
������ $��0q�6Q   $ 5q�6e   ._�5&      � $��Stretch=�6    5'      �� 5(      $��80 �6/   �� 5)      .+�5+      �� $��0��6    $��80��60   $��Uniform=�6<   ��6N   �� *	LargeIcon+�� 5,      $��1��6%   $	��2,0q�65   $��Center=�6B   .+�5-      �� $
��Bold"�6>   ��6(   �� *Name+6>   �� 5.      $��0,0,0,0q�6E   ��6(   �� *Description+6E   5/      50      51      52      �� 54      " ��66   .�55      ��  � ��
 ���� 6/   �� 56       � ��$
 Left=�68   �� 57       � ��$
 Left=�6?   �� 58       � ��
 ���� 60   �� 59       � ��$ 0q�64   �� 5:       � �� .  6B   �� 5;       �@��AlternationCount$ 0��65   �� 5<       � �� 5=      �� 5>      " ��6    .�5?      ��  � ��
 ���� 6;   5@      5B      5C      5E      .��5H      � -   5        	 �Name$	 
LayoutRoot��5H      
 � RowDefinitions
 5I   
   �� 5J      $��* �6   �� 5K      $
��Auto �6   �� 5L      $
��Auto �6   .+�5P   
   ]� -   5         ]�Name$ ContentArea��5P      $��0��6)   .+�5R      # 	 Stema.Controls.NavigationPane  -   5          Name$ navigationPane��5R       $��0��68   $
��Left=�6B   {�.  $y�	Collapsed=�6o   + 	 %Stema.Controls.NavigationPaneExpander  5S      -   5          Name$ NavigationPaneExpander��5S   (   $	��300��6H   $��0,0,2,1q�6T   $��Right=�6e      Orientation$ Right=�6|   $�� ��6�      IsMinimized 6�   ��  �� Path$ 	ShowNotes�.@�5T      �� $��Notes��6)   .��5U      �    	CanResize .  .+�5V        NotesModule.NotesControl  $��10q�6K    � DataContext 6-   �� *Notes+6K   5W      5X      5Z      �� 5]      $��0��6   ��. $
��Auto=�6@   $
��Auto=�6c   .��5^      ��   MetroTab#���  6R    �� SelectedIndex 6   �� $ tabIndex� �� Mode$ TwoWay=�.@�5`      ��   MetroTabItem#���  6;   
����  �$	��210��6u   ��6   �� $
 Name�.��5a      �� -   5         ��Name$ SubFoldersList��5a   #   #��[ 6d   #��[  6�   $��0,30,0,0q�6�   ��6;   �� $ SubProjects���5b      �� 5c   "   " ��6(   .�5d   &   ��   Infra.MVVM.EventToCommand �@ Event$ MouseDoubleClick��6R   �� 5e   &    �@ Command 6T   ��  �� ElementName$ SubFoldersList��$$ DataContext.DoubleClickCommand��� 5f   &    �@ CommandParameter 6]   �� 5g   #   5i      5j      �� 5l      $��
Properties��6   #���  62   
���  ��.��5m      � 
 5n      �� 5o   "   $��30 �60   �� 5p   "   $
��Auto �60   �� 5q   "   $��30 �60   �� 5r   "   $
��Auto �60   �� 5s   "   $��30 �60   �� 5t   "   $
��Auto �60   �� 5u   "   $��30 �60   �� 5v   "   $
��Auto �60   �� 5w   "   $��30 �60   �� 5x   "   $
��Auto �60    5z      �� 5{   "   $
��Auto �63   �� 5|   "   $��6 �63   �� 5}   "   $��* �63   .+�5�      �� $��1��6$   $��0��61   $��Name:��6A     WorkspaceLabelsStyle#���  6Q   �� 5�      $��1��6&   $��2��63   $
��Left=�6�   $��Center=�6�   $��Center=�6  $	��300��61  ��6C   �� $
 Name� �� ValidatesOnDataErrors .  �� ValidatesOnExceptions .  �� NotifyOnValidationError .  �� UpdateSourceTrigger$ PropertyChanged=�61  �� 5�      $��3��6$   $��0��61   $��Description:��6A   #���  6X   �� 5�      $��3��6&   $��2��63   $
��Left=�6�   $��Center=�6�   $��Center=�6  $	��400��68  ��6C   �� $ Description� .  .  . $ PropertyChanged=�68  �� 5�      $��5��6$   $��0��61   $��Created On:��6A   #���  6W   �� 5�      $��5��6&   $��2��63   $
��Left=�6�   $��Center=�6�   $��Center=�6�   $	��200��6   �� 
IsReadOnly . ��6C   �� $ CreationDate�+  %ExplorerModule.FolderDetailsViewModel   DateTimeFormat �� StringFormat# Z 6  �� 5�      $��7��6$   $��0��61   $��Modified On:��6A   #���  6X   �� 5�      $��7��6&   $��2��63   $
��Left=�6�   $��Center=�6�   $��Center=�6�   $	��200��6   . ��6C   �� $ LastUpdateTime�# Z 6  �� 5�      $��9��6$   $��0��61   $��Modified By:��6A   #���  6X   �� 5�      $��9��6&   $��2��63   $
��Left=�6h   $��Center=�6�   $��Center=�6�   $	��200��6�    . ��6C   �� $ LastUpdateUser�6�   5�      5�      �� 5�      $��Certificates��6   #���  64   
���� ��  Infra.DragDrop.DragDrop   IsDragSource .     IsDropTarget .     UseDefaultDragAdorner  . !   DropHandler! 5�   _   �� .��5�      � 
 5�      �� 5�   "   $��10 �60   �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60   �� 5�   "   $��20 �60   �� 5�   "   $
��Auto �60   �� 5�   "   $��20 �60   �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60   �� 5�   "   $
��Auto �60    5�      �� 5�   "   $
��Auto �63   �� 5�   "   $��6 �63   �� 5�   "   $��* �63   .+�5�      �� $��1��6$   $��0��61   $��Station Certificates: ��6A   #���  6b   $��18(�6�   $
��Bold"�6�   �� 5�      $��3��6'   $��0��64   $��3��6D   $��70��6Y   $��Stretch=�6�   $��0,8,0,0q�6�   $��Stretch=�6�   " �� SelectionMode$" Single=�5�   #   #��[ 6:    .  .   . # �� SelectedItem# 5�   h   �� $ selectedCertificate���6�   �� *Certificates+! 5�   c   ��   5�   "   ��%Y   '��       �� 5�   &   " ��6,   .�5�   *   ��  � y�$ 	Collapsed=�6G   5�   '   5�   #   $ � ContextMenu$ 5�   "   �� 5�   &   .@�5�   *   m� $��Delete��63   % m� Command% 6D   �� *DeleteCertificateCommand+�� 5�   *   m� 5�   *   $��Show Certified Stations��63   % 6T   �� *" StationCertificateStationCommand+5�   '   & �� View& 5�   "   �� 5�   &   ' �� AllowsColumnReorder' .  .P�5�   *   �� ( �� Width$	( 300��69   ) �� DisplayMemberBinding) 6E   �� *	CerName+5�   '   5�      �� 5�      $��5��6(     HorizontalSeparatorStyle#���  65   $��0,0,0,0q�6h   $��3��6y   �� 5�      $��7��6$   $��0��61   $��User Certificates: ��6A   
���   �$��18(�6s   $
��Bold"�6�   �� 5�      $��9��6'   $��0��64   $��3��6D   $��70��6Y   $��Stretch=�6�   $��0,8,0,0q�6�   $��Stretch=�6�   $" Single=�5�   #   #��[ 6:    .  .   . # 5�   h   �� $ SelectedUserCertificate���6�   �� *UserCertificates+! 5�   c   ��   5�   "   ��%Y   '��       �� 5�   &   " ��6,   .�5�   *   ��  � y�$ 	Collapsed=�6G   5�   '   5�   #   $ 5�   "   �� 5�   &   .@�5�   *   m� $��Delete��63   % 6D   �� *DeleteUserCertificateCommand+5�   '   & 5�   "   �� 5�   &   ' .  .P�5�   *   �� $	( 300��69   ) 6E   �� *UserCertificateName+5�   '   5�      5�      5�      5�      5�      5�      �� 5�   
   $��1��6   #���  6!   $��0,0,15,0q�6T   � 5�   
   $��2��6    5�      �� 5�      $
��Auto �6#   �� 5�      $��* �6#   �� 5�      $
��Auto �6#   .+�5�      �� $��0��6    # WorkspaceActionsStackPanelStyle#���  6)   .+�5�      �� -   5        $��
btnAddCert��5�      $�� Add Station Certificate ��6+   $��0,0,0,0q�6r   ��6O   �� *AddCertCommand+6r   �� 5�      -   5        $��btnUserCertificate��5�      $�� Add User Certificate ��63   $��20,0,0,0q�6�   ��6T   �� *AddUserCertificateCommand+6�   �� 5�      -   5        $��btnAddContent��5�      $�� Add Content ��6.   $��20,0,0,0q�6l   ��6F   �� *AddContentCommand+6l   5�      �� 5�      $��2��6   #���  6)   .+�5�      �� -	   5        $��btnSave��5�      $�� Save ��6(   $��20,0,0,0q�6Y   ��69   �� *SaveCommand+6Y   5�      5�      5�      5�      