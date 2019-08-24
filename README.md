![](https://raw.githubusercontent.com/michaelmnich/Sioux/Main/Sioux/img/repo.jpg)


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

**MainPage**
Main page 

**Test**
Guide page for tests

Adding new page is described in class "Program":
```

  Test testPage = new Test(webFabric.PagesWoeker);
webFabric.AddPage("test", testPage);


```

**BasePage**
Abstrac page class. All pages should extend that.

If you whnt to add costum page to project without recompilation
Create class that extends **BasePage**.

For example:

```
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ManagingWebSerwer.Conections;
using ManagingWebSerwer.dao;
namespace ManagingWebSerwer.pages.Test
{
    class SamplePageToCompile : BasePage
    {

        public SamplePageToCompile()
        {          
        }

        public override string GetContent()
        {
            return "<br><hr><p style='text-align: center;'>Sample compilated page</p>";
        }


        public override void Set_GET_Params(Dictionary<string, string> Get_params)
        {
            _Get_params = Get_params;
        }

        public override void Set_SET_Params(Dictionary<string, string> Set_params)
        {
            _Set_params = Set_params;
        }

        public override void Set_Cockie_Params(Dictionary<string, Cookie> Cockie_params)
        {
            _Cookie_params = Cockie_params;
        }

        public override string GetUri()
        {
            return "sample"; //This is page url. So to access that page you must type "http://localhost:8080/sample/"
        }
    }
}

```

All pages are store in:  MainSiouxdirecotry/Pages:

For example "C:\Users\admin\Source\Repos\Sioux\Sioux\bin\Debug\pages"


![Bez tytułu](https://user-images.githubusercontent.com/3948281/63641172-23456080-c6aa-11e9-98d5-9fbb5ec15fd5.png)




