# Rule and game modifications

I have implemented the core game to be close as possible to the original rules.
One rule is a little bit strange. The rule says that _If multiple triggered effects require a target, the same card may not be selected twice in the same link._[^effects]. This rule is clear for one player but how should work for 2 or more players that are targeting at the same time? This is practically impossible to obey this rule because targeting must be independent and simultaneous. I implemented this rule in a way that the same card could be selected at most once in the same link but is not independent (if the first effect targets card A then the second effect knows that card A is already targeted). I do not choose the second option -- not implementing the rule at all because then must be solved which effect is performed and which is not.
For instance, when the same card is targeted by the steal effect and by the destroy effect which effect should "win"? Additionally, the rule says that the effects are resolved simultaneously.

[^effects]: From the [official wiki](http://unstablegameswiki.com/index.php?title=UU_Game_Terminology_-_Effects)

The solution is the effects are not resolved independently. The effects are resolved in the order when they were added to the chain link. But it still mimics that effects are simultaneously resolved. All effects see the same state and they are independent except for the targeting.

The rule which is not implemented at all are following sentences after the previous rule:
_If there are not enough cards to target, you may only trigger as many effects as you have options for targets. You must select targets for mandatory effects before selecting targets for optional effects._[^effects]. In the implementation, I do not distinguish between mandatory and optional effects during the resolution of the chain link. However, I still check the requirements of a card when a player plays the card. For instance, if a player plays a spell card with the effect then he must hold at least one other card in his hand. In the implementation, I do not trigger as many effects as a player has options for targets because all triggered effects are added to the chain link and as I said previous paragraph, the effects are resolved in the order when they were added to the chain link.

Then I do not implement some cards. First card is "Nanny cam" with effect _Your hand must be visible to all player at all times._ This decision was firstly made because I do want to implement the tracking visibility of cards. After I realized I must implement it for Monte Carlo Agent then I still do not decide to implement this card. The reason is very simple, no agent use it and the real impact of this card is arguable.

After some time I find out that I did implement more cards: "Angel Unicorn", "Extremely Destructive Unicorn", "Extremely Fertile Unicorn", "Llamacorn", "Magical Kittencorn", "Unicorn Phoenix" and "Zombie Unicorn". The reason was I used this website[^unicorndb] and these cards were not included. I think it does not matter because the game still contains 119 cards and for testing the agents, it is not necessary to have all cards.

[^unicorndb]: https://www.unicornsdatabase.com/packs/base