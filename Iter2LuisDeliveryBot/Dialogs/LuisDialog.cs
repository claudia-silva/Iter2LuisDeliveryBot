using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Threading;

namespace LuisDeliveryBot.Dialogs
{
    [Serializable]
    [LuisModel("caa3fcf3-7f0c-4a7f-af28-6554cbbb4fd8", "d9bdd7eae973449191a235a6760435f1")]

    public class LuisDialog : LuisDialog<object>
    {
        string sName;
        string sTrackingNo;
        string sAction;
        string sAddress = "Brunel University London";

        // methods to handle LUIS intents 
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"I'm sorry! I don't know what you want. Try again or type menu for options";
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Hello")]
        public async Task Hello(IDialogContext context, LuisResult result)
        {
            string message = $"Hello, I am Delivery Bot. What is your name?";
            await context.PostAsync(message);
            context.Wait(this.NameReceived);
        }

        private async Task NameReceived(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            sName = message.Text;

            string msg = $"Hi " + sName + ". How can I help you today?";
            await context.PostAsync(msg);
            context.Wait(this.MessageReceived);
        }
    }
}