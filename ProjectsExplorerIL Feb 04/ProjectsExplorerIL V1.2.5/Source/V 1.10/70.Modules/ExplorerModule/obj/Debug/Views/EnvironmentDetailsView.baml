   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5      E AInfra.MVVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:Infra.MVVM;assembly=Infra.MVVM
Infra.MVVM 5      I EExplorerModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:ExplorerModuleExplorerModule 5      +   %ExplorerModule.EnvironmentDetailsView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5      =mvvm,clr-namespace:Infra.MVVM;assembly=Infra.MVVM     5      .localclr-namespace:ExplorerModule     5      P LNavigationPane, Version=1.0.5037.19256, Culture=neutral, PublicKeyToken=nullnpNavigationPane  5        � 	Resources  5
       �� MergedDictionaries �� Source   ContentListViewStyle �� 
TargetType �� Property �� Value  SubFolderTileTemplate �� DataType.  (ExplorerModule.TreeViewNodeViewModelBase �� Padding � ColumnDefinitions�� 5   
    5      �� 5      $Y Spack://application:,,,/ExplorerModule;component/Resources/Themes/MetroTabStyle.xaml?�6%   %�  &	        &	 �    �� 5      " ��61   .�5      ��  � ��
 ���� 6/   �� 5       � ��$
 Left=�68   �� 5       � ��$
 Left=�6?   �� 5       � ��
 ���� 60   �� 5       � ��$ 0q�64   �� 5       � �� .  6B   �� 5       �@��AlternationCount$ 0��65   �� 5       � �� 5      �� 5      " ��6    .�5      ��  � ��
 ���� 6;   5      5       5!      �� 5#      "  69   .R�5$      �� $��Stretch=�6   
������ $��0q�6Q   $ 5q�6e   ._�5%      � $��Stretch=�6    5&      �� 5'      $��80 �6/   �� 5(      .+�5*      �� $��0��6    $��80��60   $��Uniform=�6<   ��6N   �� *	LargeIcon+�� 5+      $��1��6%   $	��2,0q�65   $��Center=�6B   .+�5,      �� $
��Bold"�6>   ��6(   �� *Name+6>   �� 5-      $��0,0,0,0q�6E   ��6(   �� *Description+6E   5.      5/      50      51      53      .��56      ]� -   5        	 ]�Name$	 
LayoutRoot��56      .+�58   
   �� $��0��6   ��. $
��Auto=�6<   $
��Auto=�6_   .��59      ��   MetroTab#���  6   .@�5:      ��   MetroTabItem#���  67   
����  ���6   �� 
 �� Path$

 Name�.��5;      �� -   5         ��Name$ SubFoldersList��5;      #��[ 6_   #��[  6�   $��0,30,0,0q�6�   ��67   �� $
 
SubFolders���5<      �� 5=      " ��6$   .�5>   "   ��   Infra.MVVM.EventToCommand �@ Event$ MouseDoubleClick��6N   �� 5?   "    �@ Command 6P   ��  �� ElementName$ SubFoldersList��$$
 DataContext.DoubleClickCommand��� 5@   "    �@ CommandParameter 6Y   �� 5A      5C      5D      �� 5F      $��Environment Details��6   #���  67   
���  ��.��5G      �  � RowDefinitions 5H      �� 5I      $��30 �6,   �� 5J      $
��Auto �6,    5L      �� 5M      $
��Auto �6/   �� 5N      $��6 �6/   �� 5O      $��* �6/   .+�5S      �� $��1��6    $��0��6-   $��_Name:��6=     WorkspaceLabelsStyle#���  6u    �� Target 6N   �� $ NameTxt��6u   �� 5T      -   5         ��Name$ NameTxt��5T   "   $��1��63   $��2��6@   {�.  $
��Left=�6�   $��Center=�6�   $��Center=�6  $	��300��62  ��6b   �� $

 Name� �� ValidatesOnDataErrors .  �� ValidatesOnExceptions .  �� UpdateSourceTrigger$ PropertyChanged=�62  5V      5W      5X      5Y      5Z      5\      