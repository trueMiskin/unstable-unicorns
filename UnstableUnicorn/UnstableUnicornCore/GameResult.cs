using System.Text.Json.Serialization;

namespace UnstableUnicornCore {
    /// <summary>
    /// Data struct for storing game results that can be serialized in JSON
    /// </summary>
    public struct GameResult {
        public int PlayerId { get; init; }
        [JsonIgnore]
        public APlayer Player;
        public string PlayerType => Player.GetType().ToString();
        public int NumUnicorns { get; init; }
        public int SumUnicornNames { get; init; }

        public long AverageResponse => Player.AvarageResponse;

        public GameResult(int playerId, APlayer player, int numUnicorns, int sumUnicornsLen) {
            PlayerId = playerId;
            Player = player;
            NumUnicorns = numUnicorns;
            SumUnicornNames = sumUnicornsLen;
        }
    }
}
