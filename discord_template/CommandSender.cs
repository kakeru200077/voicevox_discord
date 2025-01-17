﻿using System.Text;

namespace voicevox_discord
{
    internal class CommandSender
    {
        public readonly string[] m_CommandList;
        public readonly Ids m_Ids;
        public readonly string m_ApiVersion;

        public CommandSender(string directoryPath, Ids ids, string apiVersion)
        {
            if (directoryPath.IsNullOrEmpty()) { throw new Exception($"{nameof(directoryPath)}が不正です。\nnullもしくは空白です。"); }
            //コマンドファイル一覧を取得
            if (!Directory.Exists(directoryPath)) { throw new Exception($"指定されたパス{directoryPath}は存在しません。"); }
            var CommandPathList = Directory.GetFiles(directoryPath, "*.json");
            if (CommandPathList.Length <= 0) { throw new Exception("指定されたパス内にjsonファイルが存在しませんでした。"); }

            //コマンドファイルの中身を取り出す
            m_CommandList = new string[CommandPathList.Length];
            int i = 0;
            foreach (string CommandPath in CommandPathList)
            {
                m_CommandList[i] = File.ReadAllText(CommandPath);
                i++;
            }

            if (ids == null) { throw new ArgumentNullException("id"); }
            m_Ids = ids;

            if (Tools.IsNullOrEmpty(apiVersion)) { throw new Exception($"{nameof(apiVersion)}がNullもしくは空白です。"); }
            m_ApiVersion = apiVersion;
        }

        public void RequestSender()
        {
            if (m_CommandList == null) { throw new NullReferenceException($"{nameof(m_CommandList)}が不正です。\nnullもしくは空白です。"); }
            if (m_Ids == null) { throw new NullReferenceException("ids"); }
            if (m_Ids.m_GuildIds == null) { throw new NullReferenceException("ids.guild_ids"); }

            foreach (string jsonCommand in m_CommandList)
            {
                foreach (string guild_id in m_Ids.m_GuildIds)
                {
                    HttpRequestMessage request = GetHeader(guild_id);

                    if (jsonCommand.IsNullOrEmpty()) { throw new Exception("json_commandが不正です。\njson_commandがnullもしくは空白です。"); }

                    //HttpRequestMessageのコンテンツを設定する
                    HttpRequestMessage sendRequest = RequestContentBuilder(request, jsonCommand);

                    //送信する
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = client.Send(sendRequest);
                    Console.WriteLine(response.ToString());
                }
            }
        }

        //
        //ヘッダーの追加処理
        private HttpRequestMessage GetHeader(string guild_id)
        {
            if (guild_id.IsNullOrEmpty()) { throw new Exception("guild_idが不正です。\nguild_idがnull、もしくは空白です。"); }
            if (m_Ids.m_ApplicationId.IsNullOrEmpty()) { throw new Exception("application_idが不正です。\napplication_idがnull、もしくは空白です。"); }
            if (m_Ids.m_Token.IsNullOrEmpty()) { throw new Exception("Tokenが不正です。\nTokenがnull、もしくは空白です。"); }

            string url = $"https://discord.com/api/v{m_ApiVersion}/applications/{m_Ids.m_ApplicationId}/guilds/{guild_id}/commands";
            UriBuilder builder = new UriBuilder(new Uri(url));
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, builder.Uri);
            request.Headers.Add("Authorization", "Bot " + m_Ids.m_Token);

            return request;
        }

        //
        //リクエストメッセージにコンテンツとしてjson形式のコマンド一覧を追加
        private HttpRequestMessage RequestContentBuilder(HttpRequestMessage requestMessage, string json_command)
        {
            Console.WriteLine(json_command);

            //渡されたjson形式のコマンド情報をコンテンツに設定する
            if (json_command.IsNullOrEmpty()) { throw new Exception("json_commandが不正です。\njson_commandがnullもしくは空白です。\n"); }
            requestMessage.Content = new StringContent(json_command, Encoding.UTF8, "application/json");

            return requestMessage;
        }
    }
}