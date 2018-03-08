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
    public class ReArrangeDialog
    {
        string sName;
        string sTrackingNo;
        string sAction;
        private string optionSelected;

        public async Task ReArrangeTime(IDialogContext context, IAwaitable<string> result)
        {
            optionSelected = await result;
            switch (optionSelected)
            {
                case "Yes":
                    PromptDialog.Choice(context, this.ReArrangeTimeResumeAfter, new List<string>() { "10:00AM", "11:00AM", "12:00PM", "13:00PM", "14:00PM", "15:00PM" }, "These are the available times, Please select an option?");
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
                    PromptDialog.Choice(context, this.ReArrangeDateResumeAfter, new List<string>() { "Monday 12th March", "Tuesday 13th March", "Wednesday 14th March" }, "These are the available dates, Please select an option?");
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