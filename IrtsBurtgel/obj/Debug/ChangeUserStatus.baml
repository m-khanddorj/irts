   M S B A M L   `   `   `  ����F  BIrtsBurtgel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:IrtsBurtgelIrtsBurtgel  5   	   "    IrtsBurtgel.ChangeUserStatus   5   	   U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:IrtsBurtgel     5   	     � Title$0  *Гишүүний төлөв өөрчлөх��5   	   $	��500��6(   $	��400��65    � 
ResizeMode$ NoResize=�6A    � WindowStartupLocation$ CenterOwner=�6W   .��5	      � $��10q�6    � ColumnDefinitions 5
   
   �� 5      6"   �� 5      6"    � RowDefinitions 5   
   �� 5      $��40 �6   6*   �� 5      $��* �6   6)   �� 5      $��40 �6   6*   �� 5      $��40 �6   6*   �� 5      $��40 �6   6*   �� 5      $��40 �6   6*   �� 5      $��40 �6   6*   .+�5   
   �� $��0��6   $��2��6    $��0��64   $
��Left=�6A   $��Center=�6\   .��6w   Төлөвийн түүх6�   �� 5   
   -   5         ��Name$ userStatusStory��5      $��0��6+   $��2��6;   $��1��6O   6^   �� 5   
   -   5         ��Name$ userName��5   -   $��0��6   $��2��6    $��Center=�6?   $��Center=�6\   .��6x   Гишүүний нэр6�   �� 5   
   $��1��6   $��2��6    $
��Left=�6-   $��Center=�6H    J� RenderTransformOrigin$ -0.357,0.577&�6c   .��6�   	Одоо:6�   �� 5   
   -   5        $ currentState��5       $��1��6   $
��Left=�66   $��55,7,0,0q�6Q   $��2��6c   $	��Top=�6p   $
��Bold"�6�   
�������
���  ��6�   �� 5   
   $��0��6   $��Center=�6    $��3��6;   $��33��6H   .��6T   !Төлөв эхлэх өдөр:6g   �� 5   
   $��1��6   $��Center=�6    $��3��6;   $��33��6H   .��6T   #!Төлөв дуусах өдөр:6h   (  "System.Windows.Controls.DatePicker  5   
   -   5          Name$ 	startDate��5   >   $��4��6   $��0��6"   $��30��62   6Q     5   
   -   5        $ endDate��5   >   $��4��6   $��1��6"   $��30��62   6Q   �� 5    
   $��0��6   $��5��6    $��Center=�6-   $��27��6H   .��6T   Төлөв:6\   �� 5!   
   -   5        	 ��Name$	 combobox��5!      $��30��6%   $��1��61   $��5��6A   5#      �� 5$   
   -   5        $��1��5$      $��6��6!   $��75��6.   $��30��69   $��Right=�6Z   $��Center=�6v   .��6�   Хадгалах6�   5%      5&      