   M S B A M L   `   `   `  ����P  LNavigationPane, Version=1.0.5037.19255, Culture=neutral, PublicKeyToken=null/clr-namespace:Stema.ControlsStema.Controls  5      E'clr-namespace:Stema.Controls.ConvertersStema.Controls.Converters  5      -clr-namespace:Stema.WindowsStema.Windows  5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source<    6Stema.Controls.Converters.BooleanToVisibilityConverter    BooleanToVisibilityConverter6   0Stema.Controls.Converters.BooleanNegateConverter  BooleanNegateConverter  NavigationPaneToggleButton �� 
TargetType �� Property �� Value �� 
TargetType ��Name �� Triggers P� Property	 P� Value
 �� 
TargetName �� 
ResourceId �� TypeInTargetAssembly#   Stema.Controls.NavigationPane'   !Stema.Controls.NavigationPaneItem)   #Stema.Controls.NavigationPaneButton  Name   Image   Image   
ImageSmall   
ImageSmall   	ImageType   Anchor   	CheckMode �� HeaderStringFormat �� ContentStringFormat   ButtonToolTipGesture � ToolTip   	IsChecked �� Mode �� RelativeSource   IsContentVisible �� Path �� AncestorType �� 	Converter  ��Name! �� ContentStringFormat" ]� 
Conditions# �� 
SourceName$ �� Property% �� Value& P� 
SourceName(   "Stema.Controls.NavigationPanePanel  ExpanderItemStyle+   %Stema.Controls.NavigationPaneSplitter'  Name+   %Stema.Controls.NavigationPaneExpander  NavigationItemStyle  ExpanderHeaderTemplate # ExpanderPopupResizePreviewStyle( � Cursor  ExpanderItemsHeader) �� DataType6   0Stema.Controls.NavigationPaneExpanderItemsHeader* �� Orientation+ ��Name, J� SnapsToDevicePixels- J� RenderTransformOrigin. �� Angle/ �� Triggers0 �� Binding1 �� Value2 �� AncestorLevel  ResizeThumb	   Stema.Windows.Cursors3 	  SizeWE4 n�Name5 �� Padding6 � RowDefinitions7 �Name8 � AllowsTransparency9 � 	StaysOpen: � HorizontalOffset; � VerticalOffset< � PlacementTarget= �� ElementName> J� Effect3
  -System.Windows.Media.Effects.DropShadowEffect? 
  Opacity@ 
  ShadowDepthA 
  
BlurRadiusB ��Name 	 SizeGripC   
BarToolTipD ��NameE   ItemsHeadersF   ItemsHeadersTemplateG K� IsItemsHostH ��NameI ]�NameJ ��NameK �� ConverterParameterL �� 	ConverterM d�NameN   ButtonToolTipO d� 	IsCheckedP   CloseCommandQ   CloseCommandParameterR   CloseCommandTargetS �� TriggersT �Name 
 DefaultConfigureMenu&    Stema.Controls.ConfigureMenuItemU   MenuTypeV   	Separator " DefaultItemsButtonsContextMenuW  NameX   IsPopupOpenY   IsPopupOpenZ   ExpandToolTip[   ExpandToolTip\   MinimizeToolTip]   MinimizeToolTip^   
BarToolTip_   ActiveHeader`   HasResizeThumba   IsMinimizeButtonVisibleb   IsMinimizeButtonVisiblec   IsMinimizedd �� UpdateSourceTriggere   SelectedItemSubItemsf � 	Resourcesg   IsSplitterEnabledh �� ConverterParameteri   ConfigureButtonToolTipj  Namek   Orientationl   InverseAlignmentm   DisplayType�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      &localclr-namespace:Stema.Controls  5      ;localConverters'clr-namespace:Stema.Controls.Converters  5      ,localWindowsclr-namespace:Stema.Windows  5      .sys&clr-namespace:System;assembly=mscorlib  5      8d2http://schemas.microsoft.com/expression/blend/2008  5      Bmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5        5
      �� 5      $2 ,/NavigationPane;component/Themes/Common.xaml?�6   �� 5      $O I/NavigationPane;component/Themes/NavigationPane/NavigationPaneCommon.xaml?�6   �� 5      $6 0/NavigationPane;component/Resources/Buttons.xaml?�6   %�X  &	        &	      &	 &     '  �    7  &	 u
    &	 &    &	 �    &	 *    &	 �    &	 �    '  �    7  7  7 	 7   7  7  7  7  7  '  �3    &	
 �I    &	 bJ    '  �J    7  7 
 7  7  7   7      5      60     5      6*   �� 5      " d�6,   .�5      ��  � ��
 ���� 6!   �� 5       � �� 5      �� 5      " d�6   .R�5      �� $ border��6   #��z��6   #��z��6R   ._�5      �� 5       5      P� 5        �@J�IsMouseOver	 . . �5!   	   ��  �@�Cursor$
 Hand��5"      �� 5#   	   $
 border��5$       61   C� *�� $ ButtonBackgroundOver��"  + � ��5%   	   P� 5&       � ��	 . . �5'   	   �� $
 border��6&    6:   C� *�� $ ButtonBackgroundSelected��"  + � ��5(   	   5*      5,      �� 5-       � ��$ 0q�6&   5.      �� 51      "  6	   .�52      ��  � ��$ 1q�6&   �� 53       � �� 54      �� 55      "  6   .R�56      ]� .+�57        $ PART_ItemButton��6#   $	��Top=�6<   # z 6Q   # z 6q   $ Large=�58   t   $	 Top=�59   �   #��z��5:      $
��Bold"�68   $ Check=�6�   #��z��6g  #��z��6�  # z 6�  # z 5;       58      �� *
IsSelected+$ TwoWay=� �� *TemplatedParent+��6�   C� *�� $ LargeItemsMargin��"  + 59      �� $ IsMinimized� �� *FindAncestor+$ local:NavigationPaneL�8   ��5:   J   C� *�� $ ForeColorInactive��"  +��6�   C� *�� $ LargeItemsPadding��"  +5;      �� 5<      $  contentPresenter��6   #��z��63   #��z��6g   #! z 6�   $	��Top=�6�   #��z��6�   5=       5>      ]� 5?      " 5@   	   �� 5A   
   $# PART_ItemButton��6   $ �@J�IsMouseOver% . 6I   �� 5B   
   $ �@ 
IsSelected% . 6*   ./�5D   	   �� $
 PART_ItemButton��6&    6C   C� *�� $ ForeColorOver��"  + � ��5E   	   P� 5F      $& PART_ItemButton��64    �@J�IsMouseOver	 . . �5G   	   �� $
 PART_ItemButton��6&    6C   C� *�� $ ForeColorOver��"  + � ��5H   	   ]� 5I      " 5J   	   �� 5K   
   $ �@��
IsSelected% . 63   �� 5L   
   $# PART_ItemButton��69   $ �@J�IsMouseOver% .  ./�5N   	   �� $
 PART_ItemButton��5O       6:   C� *�� $ ForeColorActive��"  + � ��5P   	   P� 5Q       �@ IsInsideNavigationPane	 . . �5R   	   �� $
 contentPresenter��6&    � y�$ 	Collapsed=�6D   5S   	   P� 5T       �@ ItemDisplayType$	 Small=�6E   . �5U   	   �� $
 PART_ItemButton��6,    �@ IsContentVisible .  6I   �� 5V   	   $
 PART_ItemButton��6"    � ��$ -1,0,0,0q�6?   �� 5W   	   $
 PART_ItemButton��6*    � ��$ Right=�6G   �� 5X   	   $
 PART_ItemButton��6%    �@ 	ImageType$ Small=�6B   5Y   	   5[      5]      5^      �� 5`      "  6#   .�5a      ��  � �� 5b      �� 5c      "  6   .R�5d      �� $��2q�6   .+�5e      � $	��4,0q�6   .+�5f   	   �� 5g   	     5h      $' navigationPaneSplitter��6%   $	��Top=�6E   $��0,1,0,0q�6]   5i       5j      P� 5k       �@ 	CanResize	 .  . �5l   	   �� $
 navigationPaneSplitter��6&    � y�$ 	Collapsed=�6J   5m   	   5o      5q      5r      �� 5t      "  6%   .�5u      ��  � �� 5v      �� 5w      "  6   .R�5x      �� 5y      5{      5|      �� 5~      .R�5      �� $
��Bold"�6   $��13.333(�6    $��0,2,0,2q�62   �� 5�      ��6   �� 5�      5�      �� 5�      .�5�      ��  � �� 5�      �� 5�      .R�5�      �� ���	#FF1F1F1F$( SizeWE��6$   5�      5�      5�      �� 5�      ")  6,   .R�5�      �� $* 
Horizontal=�6   $��0,0,16,0q�6(   .+�5�      �� $+ image��6   $��Uniform=�63   $��16��6E   $��16��6P   , . $��0,2,4,2q�6w   $- 0.5,0.5&�6�   ��6   �� *Image+z�5�      V� 5�      .~�5�      �� $	. -90Q�6   5�      5�      �� 5�      , . $��Center=�6K   ��6   �� *	Content+6K   5�      / 5�      �� 5�      0 6   �� *Image+1 6+   Z� .a�5�      �� $
 image��6    � y�$ 	Collapsed=�66   5�      �� 5�      $
1 Left��6�   0 6   �� $ Orientation� �� *FindAncestor+$" local:NavigationPaneExpanderL�$2 1��.a�5�      �� $
 image��6    � z� 5�      V� 5�      .~�5�   	   �� $. 90Q�6   5�   	   5�      5�      5�      �� 5�      " n�6   .�5�      ��  � �� 5�      �� 5�      " n�6   .R�5�      � #��z��6   5�      5�      �� 5�       � ��
 ���� 6!   �� 5�       � ��$ 3��6   �� 5�       �@�Cursor# Z3 6   5�      �� 5�      "  6	   .�5�      ��  �@ PopupResizePreviewStyle8   6.   �� 5�       � �� 5�      �� 5�      "  6   .R�5�      ]� .+�5�   	   n� $4 PART_ResizeThumb��6   8�� 
������ 6N   �� 5�   	   $ border2��6   #��z��6!   #��z��5�   	   #��z��5�   	   #5 z��6=   , . $��Right=�5�   	   ._�5�   
   � 6 5�      �� 5�      $
��Auto �6   �� 5�      $��* �6   .+�5�      � $7 
PART_Popup��6   $
��Fade=�6h   8 . 9 . $��Right=�6  $��2��6  $: -4��6)  $; 2��6?  $��0,0,4,4q�6R  #��z��6�  < 6%   �� $ = PART_MininizedSizeProvider����6�   �� *IsPopupOpen+$ TwoWay=� �� *TemplatedParent+��6c  �� *ActualHeight+$ = PART_MininizedSizeProvider��.��5�      � $��0,0,4,4q�6   > 5�      
  5�      $	? 0.3Q�6   $@ 3Q�6-   $A 4Q�6=   .+�5�      �� $ border3��6   $��1q�6�   ��6%   C� *�� $ ButtonBorder��"  +��6�   C� *�� $ ButtonBorderInnerSelected��"  +._�5�      � .+�5�      �� $) #PART_ContentHostContainerUnexpanded��6�   $5 2q�6   ��6"   C� *�� $ 
Background��"  +��6�   C� *�� $ ButtonBorderInnerSelected��"  +�� 5�      $B 	rectangle��6   8�� $��10��6M   $��10��6X   $��0��6d   $��Right=�6x   $��Bottom=�6�   $y�	Collapsed=�6�   5�      5�      5�      5�        5�      $  PART_MininizedSizeProvider��6&   $��1��6�   $y�Hidden=�6�   $
��Bold"�6�   $
( Hand��6�   $ CheckUncheck=�6�   # zC 6  $��1q�6+  $ Right=�66  $	��Top=�6E  $��Center=�6d  $��0q�6�  $��Right=�6�   6J   �� *IsPopupOpen+$ TwoWay=� �� *TemplatedParent+.��5�      ]� $��Center=�6   .+�5�      �� $ PART_HeaderHostMinimized��6   $��0,0,6,0q�66   �� 5�      $D headers��6   #��zE 6+   #��zF 6X   ��5�      �� 5�      .R�5�      �� G . $* 
Horizontal=�6.   5�      5�      5�      5�      �� 5�      $' !PART_ContentHostContainerExpanded��6   $��1��6=   ._�5�      �� $H PART_ContentHost��6   5�      �� 5�      $ border��6   ��6"   C� *�� $ TitleBackground��"  +��6�   C� *�� $ ButtonBorder��"  +��6*  C� *�� $ TitleBorder��"  +._�5�      �� $ border1��6   ��6$   C� *�� $ TitleInnerBrush��"  +��6�   C� *�� $ TitleInnerRight��"  +._�5�      ]� $I 	dockPanel��6   .+�5�      �� $J buttons��6   $* 
Horizontal=�6*   $	��Top=�6C   y�5�      �� *IsMinimizeButtonVisible+$K inverse��8L  .+�5�      d� $M 	ExpandBtn��6   $��0q�6�   $��16��6�   # zN 6�   8�� $��16��6  $	��Top=�6  O 6/   �� *IsMinimized+$ TwoWay=� �� *TemplatedParent+.��5�      �� $B arrow��6   , . ��6)   C� *�� $ 
ArrowRight��"  +6�   5�      d� 5�      $M PART_CloseButton��6   $��Right=�66   $��16��6R   $��16��6]   8�� $��2,0,0,0q�6�   $y�	Collapsed=�6�   #��zP 6�   #��zQ 6�   #��zR 6)  .��5�      �� $B close��6   , . ��6)   C� *�� $ Close��"  +6�   5�      5�      �� 5�      $ PART_HeaderHost��6   $��10,0,0,0q�6.   ._�5�      �� $  contentPresenter��6    $��Header��6:   $y�	Collapsed=�6Q   5�      5�      5�      5�      5�      5�   
   5�   	    5�      P� 5�      $& 	ExpandBtn��64    �@J�IsMouseOver	 . . �5�   	   �� $
 arrow��6     63   C� *�� $ ArrowRightOver��"  + � ��5�   	   ]� 5�      " 5�   	   �� 5�   
   $# 	ExpandBtn��6+   $ �@J�IsMouseOver% . 6B   �� 5�   
   $ �@ Orientation$% Right=�6+   ./�5   	   �� $
 arrow��6     63   C� *�� $ ArrowLeftOver��"  + � ��5  	   P� 5      �@ HasResizeThumb	 .  . �5  	   �� $
 PART_ResizeThumb��6&    � y�$ 	Collapsed=�6D   5  	   P� 5      �@ IsInsideNavigationPane	 . . �5  	   �� $ 
 PART_MininizedSizeProvider��6"    � ��$ 1,6,1,1q�6J   �� 5  	   $
 
PART_Popup��6"    � ��$
 Auto��6:   �� 5  	   $ 
 PART_MininizedSizeProvider��64    � ��$ Center=�6\   �� 5	  	   $
 border2��6+    � ��$ 0q�6@   �� 5
  	   $
 border3��6+    � ��$ 0,1,1,1q�6@   �� 5  	   $
 border3��6#    �@��Padding$ 3,0,3,3q�68   �� 5  	   $)
 #PART_ContentHostContainerUnexpanded��6+    � ��$ 0,0,0,1q�6\   �� 5  	   $
 	rectangle��6&    � y�$ Visible=�6=   �� 5  	   $
 	rectangle��6"    � ��$ 0,0,0,1q�69   5  	   P� 5      �@ IsHeaderVisible	 . . �5  	   �� $
 contentPresenter��5      � y�$ Visible=�6=   5  	   P� 5      �@ Orientation$
	 Left=�6'   . �5  	   �� $
 
PART_Popup��6%    � ��$
 Left=�6=   �� 5  	   $
 arrow��6     63   C� *�� $ 
ArrowRight��"  + � ���� 5  	   $ 
 PART_MininizedSizeProvider��6"    �@ Anchor$
 Left=�6J   �� 5  	   $
 buttons��6*    � ��$ Right=�6?   �� 5  	   $
 border1��6+    6@   C� *�� $ TitleInnerLeft��"  + � ���� 5  	   $
 
PART_Popup��6,    �@�HorizontalOffset$ 4��6D   �� 5  	   $
 headers��6*    � ��$
 Left=�6?   �� 5  	   $
 PART_ResizeThumb��6*    � ��$ Right=�6H   5  	   P� 5      �@ IsMinimized	 . . �5  	   �� $ 
 PART_MininizedSizeProvider��6&    � y�$ Visible=�6N   �� 5   	   $
 PART_HeaderHost��6&    � y�$ 	Collapsed=�6C   �� 5!  	   $
 	dockPanel��6/    � ��$ Center=�6F   5"  	   ]� 5#     " 5$  	   �� 5%  
   $ �@ IsMinimized% .  6+   �� 5&  
   $ �@ Orientation$
% Left=�6+   ./�5(  	   �� $
 arrow��6     63   C� *�� $ 	ArrowLeft��"  + � ��5)  	   ]� 5*     " 5+  	   �� 5,  
   $ �@ IsMinimized% .  6+   �� 5-  
   $ �@ Orientation$
% Left=�6+   �� 5.  
   $# 	ExpandBtn��6   $ �@J�IsMouseOver% . 6B   ./�50  	   �� $
 arrow��6     63   C� *�� $ ArrowLeftOver��"  + � ��51  	   ]� 52     " 53  	   �� 54  
   $ �@ IsMinimized% . 6+   �� 55  
   $ �@ Orientation$
% Left=�6+   �� 56  
   $# 	ExpandBtn��6   $ �@J�IsMouseOver% . 6B   ./�58  	   �� $
 arrow��6     63   C� *�� $ ArrowRightOver��"  + � ��59  	   ]� 5:     " 5;  	   �� 5<  
   $ �@ IsMinimized% . 6+   �� 5=  
   $ �@ Orientation$% Right=�6+   ./�5?  	   �� $
 arrow��6     63   C� *�� $ 	ArrowLeft��"  + � ��5@  	   ]� 5A     " 5B  	   �� 5C  
   $ �@ IsMinimized% . 6+   �� 5D  
   $ �@ Orientation$% Right=�6+   �� 5E  
   $# 	ExpandBtn��6   $ �@J�IsMouseOver% . 6B   ./�5G  	   �� $
 arrow��6     63   C� *�� $ ArrowLeftOver��"  + � ��5H  	   ]� 5I     " 5J  	   �� 5K  
   $ �@ IsMinimized% .  6+   �� 5L  
   $ �@ Orientation$% Right=�6+   �� 5M  
   $# 	ExpandBtn��6   $ �@J�IsMouseOver% . 6B   ./�5O  	   �� $
 arrow��6     63   C� *�� $ ArrowRightOver��"  + � ��5P  	   ]� 5Q     " 5R  	   �� 5S  
   $ �@ IsCloseButtonVisible% . 64   �� 5T  
   $ �@ IsMinimized% .  6+   ./�5V  	   �� $
 PART_CloseButton��5W      � y�$ Visible=�6=   �� 5X  	   $
 PART_CloseButton��5Y      � ��$ 6,0,0,0q�69   �� 5Z  	   $
 buttons��6*    � ��$ Right=�6?   5[  	   ]� 5\     " 5]  	   �� 5^  
   $ �@ IsMinimized% . 6+   �� 5_  
   $ �@ IsInsideNavigationPane% . 66   ./�5a  	   �� $
 
PART_Popup��6,    �@�HorizontalOffset$ 0��6D   �� 5b  	   $
 
PART_Popup��6*    �@�VerticalOffset$ 0��6B   �� 5c  	   $
 contentPresenter��6&    � y�$ Visible=�6D   5d  	   P� 5e     $& PART_CloseButton��64    �@J�IsMouseOver	 . . �5f  	   �� $
 close��6     63   C� *�� $ 	CloseOver��"  + � ��5g  	   5i     5k     �� 5l      � �� 6!   C� *�� $ 
Background��"  +�� 5m      � �� 6!   C� *�� $ ForeColorActive��"  +�� 5n      � �� 6&   C� *�� $ BorderThickness��"  +�� 5o      � �� 6"   C� *�� $ ButtonBorder��"  +�� 5p      �@ IsHeaderVisible 6&   C� *�� $ IsHeaderVisible��"  +�� 5q      �@ IsCloseButtonVisible 6+   C� *�� $ IsCloseButtonVisible��"  +�� 5r      � ��8  6%   �� 5s      � ��8  6)   �� 5t      �@ ItemsHeadersTemplate8  6+   5u     �� 5w     "  6	   S 5x     P� 5y      �@J�IsMouseOver	 . . �5z     ��  � �� 5{     C� *�� $ ForeColorOver��"  +5|     .�5~     ��  � �� 6&   C� *�� $ ButtonBorderThickness��"  +�� 5      � �� 5�     �� 5�     "  6   .R�5�     �� $ border��6   #��z��6   #, z, 6�  5 6R   C� *�� $  ButtonBorderInnerThickness��"  +��6�   C� *�� $ ButtonBackground��"  +��6e  C� *�� $ ButtonBorder��"  +._�5�     � $
T grid��6   .+�5�  	   �� $B shine��6   $y�Hidden=�6�   ��6"   C� *�� $ ButtonShineBrush��"  +6�   ]� 5�  	   $I 	dockPanel��6   #��z��6&   .+�5�  
   �� $+ image��6   #��z 6   , . $��UniformToFill=�6[   $y�	Collapsed=�6s   $��24��6�   $��24��6�   $��0,0,2,0q�6�   �� 5�  
   $+ 
imageSmall��6   $��16��6$   #��z 60   , . $��16��6q   $��UniformToFill=�6|   $y�	Collapsed=�6�   $��0,0,2,0q�6�   � 5�  
   $T headerContent��6   #��z��6&   #��z��6e   .+�5�     �� $  contentPresenter��6   #��z��67   #��z��6k   #! z 6�   5�     5�  
   5�  	   5�      5�     P� 5�      �@ DisplayState$	 Normal=�6(   . �5�  	   �� $
 border��6'    6;   C� *�� $ ButtonBorderInactive��"  + � ��5�  	   P� 5�      �@ DisplayState$	 Pressed=�6(   . �5�  	   �� $
 border��6&    6:   C� *�� $ ButtonBorderInnerPressed��"  + � ���� 5�  	   $

 grid��6&    68   C� *�� $ ButtonBackgroundPressed��"  + � ��5�  	   P� 5�      �@ DisplayState$	 Checked=�6(   . �5�  	   �� $
 border��6&    6:   C� *�� $ ButtonBorderInnerSelected��"  + � ���� 5�  	   $

 grid��6&    68   C� *�� $ ButtonBackgroundSelected��"  + � ���� 5�  	   $
 border��6"    66   C� *�� $ ButtonShadow��"  + �@J�Effect�� 5�  	   $
 shine��6&    � y�$ Visible=�69   5�  	   P� 5�      �@ DisplayState$
	 Over=�6(   . �5�  	   �� $
 border��5�      63   C� *�� $ ButtonBorderInnerOver��"  + � ���� 5�  	   $

 grid��5�      61   C� *�� $ ButtonBackgroundOver��"  + � ���� 5�  	   $
 shine��6&    � y�$ Visible=�69   �� 5�  	   $
 border��6"    66   C� *�� $ ButtonShadow��"  + �@J�Effect5�  	   ]� 5�     " 5�  	   �� 5�  
   $ �@ DisplayState$% Normal=�6,   �� 5�  
   $ �@ 
IsVertical% . 6*   ./�5�  	   �� $

 grid��6&    68   C� *�� $ ButtonBackgroundVer��"  + � ��5�  	   ]� 5�     " 5�  	   �� 5�  
   $ �@ DisplayState$% Pressed=�6,   �� 5�  
   $ �@ 
IsVertical% . 6*   ./�5�  	   �� $

 grid��5�      6/   C� *�� $  ButtonBackgroundPressedVer��"  + � ��5�  	   ]� 5�     " 5�  	   �� 5�  
   $ �@ DisplayState$
% Over=�6,   �� 5�  
   $ �@ 
IsVertical% . 6*   ./�5�  	   �� $

 grid��6&    68   C� *�� $ ButtonBackgroundOverVer��"  + � ���� 5�  	   $
 shine��6&    � y�$ Hidden=�69   5�  	   ]� 5�     " 5�  	   �� 5�  
   $ �@ DisplayState$% Checked=�6,   �� 5�  
   $ �@ 
IsVertical% . 6*   ./�5�  	   �� $

 grid��6&    68   C� *�� $! ButtonBackgroundSelectedVer��"  + � ���� 5�  	   $
 shine��6&    � y�$ Hidden=�69   5�  	   P� 5�      �@ Anchor$
	 Left=�6"   . �5�  	   �� $
 contentPresenter��6+    �@�LayoutTransform 5�  
   V� 5�     .~�5�     �� $	. -90Q�6   5�     5�  
   �� 5�  	   $
 image��6*    � ��$ Bottom=�6=   �� 5�  	   $
 
imageSmall��6*    � ��$ Bottom=�6B   �� 5�  	   $
 image��6"    � ��$ 0,2,0,0q�65   �� 5�  	   $
 
imageSmall��6"    � ��$ 0,2,0,0q�6:   �� 5�  	   $
 	dockPanel��6/    � ��$ Center=�6F   5�  	   P� 5�      �@ Anchor$	 Right=�6"   . �5�  	   �� $
 contentPresenter��6+    �@�LayoutTransform 5�  
   V� 5�     .~�5�     �� $. 90Q�6   5�     5�  
   �� 5�  	   $
 
imageSmall��6*    � ��$	 Top=�6B   �� 5�  	   $
 image��6*    � ��$	 Top=�6=   �� 5�  	   $
 
imageSmall��6"    � ��$ 0,0,0,2q�6:   �� 5�  	   $
 image��6"    � ��$ 0,0,0,2q�65   �� 5�  	   $
 	dockPanel��6/    � ��$ Center=�6F   5�  	   P� 5�      �@ 	ImageType$	 Small=�6%   . �5�  	   �� $
 
imageSmall��6&    � y�$ Visible=�6>   �� 5�  	   $
 border��6+    6?   C� *�� $  ButtonSmallBorderThickness��"  + � ��5�  	   P� 5�      �@ 	ImageType$	 Large=�6%   . �5�  	   �� $
 image��6&    � y�$ Visible=�69   5�  	   P� 5�      �@ IsContentVisible	 .  . �5�  	   �� $
 headerContent��6&    � y�$ 	Collapsed=�6A   �� 5�  	   $
 image��6"    � ��$ 0q�65   �� 5�  	   $
 
imageSmall��6"    � ��$	 3,0q�6:   �� 5�  	   $
 	dockPanel��6"    � ��$ 0q�69   �� 5�  	   $
 	dockPanel��6/    � ��$ Center=�6F   5�  	   P� 5�      �@ 	ImageType$	 	Undefined=�6%   . �5�  	   �� $
 border��6+    6?   C� *�� $  ButtonSmallBorderThickness��"  + � ��5�  	   5�     5�     �� 5�      � ��$	 2,0q�6   �� 5�      � ��$ Center=�6/   �� 5�      � ��$
 Left=�61   �� 5�      �@ Anchor$	 Top=�6   �� 5�      � �� 6!   C� *�� $ ForeColorActive��"  +�� 5�      �@J�SnapsToDevicePixels . 6*   5�     �� 5�     .@�5�       $U MoreButtons=�6     5�     $U FewerButtons=�6     5�     $U Options=�6     5�     $U 	AddRemove=�6     5      $U HiddenItems=�6   $	V Top=�63   5     �� 5     .@�5       $U 	NewWindow=�6     5     $U Options=�6   5     �� 5     "  6	   .�5     ��  �@ ItemsButtonsContextMenu8   6.   �� 5      �@ ConfigureMenu8  6$   �� 5	      � |� .  6    �� 5
      � �� 5     �� 5     "  6   .R�5     ]� .+�5     n� $4 PART_ResizeThumb��6   $��Right=�6(   8�� 6?   �� 5     $ border��6   #��z��6   #��z��6S   #��z��6   ._�5  	   � 6 5  
   �� 5     �� 5     $
��Auto �6   �� 5     $
��Auto �6   �� 5     $
��Auto �6   �� 5     $
��Auto �6   .+�5  
     $W PART_ContentHost��6'   #Y zX 5  
   
������ #��z��6O   #��z��5  
   $��0��6-   #[ zZ 6:   #] z\ 5  
   #C z^ 6>   8�� #��z_ 5  D   ` .  #b za 6�   c 5  
   �� *IsMinimized+$ TwoWay=� �� *TemplatedParent+$d PropertyChanged=���5     �� 5     .R�5      ]� G . ��. 6*   5!     .@�5#     �� $��SelectedContent��6   5$     �� 5%  
   $D selectedItemsSubItems��6   $��1��67   $y�	Collapsed=�6D   #��ze 6[   f 5&     ���� 5'     '        "  6   .�5(     ��  �@ Anchor$
 Left=�6&   �� 5)      � ��$ 2q�6&   �� 5*      � ��$ 1,4,0,0q�6'   �� 5+      �@ 	ImageType$ Small=�6)   5,     5-     5.       5/  
   $��2��6'   $��1q�67  #{�zg 50  �   ��5/  4   C� *�� $ 
Background��"  +��6�   C� *�� $ ButtonBorder��"  +y�50  '   �� *HasSplitter+8   �� *TemplatedParent+$h inverse��6�     51  
   $��3��6$   #��z��61   ]� 52  
   $��25��6   $��4��6    #��z��6-   ��6P   C� *�� $ ButtonBackground��"  +y�53     �� *IsSmallItemsVisible+8   �� *TemplatedParent+$h inverse��.+�55       $ PART_ConfigureMenuButton��6&   $��1��6H   # zi 6X   $��-1,0,0,0q�56     $��Right=�6   $��15��69   $��Right=�6D   $��Center=�6[   .��57     �� $B 	rectangle��6   $��6��6�   $��3��6�   ��6)   C� *�� $ 	ArrowDown��"  +6�   58       59     $j 
SmallItems��6%   $k 
Horizontal=�69   l . $m Small=�6j   $��16��6~   5:     �� 5;  
   $$ PART_PaneMininizedSizeProvider��6   $
��Left=�69   $��5��6T   $��Bottom=�6e   
���   �#5 z��6�   $y�Hidden=�6�   ._�5<       $ Large=�6&    .  $
��Auto��6Q   $
��Auto��6_   $��Center=�6l   $��Center=�6�   $ Check=�6�    . 6�   5=     5>  
   5?  	   5@      5A     P� 5B      �@ HasResizeThumb	 .  . �5C  	   �� $
 PART_ResizeThumb��6&    � y�$ 	Collapsed=�6D   5D  	   P� 5E      �@ IsMinimized	 . . �5F  	   �� $
 
SmallItems��6&    � y�$ 	Collapsed=�6>   �� 5G  	   $
 selectedItemsSubItems��6&    � y�$ Visible=�6I   5H  	   P� 5I     $& PART_ConfigureMenuButton��64    �@J�IsMouseOver	 . . �5J  	   �� $
 	rectangle��6     67   C� *�� $ ArrowDownOver��"  + � ��5K  	   5M     5O     �� 5P      � �� 6!   C� *�� $ 
Background��"  +�� 5Q      � �� 6!   C� *�� $ ForeColorActive��"  +�� 5R      � �� 6&   C� *�� $ BorderThickness��"  +�� 5S      � �� 6"   C� *�� $ ButtonBorder��"  +�� 5T      � �� 6   C� *�� $ Padding��"  +�� 5U      �@ IsMinimized .  6"   5V     5X     