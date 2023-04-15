# Game implementation

## Existing implementations

There exists multiple implementations of the game in the Tabletop Simulator.
These implementations are not suitable for this work because the game itself
is not simulated. All effects are resolved by players. It is only a board game in digital form. This type of implementation is good during the coronavirus lockdown when you can't play the game in person and the only option is to play online.

## Used frameworks

The game simulator was implemented in C# .NET 6. The game is implemented as a console application. For unit testing, I used the xUnit framework. I did not spend much time choosing unit testing frameworks because the frameworks are a lot similar and the only difference is syntax. I chose this framework because I was the most familiar with it. I had written a lot of tests in the MSTest framework and I did not recognize a lot of differences. The unit testing is an important part of the project because multiple times I made a small change and a lot of other functionality quietly break. Without unit testing, I maybe would not notice the problem or noticed it after a long time.

For the evolutionary algorithm, I used the GenericSharp library. At first look, it seemed like a good choice and I did not find other usable libraries in C# but I did know if I chose correctly. Maybe for me was best to implement it myself and I would not have to spend time watching at the library because there is not practically any documentation. On other hand, the library source code is not complicated because the genetic algorithms are not complicated but sometimes you realize that some functionality behaves differently than you expect.

## Core game implementation

The main problem that the simulator must solve is the resolution of effects. The most of game is about a card is played and the card has these effects. In the game, there are three types of effects: one-time effects, triggered effects and continuous effects. The one-time effects are quite simple, there are added to the effect chain link and at right time are resolved. The trigger effects have a condition and when the condition is met, the effect is added to the effect chain link. From the implementation point of view, the trigger effects can be easily implemented as subscriber to some event, the trigger condition and the one-time effect that can be added to the effect chain link. The continuous effects are the most complicated because they are always active. Some component must remember the continuous effects and change the behavior of the game according to the continuous effects.

For the implementation we have one constraint: We want to be able to add new cards and effects to the game. In the C# is possible to load a library and use it by reflection. This way is very easy to make a plugin to the existing application (for example, a new card). Of course, I do not expect that every effect which you imagine can be implementable without an editing simulator but be open as possible. The most restricted type of effect is the continuous effect because the components in the simulator must be able to handle the continuous effects. I decide to hardcode the effects of continuous effects in the simulator because to implement for instance _Triggered effects of your Unicorn cards do not activate._ (Blinding Light) I think is not possible without editing the simulator.

From the constraint, we can see that the (one-time, trigger) effects can not be implemented in the core simulator (GameController). Most of the functionality must be implemented in the effect itself and the GameController just do the resolution of the effects (resolving the chain link). For the effect resolution, the best option was to use event-driven programming. Event-driven programming is a pattern where the flow of the program is determined by events such as mouse clicks or key presses. For the simulator, the events are the effects. The cards register the effects (events) and during the resolution of the chain link, all the effects will be resolved. Event-driven programming is a good choice because each effect can override the methods which are called by GameController to implement the effect at the right time. The GameController does not need to know about the effect implementation.

For the trigger effects, I used the observer pattern. The observer pattern is a special type of event-driven programming based on the simple idea where the subject (GameController) notifies the observers (trigger effects) when something happens, for example, a card left stable.

## Steps of the effect resolution

The resolution of one chain link is divided into seventh steps:

1. Choosing targets of the effect
2. Triggering the effects that may change the targets of the effects. For example, _If 1 of your Unicorn cards would be destroyed, you may SACRIFICE this card instead._ (Black Knight Unicorn)
3. Triggering the effects that may change the card location. For example, _If this card would be sacrificed, destroyed, or returned to your hand, return it to the Nursery instead._ (Baby Unicorn)
4. Triggering the effects on the event `PreCardLeftStable`. It is all effects that must be resolved before the "owning" card left the stable. The term "owning" means that the effect is written on the card that is leaving the stable. For instance, _If this card is sacrificed or destroyed, you may DESTROY a Unicorn card._ (Stabby The Unicorn)
5. Unregistering all the effects belonging to the card that are targets of some effect in the chain link and they are on the table -- unicorn in stable/upgrade/downgrade. Cards in hand have no effect in the game, therefore, they did not need to be unregistered.
6. Performing the effects in the chain link.
7. Registering all the effects of the cards that were targets of some effect and they are on the table.

The first three steps are not much interesting but the steps must be in this order because some effect can theoretically save a card from destroy effect like Black Knight Unicorn but then this card can go back to the hand of the stable owner. This card would be a little bit strong but now it does not matter. The second reason is that it makes sense firstly to find out that the card is still a target of the effect and afterward to find out where the card should be located after the effect.

Now is the right place to introduce reactive trigger effects. It is a special type of trigger effect that is triggered during chain link resolution and this trigger effect creates an effect that is added to the __current__ chain link. It seems to be like a normal trigger effect but a normal trigger effect adds effect to the __next__ chain link. Another difference is that a reactive trigger effect is added during resolution -- during step 2 or step 3. This added effect must bear in mind that this effect will not be called `ChooseTargets` function. If the effect needs to set up the card target, remove targets from other effects or change the location of the card, it must be done in the `InvokeReactionEffect` function. On other hand, the non-reactive effect should never implement the `InvokeReactionEffect` function.

In the fourth step, only the effects of the owning cards that are targeted by the effect should be added to the __next__ chain link. The reason is that this effect can not be registered during `CardLeftStable` event because the effects of owning a card will be already unregistered (step 5). For the effects that __check__ if some card has left the stable use `CardLeftStable` event. The reason comes from the game rules -- effects must be resolved simultaneously. If card A and card B are targeted by the same or different effect and card B should be triggered when card A left the stable then it does not make sense to trigger this effect because, at the same time, card B is not already in the stable.

The rest of the steps are quite straightforward. Step 5 unregister the effects of the cards that are targeted by the effect. Step 6 performs the effect. If the card has left or entered the stable, the appropriate event is published (`CardLeftStable`, `CardEnteredStable`). Step 7 registers the effects of the cards that have been targeted by the effect.

## State machine

Some agents want to copy the state of the game and then run simulations of the game.
Getting the state of the game is not the property for free. If some agent copies the current state then when the game started to simulate from that point the simulation must continue from the point where was copied (even from the middle of the chain link resolution). This is done by two conditions. The `GameController` must be the state machine and all effects must be the state machine too. If the effect does at most one prompt to any player then it is a state machine already otherwise it must remember if some prompt was already asked.

## Copy game state

Copy game state is implemented by the function `Clone` in `GameController` which makes a deep copy of all objects (cards, effects, game state). This function has two parameters. The first parameter says which player making a copy of the game state. It is important because this game is partially observable and the players did not know the same information about other players cards in hand. The second parameter is the mapping function from the original game players/agents to new ones. This makes sense because when you decide which action should you play, you are not 100% sure what the opponent will do. If you copy the game state with actual agents then it will be cheating. Secondly, the agents __are__ part of the game state, therefore, they __cannot be reused__ in the copied game state. Lastly, if you copy the agent which represents a real player then this player will not play hundreds of games for a single decision in the game.

During cloning, we must convert objects of the original game state to the new objects. In all base class effects, this is done for you but if you create a new effect where you store some additional information then you must convert it too. When we don't convert, for example, a card then we want to move the card to a different location then the card will be moved in the __original__ game state! This can easily occur in trigger effects when some lambda function stores something from the original game state. The way to solve this problem is to store only indexes of the objects. The `GameController` has an `_allCards` property where is stored all cards in the game. In the `Players` property is stored all players in the game.

## Creating a new card

Creating a new card is quite simple. Create a new class that inherits from `CardTemplateSource`. This class must implement one method which returns a `CardTemplate` object. The `CardTemplateSource` has one field called `EmptyCard` that returns you a new empty object of `CardTemplate`. Then by the fluent syntax set the properties of the card. At least you should set `Name` and `CardType`.

You can add one or more effects to the card. By the `Cast` method, you can add a one-time effect other method names are self-explanatory. The methods can be called multiple times for adding more effects of the same type. The methods want effect factories (`FactoryEffect` or `ContinuousFactoryEffect`) because the effect on the card can be resolved multiple times during one game. The easiest way to solve this problem is to create a new effect whenever the effect must be added to the effect chain link or register the new continuous effect.

## Creating a new effect

When you want to create a new one-time effect then inherit the `AEffect` base class. When you make a more specific effect, for example, the destroy effect then inherit one of the destroy effects that are already implemented. The reason is that some effects are activated on destroy effect and the checking is done if the effect inherits some class.

The base class implements a helper function on validation selection, maintaining the cards that are already selected in the current chain link and removing the card from that list.

Some effects can give us (as a player) additional information, for instance, which cards the other player has in his hand. For this situation, there is the `CardVisibilityTracker` component in `GameController` that is maintaining this information. On other hand, the same or other effects can remove this information, for example, the choose effect. Player A see player B's cards and took one of them. Player A knows all cards of player B but other players know nothing about player B's cards. Even if before the choose effect, player C knew all cards of player B then after the choose effect, player C did not know which card was took by player A. We must somehow maintain this knowledge for player C and the easy solution is just to remove all information about player B's cards. 

The continuous effects must inherit the `AContinuousEffect` base class. The continuous effect implementation is much simpler than the one-time effect. Only override the method which you want to implement.
The trigger effect is only a composition of the trigger predicate, the list of events to listen and the effect factory for the one-time effect.

## Creating a new agent

Creating a new agent is not much complicated. Create a new class that inherits the `APlayer` base class and implements all methods. Each method has a short description of when this method is used. There is one condition, the agent __should not cheat__. To make an agent that cheats is easy because you can look at other player cards -- all information is accessible through `GameController`. You can add some cards to your hand by `GameController`. You can find out who is other players (for instance, for cooperation) and many other things.

Additionally, the agent should not modify the list of available actions. The list can be a list of effects, cards or players. Do not modify it because effects do not make a copy of this list before the agent is called. It is not hard to fix this problem but this solution was choosen primarly due to performance reasons (do not make tons of list copies). 