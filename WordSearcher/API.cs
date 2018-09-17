using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace WordSearcher
{
    public class Api
    {
        private readonly string _authKey;
        private readonly RestClient _client;

        public Api(string serverUri, string authKey)
        {
            _authKey = authKey;
            _client = new RestClient(serverUri);
            _client.AddHandler("application/json", new JsonDeserializer());
        }
        public bool Connect()
        {
            SendRequest("/task/game/start/",out var success);
            return success;
        }

        public bool[,] MoveUp()
        {
            return Move(Side.Up);
        }

        public bool[,] MoveDown()
        {
            return Move(Side.Down);
        }

        public bool[,] MoveLeft()
        {
            return Move(Side.Left);
        }

        public bool[,] MoveRight()
        {
            return Move(Side.Right);
        }

        public Stats GetStats()
        {
            var response = SendRequest("/task/game/stats/", out _, Method.GET);
            return ParseStats(response);
        }

        public Stats SendWords(IEnumerable<string> words)
        {
            var response = SendRequest("/task/words/",out _, body: words);
            return ParseStats(response, false);
        }

        public Stats Close()
        {
            var response = SendRequest("/task/game/finish", out _);
            return ParseStats(response, false);
        }

        
        private bool[,] Move(Side side)
        {
            var sideName = Enum.GetName(typeof(Side), side)?.ToLower();
            var content = SendRequest($"/task/move/{sideName}", out _);
            var field = ParseField(content);
            return field;
        }

        private string SendRequest(string path, out bool success, Method method = Method.POST, object body = null)
        {
            var request = new RestRequest(path)
            {
                Method = method
            };
            request.AddHeader("Authorization", $"token {_authKey}");
            if (body != null)
                request.AddJsonBody(body);
            var response = _client.Execute(request);
            success = response.StatusCode == HttpStatusCode.OK;
            return success 
                ? response.Content 
                : throw new HttpException((int)response.StatusCode, response.ErrorMessage);
        }

        private static bool[,] ParseField(string content)
        {
            var body = content.Substring(1, content.Length - 2)
                              .Split(new[] { "\\r\\n" }, StringSplitOptions.None);
            var height = body.Length;
            var width = body[0].Length;
            var field = new bool[width, height];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                    field[x, y] = body[y][x] == '1';
            }
            return field;
        }
        
        private static Stats ParseStats(string content, bool fullStat = true)
        {
            var stats = JsonConvert.DeserializeObject<Dictionary<string, int>>(content);
            return fullStat
                ? new Stats(stats["words"], stats["points"], stats["moves"])
                : new Stats(stats["points"]);
        }        
        
        private enum Side
        {
            Up,
            Down,
            Left,
            Right
        }

        public class Stats
        {
            public Stats(int words, int points, int moves)
            {
                Words = words;
                Points = points;
                Moves = moves;
            }

            public Stats(int points) => Points = points;

            public int Words { get; }
            public int Points { get; }
            public int Moves { get; }
        }
    }
}