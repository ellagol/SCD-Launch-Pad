   M S B A M L   `   `   `  ����E  AInfra.MVVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:Infra.MVVM;assembly=Infra.MVVM
Infra.MVVM  5      L HContentMgmtModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:ContentMgmtModuleContentMgmtModule 5      $   ContentMgmtModule.CMSearchView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      =mvvm,clr-namespace:Infra.MVVM;assembly=Infra.MVVM     5      1localclr-namespace:ContentMgmtModule     5      .��5	      � -   5          �Name$  
LayoutRoot��5	      
������  � ColumnDefinitions 5
   
   �� 5      $	��150 �6   �� 5      $��6 �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��20 �6   �� 5      $
��Auto �6   �� 5      $��20 �6   �� 5      $
��Auto �6   �� 5      $��20 �6    � RowDefinitions 5   
   �� 5      $��18 �6   �� 5      $
��Auto �6   �� 5      $��16 �6   �� 5      $
��Auto �6   �� 5      $��16 �6   �� 5      $
��Auto �6   .+�5   
   �� $��1��6   $��0��6   $��Search Term:��6-      WorkspaceLabelsStyle#���   6D   
�������$��14(�6�   �� 5    
   $��1��6   $��2��6   $
��Left=�6�   $��Center=�6�   $��Center=�6  $	��404��6#   �� 	MaxLength$ 30��6/  ��6/   ��  �� Path$ 
SearchTerm� �� ValidatesOnDataErrors .  �� ValidatesOnExceptions .  �� NotifyOnValidationError .  �� UpdateSourceTrigger$ PropertyChanged=�6/  �� 5#   
   $��3��6   $��0��6   $��
Search On:��6-   #���   6B   
�������$��14(�6�   $	��Top=�6�   $
��Left=�6�   � 5%   
   $��3��6   $��2��6    5&      �� 5'      $
��Auto �6#   �� 5(      $��20 �6#   �� 5)      $
��Auto �6#   �� 5*      $��20 �6#   �� 5+      $
��Auto �6#   �� 5,      $	��59* �6#    5.      �� 5/      $
��Auto �6    �� 50      $��14 �6    �� 51      $
��Auto �6    .+�54      �� $��0��6   $��0��6$   $��Folder Name��64   $��Center=�6J   $��	-20,0,0,0q�6�   
�������$��14(�6�   	 d� 	IsChecked	 6e   �� $ SearchOnFolderName�6�   �� 55      $��0��6   $��2��6$   $��Content Name��64   $��Center=�6K   $��40,0,0,0q�6�   
�������$��14(�6�   	 6f   �� $ SearchOnContentName�6�   �� 56      $��0��6   $��4��6$   $��Version Name��64   $��Center=�6K   $��40,0,0,0q�6�   
�������$��14(�6�   	 6f   �� $ SearchOnVersionName�6�   �� 58      $��2��6   $��0��6$   $��	File Name��64   $��Center=�6H   $��	-20,0,0,0q�6�   
�������$��14(�6�   	 6c   �� $ SearchOnFileName�6�   �� 59      $��2��6   $��2��6$   $
��User��64   $��Center=�6C   $��40,0,0,0q�6�   
�������$��14(�6�   {�. 	 6^   �� $ SearchOnUser�6�   5;      �� 5=   
   $��1��6   $��3��6   $��80��6/   $�� Search ��6:   $��Right=�6q   ��6M   �� *CMSearchCommand+6q   �� 5>   
   $��5��6   $��5��6   $��90��6/   $�� Select All ��6:   $��Right=�6�   ��6Q   �� *SelectAllParametersCommand+6�   �� 5?   
   $��5��6   $��7��6   $��90��6/   $�� Clear ��6:   $��Right=�6z   ��6L   �� *ClearAllParametersCommand+6z   5A      5B      