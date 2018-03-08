using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Threading;
using System.Collections.Generic;

namespace Iter2LuisDeliveryBot.Dialogs
{
    [Serializable]
    [LuisModel("caa3fcf3-7f0c-4a7f-af28-6554cbbb4fd8", "d9bdd7eae973449191a235a6760435f1")]

    public class LuisDialog : LuisDialog<object>
    {
        string sName;
        string sTrackingNo;
        string sAction;
        string sAddress = "Brunel University London, Kingston Lane, UB8 3PH, Uxbridge";
        private string optionSelected;

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

        [LuisIntent("Name")]
        public async Task Name(IDialogContext context, LuisResult result)
        {
            string message = $"Hi " + result.Entities[0].Entity;
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("TrackParcel")]
        public async Task TrackParcel(IDialogContext context, LuisResult result)
        {
            sAction = "TrackParcel";
            RequestTrackingNo(context, result);
        }

        [LuisIntent("Time")]
        public async Task Time(IDialogContext context, LuisResult result)
        {
            sAction = "Time";
            RequestTrackingNo(context, result);
        }

        [LuisIntent("Date")]
        public async Task Date(IDialogContext context, LuisResult result)
        {
            sAction = "Date";
            RequestTrackingNo(context, result);
        }

        [LuisIntent("Address")]
        public async Task Address(IDialogContext context, LuisResult result)
        {
            sAction = "Address";
            RequestTrackingNo(context, result);
        }

        [LuisIntent("LocalServicePoint")]
        public async Task LocalServicePoint(IDialogContext context, LuisResult result)
        {
            sAction = "LocalServicePoint";
            RequestTrackingNo(context, result);
        }

        public async Task RequestTrackingNo(IDialogContext context, LuisResult result)
        {
            string message = $"What is your tracking number?(TRA1234)";
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("TrackingNo")]
        public async Task TrackingNo(IDialogContext context, LuisResult result)
        {
            string message = "";
            sTrackingNo = result.Entities[0].Entity;

            if (sAction == "TrackParcel")
            {
                message = $"Your parcel { this.sTrackingNo } will be delivered today";
                await context.PostAsync(message);
                context.Wait(this.MessageReceived);
            }
            else if (sAction == "Time")
            {
                PromptDialog.Choice(context, this.ReArrangeTime, new List<string>() { "Yes", "No" }, $@"Your parcel with Track No: { this.sTrackingNo } is being delivered on " + DateTime.Now.ToString("HH:mm:ss") + ", Would you like to change the time of delivery?");
            }
            else if (sAction == "Date")
            {
                PromptDialog.Choice(context, this.ReArrangeDate, new List<string>() { "Yes", "No" }, $@"Your parcel with Track No: { this.sTrackingNo } is being delivered on " + DateTime.Now.ToString("dd MMM yyyy") + ", Would you like to change the day of delivery?");
            }
            else if (sAction == "Address")
            {
                PromptDialog.Choice(context, this.ChangeAddressNextSteps, new List<string>() { "Yes", "No" }, $@"Your parcel with Track No: { result.Entities[0].Entity } is being delivered to the Address: " + sAddress + ", Would you like to change this?");
            }
            else if (sAction == "LocalServicePoint")
            {
                PromptDialog.Choice(context, this.LocalServicePointNextSteps, new List<string>() { "Yes", "No" }, $@"Your parcel with Track No: { result.Entities[0].Entity } is being delivered to the Address: " + sAddress + ", Would you like to change this to a local service point?");
            }

        }

        private async Task TrackingNumber(IDialogContext context)
        {
            string msg = $"What is your tracking number?(TRA1234)";
            await context.PostAsync(msg);
            //context.Wait(this.TrackingNoReceived);
            context.Wait(this.MessageReceived);
        }


        public async Task ReArrangeTime(IDialogContext context, IAwaitable<string> result)
        {
            optionSelected = await result;
            switch (optionSelected)
            {
                case "Yes":
                    PromptDialog.Choice(context, this.ReArrangeTimeResumeAfter, new List<string>() { "10:00AM", "11:00AM", "12:00PM", "13:00PM" }, "These are the available times, Please select an option?");
                    break;
                case "No":
                    // PromptDialog.Text(context, ChangeAddressResumeAfter, "");
                    break;
            }
        }

        public async Task ReArrangeTimeResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            optionSelected = await result;
            PromptDialog.Text(context, NextSteps, $@"Your parcel with Track No: {this.sTrackingNo} will be delivered to you at {this.optionSelected}");
        }


        public async Task ReArrangeDate(IDialogContext context, IAwaitable<string> result)
        {
            optionSelected = await result;
            switch (optionSelected)
            {
                case "Yes":
                    PromptDialog.Choice(context, this.ReArrangeDateResumeAfter, new List<string>() { "Monday", "Tuesday", "Wednesday" }, "These are the available dates, Please select an option?");
                    break;
                case "No":
                    // PromptDialog.Text(context, ChangeAddressResumeAfter, "");
                    break;
            }
        }

        public async Task ReArrangeDateResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            optionSelected = await result;
            PromptDialog.Text(context, NextSteps, $@"Your parcel with Track No: {this.sTrackingNo} will be delivered to you on {this.optionSelected}");
        }

        public async Task ChangeAddressNextSteps(IDialogContext context, IAwaitable<string> result)
        {
            optionSelected = await result;
            switch (optionSelected)
            {
                case "Yes":
                    PromptDialog.Text(context, ChangeAddress, "What would you like to change it to?");
                    break;
                case "No":
                    // PromptDialog.Text(context, ChangeAddressResumeAfter, "");
                    break;
            }
        }

        public async Task LocalServicePointNextSteps(IDialogContext context, IAwaitable<string> result)
        {
            optionSelected = await result;
            switch (optionSelected)
            {
                case "Yes":
                    await this.ChangeToCollection(context);
                    break;
                case "No":
                    // PromptDialog.Text(context, ChangeAddressResumeAfter, "");
                    break;
            }
        }

        public async Task ChangeToCollection(IDialogContext context)
        {
            await context.PostAsync("These are the nearest Local Service Points to you:");
            PromptDialog.Choice(context, this.CollectionChangeResumeAfter, new List<string>() { "Asda", "Sainsburys", "Tesco" }, "These are the nearest Local Service Points to you: Please choose one?");
        }

        public async Task CollectionChangeResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            optionSelected = await result;
            PromptDialog.Text(context, NextSteps, $@"Your parcel with Track No: {this.sTrackingNo} will now be delivered to {this.optionSelected}");
        }

        public async Task ChangeAddress(IDialogContext context, IAwaitable<string> result)
        {
            optionSelected = await result;
            PromptDialog.Text(context, NextSteps, $@"Your parcel with Track No: {this.sTrackingNo} will now be delivered to {this.optionSelected}");
        }


        public async Task NextSteps(IDialogContext context, IAwaitable<string> result)
        {
            await context.PostAsync("Is there anything else that DeliveryBot can help you with?");
            PromptDialog.Choice(context, this.NextStepsResumeAfter, new List<string>() { "Yes", "No" }, "Please select an option?");
        }

        public async Task NextStepsResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            optionSelected = await result;

            switch (optionSelected)
            {
                case "Yes":
                    string msg = $"Hi " + sName + ". How can I help you today?";
                    await context.PostAsync(msg);
                    context.Wait(this.MessageReceived);
                    break;
                case "No":
                    PromptDialog.Text(context, NextStepsResumeAfter, "Thank you for using DeliveryBot, Hope to speak with you again soon!");
                    break;
            }
        }
    }
}

