﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components.Extensions;

namespace AndOS.Infrastructure.CloudStorage;

public class CloudStorageServiceFactory(IServiceProvider serviceProvider) : ICloudStorageServiceFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ICloudStorageService Create(AndOS.Core.Enums.CloudStorage cloudStorageService)
    {
        var service = this._serviceProvider.GetRequiredKeyedService<ICloudStorageService>(cloudStorageService.GetDescription());
        return service;
    }
}