   M S B A M L   `   `   `  ����E  AInfra.MVVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:Infra.MVVM;assembly=Infra.MVVM
Infra.MVVM  5      I EExplorerModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:ExplorerModuleExplorerModule 5         ExplorerModule.SearchView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      =mvvm,clr-namespace:Infra.MVVM;assembly=Infra.MVVM     5      .localclr-namespace:ExplorerModule     5      .��5      � -   5          �Name$  
LayoutRoot��5      
������  � ColumnDefinitions 5	   
   �� 5
      $	��150 �6   �� 5      $��6 �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��20 �6   �� 5      $
��Auto �6   �� 5      $��20 �6   �� 5      $
��Auto �6   �� 5      $��20 �6    � RowDefinitions 5   
   �� 5      $��18 �6   �� 5      $
��Auto �6   �� 5      $��16 �6   �� 5      $
��Auto �6   �� 5      $��16 �6   �� 5      $
��Auto �6   .+�5   
   �� $��1��6   $��0��6   $��Search Term:��6-      WorkspaceLabelsStyle#���   6D   
�������$��14(�6�   �� 5   
   $��1��6   $��2��6   $
��Left=�6�   $��Center=�6�   $��Center=�6  $	��404��6#   �� 	MaxLength$ 30��6/  ��6/   ��  �� Path$ 
SearchTerm� �� ValidatesOnDataErrors .  �� ValidatesOnExceptions .  �� NotifyOnValidationError .  �� UpdateSourceTrigger$ PropertyChanged=�6/  �� 5"   
   $��3��6   $��0��6   $��
Search On:��6-   #���   6B   
�������$��14(�6�   $	��Top=�6�   � 5$   
   $��3��6   $��2��6    5%      �� 5&      $
��Auto �6#   �� 5'      $
��Auto �6#   �� 5(      $
��Auto �6#   �� 5)      $	��59* �6#    5+      �� 5,      $
��Auto �6    �� 5-      $��14 �6    �� 5.      $
��Auto �6    .+�51      �� $��0��6   $��0��6$   $
��Name��64   $��Center=�6C   $��-6,0,0,0q�6�   
�������$��14(�6�   	 d� 	IsChecked	 6^   �� $ SearchOnName�6�   �� 52      $��0��6   $��1��6$   $��Description��64   $��Center=�6J   $��30,0,0,0q�6�   
�������$��14(�6�   	 6e   �� $ SearchOnDescription�6�   �� 53      $��0��6   $��2��6$   $
��User��64   $��Center=�6C   $��30,0,0,0q�6�   
�������$��14(�6�   	 6^   �� $ SearchOnUser�6�   �� 54      $��2��6   $��0��6$   $��Project Code��64   $��Center=�6K   $��-6,0,0,0q�6�   
�������$��14(�6�   	 6f   �� $ SearchOnProject�6�   �� 55      $��2��6   $��1��6$   $��Notes��64   $��Center=�6D   $��30,0,0,0q�6�   
�������$��14(�6�   {�. 	 6_   �� $ SearchOnNotes�6�   �� 56      -   5        $��2��56      $��2��6$   $��Content Name��64   $��Center=�6K   $��30,0,0,0q�6�   
�������$��14(�6�   {�. 	 6f   �� $ SearchOnContent�6�   �� 57      $��3��6   $��Project Step��6'   $��Center=�6>   $��
15,7,-94,7q�6�   
�������$��14(�6�   {�. 	 6Y   �� $ SearchOnStep�6�   59      �� 5;   
   $��1��6   $��3��6   $��80��6/   $�� Search ��6:   $��Right=�6o   ��6M   �� *SearchCommand+6o   �� 5<   
   $��5��6   $��5��6   $��90��6/   $�� Select All ��6:   $��Right=�6�   ��6Q   �� *SelectAllParametersCommand+6�   �� 5=   
   $��5��6   $��7��6   $��90��6/   $�� Clear ��6:   $��Right=�6z   ��6L   �� *ClearAllParametersCommand+6z   5@      5B      