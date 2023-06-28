/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System;

namespace UnstableUnicornCore.BasicEffects {
    public class MoveCardBackToPreviousStable : AEffect {
        /// <summary>
        /// Helper effect which return card back to previous stable
        /// 
        /// Used for effects which says: Steal unicorn then return this card back when....
        /// </summary>
        /// <param name="owningCard"></param>
        /// <param name="previousPlayer"></param>
        public MoveCardBackToPreviousStable(Card owningCard, APlayer previousPlayer) : base(owningCard, 0) {
            CardTargets.Add(owningCard);
            TargetOwner = previousPlayer;
            TargetLocation = CardLocation.OnTable;
        }
        public override void ChooseTargets(GameController gameController) { }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets)
                // if card is not on table => do nothing
                // can happen when Seductive unicorn steal some unicorn
                // and both are destroyed in same chain link
                if (card.Location == CardLocation.OnTable) {
                    card.MoveCard(gameController, TargetOwner, TargetLocation);

                    // if this card was targeted by another effect in actual chain link, then
                    // card effect must be registered only once !!!
                    if (gameController.CardsWhichAreTargeted.TryGetValue(card, out AEffect? effect)){
                        if (effect == null)
                            throw new NullReferenceException("Should not happen.");

                        effect.CardTargets.Remove(card);
                        gameController.CardsWhichAreTargeted[card] = this;
                    } else {
                        gameController.CardsWhichAreTargeted.Add(card, this);
                    }
                }
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
