   M S B A M L   `   `   `  ����  ContentManager5    /ContentManager.ProgressBar.View.ProgressBarView   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   $	��190��5   	   $	��385��6     � DataContext  5   	   �� *ProgressBarDataProvider+   Locator �� Source# [  .��5      � 
����d� J� Opacity$	 0.9Q�6'    � RowDefinitions 5   
   �� 5      $��37 �6   �� 5	      $��37 �6   �� 5
      $��30 �6   �� 5      $��* �6   .+�5   
   �� $��0��6r   $
��Left=�6   $��	10,13,0,5q�6�   ��6   ��  �� Path$ ProgressMessageSource� �� Mode$ OneWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5   
   $��1��6w   $
��Left=�6�   $��	10,13,0,5q�6�   ��6   �� $  ProgressMessageDestination�$ OneWay=�$ PropertyChanged=�6�   � 5   
   $��2��6    � ColumnDefinitions 5      �� 5      $
��0.8* �6#   �� 5      $
��0.2* �6#   .+�5      � $��0��6   $
��10,3q�6*   $��30��68   $��Stretch=�6�    � Value 6D   �� $ Progress�$ TwoWay=�$ PropertyChanged=�	 � Maximum	 6�   �� $ ProgressMax�$ TwoWay=�$ PropertyChanged=�6�   �� 5      $��1��6   $��Center=�6�   $��Center=�6�   ��6)   �� $ ProgressMessage�$ OneWay=�$ PropertyChanged=�6�   5      �� 5   
   $��Cancel��6   $��3��6B   $��75��6O   $��Center=�6Z   $��0,5,0,15q�6w   ��6"   �� $ Cancel�6w   5      5      