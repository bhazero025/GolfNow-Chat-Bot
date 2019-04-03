using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;
using Newtonsoft.Json;

///<author>Andrew Millward</author>

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace testAWSLambda
{
    public class Function
    {

        /// <summary>
        /// Handles events from Lex
        /// </summary>
        /// <param name="lexEvent">Information about the lex event.</param>
        /// <param name="context">Lambda execution variables.</param>
        /// <returns>A response to the lex event</returns>
        public LexResponse FunctionHandler(LexEvent lexEvent, ILambdaContext context)
        {
            //Time slot entered by user.
            string time;
            //The text of the response message.
            string textOut;
            //Key-Value pairs of options, response text, and other things returned by the API.
            IDictionary<string, string> attributes;
            //What (if any) information to populate slots with in the next intent.
            IDictionary<string, string> slots = new Dictionary<string, string>();
            
            //Detrimine the intent that triggered the logic.
            switch (lexEvent.CurrentIntent.Name)
            {
                //An example case.
                case "ditto" :
                    //Example slot population.
                    slots.Add("actor", "Bob Hoskins");
                    return elicitSlot(lexEvent.SessionAttributes, "Test", slots, "time", new LexResponse.LexMessage { ContentType = "PlainText", Content = "This is the new intent!! Enter a time..." });
                //User wants tee times via a given zip code.
                case "getTeeTimeByZip":
                    string zip;
                    string nextIntent = "chooseOption";
                    string slotToElicit = "option";

                    //Gather user inputs.
                    if (!lexEvent.CurrentIntent.Slots.TryGetValue("zip", out zip))
                        zip = "No zip...";
                    if (!lexEvent.CurrentIntent.Slots.TryGetValue("time", out time))
                        time = "No time...";

                    //Colate arguments for the API.
                    TeesByZipArgs teesByZip = new TeesByZipArgs(zip, "15", time);
                    //Contact API and get resultes.
                    attributes = new ApiConnectionBroker().ApiHandler(teesByZip);
                    //Seperate out the output text.
                    textOut = attributes["output"];
                    attributes.Remove("output");
                    //add options to lexEvent Session Attribute vars that we will perserve.
                    attributes.ToList().ForEach(x => lexEvent.SessionAttributes[x.Key] = x.Value);

                    //Elicit the next intent to choose between options.
                    return elicitSlot(lexEvent.SessionAttributes, nextIntent, slots, slotToElicit, new LexResponse.LexMessage { ContentType = "PlainText", Content = textOut + "Enter the number of a suitable option." });
                //User wants tee times via given city, state(optional), and country(optional).
                case "getTeeTimeByCity":
                    string city;
                    string state;
                    string country;

                    //Gather user inputs.
                    if (!lexEvent.CurrentIntent.Slots.TryGetValue("city", out city))
                        city = "No city...";
                    if (!lexEvent.CurrentIntent.Slots.TryGetValue("time", out time))
                        time = "No time...";
                    if (!lexEvent.CurrentIntent.Slots.TryGetValue("state", out state))
                        time = "No state...";
                    if (!lexEvent.CurrentIntent.Slots.TryGetValue("country", out country))
                        time = "No country...";

                    //Colate arguments for the API.
                    TeesByCityArgs teesByCity = new TeesByCityArgs(city, state, country, time);
                    //Contact API and get resultes.
                    attributes = new ApiConnectionBroker().ApiHandler(teesByCity);
                    //Seperate out the output text.
                    textOut = attributes["output"];
                    attributes.Remove("output");
                    //add options to lexEvent Session Attribute vars that we will perserve.
                    attributes.ToList().ForEach(x => lexEvent.SessionAttributes[x.Key] = x.Value);

                    return Close(lexEvent.SessionAttributes, "Fulfilled", new LexResponse.LexMessage { ContentType = "PlainText", Content = textOut });
                //The user has been presented with options and has chosen one.
                case "chooseOption":
                    //the index of the option chosen.
                    string option;

                    //Ensure an option was chosen.
                    if(lexEvent.CurrentIntent.Slots.TryGetValue("option", out option))
                    {
                        //Ensure the option selected exists.
                        if (lexEvent.SessionAttributes["rateOption" + option] != "")
                        {
                            TeeTimeRateSelection selection = JsonConvert.DeserializeObject<TeeTimeRateSelection>(lexEvent.SessionAttributes["rateOption" + option]);
                            RateInvoiceArgs args = new RateInvoiceArgs(selection.FacilityID, selection.RateID);
                            attributes = new ApiConnectionBroker().ApiHandler(args);
                            //Seperate out the output text.
                            textOut = attributes["output"];
                            attributes.Remove("output");
                            //textOut = "Rate found: " + selection.RateID + " at facility: " + selection.FacilityID;
                        }
                        else
                            textOut = "Rate not found.";
                    }
                    else
                        textOut = "Rate number not found.";
                    return Close(lexEvent.SessionAttributes, "Fulfilled", new LexResponse.LexMessage { ContentType = "PlainText", Content = textOut });
                //The triggering event is unknown to this logic.
                default :
                    return Close(lexEvent.SessionAttributes, "Fulfilled", new LexResponse.LexMessage { ContentType = "PlainText", Content = "Chat Error: State Invalid." });
            }
        }

        //Function that closes the intent with a given fullfillment state.
        protected LexResponse Close(IDictionary<string, string> sessionAttributes, string fulfillmentState, LexResponse.LexMessage message)
        {
            return new LexResponse
            {
                SessionAttributes = sessionAttributes,
                DialogAction = new LexResponse.LexDialogAction
                {
                    Type = "Close",
                    FulfillmentState = fulfillmentState,
                    Message = message
                }
            };
        }

        //Function that chains to an intent. This allows you to specify the intent to initiate, the slot the next user input will be targeting, and populate other slots.
        protected LexResponse elicitSlot(IDictionary<string, string> sessionAttributes, string intentName, IDictionary<string,string> slots, string elicitSlot, LexResponse.LexMessage message)
        {
            return new LexResponse
            {
                SessionAttributes = sessionAttributes,
                DialogAction = new LexResponse.LexDialogAction
                {
                    Type = "ElicitSlot",
                    IntentName = intentName,
                    Slots = slots,
                    SlotToElicit = elicitSlot,
                    Message = message
                }
            };
        }
    }
}
