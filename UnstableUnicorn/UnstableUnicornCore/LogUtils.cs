using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UnstableUnicornCore {
    public enum VerbosityLevel { None, All };

    public class GameRecord {
        public GameSummary Summary { get; init; }
        public List<TurnLog> Detail { get; init; }

        public GameRecord(int gameSeed, int gameLength, List<GameResult> result, List<TurnLog> detail) {
            Summary = new GameSummary { GameResult = result, GameSeed = gameSeed, GameLength = gameLength };
            Detail = detail;
        }
    }

    public record struct GameSummary {
        public List<GameResult> GameResult { get; init; }
        public int GameSeed { get; init; }
        public int GameLength { get; init; }
    }

    public class TurnLog {
        public int Turn { get; init; }

        [JsonIgnore]
        public APlayer PlayerOnTurn;
        public int PlayerIndex => PlayerOnTurn.PlayerIndex;

        public List<ChainLinkLog> BeginningOfTurn { get; init; } = new();
        public List<PlayerCards> PlayerCardsAfterBot { get; init; } = new();
        public List<PlayedCardLog> CardPlaying { get; init; } = new();
        public List<ChainLinkLog> EndOfTurn { get; init; } = new();

        public TurnLog(int turn, APlayer playerOnTurn) {
            Turn = turn;
            PlayerOnTurn = playerOnTurn;
        }
    }

    public record PlayerCards {
        public List<string> Hand { get; init; }
        public List<string> Stable { get; init; }
        public List<string> Upgrades { get; init; }
        public List<string> Downgrades { get; init; }

        string CardToName(Card card) => card.Name; 

        public PlayerCards(APlayer player) {
            Hand = player.Hand.ConvertAll(CardToName);
            Stable = player.Stable.ConvertAll(CardToName);
            Upgrades = player.Upgrades.ConvertAll(CardToName);
            Downgrades = player.Downgrades.ConvertAll(CardToName);
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
            Targets = targets.ConvertAll(card => string.Format("{0}; {1}; {2}", card.Name, card.Player?.PlayerIndex, Enum.GetName(typeof(CardLocation), card.Location)));
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
