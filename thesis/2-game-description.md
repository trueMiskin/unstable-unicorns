# 2. Game description

The following chapter describes the mechanics of the game in detail.

First, I will describe the terminology used in the game. It has a lot of keywords
so the first section can be used as a glossary.
Then we will look at the effects in more detail.
After that, I will describe how are resolved effects of cards.
And finally, we will talk about impossible actions.

## 2.1 Terminology

Terminology of places (card **sources**) where cards can be stored:

- **Stable** - The area where the player Unicorn cards, Upgrades cards
and Downgrades cards.
- **Nursery** - The area where baby unicorns are stored when no play owns them.
- **Deck** - The face-down pile of cards from which the players draw cards.
- **Discard pile** - The face-up pile of cards where the players put cards
when they are "destroyed" or "played". In magic the gathering, this place
is called a "graveyard".

Keywords that can be used on cards or are used in the thesis. A lot of keywords can be similar to keywords from other card games but sometimes the meaning can be slightly different.

- **Sacrifice** - Send a card from **your** stable to the discard pile.
- **Destroy** - Send a card from **any other** player's stable to the discard pile.
- **Steal** - Move a card from **any other** player's stable into **your** stable.
- **Discard** - Send a card from **your** hand to the discard pile.
- **Pull** - The player gets a random card from the source and put it into his hand.
- **Choose** - The player chooses a card from the source and put it into his hand.
- **Search** - The effect is similar to choose card effect. The player chooses a
card from the source, this card is **revealed** to all players and put into his hand.
- **Draw** - Pull a card from the deck (Draw a card from the top of the pile).
- **Play** - When the card is being played, any other player can react to the card by instant card.
- **Bring** - The brought card to the stable can not be Neigh'd (is not possible to disallow put card to the stable) but can be played any other instant card. In the base game, there are only "Neigh" and "Super Neigh" cards so any card can be played.

Each card has one of the following types:

- **Unicorn** card - The card is a unicorn and can be put into **any** stable. The unicorn type has one of these subtypes:
    - **Baby unicorn** card - The baby unicorn can occur only in the nursery or in the player's stable.
    - **Basic unicorn** card - The unicorn without any special effects.
    - **Magic unicorn** card - The unicorn that has some effect.
- **Upgrade** card - The card grants some positive effects. The upgrade card can be played into **any** stable.
- **Downgrade** card - The card grants some negative effects. The downgrade card can be played into **any** stable.
- **Magic** card - The card with a one-time effect (simply spell), it takes effect immediately when it is played and then is discarded.
- **Instant** card - The card that can be played at any time when any other player plays a card (unless the card says otherwise). If the instant card is played, then you can react to that card by playing the instant card (even though you already played the card this turn). Any number of instant cards can be chained during a single turn.

Some effects target one or more players but sometimes can be unclear if the statement includes you or not. Therefore, there is precise wording in the effect description. The following terminology for player targeting is used:

- **Any player** - Refers to any player in the game, __including__ you.
- **Any other player** - Refers to any player in the game, __excluding__ you.
- **Each player** - Refers to each player in the game, __including__ you.
- **Each other player** - Refers to each player in the game, __excluding__ you.
- **Any number of players** - Refers to any number of players you choose, __excluding__ you.

## 2.2 Types of effects

There are three different types of effects in the game: one-time effect, continuous effect and trigger effect. The one-time effect has a disposable immediate impact on the game. For example, Steal a unicorn card.
The continuous effect is active as long as the card is in your stable. For instance, All your unicorns are pandas. This effect can be beneficial because all effects which target unicorns can not target your pandas but you can not win, because you have no unicorns in your stable!
The trigger effect is activated when some event occurs. For example, If this card is in your stable at beginning of your turn, you may draw an extra card. The meaning of an extra card is that you may draw a card in the draw phase if you have drawn a card. If some effect disallows you to draw a card or even skip the draw phase, you can not draw an extra card.

Some effects like "draw an extra card" are good to consult more detailed rules that can be found on the official wiki. Sometimes there are described some special cases.

Effects can be combined in different ways. The first combination is simple "and". Perform the first and the second effect simultaneously. The second combination "then",
first must be resolved the first effect, then the second effect.
The last combination is conditional "if you do". The effect after "if you do" is performed only if the player performed the first effect. For example, If this card is in your stable at the beginning of your turn, you may sacrifice a card. If you do, destroy a card.

TODO: after for example, should be an uppercase letter?
TODO: should be edited texts from cards?

## 2.3 Resolving of cards effects

In this section, I will describe the core of the game mechanics - how are resolved effects of the cards.

At the same time must be resolved one or more effects. Typically more effects must be resolved at the beginning of the turn or the end of the turn. Effects that must be resolved at the same time must be resolved simultaneously and independently. These effects are in the first chain link. After the first chain link is resolved simultaneously and independently, some effects may be triggered as a reaction to the first chain link. All these triggered effects form the second chain link. This is repeated until the next chain link is empty. The card is resolved when the whole effect chain is resolved (all chain links are resolved).

TODO: picture of the chain link

This approach to how are resolved effects can be a little bit confusing because even people typically think in sequential order. Additionally, these have some consequences for instance, when two effects can be activated at beginning of the turn, then you must decide which effects will be activated and resolve them simultaneously. You can not activate the first effect, see the result and then decide if you activate the second effect.

## 2.4 Impossible actions

Effects can be mandatory or optional. Mandatory effects must be resolved. All **magic** cards have mandatory effects. **Unicorn** cards can have mandatory or optional effects. Optional effects use the "may" in the effect description.

In general, players should not play cards with mandatory effects that can not be resolved during unloading the card/when a player says he wants to play a given card, for instance, you play a card with the effect discard a card but you have not any card in your hand. But is fine, when you have only a "Neigh" card in your hand and you play this card during the reaction phase. In this case, you do not discard any card (your hand is empty) and you do not violate the rules.

Sometimes another player plays effect or some card in your stable says that you must do some impossible action. In this case, this effect is ignored. There are some examples of this situation:

- **Discard** a card from your hand when you have not any card in your hand.
- **Sacrifice** a card from your stable when you have not any card in your stable or you have a card in your stable but it can not be sacrificed.