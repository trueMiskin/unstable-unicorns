/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace UnstableUnicornCore.Agent {
    public class DefaultConsolePlayer : APlayer {
        protected override bool ActivateEffectCore(AEffect effect) {
            Console.WriteLine("Do you want activate effect {0} of card {1}?", effect, effect.OwningCard.Name);

            string? ans = "";
            while (ans == "") {
                Console.Write("Activate? Y/N: ");
                ans = Console.ReadLine()?.ToLower();

                switch (ans) {
                    case "y":
                    case "yes":
                        ans = "y";
                        break;
                    case "n":
                    case "no":
                        ans = "n";
                        break;
                    default:
                        Console.WriteLine("Unknow answer! Try again.");
                        ans = "";
                        break;
                }
            }

            return ans == "y";
        }

        private List<APlayer> AskOnPlayerSelection(string msg, int number, List<APlayer> players) {
            Console.WriteLine(msg);

            List<bool> selected = new();
            foreach (APlayer _ in players)
                selected.Add(false);

            for (int i = 0; i < number; i++) {
                Console.WriteLine("Available players:");
                for (int pIdx = 0; pIdx < players.Count; pIdx++) {
                    APlayer player = players[pIdx];
                    if (!selected[pIdx])
                        Console.WriteLine("{0} - player {1} (hand size {2}, stable {3}, downgrades {4}, upgrades {5})",
                            pIdx + 1, player == this ? "(you)" : "",
                            player.Hand.Count, player.Stable.Count, player.Downgrades.Count, player.Upgrades.Count);
                }

                if (number != 1)
                    Console.Write("Selection {0}/{1}: ", i + 1, number);
                else
                    Console.Write("Selection: ");

                string? answer = Console.ReadLine();
                if (int.TryParse(answer, out int selectedNumber)) {
                    if (selectedNumber >= 1 && selectedNumber <= players.Count)
                        if (!selected[selectedNumber - 1])
                            selected[selectedNumber - 1] = true;
                        else {
                            Console.WriteLine("Player is already selected!");
                            i--;
                        }
                    else {
                        Console.WriteLine("Number not in available range!");
                        i--;
                    }
                } else {
                    Console.WriteLine("This is not a number!");
                    i--;
                }
            }

            List<APlayer> ans = new();
            for (int i = 0; i < players.Count; i++)
                if (selected[i])
                    ans.Add(players[i]);

            return ans;
        }

        private List<Card> AskOnCardSelection(string msg, int number, List<Card> cards, bool selectionNothingAvailable = false,
                bool checkIfCardCanBePlayed = false) {
            Console.WriteLine(msg);

            List<bool> selected = new();
            foreach (Card _ in cards)
                selected.Add(false);

            Predicate<Card> isCardPlayable = card => !checkIfCardCanBePlayed || card.CardType != ECardType.Instant && card.CanBePlayed(this);
            for (int i = 0; i < number; i++) {
                Console.WriteLine("Available targets:");
                for (int cIdx = 0; cIdx < cards.Count; cIdx++)
                    if (!selected[cIdx])
                        Console.WriteLine("{0} - card {1} {2} {3}", cIdx + 1, cards[cIdx].Name,
                            cards[cIdx].Player == this ? "(your card)" : "",
                            isCardPlayable(cards[cIdx]) ? "" : "(can't be played)");

                if (selectionNothingAvailable)
                    Console.WriteLine("{0} - select nothing", cards.Count + 1);

                if (number != 1)
                    Console.Write("Selection {0}/{1}: ", i + 1, number);
                else
                    Console.Write("Selection: ");

                string? answer = Console.ReadLine();
                if (int.TryParse(answer, out int selectedNumber)) {
                    if (selectionNothingAvailable && selectedNumber == cards.Count + 1)
                        return new List<Card>();


                    if (selectedNumber >= 1 && selectedNumber <= cards.Count) {
                        if (!isCardPlayable(cards[selectedNumber - 1])) {
                            Console.WriteLine("This card can't be played.");
                            i--;
                        } else if (!selected[selectedNumber - 1])
                            selected[selectedNumber - 1] = true;
                        else {
                            Console.WriteLine("This card is already selected!");
                            i--;
                        }
                    } else {
                        Console.WriteLine("Number not in available range!");
                        i--;
                    }
                } else {
                    Console.WriteLine("This is not a number!");
                    i--;
                }
            }

            List<Card> ans = new();
            for (int i = 0; i < cards.Count; i++)
                if (selected[i])
                    ans.Add(cards[i]);

            return ans;
        }

        protected override List<APlayer> ChoosePlayersCore(int number, AEffect effect, List<APlayer> playersWhichCanBeSelected) {
            return AskOnPlayerSelection(
                string.Format("Which players should be selected for effect {0} of card {1}?", effect, effect.OwningCard.Name),
                number,
                playersWhichCanBeSelected
            );
        }

        protected override Card? PlayInstantOnStackCore(List<Card> stack, List<Card> availableInstantCards) {
            var selection = AskOnCardSelection(
                string.Format("Play instant card on stack? Current stack: {0}", string.Join(", ", stack.Select(card => card.Name))),
                1,
                availableInstantCards,
                selectionNothingAvailable: true
            );
            if (selection.Count == 0)
                return null;

            return selection[0];
        }

        protected override APlayer WhereShouldBeCardPlayedCore(Card card) {
            if (card.CardType == ECardType.Spell)
                return this;

            return AskOnPlayerSelection(
                string.Format("To which stable should be card {0} played?", card.Name),
                1,
                new List<APlayer>(GameController.Players)
            )[0];
        }

        protected override List<Card> WhichCardsToDestroyCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return AskOnCardSelection(
                string.Format("Which card(s) should be destroyed by effect of card {0}?", effect.OwningCard.Name),
                number,
                cardsWhichCanBeSelected
            );
        }

        protected override List<Card> WhichCardsToDiscardCore(int number, AEffect? effect, List<Card> cardsWhichCanBeSelected) {
            return AskOnCardSelection(
                effect == null ? "Which cards should be discarded (end of turn)?" :
                string.Format("Which card(s) should be discarded by effect of card {0}?", effect.OwningCard.Name),
                number,
                cardsWhichCanBeSelected
            );
        }

        protected override List<Card> WhichCardsToGetCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return AskOnCardSelection(
                string.Format("Which card(s) cards you want to get from effect of card {0}?", effect.OwningCard.Name),
                number,
                cardsWhichCanBeSelected
            );
        }

        protected override List<Card> WhichCardsToMoveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return AskOnCardSelection(
                string.Format("Which card(s) cards to move by effect of card {0}?", effect.OwningCard.Name),
                number,
                cardsWhichCanBeSelected
            );
        }

        protected override List<Card> WhichCardsToReturnCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return AskOnCardSelection(
                string.Format("Which card(s) should be returned by effect of card {0}?", effect.OwningCard.Name),
                number,
                cardsWhichCanBeSelected
            );
        }

        protected override List<Card> WhichCardsToSacrificeCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return AskOnCardSelection(
                string.Format("Which card(s) should be sacrificed by effect of card {0}?", effect.OwningCard.Name),
                number,
                cardsWhichCanBeSelected
            );
        }

        protected override List<Card> WhichCardsToSaveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return AskOnCardSelection(
                string.Format("Which card(s) should be saved (from effect which are targeted) by effect of card {0}?", effect.OwningCard.Name),
                number,
                cardsWhichCanBeSelected
            );
        }

        protected override List<Card> WhichCardsToStealCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return AskOnCardSelection(
                string.Format("Which card(s) should be stolen by effect of card {0}?", effect.OwningCard.Name),
                number,
                cardsWhichCanBeSelected
            );
        }

        protected override Card? WhichCardToPlayCore() {
            var selection = AskOnCardSelection(
                string.Format("Which card should be played?"),
                1,
                Hand,
                selectionNothingAvailable: true,
                checkIfCardCanBePlayed: true
            );
            if (selection.Count == 0)
                return null;

            return selection[0];
        }

        protected override AEffect WhichEffectToSelectCore(List<AEffect> effectsVariants) {
            Console.WriteLine("Which effect to select?");

            while (true) {
                Console.WriteLine("Available effects:");
                for (int cIdx = 0; cIdx < effectsVariants.Count; cIdx++)
                    Console.WriteLine("{0} - effect {1}", cIdx + 1, effectsVariants[cIdx]);

                Console.Write("Selection: ");

                string? answer = Console.ReadLine();
                if (int.TryParse(answer, out int selectedNumber)) {
                    if (selectedNumber >= 1 && selectedNumber <= effectsVariants.Count)
                        return effectsVariants[selectedNumber - 1];
                    else
                        Console.WriteLine("Number not in available range!");
                } else
                    Console.WriteLine("This is not a number!");
            }
        }
    }
}
