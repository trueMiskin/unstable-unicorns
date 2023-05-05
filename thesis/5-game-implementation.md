# Game implementation

## Existing implementations

There exists multiple implementations of the game in the Tabletop Simulator.
These implementations are unsuitable for this work because the game is not simulated. All effects are resolved by players. It is only a board game in digital form.

## Used frameworks

The game simulator was implemented in C# .NET 6. We used one of the C# features, Nullable reference types. This feature semantically changes that all reference objects are disallowed by default to store null values. It additionally warns about assigning a null value to a not-null value type. Programming with this feature is a little bit different, but I think it is a really good feature that prevents a lot of bugs. More info about this feature can be found [here](https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references).

The game is implemented as a console application. For unit testing, we used the xUnit framework. We were the most familiar with it.

For the evolutionary algorithm, we used the GenericSharp library. We did not find other usable libraries in C#.

Finally, for the command line parser, we used the System.CommandLine library. It is a new library for parsing command-line arguments. We are using the preview version but it works well. Other libraries have problem with subcommands or parameter restrictions on values. Additionally, the usage is very similar to `argparse` library in Python.

## Core game implementation

The main problem that the simulator must solve is the resolution of effects. Most of the game is about a card being played and the card has these effects. In the game, there are three types of effects: one-time effects, triggered effects and continuous effects. The one-time effects are quite simple. They are added to the effect chain link and are resolved at right time. The trigger effects have a condition and when the condition is met, the effect is added to the effect chain link. From the implementation point of view, the trigger effects can be easily implemented as a subscriber to some event, the trigger condition and the one-time effect that can be added to the effect chain link. The continuous effects are the most complicated because they are always active. Some component must remember the continuous effects and change the behavior of the game according to the continuous effects.

For the implementation, we have one constraint: We want to be able to add new cards and effects to the game. In C#, a library can be loaded and used by reflection. This way, it is very easy to make a plugin to the existing application (for example, a new card). Most card effects can be implemented in this way. However, some may require changes in the simulator itself. The most restricted type of effect is the continuous effect because the components in the simulator must be able to handle the continuous effects. I have hardcoded the continuous effects in the simulator because implementing, for instance, _Triggered effects of your Unicorn cards do not activate._ (Blinding Light) is not possible without editing the simulator.

From the constraint, we can see that the (one-time, trigger) effects cannot be implemented in the core simulator (GameController). Most of the functionality must be implemented in the effect itself and the GameController just resolves the effects (the chain link). For the effect resolution, the best option is to use event-driven programming. Event-driven programming is a pattern where the flow of the program is determined by events such as mouse clicks or key presses. For the simulator, the events are the effects. The cards register the effects (events) and during the resolution of the chain link, all the effects will be resolved. Event-driven programming is a good choice because each effect can override the methods which are called by GameController to implement the effect at the right time. The GameController does not need to know about the effect implementation.

For the trigger effects, we used the observer pattern. The observer pattern is a special type of event-driven programming based on the simple idea where the subject (GameController) notifies the observers (trigger effects) when something happens, for example, a card left stable.

## Steps of the effect resolution

The resolution of one chain link is divided into seven steps:

1. Choosing targets of the effect
2. Triggering the effects that may change the targets of the effects. For example, _If 1 of your Unicorn cards would be destroyed, you may SACRIFICE this card instead._ (Black Knight Unicorn)
3. Triggering the effects that may change the card location. For example, _If this card would be sacrificed, destroyed, or returned to your hand, return it to the Nursery instead._ (Baby Unicorn)
4. Triggering the effects on the event `PreCardLeftStable`. It is all effects that must be resolved before the "owning" card left the stable. The term "owning" means that the effect is written on the card that is leaving the stable. For instance, _If this card is sacrificed or destroyed, you may DESTROY a Unicorn card._ (Stabby The Unicorn)
5. Unregistering all the effects belonging to the card that are targets of some effect in the chain link and they are on the table -- unicorn in stable/upgrade/downgrade. Cards in hand have no effect in the game, therefore, they did not need to be unregistered.
6. Performing the effects in the chain link.
7. Registering all the effects of the cards that were targets of some effect and they are on the table.

The first three steps are not very interesting, but the steps must be in this order because some effect can theoretically save a card from destroy effect like Black Knight Unicorn, but then this card can go back to the hand of the stable owner. The second reason is that it makes sense first to find out that the card is still a target of the effect and afterward to find out where the card should be located after the effect.

Now is the right place to introduce reactive trigger effects. It is a special type of trigger effect that is triggered during chain link resolution and this trigger effect creates an effect that is added to the __current__ chain link. It works as a normal trigger effect, but a normal trigger effect adds effect to the __next__ chain link. Another difference is that a reactive trigger effect is added during resolution -- during step 2 or step 3. This added effect must bear in mind that it will not be called for this effect the `ChooseTargets` function. If the effect needs to set up the card target, remove targets from other effects or change the location of the card, it must be done in the `InvokeReactionEffect` function. On the other hand, the non-reactive effect should never implement the `InvokeReactionEffect` function.

In the fourth step, only the effects of the owning cards that are targeted by the effect should be added to the __next__ chain link. The reason is that this effect cannot be registered during `CardLeftStable` event because the effects of owning a card will be already unregistered (step 5). For the effects that __check__ if some card has left the stable use `CardLeftStable` event. The reason comes from the game rules -- effects must be resolved simultaneously. If card A and card B are targeted by the same or different effect and card B should be triggered when card A left the stable then it does not make sense to trigger this effect because, at the same time, card B is not already in the stable.

The rest of the steps are quite straightforward. Step 5 unregister the effects of the cards that are targeted by the effect. Step 6 performs the effect. If the card has left or entered the stable, the appropriate event is published (`CardLeftStable`, `CardEnteredStable`). Step 7 registers the effects of the cards that have been targeted by the effect.

## Resolving instant cards

Whenever a card is played other players can react to this card by playing instant cards. In the normal game, the order of the instant cards in the stack is determined by who played quicker the card. In the simulator, we cannot use this because the `GameController` asks each player if they want to play an instant card. Then it has a list of reactions of players that want to react and we cannot say this player played the card quicker than the other player. Therefore, we pick randomly the player reaction and this player was the "quickest". Afterward, we ask all players if they want to react to the reaction on the card on the top of the stack. This is repeated until there are no cards on the stack.

## State machine

Some agents want to copy the state of the game and then run simulations of the game.
Getting the state of the game is not free. If some agent copies the current state then when the game started to simulate from that point the simulation must continue from the point where it was copied (even from the middle of the chain link resolution). This is done by two conditions. The `GameController` must be the state machine and all effects must be the state machine too. If the effect does at most one prompt to any player then it is a state machine already otherwise it must remember if some prompt was already asked.

## Copy game state

Coping of the game state is implemented by the function `Clone` in `GameController` which makes a deep copy of all objects (cards, effects, game state). This function has two parameters. The first parameter says which player is making a copy of the game state. It is important because this game is partially observable and the players did not know the same information about other players cards in hand. The second parameter is the mapping function from the original game players/agents to new ones. This makes sense because when you decide which action should you play, you are not 100% sure what the opponent will do. If you copy the game state with actual agents then it will be cheating. Secondly, the agents __are__ part of the game state, therefore, they __cannot be reused__ in the copied game state. Lastly, if we copy the agent which represents a real player then this player will not play hundreds of games for a single decision in the game.

During cloning, we must convert objects of the original game state to new objects. In all base class effects, this is done automatically, but if we create a new effect where we store some additional information then we must convert it too. When we don't convert, for example, a card then we want to move the card to a different location then the card will be moved in the __original__ game state! This can easily occur in trigger effects when some lambda function stores something from the original game state. The way to solve this problem is to store only indexes of the objects. The `GameController` has an `_allCards` property where all cards in the game are stored. In the `Players` property, all players in the game are stored.

## Random actions

Some effects have random elements, such as the pull card effect or some agents want to make random decisions. If every component creates its random number generator, then the game will not be reproducible. This is bad for several reasons. The debugging of the game simulator is much harder. When we do experiments, then we cannot rerun the experiment with the same random seed. For that reason, the `GameController` has a `Random` property that provides a global random number generator for the game. We use this random number generator whenever we need to generate a random number.

## Creating a new card

Creating a new card is quite simple. Create a new class that inherits from `CardTemplateSource`. This class must implement one method which returns a `CardTemplate` object. The `CardTemplateSource` has one field called `EmptyCard` that returns a new empty object of `CardTemplate`. Then sets the properties of the card by the fluent syntax. At least should be set `Name` and `CardType`.

It is possible to add one or more effects to the card. We can add a one-time effect by the `Cast` method. Other method names are self-explanatory. The methods can be called multiple times for adding more effects of the same type. The methods accept effect factories (`FactoryEffect` or `ContinuousFactoryEffect`) because the effect on the card can be resolved multiple times during one game. The easiest way to solve this problem is to create a new effect whenever the effect must be added to the effect chain link or whenever a new continuous effect is registered.

## Creating a new effect

When we want to create a new one-time effect, we inherit the `AEffect` base class. When we make a more specific effect, for example, the destroy effect, then we inherit one of the destroy effects that are already implemented. The reason is that some effects are activated on destroy effect, and a check, if the effect inherits from this class, is done.

The base class implements a helper function on validation selection, maintaining the cards that are already selected in the current chain link and removing the card from that list.

Some effects can give us (as a player) additional information, for instance, which cards the other player has in his hand. For this situation, there is the `CardVisibilityTracker` component in `GameController` that is maintaining this information. On other hand, the same or other effects can remove this information, for example, the choose effect. Player A see player B's cards and took one of them. Player A knows all cards of player B but other players know nothing about player B's cards. Even if before the choose effect, player C knew all cards of player B then after the choose effect, player C did not know which card was took by player A. We must somehow maintain this knowledge for player C and the easy solution is just to remove all information about player B's cards. 

The continuous effects must inherit the `AContinuousEffect` base class. The continuous effect implementation is much simpler than the one-time effect. Only override the method which you want to implement.
The trigger effect is only a composition of the trigger predicate, the list of events to listen and the effect factory for the one-time effect.

## Creating a new agent

Creating a new agent is not complicated. We need to create a new class that inherits from the `APlayer` base class and implements all methods. Each method has a short description of when this method is used. There is one condition, the agent __should not cheat__. Making an agent that cheats is easy because it can look at other player cards -- all information is accessible through `GameController`. We can add some cards to the agent's hand by `GameController`. We can find out who other players are (for instance, for cooperation) and many other things.

Additionally, the agent should not modify the list of available actions. The list can be a list of effects, cards or players. Do not modify it because effects do not make a copy of this list before the agent is called. It is not hard to fix this problem, but this solution was chosen primarily due to performance reasons (not to make lots of list copies).