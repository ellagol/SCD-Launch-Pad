   M S B A M L   `   `   `  ����@  <ATSUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:ATSUI.CustomControlsATSUI.CustomControls  5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35T PRibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=nullW SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35   HeaderTemplate  ]�Name ��Name  MetroCircleButtonStyle.   (System.Windows.Interactivity.Interaction    Triggers/  )System.Windows.Interactivity.EventTrigger   	EventName.  (System.Windows.Interactivity.TriggerBase   Actions.  (ATSUI.CustomControls.SetFlyoutOpenAction   Value8  2System.Windows.Interactivity.TargetedTriggerAction   TargetObject �� RelativeSource �� AncestorType!   ATSUI.CustomControls.Flyout	 j� Stretch
 !� Visual J� Clip � UseLayoutRounding �� Left
 �� Top �� Stretch ��Name �� Triggers �� Value �� Binding �� 
TargetName �� Property �� Value �� ScaleX �� Angle  FlyoutTemplate �� 
TargetType �Name'  !System.Windows.VisualStateManager   VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard g� 	BeginTime �� TargetProperty  �� 
TargetName9	  3System.Windows.Media.Animation.EasingDoubleKeyFrame! O� KeyTime" O� Value# 	  EasingFunction.
  (System.Windows.Media.Animation.CubicEase7  1System.Windows.Media.Animation.EasingFunctionBase$   
EasingMode% U�Name& ��Name'   HeaderTemplate( �� Triggers) �� 
TargetType* �� 	Resources  Key+ �� Color-  'System.Windows.Input.KeyboardNavigation  ButtonNormalBackground, �� EndPoint- �� 
StartPoint. � Color/ � Offset0 A�Name1 J� SnapsToDevicePixels2 P� Property3 P� Value�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      /Controls"clr-namespace:ATSUI.CustomControls  5      Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      %A  &	       &	 O    '  @    7   7  &	     &	 �    �� 5      .R�5   
   ]� $  dpHeader��6   $��10q�5	      $	��Top=�5
      ��. .+�5      �� $	 nav��6   $
��Left=�5      
�������#���  5      $��40��5      $��40��5      $��Segoe UI Symbol)�5      $��16(�5      $	��Top=�5       5        5      $ Click��6%   . 5         .   67   ��  �� "  6�   5      .��5      �� $��20��6   $��15��5      ��5      !� 5      $
	 Fill=�6&   
 5      �� 5   "   $��48��6)   $��48��5    )   4 ��        � l�    � l� l��     l��         .  .+�5#   &   F� $��25��6+   $��18��5$   +   $ 12��5%   +   $ 15��5&   +   $
 Fill=�5'   +   
�������a���� � ��O� � ���ѐ�����@o@@������@���@�4 6n������� �@o@��� ѐ���O� � ��O5)   +   5*   #   5,      5.      5/      �� 50      $ PART_BackButton��51      
�������$��20(�53      $��15,0,0,0q�54      $��Center=�55      ��50      �� 55      56       57   
   �� 58      $
 Left��6~    6   �� *
Position+ �� "  .a�59      �� $	 nav��6    � ��$ Right=�5;      �� 5<      $ PART_BackButton��6    �@��TextAlignment$ Right=�5>      �� 5?      $ PART_BackButton��6    � ��$ 0,0,15,0q�5A      �� 5B      $	 nav��6    �@�LayoutTransform 5D      �� 5E      $ -1Q�6)   5G      5H      �� 5I      $	 Top��6~    6   �� *
Position+ �� "  .a�5J      �� $	 nav��6    �@�LayoutTransform 5L      �� 5M      $	 -90Q�6*   5O      �� 5P      $ dpHeader��6    � ��$ Bottom=�6L   5Q      �� 5R      $ Bottom��6~    6   �� *
Position+ �� "  .a�5S      �� $	 nav��6    �@�LayoutTransform 5U      �� 5V      $ 90Q�6*   5X      5Y      5[      �� 5]      "  5^      .R�5_   
   � $
 root��6   #��z��5`      z�5a      T� 5b       5d        5e      . 5f        Default��6"     5g      Hide��. 5h      �� .��5i      U� $ 00:00:00X�6<   $8 2(UIElement.RenderTransform).(TranslateTransform.X)�5j   <   $
  root��5k   <   .\�5l   "   	  $! 00:00:00.500��67   $" 0Q�5m   7   	hideFrame��# 5o   &   
  5p   *   $$ EaseOut=�64   5r   #   5s      U� 5t      $ 00:00:00X�6<   $8 2(UIElement.RenderTransform).(TranslateTransform.Y)�5u   <   $
  root��5v   <   .\�5w   "   	  $! 00:00:00.500��67   $" 0Q�5x   7   
hideFrameY��# 5z   &   
  5{   *   $$ EaseOut=�64   5}   #   5~      5      5�        5�      Show��. 5�      �� .��5�      U� $% trabslateShow��5�   <   $ 00:00:00X�5�   <   $8 2(UIElement.RenderTransform).(TranslateTransform.X)�5�   <   $
  root��5�   <   .\�5�   "   	  $! 00:00:00.600��67   $" 0Q�5�   7   	showFrame��# 5�   &   
  5�   *   $$ EaseOut=�64   5�   #   5�      U� 5�      $ 00:00:00X�6<   $8 2(UIElement.RenderTransform).(TranslateTransform.Y)�5�   <   $
  root��5�   <   .\�5�   "   	  $! 00:00:00.600��67   $" 0Q�5�   7   
showFrameY��# 5�   &   
  5�   *   $$ EaseOut=�64   5�   #   5�      5�      5�      5�      .+�5�      ]� .+�5�      �� $& PART_Header��6#   $	��Top=�5�   #   $��Header��5�   #   #��z' 5�   #   �� 5�      $& PART_Content��6#   $��Bottom=�5�   #   5�      5�      5�      ( 5�   
   �� 5�      $	 Top��5�       5�      �� *
Position+ �� *Self+.a�5�      �� $ PART_Header��6    � ��$
 Left=�5�      �� 5�      $ PART_Content��6    � ��$ Right=�5�      5�      �� 5�      $ Bottom��5�       5�      �� *
Position+ �� *Self+.a�5�      �� $ PART_Header��6    � ��$
 Left=�5�      �� 5�      $ PART_Content��6    � ��$ Right=�5�      5�      5�      �� 5�      ")  6   * 5�   
   ���� 5�      LabelTextBrush $+ White��5�      5�      .�5�   
   ��  �@ TabNavigation$ Cycle=�5�      �� 5�   
    � ��$ Stretch=�5�      �� 5�   
    � ��$ Stretch=�5�      �� 5�   
    �@ HeaderTemplate8   5�      �� 5�   
    � �� �	#FF2525255�      �� 5�   
    � ��8  5�      �� 5�   
    �@�FocusVisualStyle 5�      Z� 5�      �� 5�      $	, 0,1&�69   $	- 0,0&�6H   .��5�   
   � $. #F3F3F3��6   $/ 0Q�6'   � 5�   
   $. #EBEBEB��6   $	/ 0.5Q�6'   � 5�   
   $. #DDDDDD��6   $	/ 0.5Q�6'   � 5�   
   $. #CDCDCD��6   $/ 1Q�6'   5�      �� 5�      ") ��6+   .�5�   
   ��  � ��# �  6'   �� 5�   
    � �� �	#FF7070706(   �� 5�   
    � ��$ 1q�6,   �� 5�   
    � ��$ Center=�67   �� 5�   
    � ��$ Center=�65   �� 5�   
    � ��$ 1q�6$   �� 5�   
    � �� 5�      �� 5�      " ��6"   .R�5�      � $
 grid��6   ���	#02FFFFFF.+�5�      A� $0 	ellipsebg��6"   $��4q�5�   "   #��z��5�   "   $��0��5�   "   $��Stretch=�5�   "   A� 5�      $0 ellipse��6"   $��4q�5�   "   #��z��5�   "   $��2��5�   "   $��Stretch=�5�   "   �� 5�      #��z��6+   #��z��5�   +   ��. #1 z1 5�   +   #��z��5�   +   5�      ( 5�      P� 5�      2 � ��3 . . �5�      �� $ 	ellipsebg��6%    � ��
 �   �6L   �� 5�      $ 	ellipsebg��6%    �@J�Opacity$	 0.2Q�6O   5�      P� 5�      2 �@J�IsMouseOver3 . . �5�      �� $ ellipse��6%    �@J�Opacity$ 1Q�6M   5�      P� 5�      2 �@J�IsMouseOver3 .  . �5�      �� $ ellipse��6%    �@J�Opacity$ .5Q�6M   5�      P� 5      2 �@J�IsKeyboardFocused3 . 6?   P� 5     2 �@d�	IsChecked$
3 trueY�6D   P� 5     2 � {�3 .  . �5     ��  � �� �#ADADAD6;   5     5     5     5	     5     