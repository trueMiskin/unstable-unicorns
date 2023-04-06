# Introduction

Unstable Unicorns is a game from 2017 that started on Kickstarter.
When the campaign on Kickstarter ended 33,720 backers pledged $1,865,140[^wiki].

[^wiki]: From the [Wikipedia](http://unstablegameswiki.com/index.php?title=2017_Kickstarter) article.

Kickstarter is a crowdfunding platform where you present your idea/work and you
are finding funds to start/finish/make more copies of the project.
Backers are persons who donate to specific projects and typically
they get a copy of the product or a reward for their donation (for example
t-shirt, tin figurine etc.). The campaign typically has more tiers of rewards
based on the amount of donation.

The game become popular and the creators of the game released several expansions.
Unfortunately, the expansions are not translated into the Czech language.

The game is a card game for 2-8 players. Each player has a stable of unicorns
and the goal is to be the first player to have 6 or 7 unicorns (based on the number
of playing players) in their stable after all effects are resolved.

- 2-5 players: 7 unicorns
- 6-8 players: 6 unicorns

If more players have most of the unicorns in their stable then the player with the
biggest sum of letters in the names of their unicorns wins. If more players have
the same biggest sum of letters then everyone loses.

In this thesis, all games will be played with 6 players and the win condition is
to have 6 unicorns in your stable. The game implementation itself can handle a different
number of players but the AI agents can play different strategies based on the
number of players.

All players start with 5 cards in their hand and 1 unicorn called "Baby Unicorn"
in their stable. The game is turn-based and each turn has 4 phases:

- Beginning of Turn phase
- Draw phase
- Action phase
- End of Turn phase

At the beginning of the turn, there are resolved all effects which are triggered at
the beginning of the turn. Then the player draws a card from the deck and can play
normally one card from their hand.

After the player announces that he wants to play a given card, other players can play
cards as a reaction.
In the base game, there are only 2 cards that can be played as reaction cards:
"Neigh" and "Super Neigh". In the expansions, there are a lot more cards that
can be played as reaction cards.
Played cards are stacked on top of each other. When nobody wants to play a card, the
top card is resolved. Then anybody can play a card as a reaction to the card which
is on the top of the stack. This process is repeated until the stack is empty.

After the action phase, all effects which are triggered at the end of the turn are resolved.

This is a brief overview of the game. The game has a lot of rules, cards and effects
which will be described in the next chapter.

This thesis has several goals: The first goal of this thesis is to implement the game
itself because there is no implementation of the game available except implementation
to the Tabletop Simulator game engine which is not suitable for this work.
The second goal is to implement AI agents that can play the game. The third goal is
to compare the performance of the implemented AI agents.