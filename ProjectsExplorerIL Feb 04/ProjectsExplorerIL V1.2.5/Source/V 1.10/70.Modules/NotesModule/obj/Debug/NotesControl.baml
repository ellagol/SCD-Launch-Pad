   M S B A M L   `   `   `  ����E  AInfra.MVVM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:Infra.MVVM;assembly=Infra.MVVM
Infra.MVVM  5      F BNotesModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:NotesModuleNotesModule 5         NotesModule.NotesControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      =mvvm,clr-namespace:Infra.MVVM;assembly=Infra.MVVM     5      +localclr-namespace:NotesModule     5      $	��480��5       $	��265��6-   
�����F�  � 	Resources  5
       �� MergedDictionaries �� Source+  %NotesModule.BoolToVisibilityConverter   VisibilityConverter  ListBoxTemplate 9�Name �� Orientation �� StringFormat �� Path �� Triggers �� Binding	 �� Value
 �� Property �� Value �� Padding � RowDefinitions �� RelativeSource �� AncestorType  BackgroundC1  BackgroundC2  BackgroundOfficeBrush �� 
StartPoint �� EndPoint � Color � Offset  LabelsStyle �� 
TargetType�� 5   
    5      �� 5      $I CResources/Themes/CoreControls/MetroLight/MetroLight.Styles.WPF.xaml?�6%   �� 5      $N HResources/Themes/CoreControls/MetroLight/MetroLight.MSControls.Core.xaml?�6%   �� 5      $G AResources/Themes/CoreControls/MetroLight/MetroLight.ListView.xaml?�6%   �� 5      $E ?Resources/Themes/CoreControls/MetroLight/MetroLight.Slider.xaml?�6%   %�  &	        &	      &	 r    &	 �    &	 �    &	 <      5      6.   �� 5      .R�5      �� 
������ $��0q�63   $��2q�6G   ._�5      9� $ Expander��6   
������ ��5      �� 5      
������ ._�5   "   �� $ 
Horizontal=�6-   .+�5   &   �� $��DemiBold"�60   ��5   *   `� 5    .   $ 	{0} - {1}��6;   .1�5!   2   �� $ 	NoteTitle�6:   �� 5"   2   $ Description�6:   5#   /   ��5%   *   �� 5&   .    5'   2   �� 5(   6    6B   �� *
SpecialInd+	 True6a   .a�5)   :   �� 
 � ��
 �  ��6a   5*   7   �� 5+   6    6B   �� *
SpecialInd+		 False6a   .a�5,   :   �� 
 � ��
 �   �6a   5-   7   �� 5.   6    6B   �� *
NoteStatus+	 D6a   .a�5/   :   �� 
 � ��
 �����6a   50   7   52   /   54   '   55   #   56      .��58      �� $��0q�6!   
������ $ 2q�6O   ._�59      �  5:   "   �� 5;   &   $
��Auto �64   �� 5<   &   $��0 �64   �� 5=   &   $
��Auto �64   �� 5>   &   $��4 �64   �� 5?   &   $
��Auto �64   �� 5@   &   $��4 �64   �� 5A   &   $
��Auto �64   .+�5C   "   �� $��0��6-   $ 
Horizontal=�6:   $��	-4,0,0,-8q�6S   .+�5D   &   �� $��Created By:��6,   $��10(�6B   $��Normal"�6P   $
��Left=�6d   �� 5E   &   $��10(�6O   $
��Bold"�6]   $
��Left=�6o   $��-6,0,0,0q�6�   ��6,   �� $ 	CreatedBy�6�   5F   #   �� 5G   "   $��2��6-   $ 
Horizontal=�6:   $��-4,0,0,0q�6S   .+�5H   &   �� $	��On:��6,   $��10(�6:   $��Normal"�6H   $
��Left=�6\   $	��Top=�6~   �� 5I   &   $��10(�6R   $
��Bold"�6`   $
��Left=�6r   $	��Top=�6�   $��-6,0,0,0q�6�   ��6,   �� $ CreationDate�6�   5J   #   �� 5K   "   $��4��6,   $
��Wrap=�6X   ��69   �� $ NoteText�6X   �� 5L   "   $��6��6-   $ 
Horizontal=�6:   $��Right=�6S   .+�5M   &   �� $
��Edit��6-   $��80��6<   $��0,2,10,2q�6G   ��5N   -   ��  �� *FindAncestor+" ��$! DataContext.EditNoteCommand���5O   -   �� �� 5P   &   $��80��6\   $��0,2,2,2q�6g   ��6-   �� $ NoteStatusDescription���5Q   -   ��  �� *FindAncestor+" ��$" DataContext.EnDisNoteCommand���5R   -   �� 5S   #   ��5U   "   �� 5V   &    5W   *   �� 5X   .    6:   �� *
NoteType+	 W6W   .a�5Y   2   �� 
 � ��
 �����6T   5Z   /   �� 5[   .    6:   �� *
NoteType+	 C6W   .a�5\   2   �� 
 � ��
 �����6T   5]   /   �� 5^   .    6:   �� *NoteStatusText+	 D6]   .a�5_   2   �� 
 � ��
 �����6T   5`   /   5b   '   5d      5e      5f      ��5h      �� 5i       5j      �� 5k   "    6.   �� *
NoteType+	 W6K   .a�5l   &   �� 
 � ��
 �����6J   5m   #   �� 5n   "    6.   �� *
NoteType+	 C6K   .a�5o   &   �� 
 � ��
 �����6J   5p   #   �� 5q   "    6.   �� *NoteStatusText+	 D6Q   .a�5r   &   �� 
 � ��
 �����6J   5s   #   5u      5w      5x      ��5�      	#FFCFDDEE��64   ��5�      	#FFC2D4E8��64   �� 5�      $ 0,0.5&�6@   $	 0,1&�6S   .��5�      � # �  6   $ 0Q�6F   � 5�      # �  6   $ 1Q�6F   5�      �� 5�      " ��6(   .�5�      �� 
 � ��$ Segoe UI)�6/   �� 5�      
 � ��$ 14(�6-   �� 5�      
 � ��$ Normal"�6/   �� 5�      
 � ��
 �   �6/   �� 5�      
 � ��$
 Left=�68   �� 5�      
 � ��$ Bottom=�66   �� 5�      
 � ��$ Bottom=�6=   5�      5�      .��5�      � -   5         �Name$ 	NotesRoot��5�      
�����F�$��6q�65    5�   
   �� 5�      $��* �6   �� 5�      $
��Auto �6   .+�5�   
   � $��0��6   y�6   �� *IsListVisible+ �� 	Converter# [   �� ConverterParameter$
 True����6�   �� $ ActualWidth� �� ElementName$ 	NotesRoot����6�   �� $ ActualHeight�$ 	NotesRoot�� 5�      �� 5�      $
��Auto �6    �� 5�      $��* �6    .+�5�      �� $��0��6   $��90��6#   $��Add Note��6.   $��Right=�6d   $��0,0,0,6q�6�   ��6A   �� *AddNoteCommand+6�   �� 5�      -   5         ��Name$ ExpanderListBox��5�   #   $��1��6   #��[ 6<    �� AlternationCount$ 2��6p   #��[ 5�   
     ListBoxStyle#��[ 5�      ��. $��Disabled=�5�   
   ��5�      �� $ Notes�5�   
   5�      � 5�   
   $��1��6   #��[ 5�      y�5�      �� *IsEditVisible+# [  $
 True����5�      �� $ ActualWidth�$ 	NotesRoot����6I   �� $ ActualHeight�$ 	NotesRoot�� 5�      �� 5�      $��* �6    �� 5�      $
��Auto �6    .+�5�      � $��0��6    5�      �� 5�      $��30 �6$   �� 5�      $
��Auto �6$   �� 5�      $
��Auto �6$   �� 5�      $
��Auto �6$   �� 5�      $
��Auto �6$   .+�5�      �� $��1��6   $ Vertical=�6*   $��6,6,6,0q�6A   .+�5�      �� $��Title:��6   #��[ 6-   $��-4,0,0,0q�6R   �� 5�      $��Stretch=�6�   $��60��6�   ��6   �� $ EditNoteTitle� �� ValidatesOnDataErrors .  �� ValidatesOnExceptions .  �� NotifyOnValidationError .  �� UpdateSourceTrigger$ PropertyChanged=�6�   5�      �� 5�      $��2��6   $ Vertical=�6*   $��6,6,6,0q�6A   .+�5�      �� $��Type:��6   #��[ 6,   $��-4,0,0,0q�6Q   �� 5�      $��Stretch=�6    �� DisplayMemberPath$	 Key��5�        �� SelectedValuePath$  Value��67   ! �� SelectedValue! 5�   =   �� *EditNoteType+ . " �� Mode$" TwoWay=���6�   �� *NoteTypesList+5�   7   5�      �� 5�      $��3��6   $��Special��6(   $��0,12,0,0q�6_   # d� 	IsChecked# 6:   �� *EditSpecialInd+6_   �� 5�      $��4��6   $ Vertical=�6*   $��6,0,6,0q�6A   .+�5�      �� $��Text:��6   #��[ 6,   $��-4,0,0,0q�6Q   �� 5�      $��Stretch=�6�   $ �� TextWrapping$
$ Wrap=�6�   $	��170��6�   ��6   �� $ EditNoteText� .  .  . $ PropertyChanged=�6�   5�      5�      �� 5�      $��1��6   $ 
Horizontal=�6&   $��Right=�6?   .+�5�      �� $
��Save��6   $��80��6(   $��0,2,10,2q�63   ��6E   �� *SaveNoteCommand+�� 5�      $��Cancel��6   $��80��6*   $��0,2,2,2q�65   ��6F   �� *CancelEditCommand+5�      5�      5�      5�      