using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore.BasicEffects {
    public class PullCardEffect : AEffect {
        int _numberSelectPlayers;
        List<APlayer>? playerList;
        public PullCardEffect(Card owningCard, int cardCount, int numberSelectedPlayers) : base(owningCard, cardCount) {
            TargetOwner = OwningPlayer;
            TargetLocation = CardLocation.InHand;
            _numberSelectPlayers = numberSelectedPlayers;
        }

        public override void ChooseTargets(GameController gameController) {
            playerList = OwningPlayer.ChoosePlayers(_numberSelectPlayers, false, this);
        }

        public override void InvokeEffect(GameController gameController) {
            if (playerList == null)
                throw new InvalidOperationException("Players was not selected (was not called `ChooseTargets`).");

            foreach (APlayer player in playerList) {
                int numberCardToSelect = Math.Min(_cardCount, player.Hand.Count);
                for (int i = 0; i < numberCardToSelect; i++) {
                    int selectedCard = player.GameController.Random.Next(player.Hand.Count);
                    player.Hand[selectedCard].MoveCard(gameController, TargetOwner, TargetLocation);
                }
            }
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
