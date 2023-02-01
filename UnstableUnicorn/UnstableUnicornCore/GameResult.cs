namespace UnstableUnicornCore {
    public struct GameResult {
        public int PlayerId;
        public APlayer Player;
        public int NumUnicorns;
        public int SumUnicornNames;

        public GameResult(int playerId, APlayer player, int numUnicorns, int sumUnicornsLen) {
            PlayerId = playerId;
            Player = player;
            NumUnicorns = numUnicorns;
            SumUnicornNames = sumUnicornsLen;
        }
    }
}
