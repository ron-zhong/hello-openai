﻿using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

// Initialize configuration
var builder = new ConfigurationBuilder()
                .AddUserSecrets<Program>();
IConfiguration Configuration = builder.Build();

var AZURE_OPENAI_ENABLED = true;
var AZURE_OPENAI_ENDPOINT = "https://poc-openai-mims.openai.azure.com/";
var AZURE_OPENAI_API_KEY = Configuration["azure-openai-api-key"] ?? string.Empty;
var OPENAI_API_KEY = Configuration["openai-api-key"] ?? string.Empty;

OpenAIClient client = AZURE_OPENAI_ENABLED
    ? new OpenAIClient(
        new Uri(AZURE_OPENAI_ENDPOINT),
        new AzureKeyCredential(AZURE_OPENAI_API_KEY))
    : new OpenAIClient(OPENAI_API_KEY);


// Sample Code: Summarize Text with Completion
string textToSummarize = @"
    Two independent experiments reported their results this morning at CERN, Europe's high-energy physics laboratory near Geneva in Switzerland. Both show convincing evidence of a new boson particle weighing around 125 gigaelectronvolts, which so far fits predictions of the Higgs previously made by theoretical physicists.

    ""As a layman I would say: 'I think we have it'. Would you agree?"" Rolf-Dieter Heuer, CERN's director-general, asked the packed auditorium. The physicists assembled there burst into applause.
:";

string summarizationPrompt = @$"
    Summarize the following text.

    Text:
    """"""
    {textToSummarize}
    """"""

    Summary:
";

Console.Write($"Input: {summarizationPrompt}");
var completionsOptions = new CompletionsOptions()
{
    Prompts = { summarizationPrompt },
};

string deploymentName = "text-davinci-003";

Response<Completions> completionsResponse = client.GetCompletions(deploymentName, completionsOptions);
string completion = completionsResponse.Value.Choices[0].Text;
Console.WriteLine($"Summarization: {completion}");