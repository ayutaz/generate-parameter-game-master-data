using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Util
{
    public static class GoogleSheetUtil
    {
        /// <summary>
        ///     ゲーム情報をスプレッドシートから取得
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static async UniTask<T> GetGameInfo<T>(string url, string sheetName)
        {
            var request = UnityWebRequest.Get($"{url}?sheetName={sheetName}");
            await request.SendWebRequest();
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError
                or UnityWebRequest.Result.DataProcessingError)
            {
                Debug.Log("fail to get card info from google sheet");
            }
            else
            {
                var json = request.downloadHandler.text;
                Debug.Log($"data:{json}");
                var data = JsonUtility.FromJson<T>(json);
                return data;
            }

            return default;
        }

        /// <summary>
        ///     ゲーム情報をスプレッドシートから取得してstring
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static async UniTask<string> GetGameInfo(string url, string sheetName)
        {
            var request = UnityWebRequest.Get($"{url}?sheetName={sheetName}");
            await request.SendWebRequest();
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError
                or UnityWebRequest.Result.DataProcessingError)
            {
                Debug.Log("fail to get card info from google sheet");
            }
            else
            {
                var json = request.downloadHandler.text;

                Debug.Log($"data:{json}");
                return json;
            }

            return default;
        }

        /// <summary>
        ///     マスターデータのキーの文字列を取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetParameterKeyList(string json)
        {
            var parseJson = JObject.Parse(json);
            var gameInfoArray = (JArray)parseJson["gameInfo"];
            var gameInfo = (JObject)gameInfoArray?[0];

            return gameInfo?.Properties().Select(key => key.Name).ToList();
        }

        /// <summary>
        ///     jsonデータをスプレッドシートに書き込み
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sheetName"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static async UniTask<bool> PostGameInfo(string url, string sheetName, string postData)
        {
            var request = new UnityWebRequest($"{url}?sheetName={sheetName}", "POST");
            var data = Encoding.UTF8.GetBytes(postData);
            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();
            Debug.Log(request.downloadHandler.text);
            return request.result == UnityWebRequest.Result.Success;
        }
    }
}