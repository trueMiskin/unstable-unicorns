/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿namespace UnstableUnicornCore {
    /// <summary>
    /// Helper class for easy creation of the card by the fluent syntax
    /// </summary>
    public abstract class CardTemplateSource {
        public CardTemplate EmptyCard { get { return new CardTemplate(); } }

        /// <summary>
        /// Returns implemented a CardTemplate
        /// </summary>
        /// <returns></returns>
        public abstract CardTemplate GetCardTemplate();
    }
}
