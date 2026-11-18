using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// WebGL ビルド補助。
// メニュー：NONO > Build WebGL
// バッチモード例：
//   Unity -batchmode -quit -projectPath <proj> -executeMethod BuildWebGL.Run
// GitHub Pages 対策として Decompression Fallback を必ず有効化する。
public static class BuildWebGL
{
    const string OutputDir = "Build/WebGL";

    [MenuItem("NONO/Build WebGL")]
    public static void Run()
    {
        // Pages は Content-Encoding を返さないため、展開フォールバックが必須
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
        PlayerSettings.WebGL.decompressionFallback = true;
        PlayerSettings.WebGL.dataCaching = true;

        string[] scenes = { "Assets/Scenes/Lakeside.unity" };
        foreach (string s in scenes)
            if (!File.Exists(s)) Debug.LogWarning($"シーンが見つかりません: {s}（パスを実際の構成に合わせてください）");

        Directory.CreateDirectory(OutputDir);
        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = OutputDir,
            target = BuildTarget.WebGL,
            options = BuildOptions.None,
        };
        BuildReport report = BuildPipeline.BuildPlayer(options);
        Debug.Log($"WebGL build: {report.summary.result}, total size = {report.summary.totalSize / (1024 * 1024)} MB, errors = {report.summary.totalErrors}");
        if (Application.isBatchMode && report.summary.result != BuildResult.Succeeded)
            EditorApplication.Exit(1);
    }
}
