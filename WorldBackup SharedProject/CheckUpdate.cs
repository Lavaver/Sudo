using System;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using com.Lavaver.WorldBackup.Core;
using System.Threading.Tasks;
using Octokit;

namespace com.Lavaver.WorldBackup.Global
{
    public class CheckUpdate
    {
        public static async Task Run()
        {
            // 获取当前程序集版本
            Version localVersion = Assembly.GetEntryAssembly().GetName().Version;

            // GitHub 用户名和仓库名
            string owner = "Lavaver";
            string repo = "WorldBackup";

            // 创建 GitHubClient 实例
            GitHubClient client = new GitHubClient(new ProductHeaderValue("WorldBackup"));

            // 获取最新的 Release
            var releases = await client.Repository.Release.GetAll(owner, repo);
            var latestRelease = releases[0];

            // 获取最新 Release 的 Tag 版本
            Version latestGitHubVersion = new Version(latestRelease.TagName);

            // 比较版本
            int majorMinorComparison = new Version(localVersion.Major, localVersion.Minor).CompareTo(new Version(latestGitHubVersion.Major, latestGitHubVersion.Minor));

            if (majorMinorComparison < 0)
            {
                LogConsole.Info($"有新版本可用（{latestGitHubVersion}）！你应该至 https://github.com/{owner}/{repo}/releases 下载更新 WorldBackup 到最新版本以获得最新的功能和 bug 修复");
            }
            else if (majorMinorComparison > 0)
            {
                LogConsole.Info("当前版本比最新 Release 版本新。");
            }
            else
            {
                LogConsole.Info("当前版本是最新的");
            }
        }
    }
}

