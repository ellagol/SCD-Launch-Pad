   M S B A M L   `   `   `  ����  ContentManager,    &ContentManager.ContentUpdate.UcContent   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      $	��300��5      $	��500��6   $	��100��5      $	��440��6   $	��Top=�5	        � DataContext  5
      �� *ContentsDataProvider+   Locator �� Source# [   � 	Resources 5        EnvTemplate��%}   &	       �� 5   
   .R�5      �� ._�5      �� .+�5      �� ��6    �� *Name+5      5      5      5      .��5      �� $
��Auto=�6   $
��Auto=�68   .��5   
   �  5       �� 
TargetType �� Triggers P� Property P� Value �� Property �� Value	 �� RelativeSource
 �� Path��%�   '��       �� 5      " ��6    5      P� 5       �@=�HasError . . �5      ��  �@�ToolTip 64   �� 	 �� *Self+$)
 #(Validation.Errors)[0].ErrorContent�5      5      5       � ColumnDefinitions 5      �� 5       $	��25* �6   �� 5!      $	��75* �6    � RowDefinitions 5#      �� 5$      $��50 �6   �� 5%      $	��200 �6   �� 5&      $��40 �6   �� 5'      $��40 �6   �� 5(      $��50 �6   �� 5)      $��60 �6   .+�5+      ��   TextBoxFieldName#��[ 6   $��Name:��6B   $��0��6O   $��0��6_   �� 5,        TextBoxFieldValue#��[ 6   $��1��6A   $��0��6Q   {�6^   �� *
UpdateMode+��6   �� *ContentProperty.Name+ �� Mode$ TwoWay=� �� ValidatesOnDataErrors .  �� UpdateSourceTrigger$ PropertyChanged=��� 5.      #��[ 6   $��Description:��6B   $��0��6V   $��1��6f   �� 5/        TextBoxFieldDescription#��[ 6   $��1��6G   $��1��6W   {�6d   �� *
UpdateMode+��6�   �� *ContentProperty.Description+$ TwoWay=� . $ PropertyChanged=��� 51      #��[ 6   $��
Category :��6C   $��0��6U   $��2��6e   �� 52        ComboBoxFieldValue#��[ 6   $��1��6C   $��2��6S   #��[ 6�   {�6`   �� *
UpdateMode+��6�   �� *ContentTypeList+ �� SelectedItem 6�   �� *ContentProperty.Type+ . $ PropertyChanged=��� 54      #��[ 6   $��Certificate free:��6C   $��0��6\   $��3��6l   �� 55        CheckBoxFieldValue#��[ 6   $��1��6C   $��3��6S   {�6`   �� *
UpdateMode+ d� 	IsChecked 6�   �� *!ContentProperty.CertificateFree+�� 57      $��0��6   $��4��6)    �� Orientation$ 
Horizontal=�66   .+�58      �� #��[ 6   $��Icon:��6F   �� 59        ImageFieldValue#��[ 6   ��6B   �� $

 Text� �� ElementName$ TextBoxIcon��5:      � 5;      $��1��6   $��4��6#    5<      �� 5=      $��* �6'   �� 5>      $
��Auto �6'   .+�5@      �� -   5         ��Name$ TextBoxIcon��5@   E   #��[ 6   $��0��6Z   {�6k   �� *
UpdateMode+��6�   �� *ContentProperty.Icon+$ TwoWay=� . $ PropertyChanged=��� 5A        BrowseButton#��[ 6   $��1��6?   $��..��6O   $��Right=�6}   {�6\   �� *
UpdateMode+��6�   �� *BrowseIconContent+5B      � 5C      $��Right=�6   $��0��6/   $��2��6?   $��5��6S   {�6`   �� *UpdateModeActive+y�6�   �� $
 
UpdateMode�$ OneWay=� 	 BoolToVisibility �� 	Converter# [	  5D      �� 5E      $
��Auto �6'   �� 5F      $
��Auto �6'   .+�5H      ��  
 UpdateButton#��[
 6   $��0��6^   $��0��6n   $
��5,10q�6{   ��6?   �� *
ActionName+��6�   �� *ActionExecuteContent+�� 5I      #��[
 6   $��Cancel��6?   $��1��6P   $��0��6`   $
��5,10q�6m   ��6{   �� *CancelExecuteContent+5J      5K      5L      5M      