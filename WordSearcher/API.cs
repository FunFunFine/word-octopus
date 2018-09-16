using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;

namespace WordSearcher
{
    public class Api : IDisposable
    {
        private readonly string _authKey;
        private readonly RestClient _client;

        public Api(string serverUri, string authKey)
        {
            _authKey = authKey;
            _client = new RestClient(serverUri);
            _client.AddHandler("application/json", new RestSharp.Deserializers.JsonDeserializer());
        }

        public void Close()
        {
            var request = new RestRequest("/task/game/finish") { Method = Method.POST };
            request.AddHeader("Authorization", "token " + _authKey);
            var response = _client.Execute(request);


        }

        private enum Side
        {
            Up, Down, Left, Right
        }

        public bool[,] MoveUp() => Move(Side.Up);
        public bool[,] MoveDown() => Move(Side.Down);
        public bool[,] MoveLeft() => Move(Side.Left);
        public bool[,] MoveRight() => Move(Side.Right);


        private bool[,] Move(Side side)
        {
            var sideName = Enum.GetName(typeof(Side), side)?.ToLower();
            if (sideName == null)
                throw new ArgumentException("Wrong side");
            var request = new RestRequest($"/task/move/{sideName}") { Method = Method.POST };
            request.AddHeader("Authorization", "token " + _authKey);
            var response = _client.Execute(request);
            var field = ParseField(response.Content.Substring(1).Substring(0, response.Content.Length - 2));
            return field;
        }

        public static bool[,] ParseField(string content)
        {
            var body = content.Split(new[] { "\\r\\n" }, StringSplitOptions.None);

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



        public Stats GetStats()
        {
            var request = new RestRequest("/task/game/stats") { Method = Method.GET };
            request.AddHeader("Authorization", "token " + _authKey);
            var response = _client.Execute(request);
            var statsJson = response.Content;
            var stats = JsonConvert.DeserializeObject<Dictionary<string, int>>(statsJson);
            return new Stats(stats["words"], stats["points"], stats["moves"]);

        }
        public void Connect()
        {

            var request = new RestRequest("/task/game/start") { Method = Method.POST };
            request.AddHeader("Authorization", "token " + _authKey);
            request.AddParameter("test", true);
            var response = _client.Execute(request);
        }

        public Stats SendWords(IEnumerable<string> words)
        {
            var wordsJson = JsonConvert.SerializeObject(words.ToList());
            var request = new RestRequest("/task/words/") { Method = Method.POST };
            request.AddHeader("Authorization", "token " + _authKey);          
            request.AddJsonBody(words);
            var response = _client.Execute(request);
            var stats = JsonConvert.DeserializeObject<Dictionary<string, int>>(response.Content);
            return new Stats(stats["points"]);
        }
        public class Stats
        {
            public int Words { get; }
            public int Points { get; }
            public int Moves { get; }

            public Stats(int words, int points, int moves)
            {
                Words = words;
                Points = points;
                Moves = moves;
            }

            public Stats(int points) => Points = points;
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Api()
        {
            ReleaseUnmanagedResources();
        }
    }

}