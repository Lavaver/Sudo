# X-IPA WorldBackup CLI+ | 一款控制台冷备份小工具

原本想要做 GUI 的，但 GUI 的表现不好，况且我本身绝大多数项目都从事控制台，于是就有了这个项目

> WorldBackup 不只是个存档备份工具，其实只要涉及冷备份的它都可以用，不一定限制在存档


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

~~`-WebDAV <Address> <Account> <Password> <SourceFilePath> [<DestinationPath>] [<PreAuthenticate:true/false>] [<Buffer>] <Upload/Download/Delete/NewFolder/List>` - 使用 Semeru Module 将文件上传到服务器。亦可进行下载、删除文件和创建文件夹操作。~~

> 旧版方法已弃用。请使用下方展示的新方法。

`-WebDAV <SourceFilePath> [<DestinationPath>] <Upload/Download/Delete/NewFolder>` - 新版异步 WebDAV 操作体验，大刀阔斧的削减了繁琐的参数输入

`-bedrock` - 备份基岩版全部存档（需要管理员权限）

> Semeru Module 根据不同的操作模式会改变这些参数的可用性。详见附录《Semeru Module 操作参数关系表》。

## 灵感

该项目灵感来自于著名的版本控制软件 Git ，实现了一些基本且重要的功能：

- 唯一标识符：类似于 Git 的提交哈希，旨在保证每个备份都是可追溯的
- 时间戳：使用直观易懂的日期+时间方式记录每个备份，更适合大规模整理
- 明确的路径管理：每个备份均拥有极为明确的绝对路径，借鉴于 Git 的文件树
- 日志：这个就不用我多说了懂得都懂，它很重要
- 备份数据库：Git 的精华，使用数据库可快速存储、查阅、删除及恢复备份，保证一致性的同时也可追溯备份日期

## 特别感谢

- Octokit - 辅助支持了软件版本更新检查功能（也是目前唯一一个使用的第三方包）

## 附录：Sumeru Module 操作参数关系表

| 参数 | 在 Upload 的可用性 | 在 Download 的可用性 | 在 Delete 的可用性 | 在 NewFolder 的可用性 | 在 List 的可用性 |
| --- | --- | --- | --- | --- | --- |
| SourceFilePath | 必须 | 必须（用于指代保存路径） | 无需 | 无需 | 无需 |
| DestinationPath | 可选 | 必须 | 必须 | 必须 | 无需 |

> 无需或可选的参数可以使用 `~` 指代忽略这个参数

> 针对于 RL 6.0 以前版本的用法：
>```shell
> [当前路径]> WorldBackup -WebDAV [Address] [Account] [Password] [SourceFile] [DestinationPath] [true/false] 4096 Upload
>```
> 针对于 RL 6.0 以后版本的用法：
>```shell
> [当前路径]> WorldBackup -WebDAV <SourceFilePath> [DestinationPath] Upload
>```
> 可以看到，在进行更新之后，原先的繁琐参数将大幅度缩减，将重要的用户名、密码与服务器地址迁移到配置文件中，以方便后续使用

## 关于之后本项目的代码维护方向

这个项目在大多数人看来已经完善到不能再完善的地步了，但因为这个项目在我眼里拥有特殊意义，所以此后将继续（保持非连续性）维护。

但由于学业原因可能更新的会非常吃力，不过这从某种角度上说也一定程度上帮我分担了部分艰巨任务

> 但学校机房电脑仍用着上古的 Windows 7 ，并且像 Git 一类的版本控制软件（以及一大堆开发环境）也没有安装（不过也不是坏到没边，至少还有 VS 2019 可以用（不过开发负载只有 C++ 我是不能忍的（PS：我只在 C# 可以发挥一席之地）），各种组件处于东缺西缺的状态，网速还慢的一批（甚至连基本的 [VS Code Web 版](https://vscode.dev) 都加载不出来...即使带了电脑也要面对很多现实问题

## 已过时类或方法

| 所属类 | 方法名 | 即将/已弃用时间 | 警告 | 原因 |
| --- | --- | --- | --- |
| `AbstractID` | 全部 | 2024/9/11 | LAW00EF/8 | 可能的法律与伦理风险 |
| `AkashaTerminal` | 全部 | 2024/10/01 | 0x2800FED/RW | 代码过时 + 非可等待性异步 |

## 最近更新速报 | RL 6.0

- 完全弃用 `com.Lavaver.WorldBackup.Sumeru` 命名空间下全部代码，取而代之的是新式的 `com.Lavaver.WorldBackup.async` 命名空间。

> 鉴于本版本对于 WebDAV 模块影响很大（最显著改变是有相当一部分原本在控制台输入的参数迁移到了软件配置文件中），为了安全和便利性，请尽快更新。