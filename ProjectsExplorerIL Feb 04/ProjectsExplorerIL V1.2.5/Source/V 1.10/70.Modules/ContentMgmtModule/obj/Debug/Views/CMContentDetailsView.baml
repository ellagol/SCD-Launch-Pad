   M S B A M L   `   `   `  ����E  AInfra.MVVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:Infra.MVVM;assembly=Infra.MVVM
Infra.MVVM  5      L HContentMgmtModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:ContentMgmtModuleContentMgmtModule 5      J FInfra.DragDrop, Version=0.1.3.10, Culture=neutral, PublicKeyToken=nullG4clr-namespace:Infra.DragDrop;assembly=Infra.DragDropInfra.DragDrop 5      ,   &ContentMgmtModule.CMContentDetailsView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      =mvvm,clr-namespace:Infra.MVVM;assembly=Infra.MVVM     5      1localclr-namespace:ContentMgmtModule     5      P LNavigationPane, Version=1.0.5037.19256, Culture=neutral, PublicKeyToken=nullnpNavigationPane  5      =dd4clr-namespace:Infra.DragDrop;assembly=Infra.DragDrop  5      :d2http://schemas.microsoft.com/expression/blend/2008  5	      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5
        � 	Resources  5       �� MergedDictionaries �� Source   BoolToVisibilityConverter�� 5   
    5      �� 5      $\ Vpack://application:,,,/ContentMgmtModule;component/Resources/Themes/MetroTabStyle.xaml?�6%   �� 5      $c ]pack://application:,,,/ContentMgmtModule;component/Resources/Themes/CustomControlsStyles.xaml?�6%   %   &	        �� 5      6+   5      .��5      � -   5         �Name$ 
LayoutRoot��5       � RowDefinitions 5    
   �� 5!      $��* �6   �� 5"      $
��Auto �6   �� 5#      $
��Auto �6   .+�5&   
   ]� -   5         ]�Name$ ContentArea��5&      $��0��6)   .+�5)      �� $��0��6   ��. $
��Auto=�6@   $
��Auto=�6c   .��5*      ��   MetroTab#���  6R    �� SelectedIndex 6   ��  �� Path$ tabIndex� �� Mode$ TwoWay=�.@�5,      �� $��
Properties��6     MetroTabItem#���  62   
���  ��.��5-      �  5.      �� 5/   "   $��30 �60   �� 50   "   $
��Auto �60   �� 51   "   $��30 �60   �� 52   "   $
��Auto �60   �� 53   "   $��30 �60   �� 54   "   $
��Auto �60   �� 55   "   $��30 �60   �� 56   "   $
��Auto �60   �� 57   "   $��30 �60   �� 58   "   $
��Auto �60   �� 59   "   $��30 �60   �� 5:   "   $
��Auto �60   �� 5;   "   $��30 �60   �� 5<   "   $
��Auto �60   �� 5=   "   $��30 �60   �� 5>   "   $
��Auto �60   	 � ColumnDefinitions	 5@      �� 5A   "   $	��120 �63   �� 5B   "   $��6 �63   �� 5C   "   $	��300 �63   �� 5D   "   $��6 �63   �� 5E   "   $
��auto �63   .+�5I      �� $��1��6$   $��0��61   $��Name:��6A     WorkspaceLabelsStyle#���  6Q   �� 5J      $��1��6&   $��2��63   $
��Left=�6	  $��Center=�6$  $��Center=�6?  $	��300��6a  ��6C   �� $ ContentName�
 �� ValidatesOnDataErrors
 .  �� ValidatesOnExceptions .  �� NotifyOnValidationError .  �� UpdateSourceTrigger$ PropertyChanged=�{�6�   �� *UpdateModeProperty+6a  �� 5M      $��3��6$   $��0��61   $��Description:��6A   #���  6X   �� 5N      $��3��6&   $��2��63   $
��Left=�6  $��Center=�6+  $��Center=�6F  $	��300��6h  $	��0,2q�6t  ��6C   �� $ ContentDescription�
 .  .  . $ PropertyChanged=�{�6�   �� *UpdateModeProperty+6t  �� 5Q      $��5��6$   $��0��61   $��	Category:��6A   #���  6U   �� 5R      $��5��6'   $��2��64   $
��Left=�6D   $��Center=�6_   $��Center=�6z   $	��300��6�    �� DisplayMemberPath$ Value��6e   �� SelectedValuePath$	 Key��6  ��6�   �� *ContentTypesList+{�6�   �� *UpdateModeProperty+ �� SelectedValue 6�   �� *EditContentType+
 . $ PropertyChanged=�6  �� 5U      $��7��6$   $��0��61   $��Certificate free:��6A   #���  6]   �� 5V      $��7��6'   $��2��64   $
��Left=�6  $��Center=�6/  $��Center=�6J   d� 	IsChecked 6D   �� $ CertificateFree�
 .  .  . $ PropertyChanged=�{�6�   �� *UpdateModeProperty+6J  �� 5Y      $��7��6$   $��2��61   $��ATR :��6A   #���  6Q   $��62��6�   $��	0,0,179,0q�6�   �� 5Z      $��7��6'   $��2��64   $��Center=�6  $��Center=�6'  $��	121,7,0,7q�6I  $
��Left=�6\  $��16��6w   6D   �� $ ATRInd�
 .  .  . $ PropertyChanged=�{�6�   �� *UpdateModeProperty+6w  �� 5^      $��9��6$   $��0��61   $��Icon:��6A   #���  6Q   �� 5_      $��9��6&   $��2��63   $
��Left=�6  $��Center=�6'  $��Center=�6B  $	��300��6d  ��6C   �� $ TargetIconPath�
 .  .  . $ PropertyChanged=�{�6�   �� *UpdateModeProperty+6d  �� 5`      -   5        $��
btnAddPath��5`   B   $��9��6%   $��4��62   $	��...��6T   $��30��6�   $
��Left=�6�   ��6c   �� *BrowseIconContent+{�6�   �� *UpdateModeProperty+6�   �� 5d      $��11��6$   $��0��62   $��Created On:��6B   #���  6X   �� 5e      $��11��6&   $��2��64    �� 
IsReadOnly . $
��Left=�6�   $��Center=�6  $��Center=�6*  $	��300��6L  $	��0,2q�6X  ��6D   �� $ CreationDate�
 .  .  . $ PropertyChanged=�6X  �� 5h      $��13��6$   $��0��62   $��Last Update Date:��6B   #���  6^   �� 5i      $��13��6&   $��2��64    . $
��Left=�6�   $��Center=�6  $��Center=�6,  $	��300��6N  $	��0,2q�6Z  ��6D   �� $ LastUpdateTime�
 .  .  . $ PropertyChanged=�6Z  �� 5l      $��15��6$   $��0��62   $��Last Update User:��6B   #���  6^   �� 5m      $��15��6&   $��2��64    . $
��Left=�6�   $��Center=�6  $��Center=�6,  $	��300��6N  $	��0,2q�6Z  ��6D   �� $ LastUpdateUser�
 .  .  . $ PropertyChanged=�6Z  5p      5q      5s      5t      5u      � 5x   
   $��2��6   y�6   �� $ 
ActionMode� �� 	Converter# [  .+�5y      �� $��1��6    �� Orientation$ 
Horizontal=�6&   $��Right=�6?   $��0,0,45,0q�6[   .+�5z      �� $��80��68   $��0,2,10,2q�6C   ��6   �� *
ActionName+��6U   �� *SaveCommand+5|      5}      5      5�      