﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Util;

namespace Editor
{
    public class GenerateAPICode : EditorWindow
    {
        private const string GeneratePath = "/Scripts/API";

        [MenuItem("Tools/APIコード更新")]
        private static async void GenerateCode()
        {
            var json = await GoogleSheetUtil.GetGameInfo(GenerateCodeConstant.sheetURL, GenerateCodeConstant.sheetName);
            var keys = GoogleSheetUtil.GetParameterKeyList(json);
            var parameterString = CreateParameterContents(keys);
            var content = CreateScriptContent(GenerateCodeConstant.sheetName, parameterString);

            CreateScript(GeneratePath + "/EnemyAPI.Generated.cs", content);
        }

        /// <summary>
        ///     スクリプトファイルを生成
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        private static void CreateScript(string path, string content)
        {
            path = Application.dataPath + "/" + path;

            using (var writer = new StreamWriter(path, false))
            {
                writer.WriteLine(content);
            }

            AssetDatabase.Refresh();
        }


        /// <summary>
        ///     スクリプトの内容を生成
        /// </summary>
        /// <param name="className"></param>
        /// <param name="parameterContents"></param>
        /// <returns></returns>
        private static string CreateScriptContent(string className, string parameterContents)
        {
            return $@"// <auto-generated/>
// このコードは自動生成されたものです。手動で編集しないでください。

namespace API
{{
    public class {className}
    {{
        {parameterContents}
    }}
}}";
        }

        /// <summary>
        ///     パラメータの内容を生成
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private static string CreateParameterContents(IReadOnlyList<string> keys)
        {
            var parameterString = new StringBuilder();
            for (var keyIndex = 0; keyIndex < keys.Count; keyIndex++)
            {
                var key = keys[keyIndex];
                if (keyIndex == 0)
                    parameterString.Append("public string " + key + ";\n");
                else if (keyIndex == keys.Count - 1)
                    parameterString.Append("\t\tpublic string " + key + ";");
                else
                    parameterString.Append("\t\tpublic string " + key + ";\n");
            }

            return parameterString.ToString();
        }
    }
}