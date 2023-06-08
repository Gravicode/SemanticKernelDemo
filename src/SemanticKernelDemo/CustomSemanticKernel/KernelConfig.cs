// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using CustomSemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.AI.ImageGeneration;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Diagnostics;
using Microsoft.SemanticKernel.Reliability;
using Microsoft.SemanticKernel.Text;

namespace Microsoft.SemanticKernel;

/// <summary>
/// Semantic kernel configuration.
/// TODO: use .NET ServiceCollection (will require a lot of changes)
/// </summary>
public partial class KernelConfig
{
  
    /// <summary>
    /// Image generation service factories
    /// </summary>
    public Dictionary<string, Func<IKernel, IImageCustom>> ImageCustomServices { get; } = new();

    
    /// <summary>
    /// Default image generation service.
    /// </summary>
    public string? DefaultImageCustomServiceId { get; private set; }


    /// <summary>
    /// Add to the list a service for image generation, e.g. OpenAI DallE.
    /// </summary>
    /// <param name="serviceId">Id used to identify the service</param>
    /// <param name="serviceFactory">Function used to instantiate the service object</param>
    /// <returns>Current object instance</returns>
    /// <exception cref="KernelException">Failure if a service with the same id already exists</exception>
    public KernelConfig AddImageCustomService(
        string serviceId, Func<IKernel, IImageCustom> serviceFactory)
    {
        Verify.NotEmpty(serviceId, "The service id provided is empty");
        this.ImageCustomServices[serviceId] = serviceFactory;
        if (this.ImageCustomServices.Count == 1)
        {
            this.DefaultImageCustomServiceId = serviceId;
        }

        return this;
    }

    
    #region Get

    
    /// <summary>
    /// Get the image generation service id matching the given id or the default if an id is not provided or not found.
    /// </summary>
    /// <param name="serviceId">Optional identifier of the desired service.</param>
    /// <returns>The image generation service id matching the given id or the default.</returns>
    /// <exception cref="KernelException">Thrown when no suitable service is found.</exception>
    public string GetImageCustomServiceIdOrDefault(string? serviceId = null)
    {
        if (string.IsNullOrEmpty( serviceId) || !this.ImageCustomServices.ContainsKey(serviceId!))
        {
            serviceId = this.DefaultImageCustomServiceId;
        }

        if (string.IsNullOrEmpty( serviceId))
        {
            throw new KernelException(KernelException.ErrorCodes.ServiceNotFound, "Image generation service not available");
        }

        return serviceId;
    }

    #endregion

   
}
