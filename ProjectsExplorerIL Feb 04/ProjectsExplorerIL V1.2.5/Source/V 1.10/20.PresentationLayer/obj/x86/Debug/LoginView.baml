   M S B A M L   `   `   `  ����a  ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes  5      @ <ATSUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ATSUIATSUI 5      ;"clr-namespace:ATSUI.CustomControlsATSUI.CustomControls 5         ATSUI.LoginView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35T PRibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=nullW SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      oMicrosoft_Windows_ThemesJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero     5      %localclr-namespace:ATSUI     5      3ctls"clr-namespace:ATSUI.CustomControls     5      $��Center=�5      $��Center=�6"   
������ 
�������$	��300��5        � 	Resources  5
          BooleanToVisibilityConverter " WatermarkedPasswordBoxTemplate �� 
TargetType,   &Microsoft.Windows.Themes.ListBoxChrome  Name   BorderBrush   BorderThickness   
Background J� IsMouseOver   RenderMouseOver J� IsKeyboardFocusWithin	   RenderFocused
 J� SnapsToDevicePixels J� Opacity �� Path �� RelativeSource �� 	Converter ��Name �� Triggers P� Property P� Value �� 
TargetName �� Value �� Property  WatermarkedTextBoxTemplate��%  &	        &	      7   &	 �    7   �� 5   
   6'   �� 5   
   " G�6A   .R�5        $ Bd��65   # z��6A   # z��6m   # z��6�   # z 6�   #	 z 6�   
 . ._�5      � .+�5      �� $ .5Q�6�   $��Italic$�6�   $��Center=�6X  ��6    �� $+ %(ctls:WatermarkPasswordBox.Watermark)� �� *TemplatedParent+y�6�   �� *+)(ctls:WatermarkPasswordBox.ShowWatermark)+8    �� *TemplatedParent+6X  �� 5      $ PART_ContentHost��6#   #
 z
 6=   5      5       5      P� 5       � {� .  . �5      �� $ Bd��63   # � ��6C    �@ 
Background�� 5       � ��
 �����63   5      5      �� 5   
   " ��6=   .R�5        $ Bd��65   # z��6A   # z��6m   # z��6�   # z 6�   #	 z 6�   
 . ._�5      � .+�5       �� $ .5Q�6�   $��Italic$�6�   $��Center=�6P  ��6    �� $' !(ctls:WatermarkTextBox.Watermark)� �� *TemplatedParent+y�6�   �� *'%(ctls:WatermarkTextBox.ShowWatermark)+8    �� *TemplatedParent+6P  �� 5!      $ PART_ContentHost��6#   #
 z
 6=   5"      5#       5$      P� 5%       � {� .  . �5&      �� $ Bd��63   # � ��6C    �@ 
Background�� 5(       � ��
 �����63   5)      5+      5,      .��5.      � $��Stretch=�6   $��Stretch=�6)   .+�5/   
   �� 
�������$	 0.5Q�6)   $��Stretch=�6�   $��Stretch=�6�   ��67   �� $ ActualWidth� �� ElementName$ 
LayoutRoot����6r   �� $ ActualHeight�$ 
LayoutRoot��6�   �� 50   
   
����  �$��Center=�6'   $��Stretch=�6B   $��	-5,0,-5,0q�6`   ._�51      � $��Center=�6   $��Center=�6.    � ColumnDefinitions 52      �� 53      $
��Auto �6'   �� 54      $
��Auto �6'   .+�57      �� $��0��6   $	��128��6(   $	��128��64   $��4��6A   $��Center=�6R   $	��Top=�6o   $��	0,20,20,0q�6�   $0��*/Resources/Icons/256x256/AnonymousUser.png��6�   � 59      $��1��6   $��Center=�6'   $��Center=�6D   $	��400��6_    � RowDefinitions 5:      �� 5;      $
��Auto �6(   �� 5<      $
��Auto �6(   �� 5=      $
��Auto �6(   �� 5>      $
��Auto �6(   �� 5?      $
��Auto �6(   �� 5@      $
��Auto �6(   .+�5C      �� $��0��6   $��
User Login��6)     WorkspaceTitleStyle#���  6>   $��	0,10,0,10q�6l   $
��Left=�6   $��Center=�6�   �� 5E      -   5        $��txtLoginName��5E   +   $��1��6   $��Stretch=�6�   $��5,0,0,10q�6�   #���  5F      + %ATSUI.CustomControls.WatermarkTextBox   	Watermark$   User Name��6V   ��5E   ?   �� $ 	LoginName� �� ValidatesOnDataErrors .  �� ValidatesOnExceptions .  �� UpdateSourceTrigger$ PropertyChanged=�5F   V   G� 5H      -   5        $��pwdLoginPassword��5H   /   $��2��6"   .  (ATSUI.CustomControls.BindablePasswordBox   BindPassword . $��Stretch=�6  $��5,0,0,5q�6&  #��[ 5I   "   / )ATSUI.CustomControls.WatermarkPasswordBox   	Watermark$   Access Password��6]      BoundPassword 5H   t   �� $ LoginPassword�  �� Mode$  TwoWay=� . $ PropertyChanged=�5I   ]   �� 5K      -   5        $��chkRememberMe��5K   ,   $��3��6   $��Remember Me��6A   
�������$��0,20,0,0q�6�   ! d� 	IsChecked! 6j   �� $ LoginRememberMe� .  . $ PropertyChanged=�6�   �� 5M      -   5        $��chkRememberMyPassword��5M   ,   $��4��6   $��Remember My Password��6I   
�������! 6{   �� $ LoginRememberMyPassword� . $ PropertyChanged=��� 5O      $��5��6!   " �� Orientation$" 
Horizontal=�6.   $��	0,30,0,30q�6G   $��Right=�6Z   .+�5P      �� -   5        $��btnLogin��5P   !   # �� 	IsDefault# . $	��104��6B   $��Log In��6N   $��0q�6_   ��6j   �� *LoginCommand+�� 5Q      -   5        $��	btnCancel��5Q   !   $	��104��62   $��Cancel��6>   $��20,0,0,0q�6q   ��6O   �� *CancelCommand+6q   5R      5T      5U      5V      5W      5X      