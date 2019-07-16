![](https://raw.githubusercontent.com/michaelmnich/Sioux/Main/img/repo.jpg)


### **Sioux:**
Simple C# http Serwer

Main page: localhost:8080
Test page: localhost:8080/test/?info=TestInfo


### Guide classes:

**TcpSerwer**
Class contains Tcp serwer functionalities allow sending dato to clients via tcp.

Intresting Methods:
```
TcpSerwer -> Client_MessageReceived(object sender, MessageEventArgs e)
TcpSerwer -> SendMessage_to_Client(string s, string name)
TcpSerwer -> GetStatus(string name)
```

**PagesWorker**
Class maintain pages activities like cookies, Get, set etc.

**Page**
Interface for all pages

**MainPage"**
Main page 

**Test**
Guide page for tests

Adding new page is described in class **Program**:
```

  Test testPage = new Test(webFabric.PagesWoeker);
webFabric.AddPage("test", testPage);


```


