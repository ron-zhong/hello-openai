﻿using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

// Initialize configuration
var builder = new ConfigurationBuilder()
                .AddUserSecrets<Program>();
IConfiguration Configuration = builder.Build();

var IS_AZURE_OPENAI = false;
var AZURE_OPENAI_ENDPOINT = Configuration["azure-openai-endpoint"] ?? string.Empty;
var AZURE_OPENAI_API_KEY = Configuration["azure-openai-api-key"] ?? string.Empty;
var OPENAI_API_KEY = Configuration["openai-api-key"] ?? string.Empty;
var OPENAI_MODEL_NAME = IS_AZURE_OPENAI ? "chat" : "gpt-3.5-turbo";

OpenAIClient client = IS_AZURE_OPENAI
    ? new OpenAIClient(
        new Uri(AZURE_OPENAI_ENDPOINT),
        new AzureKeyCredential(AZURE_OPENAI_API_KEY))
    : new OpenAIClient(OPENAI_API_KEY);


// Sample Code: Stream Chat Messages with non-Azure OpenAI
var chatCompletionsOptions = new ChatCompletionsOptions()
{
    Messages =
    {
        new ChatMessage(ChatRole.System, "You are a helpful assistant. You will talk like a pirate."),
        new ChatMessage(ChatRole.User, "Can you help me?"),
        new ChatMessage(ChatRole.Assistant, "Arrrr! Of course, me hearty! What can I do for ye?"),
        new ChatMessage(ChatRole.User, "What's the best way to train a parrot?"),
    }
};

Response<StreamingChatCompletions> response = await client.GetChatCompletionsStreamingAsync(
    deploymentOrModelName: "gpt-3.5-turbo",
    chatCompletionsOptions);
using StreamingChatCompletions streamingChatCompletions = response.Value;

await foreach (StreamingChatChoice choice in streamingChatCompletions.GetChoicesStreaming())
{
    await foreach (ChatMessage message in choice.GetMessageStreaming())
    {
        Console.Write(message.Content);
    }
    Console.WriteLine();
}