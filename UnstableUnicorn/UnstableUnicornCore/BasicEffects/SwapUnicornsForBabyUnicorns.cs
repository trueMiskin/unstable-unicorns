using System;
using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// Swap all unicorns for baby unicorns, but this effect
    /// blocking triggering of unicorns effect except baby unicorns
    /// <see cref="AEffect.IsBlockTriggeringUnicornCards(Card, ECardType)"/>
    /// 
    /// This effect extends destroy effect, because original cards are in discard pile
    /// </summary>
    public class SwapUnicornsForBabyUnicorns : DestroyEffect {
        MoveBabyUnicornsToStable thenEffect;
        int numberCardsToSwap = 0;
        public SwapUnicornsForBabyUnicorns(Card owningCard) : base(owningCard, Int32.MaxValue, ECardTypeUtils.UnicornTarget) {
            _blockTriggeringUnicornCardsEnabled = true;

            TargetLocation = CardLocation.DiscardPile;

            thenEffect = new MoveBabyUnicornsToStable(OwningCard);
        }

        public override void ChooseTargets(GameController gameController) {
            var players = OwningPlayer.ChoosePlayers(1, true, this);

            ValidatePlayerSelection(1, players, gameController.Players);

            TargetOwner = players[0];
            CardTargets = gameController.GetCardsOnTable().FindAll(card => card.Player == TargetOwner &&
                ECardTypeUtils.UnicornTarget.Contains(card.CardType));
            // save number selected targets because baby unicorns will trigger effect to go back to nursery
            // i cant exclude baby unicorns from this effect because cards like Barbed wire will work wrong
            numberCardsToSwap = CardTargets.Count;
        }

        public override void InvokeEffect(GameController gameController) {
            foreach (var target in CardTargets)
                target.MoveCard(gameController, null, CardLocation.DiscardPile);

            thenEffect.setUp(numberCardsToSwap, TargetOwner);
            gameController.AddNewEffectToChainLink(thenEffect);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
        
        public override AEffect Clone(Dictionary<Card, Card> cardMapper,
                                      Dictionary<AEffect, AEffect> effectMapper,
                                      Dictionary<APlayer, APlayer> playerMapper) {
            var newEffect = (SwapUnicornsForBabyUnicorns)base.Clone(cardMapper, effectMapper, playerMapper);
            newEffect.thenEffect = (MoveBabyUnicornsToStable)thenEffect.Clone(cardMapper, effectMapper, playerMapper);
            effectMapper.Add(thenEffect, newEffect.thenEffect);

            return newEffect;
        }

        class MoveBabyUnicornsToStable : AEffect {
            public MoveBabyUnicornsToStable(Card owningCard) : base(owningCard, 0) {
                TargetLocation = CardLocation.OnTable;
            }

            public void setUp(int cardCount, APlayer targetOwner) {
                _cardCount = cardCount;
                TargetOwner = targetOwner;
            }

            public override void ChooseTargets(GameController gameController) {}

            public override void InvokeEffect(GameController gameController) {
                if (TargetOwner == null)
                    throw new InvalidOperationException($"{nameof(TargetOwner)} cannot be null");

                gameController.PlayerGetBabyUnicornsOnTable(TargetOwner, _cardCount);
            }

            public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
        }
    }
}
