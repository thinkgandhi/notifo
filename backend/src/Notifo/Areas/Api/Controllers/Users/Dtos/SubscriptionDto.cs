﻿// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Notifo.Domain.Subscriptions;

namespace Notifo.Areas.Api.Controllers.Users.Dtos;

public sealed class SubscriptionDto
{
    /// <summary>
    /// The topic to add.
    /// </summary>
    public string TopicPrefix { get; set; }

    /// <summary>
    /// Notification settings per channel.
    /// </summary>
    public Dictionary<string, ChannelSettingDto> TopicSettings { get; set; } = [];

    /// <summary>
    /// The scheduling settings.
    /// </summary>
    public SchedulingDto? Scheduling { get; set; }

    public static SubscriptionDto FromDomainObject(Subscription source)
    {
        var result = new SubscriptionDto
        {
            TopicPrefix = source.TopicPrefix
        };

        if (source.TopicSettings != null)
        {
            foreach (var (key, value) in source.TopicSettings)
            {
                if (value != null)
                {
                    result.TopicSettings[key] = ChannelSettingDto.FromDomainObject(value);
                }
            }
        }

        if (source.Scheduling != null)
        {
            result.Scheduling = SchedulingDto.FromDomainObject(source.Scheduling);
        }

        return result;
    }
}
