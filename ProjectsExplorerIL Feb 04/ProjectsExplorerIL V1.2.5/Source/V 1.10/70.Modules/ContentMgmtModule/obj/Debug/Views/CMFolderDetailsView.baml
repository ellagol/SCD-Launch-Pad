   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5      E AInfra.MVVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:Infra.MVVM;assembly=Infra.MVVM
Infra.MVVM 5      L HContentMgmtModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:ContentMgmtModuleContentMgmtModule 5      J FInfra.DragDrop, Version=0.1.3.10, Culture=neutral, PublicKeyToken=nullG4clr-namespace:Infra.DragDrop;assembly=Infra.DragDropInfra.DragDrop 5      +   %ContentMgmtModule.CMFolderDetailsView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5      =mvvm,clr-namespace:Infra.MVVM;assembly=Infra.MVVM     5      1localclr-namespace:ContentMgmtModule     5      P LNavigationPane, Version=1.0.5037.19256, Culture=neutral, PublicKeyToken=nullnpNavigationPane  5      =dd4clr-namespace:Infra.DragDrop;assembly=Infra.DragDrop  5        � 	Resources  5
       �� MergedDictionaries �� Source   BoolToVisibilityConverter�� 5   
    5      �� 5      $\ Vpack://application:,,,/ContentMgmtModule;component/Resources/Themes/MetroTabStyle.xaml?�6%   �� 5      $c ]pack://application:,,,/ContentMgmtModule;component/Resources/Themes/CustomControlsStyles.xaml?�6%   %   &	        �� 5      6+   5      .��5      � -   5         �Name$ 
LayoutRoot��5       � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5"   
   ]� -   5         ]�Name$ ContentArea��5"      $��0��6)   .+�5%      �� $��0��6   ��. $
��Auto=�6@   $
��Auto=�6c   .��5&      ��   MetroTab#���  6R    �� SelectedIndex 6   ��  �� Path$ tabIndex� �� Mode$ TwoWay=�.@�5(      �� $��
Properties��6     MetroTabItem#���  62   
���  ��.��5)      �  5*      �� 5+   "   $��30 �60   �� 5,   "   $
��Auto �60   �� 5-   "   $��30 �60   �� 5.   "   $
��Auto �60   �� 5/   "   $��30 �60   �� 50   "   $
��Auto �60   �� 51   "   $��30 �60   �� 52   "   $
��Auto �60   �� 53   "   $��30 �60   �� 54   "   $
��Auto �60   �� 55   "   $��30 �60   �� 56   "   $
��Auto �60   �� 57   "   $��30 �60   �� 58   "   $
��Auto �60   �� 59   "   $��30 �60   �� 5:   "   $
��Auto �60   	 � ColumnDefinitions	 5<      �� 5=   "   $	��120 �63   �� 5>   "   $��6 �63   �� 5?   "   $��* �63   .+�5C      �� $��1��6$   $��0��61   $��Name:��6A     WorkspaceLabelsStyle#���  6Q   �� 5D      $��1��6&   $��2��63   $
��Left=�6  $��Center=�6#  $��Center=�6>  $	��300��6`  ��6C   �� $ 
FolderName�
 �� ValidatesOnDataErrors
 .  �� ValidatesOnExceptions .  �� NotifyOnValidationError .  �� UpdateSourceTrigger$ PropertyChanged=�{�6�   �� *UpdateModeProperty+6`  �� 5G      $��3��6$   $��0��61   $��Description:��6A   #���  6X   �� 5H      $��3��6&   $��2��63   $
��Left=�6  $��Center=�6*  $��Center=�6E  $	��300��6g  $	��0,2q�6s  ��6C   �� $ FolderDescription�
 .  .  . $ PropertyChanged=�{�6�   �� *UpdateModeProperty+6s  �� 5K      $��5��6$   $��0��61   $��User Groups:��6A   #���  6X   �� 5L      $��5��6&   $��2��63   $
��Left=�6�   $��Center=�6�   $��Center=�6�   $	��300��6�   $
��auto��6  {�6C   �� *UserGroupUpdatePermission+��6s   �� *UserGroupTypeList+��5M   "   �� 5N   &   .R�5O   *   �� ��63   �� *Name+ d� 	IsChecked 6L   �� *	Checked+$ TwoWay=�5P   '   5R      �� 5U      $��7��6$   $��0��61   $��Created On:��6A   #���  6W   �� 5V      $��7��6&   $��2��63    �� 
IsReadOnly . $
��Left=�6�   $��Center=�6  $��Center=�6)  $	��300��6K  $	��0,2q�6W  ��6C   �� $ CreationDate�
 .  .  . $ PropertyChanged=�6W  �� 5Y      $��9��6$   $��0��61   $��Last Update Date:��6A   #���  6]   �� 5Z      $��9��6&   $��2��63    . $
��Left=�6�   $��Center=�6  $��Center=�6+  $	��300��6M  $	��0,2q�6Y  ��6C   �� $ LastUpdateTime�
 .  .  . $ PropertyChanged=�6Y  �� 5]      $��11��6$   $��0��62   $��Last Update User:��6B   #���  6^   �� 5^      $��11��6&   $��2��64    . $
��Left=�6�   $��Center=�6  $��Center=�6,  $	��300��6N  $	��0,2q�6Z  ��6D   �� $ LastUpdateUser�
 .  .  . $ PropertyChanged=�6Z  5a      5b      �� 5e      $��Default Version Name��6   #���  6<   
���  ��.��5f      �  5g      �� 5h   "   $��30 �60   �� 5i   "   $
��Auto �60   �� 5j   "   $��30 �60   �� 5k   "   $
��Auto �60   �� 5l   "   $��30 �60   �� 5m   "   $
��Auto �60   �� 5n   "   $��30 �60   �� 5o   "   $
��Auto �60   �� 5p   "   $��30 �60   �� 5q   "   $
��Auto �60   �� 5r   "   $��30 �60   	 5t      �� 5u   "   $	��120 �63   �� 5v   "   $��6 �63   �� 5w   "   $��* �63   .+�5{      �� $��1��6$   $��0��61   $��Prefix:��6A   #���  6S   �� 5|      $��1��6&   $��2��63   $
��Left=�6  $��Center=�6(  $��Center=�6C  $	��300��6e  ��6C   �� $ DefaultVNPrefix�
 .  .  . $ PropertyChanged=�{�6�   �� *UpdateModeProperty+6e  �� 5      $��3��6$   $��0��61   $��Start Value:��6A   #���  6X   �� 5�      $��3��6&   $��2��63   $
��Left=�6  $��Center=�6,  $��Center=�6G  $	��300��6i  $	��0,2q�6u  ��6C   �� $ DefaultVNStartValue�
 .  .  . $ PropertyChanged=�{�6�   �� *UpdateModeProperty+6u  �� 5�      $��5��6$   $��0��61   $��
Increment:��6A   #���  6V   �� 5�      $��5��6&   $��2��63   $
��Left=�5�   J   $��Center=�6e   $��Center=�5�   !   $	��300��6C   $	��0,2q�6O    � ToolTip$" Please set Start Value first��5�   !    ^� ShowOnDisabled . ��5�   C   �� $ DefaultVNIncrement�
 .  .  . $ PropertyChanged=�{�5�   !   �� *IsIncrementEnabled+ ^� 	IsEnabled 5�   !   �� *IsToolTipEnabled+5�      5�      5�      5�      5�      � 5�   
   $��2��6   y�6   �� $ 
ActionMode� �� 	Converter# [  .+�5�      ��  �� Orientation$ 
Horizontal=�6   $��Right=�62   $��0,0,45,0q�6N   .+�5�      �� $��80��68   $��0,2,10,2q�6C   ��6   �� *
ActionName+��6U   �� *SaveCommand+5�      5�      5�      5�      