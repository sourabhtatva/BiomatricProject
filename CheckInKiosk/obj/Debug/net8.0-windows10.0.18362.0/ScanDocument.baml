   M S B A M L   `   `   `  ����  CheckInKiosk    CheckInKiosk.ScanDocument   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ���#121212  � 	Resources  5       �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius d� 	IsChecked �� Path �� RelativeSource	 �� Mode
 �� SelectionBoxItem �� IsDropDownOpen � AllowsTransparency ��Name �� Triggers P� Property P� Value   LoadingOverlayStyle��%P	  '��       '�� �    '�� �    &	  �    �� 5
   
   " ��6   .�5      ��  � ��
 �����6+   �� 5       � �� �#1E1E1E6+   �� 5       � �� �#3336,   �� 5       � ��$ 1q�60   �� 5       � ��$ 5q�6(   �� 5       � ��$ 10q�6'   �� 5       � ��$ 16(�6)   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   #��z��6K   #��z��6w   $ 5��6�   ._�5      � .+�5   "   d� $��ToggleButton��6/   #��z��5   /   #��z��5   /   #��z��5   /    5   /   �� $ IsDropDownOpen� �� *TemplatedParent+$	 TwoWay=�.��5   &   �� $
��Left=�67   $��Center=�6R   $��3,3,25,3q�6m   #��z
 6   5   #   � 5   "   $��Popup��6(   $��Bottom=�65   #��z 5   (   $
 True��5    (   $|�False��5!   (   $��Slide=�5"   (   .��5#   &   �� #��z��6-   #��z��6W   #��z��6�   $ 5��6�   ._�5$   *   �� $	��200��67   $
��True��6G   .��5%   .   �� 5&   +   5'   '   5(   #   5)      5*      5+      5-      5.      �� 51   
   " ��6   .�52      ��  � ��
 �����6+   �� 53       � �� �#1E1E1E6+   �� 54       � �� �#3336,   �� 55       � ��$ 1q�60   �� 56       � ��$ 5q�6(   �� 57       � ��$ 10q�6'   �� 58       � ��$ 16(�6)   �� 59       � �� 5:      �� 5;      " ��6&   .R�5<      �� #��z��6!   #��z��6K   #��z��6w   $ 5��6�   ._�5=      �� $ PART_ContentHost��6+   5>      5?      5A      5B      �� 5E   
   " ��6   .�5F      ��  � �� �#1E90FF6+   �� 5G       � ��
 �����6+   �� 5H       � �� �#1E90FF6,   �� 5I       � ��$ 1q�60   �� 5J       � ��$
 10,5q�6(   �� 5K       � ��$ 10q�6'   �� 5L       � ��$ 18(�6)   �� 5M       � ��$
 Bold"�6+   �� 5N       � �� 5O      �� 5P      " ��6&   .R�5Q      �� #��z��6!   #��z��6K   #��z��6w   $ 5��6�   ._�5R      �� $��Center=�6/   $��Center=�6L   5S      5T      5V       5W      P� 5X       �@J�IsMouseOver$
 True��61   . �5Y      ��  � �� �#4682B463   �� 5Z       � �� �#4682B464   5[      5]      �� 5`   
   " �6   .�5a      ��  � �� �	#000000CC6+   �� 5b       � y�$ 	Collapsed=�6+   5c      5d      .��5f      � .+�5h   
   �� -   5         ��Name$ MainStackPanel��5h      $��Center=�6-   $��Center=�6H   .+�5i      �� $��Select Document Type��6   
�������$��24(�6G   $��10q�6U   $
��Bold"�6a   �� 5j      -   5         ��Name$ DocumentTypeComboBox��5j      .@�5k      �� $��Passport��6   �� 5l      $��
Vietnam ID��6   5m      �� 5p      -   5         ��Name$ AdditionalInfoTextBox��5p      $y�	Collapsed=�65   �� 5q      -   5        $��Submit Document��5q      6/   �� 5t      -   5         ��Name$ VerificationMessage��5t      
�������$��18(�6H   $��10q�6V   $y�	Collapsed=�6b   �� 5u      -   5        $ ErrorMessage��5u      
���  ��$��18(�6?   $��10q�6M   $y�	Collapsed=�6Y   5v      � 5y   
   -   5         �Name$ LoadingOverlay��5y      #��[  6'   .+�5z      �� $��Center=�6   $��Center=�64   .+�5{      �  � IsIndeterminate$
 True��6   $��50��65   $��50��6A   �� 5|      $��Verifying...��6   
�������$��18(�6C   $��10q�6Q   $��Center=�6]   5}      5~      �� 5�   
   -   5        $ ManualCheckInPanel��5�      $��Center=�61   $��Center=�6L   $	��600��6i   $y�	Collapsed=�6u   .+�5�      ��  �� TextAlignment$ Center=�6   
�������$��20(�6B   $��Regular"�6P   $��0,0,0,20q�6e   $
��Wrap=�6w   rpThe details you entered are incorrect. You can proceed with manual check-in or contact our agent for assistance.5�      �� 5�      -	   5        $
��Okay��5�      $	��150��6$   $��50��60   
�������
���   �$��18(�6b   $
��Bold"�6p   $��Center=�6�   5�      5�      5�      