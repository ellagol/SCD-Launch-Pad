   M S B A M L   `   `   `  ����@  <ATSUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:ATSUI.CustomControlsATSUI.CustomControls  5      E AInfra.MVVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:Infra.MVVM;assembly=Infra.MVVM
Infra.MVVM 5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35T PRibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=nullW SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   CloseButtonWidthAndHeight  X_CloseButton + 'TabItemCloseButtonNormalForegroundBrush  �� Color * &TabItemCloseButtonHoverForegroundBrush , (TabItemCloseButtonPressedForegroundBrush + 'TabItemCloseButtonNormalBackgroundBrush �� 
StartPoint �� EndPoint � Color � Offset ' #TabItemCloseButtonNormalBorderBrush * &TabItemCloseButtonHoverBackgroundBrush , (TabItemCloseButtonPressedBackgroundBrush (	 $TabItemCloseButtonPressedBorderBrush 
 TabItemCloseButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType	 ��Name
 �� CornerRadius F�Name �� StrokeStartLineCap �� StrokeEndLineCap �� Stretch �� Triggers P� Property P� Value �� 
TargetName  WorkspacesTabControl'    !ATSUI.CustomControls.TabControlEx � 	Resources  Key  WorkspacesBackgroundBrush � RowDefinitions � ColumnDefinitions �� Path J� SnapsToDevicePixels'  !System.Windows.VisualStateManager   VisualStateGroups%  System.Windows.VisualStateGroup   Name   States   System.Windows.VisualState   Name   
Storyboard �� 
TargetName �� TargetProperty	 W� To  g� Duration	! �� To  PopupBackgroundBrush" J� Opacity  WindowBackgroundBrush# ��Name  NormalTextForegroundBrush$ ��Name  BackgroundBrush% �� Opacity& K� IsItemsHost' �Name�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      /controls"clr-namespace:ATSUI.CustomControls  5      5mvvm,clr-namespace:Infra.MVVM;assembly=Infra.MVVM  5      .sys&clr-namespace:System;assembly=mscorlib  5      %�  &	        &	      &	 I     &	 j     &	 �     &	 �     &	 T    &	      &	 �    &		 h    &	
     7   7   7  7  7  7  7  7  7  7 	 7  &	 �    X�5	      14Q�67   �5
      M0,0 L10,10 M0,10 L10,0�6>   �� 5      $  #4971A3��6F   �� 5      $  White��6E   �� 5      $  #EEEE��6G   �� 5      $	 0,0&�6J   $	 1,0&�6[   ��5      � � 5      $ #5FFF��6   $	 0.0Q�6-   � 5      $ #5CCC��6   $	 1.0Q�6-   5      5      �� 5      $	 0,0&�6F   $	 1,1&�6W   ��5      � � 5      $ #5CCC��6   $	 0.0Q�6-   � 5      $ #5444��6   $	 1.0Q�6-   5      5       �� 5"      $	 0,1&�6I   $	 1,0&�6Z   ��5$      � � 5%      $ 	#FFDF2B1E��6   $	 0.0Q�61   � 5&      $ 	#FFA41F00��6   $	 1.0Q�61   5'      5)      �� 5+      $	 0,0&�6K   $	 1,1&�6\   ��5-      � � 5.      $ 	#FFA41F00��6   $	 0.0Q�61   � 5/      $ 	#FFDF2B1E��6   $	 1.0Q�61   50      52      �� 54      $	 0,0&�6G   $	 1,1&�6X   ��56      � � 57      $ #5444��6   $	 0.0Q�6-   � 58      $ #5CCC��6   $	 1.0Q�6-   59      5;      �� 5=      " ��6,   .�5D   
   ��  �@J�SnapsToDevicePixels .  60   �� 5E   
    � ��8   6#   �� 5F   
    � ��8  6"   �� 5G   
    �@�Cursor$
 Hand��6#   �� 5H   
    � |� .  6&   �� 5I   
    �@�OverridesDefaultStyle . 62   �� 5J   
    � �� 5K      �� 5L      " ��6"   .R�5M      �� $	 ButtonBorder��6   $
 2��63   $��1q�6D   8�� 8�� ._�5P      � .+�5R      F� $ 
ButtonPath��6#   $��2q�67   $��2��6B   $ Round=�6V   $ Round=�6q   $ Uniform=�6�   8�� 8�� $��Center=�5U      $��Center=�5V      �� 5X      $��Center=�6/   $��Center=�6L   5Y      5Z       5[      P� 5\       �@J�IsMouseOver . . �5]      �� $ ButtonBorder��6%   8   � ��6U   �� 5^      $ 
ButtonPath��6%   8   � ��6O   5_      P� 5`       � {� .  . �5a      ��  � y�$ 	Collapsed=�6;   5b      P� 5c       � �� . . �5d      �� $ ButtonBorder��6%   8   � ��5e   7   �� 5f      $ ButtonBorder��6%   8 	  � ��5g   8   �� 5h      $ 
ButtonPath��6%   8 
  � ��5i   !   �� 5q      $ 
ButtonPath��6%    � ��$ 2.5,2.5,1.5,1.5q�6O   5r      5t      5v      5w      �� 5{      "   63    5|   
   ��_� 5}      ThumbTooltip .��5~      � #���  6    5      �� 5�      $
��Auto �6(    5�      �� 5�      $
��Auto �6+   .+�5�      �� $��0��6   $��0��6)   ��69   �� $ Thumb�5�      5�      �� 5�      '��       " ��6   .�5�      ��  � �� 5�      �� 5�      " ��6*   .R�5�      �  .  5�   "     5�   &   $ CommonStates��67   . 5�   *     $ 	MouseOver��66   . 5�   .   �� .��5�   2   W� $ 
hoverShape��5�   5   $ Opacity�5�   5   $ 1X�5�   5   $  0:0:.1D�5�   5   5�   /   5�   +     5�   *   $ Normal��66   . 5�   .   �� .��5�   2   W� $ 
hoverShape��5�   5   $ Opacity�5�   5   $ 0X�5�   5   $  0:0:.1D�5�   5   5�   /   5�   +   5�   '     5�   &   $ SelectionStates��67   . 5�   *     $ Selected��66   . 5�   .   �� .��5�   2   W� $ buttonShape��5�   5   $ Opacity�6Y   $ 1X�5�   5   $  0:0:.3D�6<   W� 5�   2   $ buttonBackgroundShape��5�   5   $ Opacity�5�   5   $ 1X�6Y   $  0D�6`   W� 5�   2   $ 
hoverShape��5�   5   $ Opacity�6X   $ 0X�5�   5   $  0:0:.1D�6<   �� 5�   2   $ 
buttonText��5�   5   $" (TextBlock.Foreground).Color�5�   5   $! WhiteX�5�   5   $  0:0:.1D�6@   5�   /   5�   +     5�   *   $ 
Unselected��66   . 5�   .   �� .��5�   2   W� $ buttonShape��5�   5   $ Opacity�5�   5   $ 0X�6Y   $  0:0:.1D�6`   W� 5�   2   $ buttonBackgroundShape��5�   5   $ Opacity�5�   5   $ 0X�6Y   $  0D�6`   W� 5�   2   $ 
hoverShape��5�   5   $ Opacity�5�   5   $ 0X�6Y   $  0:0:.1D�6`   5�   /   5�   +   5�   '   .+�5�   "   �� $��
hoverShape��6)   $��40��6;   $��0,0,0,0q�6G    . $��0q�6s   
�������._�5�   &   �� ���	#FFA1B7EA$��0q�6E   #���  6Y   $��40��6�    . 6�   5�   #   �� 5�   "   $��buttonBackgroundShape��6,   $
 Fill=�6I   $" 0Q�6X   #���  6d   $��40��6�    . 6�   �� 5�   "   $��buttonShape��6)   $" 0Q�6<   ���	#FF0343A6$	��0,2q�6`   $��40��6v    . ._�5�   &   F� F�����a� �oр�� �o��r��q
��!ȃ��XC�����XC��[��2���	 . $
 Fill=�5�   D   $��40��6S   #���  6_   5�   '   5�   #   � 5�   "    5�   &   �� 5�   *   $��* �6;   �� 5�   *   $
��Auto �6;   .+�5�   &   �� $# 
buttonText��6G   $��0��67   $��15,0,0,0q�6[   $��Calibri)�6m   $
��12pt(�6�   #���  5�   7   $��Center=�6�   ��6z   �� $ DisplayName�5�   '   �� 5�   &   $$ cmdTabItemCloseButton��6=   $��1��6-   #��� 
 6\   $��5,0,25,0q�5�   -   ��5�   �   �� $ CloseWorkSpaceCommand�5�   -   5�   #   5�       5�      P� 5�   "    �@��
IsSelected . . �5�   &   �� $ cmdTabItemCloseButton��6-    � y�$ Visible=�6f   5�   #   P� 5�   "    �@��
IsSelected .  . �5�   &   �� $ cmdTabItemCloseButton��6-    � y�$ Hidden=�6f   5�   #   P� 5�   "    �@J�IsMouseOver . . �5�   &   �� $ cmdTabItemCloseButton��6-    � y�$ Visible=�6f   5�   #   P� 5�   "    �@J�IsMouseOver .  . �5�   &   �� $ cmdTabItemCloseButton��6-    � y�$ Hidden=�6f   5�   #   5�      5�      5�      5�      .R�5�   
   �  5�      �� 5�      ��6#   �� $ WorkspacesTabItemWidth��� 5      .+�5     �� #���  6   $��0��5     $��0,0,0,0q�5      . ��5     �� 5     $	 0,0&�6*   $	 1,0&�6;   $	% 0.8Q�6J   .��5     � $ 0Q�6'   $ Transparent��62   � 5	     $ 1Q�6'   $ 	LightGray��62   5
     5     �� 5     & . $��0��5     $��0,0,0,0q�5      . 5     � 5     $' PART_SelectedContentHost��6   $��1��65   $��0,0,0,0q�6E   $��Stretch=�6V   $��Stretch=�6t   #���  6�   5     5     5     5     