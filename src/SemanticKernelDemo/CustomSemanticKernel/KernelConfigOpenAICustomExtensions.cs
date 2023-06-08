// Copyright (c) Microsoft. All rights reserved.

using Azure.Core;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.AI.ImageGeneration;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.ImageGeneration;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.TextCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.TextEmbedding;
using Microsoft.SemanticKernel.Diagnostics;

// ReSharper disable once CheckNamespace // Extension methods
namespace CustomSemanticKernel;

public static class KernelConfigOpenAICustomExtensions
{
    
    #region Images

    /// <summary>
    /// Add the OpenAI DallE image generation service to the list
    /// </summary>
    /// <param name="config">The kernel config instance</param>
    /// <param name="serviceId">A local identifier for the given AI service</param>
    /// <param name="apiKey">OpenAI API key, see https://platform.openai.com/account/api-keys</param>
    /// <param name="orgId">OpenAI organization id. This is usually optional unless your account belongs to multiple organizations.</param>
    /// <returns>Self instance</returns>
    public static KernelConfig AddOpenAIImageCustomService(this KernelConfig config,
        string serviceId, string apiKey, string? orgId = null)
    {
        Verify.NotEmpty(serviceId, "The service Id provided is empty");

        IImageCustom Factory(IKernel kernel) => new OpenAIImageCustom(
            apiKey, orgId, kernel.Config.HttpHandlerFactory, kernel.Log);

        config.AddImageCustomService(serviceId, Factory);

        return config;
    }

    #endregion
}
