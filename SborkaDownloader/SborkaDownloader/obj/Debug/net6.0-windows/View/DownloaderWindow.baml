   M S B A M L   `   `   `  ����K  GSborkaDownloader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:SborkaDownloader.ViewModelSborkaDownloader.ViewModel  5   	   =#clr-namespace:SborkaDownloader.ViewSborkaDownloader.View  5   	   +    %SborkaDownloader.View.DowloaderWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7vm(clr-namespace:SborkaDownloader.ViewModel     5   	   5local#clr-namespace:SborkaDownloader.View     5   	     � Title$  Sborka downloader��5	   	   $	��450��6#   $
��1200��60    � DataContext 5
      -   'SborkaDownloader.ViewModel.DownloaderVM  5   
    � 	Resources 5      4   .SborkaDownloader.View.LongToFilesSizeConverter   FileSizeConverter��%   &	          5   
   6)   5      .��5      �  � RowDefinitions 5   
   �� 5      $��30 �6   �� 5      $��* �6   �� 5      $��20 �6   .+�5   
   �  � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $��4* �6#   �� 5      $��3* �6#   �� 5      $��3* �6#   .+�5      �� $��Order id��6   $��0��6(   $
��Bold"�68   $��Center=�6J   $��Center=�6g   �� 5      $��	File name��6   $��1��6)   $
��Bold"�69   $��Center=�6K   $��Center=�6h   �� 5      $��Google Drive id��6   $��2��6/   $
��Bold"�6?   $��Center=�6Q   $��Center=�6n   �� 5       $��Download progress��6   $��3��61   $
��Bold"�6A   $��Center=�6S   $��Center=�6p   5!      �� 5"   
   $��Stretch=�67   $��1��6\   ��6   �� *LayoutFiles+��5#      �� 5$      .R�5%      �  5&      �� 5'      $��* �6/   �� 5(      $��4* �6/   �� 5)      $��3* �6/   �� 5*      $��3* �6/   .+�5,      �� $��0��6=   ��6$   �� *	OrderId+6=   �� 5-      $��1��6:   ��6$   �� *Name+6:   �� 5.      $��2��6B   ��6$   �� *GoogleFileId+6B   � 5/       � Minimum$ 0Q�6&   $��3��6j    � Maximum 62   �� *
FileSize+ � Value 6O   �� *
Progress+6j   �� 50      $��3��6l   $��Center=�6|   ��6$   �� *
FileSize+ �� 	Converter# [  6|   51      52      54      �� 55   
   $��3��6,   	 �� Padding$	 0 0 20 0q�69   ���	#33000000
 �� TextAlignment$
 Right=�6c   ��6   �� *Status+6c   56      57      