﻿# X-IPA WorldBackup CLI+ | 一款控制台冷备份小工具

原本想要做 GUI 的，但 GUI 的表现不好，况且我本身绝大多数项目都从事控制台，于是就有了这个项目

> WorldBackup 不只是个存档备份工具，其实只要涉及冷备份的它都可以用，不一定限制在存档

## 该软件包括如下组件/内容

- LogConsole - 统一的可缩放日志模块
- Backup Database（备份数据库）与 MySQL Remote Database - 记录备份详情
- Backup 与 FirstBackup 模块 - 提供完整的冷备份解决方案
- Database Recovery（数据库恢复） - 从数据库记录中恢复指定的备份
- NTP-C（Network Time Protocol Calibrators，网络时间协议校准器） - 实时比对 NTP 与系统时间，并在备份中直接使用 NTP 时间

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

`-del <data/database>` 针对于 RL 5.2 版本之后的新式删除备份文件或数据库的集合参数

`-recovery` - 选择备份并恢复到配置设定的来源位置

`-config` - 进入配置页面并进行软件配置

`-bedrock` - 备份基岩版全部存档（需要管理员权限）


## 关于之后本项目的代码维护方向

这个项目在大多数人看来已经完善到不能再完善的地步了，但因为这个项目在我眼里拥有特殊意义，所以此后将继续（保持非连续性）维护。

但由于学业原因可能更新的会非常吃力，不过这从某种角度上说也一定程度上帮我分担了部分艰巨任务

> 但学校机房电脑仍用着上古的 Windows 7 ，并且像 Git 一类的版本控制软件（以及一大堆开发环境）也没有安装（不过也不是坏到没边，至少还有 VS 2019 可以用（不过开发负载只有 C++ 我是不能忍的（PS：我只在 C# 可以发挥一席之地）），各种组件处于东缺西缺的状态，网速还慢的一批（甚至连基本的 [VS Code Web 版](https://vscode.dev) 都加载不出来...即使带了电脑也要面对很多现实问题

## 最近更新速报 | RL 7.1

- 包括了一些功能性改进

> （Base64 编码后更新速报） 5L2/55SoIGAtaGFsbG93c2RheWAg5Y+C5pWw6L+Q6KGM77yM6K+05LiN5a6a5Lya5pyJ5LuA5LmI5oOK5ZacLi4u