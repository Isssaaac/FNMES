# elight.mvc
一款基于Web的通用管理系统轻量级解决方案。
欢迎大家一起参与完善,别忘了给个小星星
## 快速开发
* 开发环境：VS2022 不行就更新到最新
* 实验数据库：MySQL5.7、SQLServer2014 Express、SQLite
## 系统说明
* Elight.MVC是一套基于 .NET6 + Layui开发的通用管理系统快速开发框架。
* 支持SQL Server、MySQL、PostgreSQL、SQLite和Oracle等多种数据库类型。
* 该解决方案适用于OA、电商平台、CRM、物流管理、教务管理等各类管理系统开发。
* 兼容除IE8以下所有浏览器，暂不支持移动端。
* 初始用户名：admin 密码：123456
## 变更记录
* 将EF框架改成SqlSugar
* 数据库：MySQL5.7及以上版本
* 升级了LayUI版本 目前版本:2.6.8
* 修改了原来的一些bug
* 简化了代码逻辑，更容易理解 
* 原始ASP.NET代码github：https://github.com/esofar/elight.mvc
* 新增WinForm界面框架，界面使用SunnyUI（商用请联系作者Sunny授权） https://gitee.com/yhuse/SunnyUI
* 特别说明:SunnyUI请勿升级到最新，3.1.2开始已经不支持白色Style
* 新增报表打印功能，使用FastReport.Opensource,frx文件编辑请使用Other下的FastReport.Community.2022.2.0.zip
* 打印原理，利用FastReport.OpenSource生成Pdf文件  再通过命令行PDFtoPrinter实现打印功能 详见 Elight.WebUI/Reports/readme.txt

* DB脚本在Other/DB下，目前提供MySQL和SQLServer版的SQL脚本，以及SQLite文件。其他数据库不再提供，请自己转换
* 新增代码生成工具，目前支持SQLite,MySQL和SQLServer
* 数据库默认使用SQLite，运行即可启动。
* 配置文件在Elight.WebUI/Configs目录下，需要使用其他数据库，请自己修改

### 网页效果如下
![效果1](https://gitee.com/zjwno1/Elight.MVC-ASP.NET/raw/master/Other/Images/06.png)

![效果2](https://gitee.com/zjwno1/Elight.MVC-ASP.NET/raw/master/Other/Images/07.png)

![效果3](https://gitee.com/zjwno1/Elight.MVC-ASP.NET/raw/master/Other/Images/08.png)

![效果4](https://gitee.com/zjwno1/Elight.MVC-ASP.NET/raw/master/Other/Images/09.png)

![效果5](https://gitee.com/zjwno1/Elight.MVC-ASP.NET/raw/master/Other/Images/10.png)

![效果6](https://gitee.com/zjwno1/Elight.MVC-ASP.NET/raw/master/Other/Images/11.png)

![效果7](https://gitee.com/zjwno1/Elight.MVC-ASP.NET/raw/master/Other/Images/12.png)

![效果8](https://gitee.com/zjwno1/Elight.MVC-ASP.NET/raw/master/Other/Images/13.png)


### WinForm界面如下
![效果1](https://gitee.com/zjwno1/Elight.MVC-ASP.NET/raw/master/Other/Images/01.png)

![效果2](https://gitee.com/zjwno1/Elight.MVC-ASP.NET/raw/master/Other/Images/02.png)

![效果3](https://gitee.com/zjwno1/Elight.MVC-ASP.NET/raw/master/Other/Images/03.png)

![效果4](https://gitee.com/zjwno1/Elight.MVC-ASP.NET/raw/master/Other/Images/04.png)

![效果5](https://gitee.com/zjwno1/Elight.MVC-ASP.NET/raw/master/Other/Images/05.png)

## 创作不易，给个一分也是爱
![给个一分也是爱](https://gitee.com/zjwno1/Elight.MVC-ASP.NET/raw/master/Other/Images/donate.png)


