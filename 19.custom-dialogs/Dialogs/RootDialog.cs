using System.Net.Http.Headers;
using System.Net.Http;
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Recognizers.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This is an example root dialog. Replace this with your applications.
    /// </summary>
    public class RootDialog : ComponentDialog
    {
        private IStatePropertyAccessor<JObject> _userStateAccessor;

        public RootDialog(UserState userState)
            : base("root")
        {
            _userStateAccessor = userState.CreateProperty<JObject>("result");

            // Rather than explicitly coding a Waterfall we have only to declare what properties we want collected.
            // In this example we will want two text prompts to run, one for the first name and one for the last.
            var fullname_slots = new List<SlotDetails>
            {
                new SlotDetails("first", "text", "Please enter your first name."),
                new SlotDetails("last", "text", "Please enter your last name."),
                new SlotDetails("weight", "number", "Please enter your Weight"),
                new SlotDetails("height", "number", "Please enter your Height"),
                new SlotDetails("age", "number", "Please enter your Age"),
                new SlotDetails("gender", "number", "Female(0) or Male(1)"),
                new SlotDetails("parents", "number", "Are your parents diabetic Yes(1) or No(0)"),
                new SlotDetails("siblings", "number", "Are your sister/brother diabetic Yes(1) or No(0)"),
                new SlotDetails("child", "number", "If you have children are any of them diabetic Yes(1) or No(0)"),
                new SlotDetails("member", "number", "Are any of your relatives diabetic Yes(1) or No(0)"),
                new SlotDetails("gym", "number", "Do you more than 30 min of physical activity Yes(1) or No(0)"),
                new SlotDetails("smoke", "number", "Do you smoke Yes(1) or No(0)"),
                new SlotDetails("tuberculosis", "number", "Do you have tuberculosis Yes(1) or No(0)"),
                new SlotDetails("cancer", "number", "Do you have cancer Yes(1) or No(0)"),
                new SlotDetails("obesity", "number", "Are you obese Yes(1) or No(0)"),
                new SlotDetails("hypertense", "number", "Do you have hypertension Yes(1) or No(0)"),
                new SlotDetails("VIH", "number", "Do you have HIV/AIDS Yes(1) or No(0)"),
                new SlotDetails("depresion", "number", "Have you ever suffered from depression Yes(1) or No(0)"),
                new SlotDetails("dyslipidemic", "number", "Do You Have Dyslipidemia Yes(1) or No(0)"),
                new SlotDetails("cardiovascular", "number", "Do You Have any cardiovascular condition  Yes(1) or No(0)"),
                new SlotDetails("hepatitis", "number", "Do You Have hepatitis  Yes(1) or No(0)"),
                new SlotDetails("nutrition", "number", "In the last year did you go to a nutritionist  Yes(1) or No(0)"),
                new SlotDetails("oftalmologo", "number", "In the last year did you go to a ophtalmologist  Yes(1) or No(0)"),
                new SlotDetails("podologo", "number", "In the last year did you go to a podologist  Yes(1) or No(0)"),
                new SlotDetails("motriz", "number", "In the last year did you present any motor disabilities?  Yes(1) or No(0)"),
                new SlotDetails("visual", "number", "In the last year did you present any visual disabilities?  Yes(1) or No(0)"),


            };

            // This defines an address dialog that collects street, city and zip properties.


            // Dialogs can be nested and the slot filling dialog makes use of that. In this example some of the child
            // dialogs are slot filling dialogs themselves.
            var slots = new List<SlotDetails>
            {
                new SlotDetails("fullname", "fullname"),

            };

            // Add the various dialogs that will be used to the DialogSet.

            AddDialog(new SlotFillingDialog("fullname", fullname_slots));
            AddDialog(new TextPrompt("text"));
            AddDialog(new NumberPrompt<int>("number", defaultLocale: Culture.English));

            AddDialog(new SlotFillingDialog("slot-dialog", slots));

            // Defines a simple two step Waterfall to test the slot dialog.
            AddDialog(new WaterfallDialog("waterfall", new WaterfallStep[] { StartDialogAsync, ProcessResultsAsync }));

            // The initial child Dialog to run.
            InitialDialogId = "waterfall";
        }

        private Task<bool> ShoeSizeAsync(PromptValidatorContext<float> promptContext, CancellationToken cancellationToken)
        {
            var shoesize = promptContext.Recognized.Value;

            // show sizes can range from 0 to 16
            if (shoesize >= 0 && shoesize <= 16)
            {
                // we only accept round numbers or half sizes
                if (Math.Floor(shoesize) == shoesize || Math.Floor(shoesize * 2) == shoesize * 2)
                {
                    // indicate success by returning the value
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }

        private async Task<DialogTurnResult> StartDialogAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Start the child dialog. This will run the top slot dialog than will complete when all the properties are gathered.
            return await stepContext.BeginDialogAsync("slot-dialog", null, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessResultsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // To demonstrate that the slot dialog collected all the properties we will echo them back to the user.
            if (stepContext.Result is IDictionary<string, object> result && result.Count > 0)
            {
                var form = (IDictionary<string, object>)result["fullname"];

                // Now the waterfall is complete, save the data we have gathered into UserState. 

                var obj = await _userStateAccessor.GetAsync(stepContext.Context, () => new JObject());
                obj["data"] = new JObject
                {
                    { "fullname",  $"{form["first"]} {form["weight"]} {form["last"]}" },

                };

                var diagnosis = new DiaBOT.Posts.Diagnosis
                {
                    EDAD = (int)form["age"],
                    GENERO = (int)form["gender"],
                    PADRES = (int)form["parents"],
                    HERMANOS = (int)form["siblings"],
                    HIJOS = (int)form["child"],
                    OTROS = (int)form["member"],
                    ACTIVIDAD_FISICA = (int)form["gym"],
                    TABAQUISMO = (int)form["smoke"],
                    TUBERCULOSIS = (int)form["tuberculosis"],
                    CANCER = (int)form["cancer"],
                    OBESIDAD = (int)form["obesity"],
                    HIPERTENSION = (int)form["hypertense"],
                    VIH = (int)form["VIH"],
                    DEPRESION = (int)form["depresion"],
                    DISLIPIDEMIA = (int)form["dyslipidemic"],
                    CARDIO = (int)form["cardiovascular"],
                    HEPATITIS = (int)form["hepatitis"],
                    NUTRIOLOGO = (int)form["nutrition"],
                    OFTALMOLOGO = (int)form["oftalmologo"],
                    PODOLOGO = (int)form["podologo"],
                    MOTORA = (int)form["motriz"],
                    VISUAL = (int)form["visual"],
                    PESO = (int)form["weight"],
                    ESTATURA = (int)form["height"]
                };

                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri("http://10.25.73.190:5000/evaluate");

                    var content = JsonConvert.SerializeObject(diagnosis);
                    Console.WriteLine(content);

                    var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                    var bytes = new ByteArrayContent(buffer);

                    bytes.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var evaluation = client.PostAsync("", bytes).Result;
                    string txt = "Nothing";

                    if (evaluation.IsSuccessStatusCode)
                    {
                        txt = evaluation.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(txt);

                        // txt = (yes > no) ? "Sorry, you have a high chance of diabetes" : "You seem pretty healthy, keep it up!";

                    }
                    
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(txt), cancellationToken);
                }

            }

            // Remember to call EndAsync to indicate to the runtime that this is the end of our waterfall.
            return await stepContext.EndDialogAsync();
        }
    }
}