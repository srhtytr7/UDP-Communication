# UDP-Communication
UDP communicaton example in C#
Includes synchronous and asynchronous udp communication examples in c# form application.

1. At first program, there are 1 server and 2 clients. The server reads datas from 2 clients and writes them to a listbox with the sender information(ip address and port number). Clients send datas from textboxs. Reading process made with threading so it reads continuously but it isn't effecting the main program. Threading pausing made with EventWaitHandle.

2. Second program is same as  the first program. I just did it with asynchronous receiving. So i didnt have to use threading. I used EventWaitHandle to pause the asychronous receiving.
