   M S B A M L   `   `   `  ���� F  BKailleraNET, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:KailleraNETKailleraNET      KailleraNET.ChatWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     +localclr-namespace:KailleraNET     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     � Title$
  Chat��
�������$	��290��$	��394�� � DataContext �� .��� -   $��grid1��$
��Auto��$
��Auto�� � RowDefinitions �� $
��290* ��� $	��36* � � ColumnDefinitions �� $
��538* ��� $
��Auto �.+��� -   $��richTextBox1��$��Stretch=� �� 
IsReadOnly . " �� VerticalScrollBarVisibility$ Visible=�$��0,0,0,1q�$��50��$��50�����	#FFF2F2F2|�.  �� -   $��textBox1��$
��Auto��$��Stretch=�$��Stretch=�$��0,0,0,-2q�$��1���� -   $��listBox1��$��Stretch=�$��Stretch=�$��1��$��82��
�������$��2�� �� SelectionMode$ Extended=� � ContextMenu �� .@�m� -   $��Add to Buddy List��m� -   $��Send Private Message���� -   $��gridSplitter1��$
��Auto��$
��Left=�$��Stretch=�$��2�� �� ShowsPreview . $��1��$��2��	 � 	Resources	 %  KailleraNET.ShowCurrGameCommand   ShowCurrGameCommand-  'KailleraNET.ShowServerChatWindowCommand  ShowServerChatWindowCommand(  "KailleraNET.ShowUsersWindowCommand  ShowUsersWindowCommand%  KailleraNET.ShowGameListCommand  ShowGameListCommand(  "KailleraNET.BeginDisconnectCommand  BeginDisconnectCommand"  KailleraNET.LeaveGameCommand  LeaveGameCommand��%Z   &	        &	      &	 
     &	      &	      &	                  
 J� InputBindings
 �� 
 �� Key$ Q�� �� 	Modifiers$ Controlf� �� Command# [ �� $ U��$ Controlf�# [ �� $ D��$ Controlf�# [ 