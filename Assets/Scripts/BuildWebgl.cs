#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildWebgl
{
    [MenuItem("Build/WebGL")]
    public static void Build()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);

        var scenes = from scene in EditorBuildSettings.scenes
                     where scene.enabled
                     select scene.path;

        var directory = new DirectoryInfo("Build/Webgl");

        BuildPipeline.BuildPlayer(scenes.ToArray(), directory.FullName, BuildTarget.WebGL, BuildOptions.None);
        
        File.WriteAllText(Path.Combine(directory.FullName, "index.html"), Html);
        
        Application.OpenURL($"file://{directory.FullName}");
    }

    private const string Html =
        "<!DOCTYPE html>" + "\n" +
        "<html lang=\"en-us\">" + "\n" +
        "  <head>" + "\n" +
        "    <meta charset=\"utf-8\">" + "\n" +
        "    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">" + "\n" +
        "    <title>Unity WebGL Player | Summit</title>" + "\n" +
        "    <link rel=\"shortcut icon\" href=\"TemplateData/favicon.ico\">" + "\n" +
        "    <link rel=\"stylesheet\" href=\"TemplateData/style.css\">" + "\n" +
        "    <script src=\"TemplateData/UnityProgress.js\"></script>" + "\n" +
        "    <script src=\"Build/UnityLoader.js\"></script>" + "\n" +
        "    <script>" + "\n" +
        "      var unityInstance = UnityLoader.instantiate(\"unityContainer\", \"Build/Webgl.json\", {onProgress: UnityProgress});" + "\n" +
        "    </script>" + "\n" +
        "  </head>" + "\n" +
        "  <body>" + "\n" +
        "    <div class=\"webgl-content\" style=\"width: 100%; height: 100%\">" + "\n" +
        "      <div id=\"unityContainer\" style=\"width: 100%; height: 100%\"/>" + "\n" +
        "    </div>" + "\n" +
        "  </body>" + "\n" +
        "</html>";
}
#endif