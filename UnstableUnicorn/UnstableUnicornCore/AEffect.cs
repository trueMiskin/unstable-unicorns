using System;
using System.Collections.Generic;
using System.Linq;
using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore {
    public abstract class AEffect {
        /// <summary>
        /// Which card belongs this affect
        /// </summary>
        public Card OwningCard { get; private set; }

        /// <summary>
        /// Player who owned this card in time, when this card
        /// was played
        /// 
        /// DON'T USE `OwningCard.Player` because when card is spell
        /// than this value will be resetted on null!
        /// </summary>
        public APlayer OwningPlayer { get; private set; }

        /// <summary>
        /// Number card to discard
        /// </summary>
        public int CardCount { get; protected set; }

        /// <summary>
        /// Which cards are targets of affect
        /// </summary>
        public List<Card> CardTargets { get; protected set; } = new();

        /// <summary>
        /// Where will be card after effect
        /// </summary>
        public CardLocation TargetLocation { get; protected set; }

        /// <summary>
        /// Who will owner of card after effect
        /// </summary>
        public APlayer? TargetOwner { get; protected set; }

        protected bool _blockTriggeringUnicornCardsEnabled = false;
        public bool IsBlockTriggeringUnicornCards(Card card, ECardType cardType) {
            if (!_blockTriggeringUnicornCardsEnabled)
                return false;

            return UnicornTriggerEffectsCantBeActivated.IsBlockTriggeringUnicornCards(card, cardType);
        }

        public AEffect(Card owningCard, int cardCount) {
            OwningCard = owningCard;

            if (owningCard.Player == null)
                throw new InvalidOperationException("When constructing effect player who owns card must be setted!");

            OwningPlayer = owningCard.Player;
            CardCount = cardCount;
        }

        /// <summary>
        /// Choosing targets of effect
        /// 
        /// This method should check if targets are valid and is selected required
        /// number of cards if is possible to do it
        /// 
        /// This method should not append to list `cardsWhichAreTargeted`
        /// </summary>
        public abstract void ChooseTargets(GameController gameController);

        /// <summary>
        /// Check if card meets criteria to play
        /// 
        /// This method should be overrided only by if effect.
        /// Else this requirement will be required on every time when you want play
        /// card with this effect even if effect by itself have no condition.
        /// </summary>
        /// <param name="gameController"></param>
        /// <returns></returns>
        public virtual bool MeetsRequirementsToPlay(GameController gameController) => true;

        /// <summary>
        /// Requirement which will be checked in conditional effect.
        /// </summary>
        /// <param name="gameController"></param>
        /// <returns></returns>
        public abstract bool MeetsRequirementsToPlayInner(GameController gameController);
        public abstract void InvokeEffect(GameController gameController);

        /// <summary>
        /// Some effect are reacting on other effects like card Black knight Unicorn
        /// which sacrifice himself instead other card
        /// <br/>
        /// This method call <see cref="TriggerEffect"/> in situation when is published
        /// <see cref="ETriggerSource.ChangeTargeting"/> or <see cref="ETriggerSource.ChangeLocationOfCard"/>
        /// <br/>
        /// DON'T forget to ADD new effect to <see cref="GameController.AddEffectToActualChainLink(AEffect)"/>
        /// This method should not do actual effect, only preparing. Real execution of effect should
        /// be in <see cref="InvokeEffect(GameController)"/>
        /// </summary>
        /// <param name="gameController"></param>
        /// <param name="effect"></param>
        public virtual void InvokeReactionEffect(GameController gameController, AEffect effect) {
            throw new NotImplementedException($"{this} is not reaction effect or is not implemented.");
        }

        /// <summary>
        /// Helper function that removes cards that are targeted by another effect
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="gameController"></param>
        /// <returns></returns>
        protected List<Card> RemoveCardsWhichAreTargeted(List<Card> cards, GameController gameController) {
            cards.RemoveAll(card => gameController.CardsWhichAreTargeted.ContainsKey(card));
            return cards;
        }

        /// <summary>
        /// Check if player don't select items more times, check if is selected enough items
        /// and check if selected items are from available selection (not selected some random items)
        /// </summary>
        /// <typeparam name="T">Item type (typically <see cref="APlayer"/> or <see cref="Card"/>)</typeparam>
        /// <param name="requiredSelectionSize"></param>
        /// <param name="playerSelection"></param>
        /// <param name="availableSelection"></param>
        protected void ValidatePlayerSelection<T>(int requiredSelectionSize, List<T> playerSelection, List<T> availableSelection) {
            // check if player don't select same card/player more times
            // if is found first duplication -> stop adding to Hashset
            var nonDuplicateSelection = new HashSet<T>();
            playerSelection.Any(item => !nonDuplicateSelection.Add(item));

            if (nonDuplicateSelection.Count != playerSelection.Count)
                throw new InvalidOperationException("Selected items have duplication.");

            if (playerSelection.Count != requiredSelectionSize)
                throw new InvalidOperationException("Not selected enough items.");

            if (!playerSelection.All(item => availableSelection.Contains(item)))
                throw new InvalidOperationException("Selected item which is not from available selection.");
        }

        /// <summary>
        /// Check if card is not already selected and update which cards are selected now
        /// <br/>
        /// Should be called after <see cref="ValidatePlayerSelection{T}(int, List{T}, List{T})"/>
        /// </summary>
        /// <param name="previousSelection"></param>
        /// <param name="selectionNow"></param>
        /// <param name="gameController"></param>
        protected void CheckAndUpdateSelectionInActualLink(List<Card> previousSelection, List<Card> selectionNow, GameController gameController) {
            var cardSet = gameController.CardsWhichAreTargeted;

            foreach (var card in previousSelection)
                if (!cardSet.Remove(card))
                    throw new InvalidOperationException("Something goes wrong...");

            foreach (var card in selectionNow) {
                if (cardSet.ContainsKey(card))
                    throw new InvalidOperationException("Selected card is already targeted by another effect");
                cardSet.Add(card, this);
            }
        }

        /// <summary>
        /// Deep copy effect
        /// 
        /// Method doesn't reset the <see cref="OwningPlayer"/> and <see cref="TargetOwner"/>
        /// 
        /// Effects can have stored inner effect so <paramref name="effectMapper"/> is needed
        /// </summary>
        /// <param name="cardMapper"></param>
        /// <param name="effectMapper"></param>
        /// <param name="playerMapper"></param>
        /// <returns></returns>
        public virtual AEffect Clone(Dictionary<Card, Card> cardMapper,
                                     Dictionary<AEffect, AEffect> effectMapper,
                                     Dictionary<APlayer, APlayer> playerMapper) {
            AEffect newEffect = (AEffect)MemberwiseClone();

            newEffect.OwningCard = cardMapper[newEffect.OwningCard];
            newEffect.CardTargets = CardTargets.ConvertAll(c => cardMapper[c]);

            newEffect.OwningPlayer = playerMapper[OwningPlayer];
            newEffect.TargetOwner = TargetOwner == null ? null : playerMapper[TargetOwner];

            return newEffect;
        }
    }
}
