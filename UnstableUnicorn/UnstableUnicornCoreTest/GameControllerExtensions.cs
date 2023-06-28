/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Reflection;
using System.Runtime.ExceptionServices;
using UnstableUnicornCore;

namespace UnstableUnicornCoreTest {
    public static class GameControllerExtensions {
        private static MethodInfo _resolveChainLink = null;
        public static void ResolveChainLink(this GameController gameController) {
            if (_resolveChainLink == null)
                _resolveChainLink = typeof(GameController).GetMethod("ResolveChainLink", BindingFlags.Instance | BindingFlags.NonPublic);

            try {
                _resolveChainLink.Invoke(gameController, new object[] { null });
            }catch(TargetInvocationException ex) {
                if (ex.InnerException != null)
                    // re-throw correct exception
                    ExceptionDispatchInfo.Throw(ex.InnerException);
                else
                    throw;
            }
        }

        public static void PlayCardAndResolveChainLink(this GameController gameController, Card card, APlayer player) {
            card.CardPlayed(gameController, player);
            gameController.ResolveChainLink();
        }
    }
}
