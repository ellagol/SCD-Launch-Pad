   M S B A M L   `   `   `  ����E  ATaskDialog, Version=1.7.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:TaskDialogInteropTaskDialogInterop  5      #    TaskDialogInterop.CommandLink   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      1localclr-namespace:TaskDialogInterop     5      -   5          >�Name$  UserControl��5	       J� SnapsToDevicePixels . &   System.Windows.Media.TextOptions   TextFormattingMode$ Display=�5       � 	Resources 5       �� MergedDictionaries �� Source   CmdArr3 �� 	UriSource  CmdArr2  CmdArr1  CmdShld   BooleanToVisibilityConverter7  1TaskDialogInterop.NotBooleanToVisibilityConverter # NotBooleanToVisibilityConverter0  *TaskDialogInterop.CommandLinkTextConverter  CommandLinkTextConverter?  9TaskDialogInterop.CommandLinkExtraTextVisibilityConverter + 'CommandLinkExtraTextVisibilityConverter�� 5       5      �� 5      $&  ResourceDictionaries/Common.xaml?�6   %Z  &	        &	 /     &	 ^     &	 �     &	 �     &	 �     &	 �     &	 �     �� 5      $ Resources/cmdarr3.png?�6!   �� 5      $ Resources/cmdarr2.png?�6!   �� 5      $ Resources/cmdarr1.png?�6!   �� 5      $ Resources/shield-16.png?�6!   �� 5      6"     5      6+     5      6$     5      63   5      .��5      � -   5         �Name$ 
LayoutRoot��5      .+�5       �� -   5         ��Name$ CommandLinkButton��5         NoChromeButton#���  5$        	 CommandButtonControlTemplate#��� 	 5%      ��5!      �� *	Command+	 �� Mode$	 OneWay=���5"      �� *ID+$	 OneWay=�{�5#      �� *	IsEnabled+$	 OneWay=� 5&      
 �� 
TargetType ��Name �� CornerRadius �� Padding �� EndPoint �� 
StartPoint � Color � Offset'  !System.Windows.VisualStateManager   VisualStateGroups%  System.Windows.VisualStateGroup   Transitions%  System.Windows.VisualTransition   GeneratedDuration	   To   From   States   System.Windows.VisualState   
Storyboard �� TargetProperty �� 
TargetName8	  2System.Windows.Media.Animation.EasingColorKeyFrame �� KeyTime �� Value9
  3System.Windows.Media.Animation.EasingDoubleKeyFrame O� KeyTime O� Value �� 
TargetType  �� Property! �� Value" � ColumnDefinitions# � RowDefinitions$ ��Name% �� 	Converter& J� Effect3  -System.Windows.Media.Effects.DropShadowEffect'   	Direction(   
BlurRadius)   ShadowDepth*   Opacity+ J� Opacity, ��Name- �� ConverterParameter. �� Triggers/ P� Property0 P� Value1 �� 
TargetName��%�  &		       7  7  7  7  7  7  7  �� 5'      "
 ��6;   .R�5(      �� $ CommandLinkBorder��6   $��1q�6)   $ 2��6=   $ 9q�6N   
������ ��5)      �� 5*   	   $ 0.5,1&�6   $ 0.5,0&�6.   .��5+   
   � $ White��6   $ 0Q�6%   � 5,   
   $ White��6   $
 0.33Q�6%   � 5-   
   $ White��6   $ 1Q�6%   5.   
    50        51   	   CommonStates�� 52   
     53      $ 0:0:1D�6     54      $ 0:0:0.25D�6   $ Disabled��69     55      $ Disabled��6   $ 0:0:0.25D�6,     56      $ 0:0:0.25D�6   $ Pressed��69     57      $ 0:0:0.5D�6   $ 	MouseOver��68   . 59   
     
Normal��6     5:   
   	MouseOver��. 5;      �� .��5<      �� $6 0(TextElement.Foreground).(SolidColorBrush.Color)�6)   $ CommandLinkText��6v   .g�5=      	  $ 0��6!   $ 	#FF074AE5��6-   5>      �� 5?      $6 0(TextElement.Foreground).(SolidColorBrush.Color)�6)   $ CommandLinkTextExtra��6v   .g�5@      	  $ 0��6!   $ 	#FF074AE5��6-   5A      �� 5B      $2 ,(Border.BorderBrush).(SolidColorBrush.Color)�6)   $ CommandLinkBorder��6r   .g�5C      	  $ 0��6!   $ 	#FFC6C6C6��6-   5D      �� 5E      $N H(Panel.Background).(GradientBrush.GradientStops)[1].(GradientStop.Color)�6)   $ CommandLinkBorder��6�   .g�5F      	  $ 0��6!   $ 
WhiteSmoke��6-   5G      �� 5H      $N H(Panel.Background).(GradientBrush.GradientStops)[2].(GradientStop.Color)�6)   $ CommandLinkBorder��6�   .g�5I      	  $ 0��6!   $ 
WhiteSmoke��6-   5J      U� 5K      $ (UIElement.Opacity)�6*   $ CommandLinkIcon2��6Z   .\�5L      
  $ 0��6"   $ 1Q�6.   5M      5N      5O        5P   
   Pressed��. 5Q      �� .��5R      �� $2 ,(Border.BorderBrush).(SolidColorBrush.Color)�6)   $ CommandLinkBorder��6r   .g�5S      	  $ 0��6!   $ 	#FFADADAD��6-   5T      �� 5U      $N H(Panel.Background).(GradientBrush.GradientStops)[1].(GradientStop.Color)�6)   $ CommandLinkBorder��6�   .g�5V      	  $ 0��6!   $ 	#FFF2F3F3��6-   5W      �� 5X      $N H(Panel.Background).(GradientBrush.GradientStops)[0].(GradientStop.Color)�6)   $ CommandLinkBorder��6�   .g�5Y      	  $ 0��6!   $ 	#FFF2F3F3��6-   5Z      �� 5[      $N H(Panel.Background).(GradientBrush.GradientStops)[2].(GradientStop.Color)�6)   $ CommandLinkBorder��6�   .g�5\      	  $ 0��6!   $ 	#FFF2F3F3��6-   5]      �� 5^      $6 0(TextElement.Foreground).(SolidColorBrush.Color)�6)   $ CommandLinkText��6v   .g�5_      	  $ 0��6!   $ 	#FF062073��6-   5`      �� 5a      $6 0(TextElement.Foreground).(SolidColorBrush.Color)�6)   $ CommandLinkTextExtra��6v   .g�5b      	  $ 0��6!   $ 	#FF062073��6-   5c      5d      5e        5f   
   Disabled��. 5g      �� .��5h      U� $ (UIElement.Opacity)�6*   $ CommandLinkTextExtra��6Z   .\�5i      
  $ 0��6"   $	 0.5Q�6.   5j      U� 5k      $ (UIElement.Opacity)�6*   $ CommandLinkText��6Z   .\�5l      
  $ 0��6"   $	 0.5Q�6.   5m      U� 5n      $ (UIElement.Opacity)�6*   $ CommandLinkIcon4��6Z   .\�5o      
  $ 0��6"   $	 0.5Q�6.   5p      U� 5q      $ (UIElement.Opacity)�6*   $ CommandLinkIcon3��6Z   .\�5r      
  $ 0��6"   $ 1Q�6.   5s      U� 5t      $ (UIElement.Opacity)�6*   $ CommandLinkIcon1��6Z   .\�5u      
  $ 0��6"   $ 0Q�6.   5v      5w      5x      5y   
   ._�5{      � $ CommandLinkLayout��6    5|   	   ���� 5}   
   '��       " ��6   .�5~      ��   � ��$! 0,4,9,0q�6$   �� 5        � ��$! Stretch=�6/   5�      �� 5�   
   '��       " ��6   .�5�      ��   � ��$
! Wrap=�6*   �� 5�        � ��! �	#FF151C556(   5�      5�   
   " 5�   	   �� 5�   
   $
��Auto �6   �� 5�   
   $��1* �6   # 5�   	   �� 5�   
   $
��Auto �6   �� 5�   
   $
��Auto �6   .+�5�   	   �� $$ CommandLinkIcon1��6   #���  6)   y�5�   	   �� *IsElevationRequired+8%   & 5�   
     5�      $	' 190Q�6   $( 5Q�6,   $) 2Q�6;   $	* 0.4Q�6K   5�   
   �� 5�   	   $$ CommandLinkIcon2��6   #���  6)   $+ 0Q�6L   y�5�   	   �� *IsElevationRequired+8%  �� 5�   	   $$ CommandLinkIcon3��6   #���   6)   $+ 0Q�6L   y�5�   	   �� *IsElevationRequired+8%  �� 5�   	   $$ CommandLinkIcon4��6   #���  6)   $��0,4,7,0q�6L   y�5�   	   �� *IsElevationRequired+8%  �� 5�   	   $, CommandLinkText��6   $��1��6,   $��16(�6<   �� 6K   .F�5�      �� ��6   �� *Text+$- 1��8%  $	 OneWay=�5�      6   �� 5�   	   $, CommandLinkTextExtra��6   $��1��61   $��1��6A   $��12(�6�   $��0,1,0,0q�67  ��6N   �� *Text+$- 2��8%  $	 OneWay=�y�6�   �� *Text+8%  $	 OneWay=�67  5�   	   5�      . 5�      P� 5�      / �@J�	IsFocused0 . . �5�   	   �� $1 CommandLinkBorder��6     � ��! �	#FF7FE5FF6F   5�   	   5�      5�      5�      5�      5�      