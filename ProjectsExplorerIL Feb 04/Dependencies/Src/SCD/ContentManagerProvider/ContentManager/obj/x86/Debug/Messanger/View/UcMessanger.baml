   M S B A M L   `   `   `  ����  ContentManager/    )ContentManager.Messanger.View.UcMessanger   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        � DataContext  5      �� *MessangerDataProvider+   Locator �� Source# [  .��5	      �  J� Opacity$	 0.7Q�66   ��6   ��  �� Path$ MessageBgColor� � RowDefinitions 5
   
   �� 5      $
��0.1* �6   �� 5      $
��0.6* �6   �� 5      $
��0.2* �6    � ColumnDefinitions 5   
   �� 5      $
��0.2* �6   �� 5      $
��0.8* �6   .+�5   
   �� $��0��6   $��0��6!   $��2��61   
������$��18(�6�   ��6F   �� $ MessageTitle�6�   �� 5   
   $��0��6   $��1��6    ��6-   �� $ MessageIcon��� 5   
   $��1��6   $��1��6!   $��WrapWithOverflow=�61   $��Center=�6t   $��Center=�6�   $��22(�6�   ��6R   �� $ MessageText�6�   � 5   
   $��2��6   $��0��6   $��2��6,    5      �� 5      $��* �6#   �� 5      $��* �6#   .+�5      �� $��10q�6   $��0��6!   $��OK��61   $ 1Q�6>   ��6J   �� $ ActionExecute�y�6q   �� $ IsAcknowledge� �� Mode$ OneWay=�  BoolToVisibility �� 	Converter# [ �� 5      $��10q�6   $��1��6!   $��Cancel��61   $ 1Q�6B   ��6N   �� $ ActionCancel�y�6t   �� $ IsAcknowledge�$ OneWay=�# [ �� 5      $��10q�6   $��2��6!   $��Close��65   $ 1Q�6E   ��6Q   �� $ ActionCancel�y�6w   �� $ IsAcknowledge�$ OneWay=�# [  �� ConverterParameter$ [Not]��5      5       5!      