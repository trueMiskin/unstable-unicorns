using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UnstableUnicornCore {
    public enum VerbosityLevel { None, All };

    public class GameRecord {
        public GameSummary Summary { get; init; }
        public List<TurnLog> Detail { get; init; }

        public GameRecord(int gameSeed, List<GameResult> result, List<TurnLog> detail) {
            Summary = new GameSummary { GameResult = result, GameSeed = gameSeed };
            Detail = detail;
        }
    }

    public record struct GameSummary {
        public List<GameResult> GameResult { get; init; }
        public int GameSeed { get; init; }
    }

    public class TurnLog {
        public int Turn { get; init; }

        [JsonIgnore]
        public APlayer PlayerOnTurn;
        public int PlayerIndex => PlayerOnTurn.PlayerIndex;

        public List<ChainLinkLog> BeginningOfTurn { get; init; } = new();
        public List<PlayedCardLog> CardPlaying { get; init; } = new();
        public List<ChainLinkLog> EndOfTurn { get; init; } = new();

        public TurnLog(int turn, APlayer playerOnTurn) {
            Turn = turn;
            PlayerOnTurn = playerOnTurn;
        }
    }

    public record ChainLinkLog {
        public List<EffectLog> Effects { get; init; } = new();
    }
    public record EffectLog {
        public string EffectType { get; init; }
        public string OwningCard { get; init; }
        public List<string> Targets { get; init; }

        public EffectLog(AEffect effect, Card owningCard, List<Card> targets) {
            EffectType = effect.GetType().ToString();
            OwningCard = owningCard.Name;
            Targets = targets.ConvertAll(card => card.Name);
        }
    }

    public record PlayedCardLog {
        public string? CardToResolve { get; init; }
        public List<StackLog> StackResolve { get; init; } = new();
        public List<ChainLinkLog> CardResolve { get; init; } = new();

        public PlayedCardLog(Card? cardToResolve) {
            CardToResolve = cardToResolve?.Name;
        }
    }

    public enum StackTypeLog { Resolved, Reacted }
    public record StackLog {
        public StackTypeLog StackTypeLog { get; init; }
        public string CardName { get; init; }
        public int OwningPlayer { get; init; }

        public StackLog(StackTypeLog stackTypeLog, Card card) {
            StackTypeLog = stackTypeLog;
            CardName = card.Name;
            OwningPlayer = card.Player!.PlayerIndex;
        }
    }

    public class LogUtils {
    }
}
