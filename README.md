# UDP-Communication
UDP communicaton example in C#
Includes synchronous and asynchronous udp communication examples in c# form application.

1. At first program, there are 1 server and 2 clients. The server reads datas from 2 clients and writes them to a listbox with the sender information(ip address and port number). Clients send datas from textboxs. Reading process made with threading so it reads continuously but it isn't effecting the main program. Threading pausing made with EventWaitHandle.

2. Second program is same as  the first program. I just did it with asynchronous receiving. So i didnt have to use threading. I used EventWaitHandle to pause the asychronous receiving.

3. Third program is communicating with an android device. It receives data and writes it to a listbox with ip and port information of the sender. Then it sends a data to tell the communation is successful(data > "success"). The device uses a udp communication app called UDP TCP Server - Free ( app link https://play.google.com/store/apps/details?id=com.aviramido.udpserver&hl=tr ). In app, you have to set the target ip ( the ip of the device which out programs running (you can learn it via command prompt using ipconfig command) ) , you have to select udp as communication type and target port ( port number our programs listens ). Then you are good to go. 
