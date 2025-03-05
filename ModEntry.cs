using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace GiveAnywhere
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.Monitor.Log(this.Config.TriggerKey.ToString());
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            if (this.Config.TriggerKey.JustPressed())
            {
                this.TriggerGiftFlow();
            }
        }

        private void TriggerGiftFlow() {
            StardewValley.Object activeObject = Game1.player.ActiveObject;
            if (activeObject == null) return;

            bool? canGiveAsGift = activeObject.canBeGivenAsGift();
            NetBool? isQuestItem = activeObject.questItem;
            if (canGiveAsGift.HasValue && canGiveAsGift.GetValueOrDefault() && (isQuestItem == null || !isQuestItem.Value) )
            {
                Game1.player.Halt();

                String dialogQuestion = $"Offer {activeObject.name} to which farmer?";

                IEnumerable<Farmer> otherFarmers = Game1.getOnlineFarmers().Where(f => f.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID);
                Response[] farmersResponses = otherFarmers.Select(farmer => new Response(farmer.UniqueMultiplayerID.ToString(), farmer.Name))
                    .Append(new Response("", "Cancel"))
                    .ToArray();
                Game1.player.currentLocation.createQuestionDialogue(dialogQuestion, farmersResponses, delegate (Farmer _, string answer) {
                    if (answer == "")
                    {
                        return;
                    }
                    Game1.player.team.SendProposal(otherFarmers.Where(f => f.UniqueMultiplayerID.ToString() == answer).First(), ProposalType.Gift, activeObject.getOne());
                    Game1.activeClickableMenu = new PendingProposalDialog();
                });
            }
        }
    }
}
