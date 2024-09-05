# X-IPA WorldBackup CLI+ | 一款控制台冷备份小工具

原本想要做 GUI 的，但 GUI 的表现不好，况且我本身绝大多数项目都从事控制台，于是就有了这个项目

> WorldBackup 不只是个存档备份工具，其实只要涉及冷备份的它都可以用，不一定限制在存档

> 注1：该项目有不同版本的分支。**标准发行版（`main` 分支）** 适用于公众使用，一般推荐非特殊需求用户使用该版本（同时，**该版本也是唯一并不需要自己手动编译的版本，每个正式发行在该分支的版本会同步发布一个已经编译好的文件在 [Releases 页面](https://github.com/Lavaver/WorldBackup/releases)**）。**`Safety-Branch` 分支**适用于对安全需求较高的场景，这个分支的代码**针对特殊需求增加了安全代码**。

> 注2：`Xinhua-SE` 分支**在这学期结束之前不会开源在此处**。等放假后再选择性开源，但部分闭源代码该不放的还是不放（

## 该软件包括如下组件/内容

- LogConsole - 统一的可缩放日志模块
- LogWriter - 面向 WorldBackup For Windows Presentation Foundation 的异步式现代后台日志记录模块

> 注：LogConsole 和 LogWriter 是两个**不同的核心组件**。LogWriter 提供了对 Windows Presentation Foundation 的更现代日志体验

- Backup Database（备份数据库） - 记录备份详情
- Backup 与 FirstBackup 模块 - 提供完整的冷备份解决方案
- Database Recovery（数据库恢复） - 从数据库记录中恢复指定的备份
- NTP-C（Network Time Protocol Calibrators，网络时间协议校准器） - 实时比对 NTP 与系统时间，并在备份中直接使用 NTP 时间
- Sumeru Online - 使用 WebDAV 模块可以快速从本地上传文件到在线存储服务

> 首先绝对不能启动一个 Minecraft 服务器...

## 如何使用

- 首先从 Releases 下载最新版本
- 解压至熟知的路径
- 你可以直接运行程序也可以先打开 cmd 再运行
- 初次启动会要求你配置 `WorldBackupConfig.xml` ，你可以通过 `-config` 参数重新运行程序以开始配置，或者你可以进入配置文件手动配置
- 再次启动软件就会开始初始化备份模块，最后开启主备份模块，每隔 15 分钟备份一次，这点你放心
- 按下 `Ctrl + C` ，程序将正常关闭（可千万不要直接叉掉啊）
- 你应该已经看到了 `备份数据库.xml` 文件，它用作记录你备份的各种信息，包括标识符（`Identifier`）、时间（`Time`）和备份路径（`Path`）
- 通过 `-database` 参数运行，你可以直接以表格形式可视化看到每个备份的详细记录（不懂 XML 小白狂喜）

## 可选参数列表

`-database` - 读取备份数据库的所有数据，并以表格形式可视化显示在终端（其中最新备份会特别标识且置顶显示）

~~`-deldatabase` - 删除整个备份数据库，但不删除整个备份文件（及其子文件夹）。这可能对于清理历史较久的记录有效果，但恢复这些备份你需要手动复制并替换文件~~

~~`-deldata` - 选择备份并删除选中备份项~~

> 原 `-deldatabase` 与 `-deldata` 参数已在 RL 5.2 版本中**完全弃用**。取而代之的是新式的 `-del <data/database>` 参数。

`-del <data/database>` 针对于 RL 5.2 版本之后的新式删除备份文件或数据库的集合参数

`-recovery` - 选择备份并恢复到配置设定的来源位置

`-config` - 进入配置页面并进行软件配置

`-WebDAV <Address> <Account> <Password> <SourceFilePath> [<DestinationPath>] [<PreAuthenticate:true/false>] [<Buffer>] <Upload/Download/Delete/NewFolder/List>` - 使用 Semeru Module 将文件上传到服务器。亦可进行下载、删除文件和创建文件夹操作。

`-bedrock` - 备份基岩版全部存档（需要管理员权限）

> Semeru Module 根据不同的操作模式会改变这些参数的可用性。详见附录《Semeru Module 操作参数关系表》。

## 灵感

该项目灵感来自于著名的版本控制软件 Git ，实现了一些基本且重要的功能：

- 唯一标识符：类似于 Git 的提交哈希，旨在保证每个备份都是可追溯的
- 时间戳：使用直观易懂的日期+时间方式记录每个备份，更适合大规模整理
- 明确的路径管理：每个备份均拥有极为明确的绝对路径，借鉴于 Git 的文件树
- 日志：这个就不用我多说了懂得都懂，它很重要
- 备份数据库：Git 的精华，使用数据库可快速存储、查阅、删除及恢复备份，保证一致性的同时也可追溯备份日期

## 附录：Sumeru Module 操作参数关系表

| 参数 | 在 Upload 的可用性 | 在 Download 的可用性 | 在 Delete 的可用性 | 在 NewFolder 的可用性 | 在 List 的可用性 |
| --- | --- | --- | --- | --- | --- |
| Address | 必须 | 必须 | 必须 | 必须 | 必须 |
| Account | 必须 | 必须 | 必须 | 必须 | 必须 |
| Password | 必须 | 必须 | 必须 | 必须 | 必须 |
| SourceFilePath | 必须 | 必须（用于指代保存路径） | 无需 | 无需 | 无需 |
| DestinationPath | 可选 | 必须 | 必须 | 必须 | 无需 |
| PreAuthenticate | 必须 | 无需 | 必须 | 无需 | 无需 |
| Buffer | 必须（`int`/`long`） | 无需 | 无需 | 无需 | 无需 |

> 无需或可选的参数可以使用 `~` 指代忽略这个参数

> 如果你只需要上传，下面提供了典型的使用方法：
>
> Windows PowerShell：
> ```shell
> PS [当前路径]> .\WorldBackup -WebDAV [Address] [Account] [Password] [SourceFile] [DestinationPath] [true/false] 4096 Upload
>```
> Command（cmd）：
>```shell
> [当前路径]> WorldBackup -WebDAV [Address] [Account] [Password] [SourceFile] [DestinationPath] [true/false] 4096 Upload
>```

## 最近更新速报 | RL 5.2 （需自行编译）

- `com.Lavaver.WorldBackup.Database.GlobalClass` 类名更改为 `com.Lavaver.WorldBackup.Global.GlobalString`
- 完全替换了全部程序的冗余配置文件位置以及数据库位置的常量
- `MainProgram.cs` 中针对于 `-deldata` 与 `-deldatabase` 这两个参数进行整合成新的 `-del <data/database>` 参数，提高紧凑性