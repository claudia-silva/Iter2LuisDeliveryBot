﻿using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Threading;
using System.Collections.Generic;

namespace Iter2LuisDeliveryBot.Dialogs
{
    public class CollectionDialog
    {
        string sName;
        string sTrackingNo;
        string sAction;
        string sAddress = "Brunel University London, Kingston Lane, UB8 3PH, Uxbridge";
        private string optionSelected;

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
                    //context.Wait(this.MessageReceived);
                    break;
                case "No":
                    PromptDialog.Text(context, NextStepsResumeAfter, "Thank you for using DeliveryBot, Hope to speak with you again soon!");
                    break;
            }
        }
    }
}