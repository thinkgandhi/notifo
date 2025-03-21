﻿// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Notifo.Domain.Integrations.Resources;

namespace Notifo.Domain.Integrations.MessageBird;

public sealed partial class MessageBirdSmsIntegration(MessageBirdClientPool clientPool) : IIntegration
{
    public static readonly IntegrationProperty AccessKeyProperty = new IntegrationProperty("accessKey", PropertyType.Text)
    {
        EditorLabel = Texts.MessageBird_AccessKeyLabel,
        EditorDescription = null,
        IsRequired = true
    };

    public static readonly IntegrationProperty OriginatorProperty = new IntegrationProperty("phoneNumber", PropertyType.Text)
    {
        EditorLabel = Texts.MessageBird_OriginatorNameLabel,
        EditorDescription = Texts.MessageBird_OriginatorNameDescription,
        IsRequired = false,
        Summary = true
    };

    public static readonly IntegrationProperty PhoneNumberProperty = new IntegrationProperty("phoneNumber", PropertyType.Number)
    {
        EditorLabel = Texts.MessageBird_PhoneNumberLabel,
        EditorDescription = null,
        IsRequired = false,
        Summary = true
    };

    public static readonly IntegrationProperty PhoneNumbersProperty = new IntegrationProperty("phoneNumbers", PropertyType.MultilineText)
    {
        EditorLabel = Texts.MessageBird_PhoneNumbersLabel,
        EditorDescription = Texts.MessageBird_PhoneNumbersDescription,
        IsRequired = false,
        Summary = false
    };

    public static readonly IntegrationProperty WhatsAppChannelIdProperty = new IntegrationProperty("whatsAppChannelId", PropertyType.Text)
    {
        EditorLabel = Texts.MessageBird_WhatsAppChannelIdLabel,
        EditorDescription = Texts.MessageBird_WhatsAppChannelIdDescription,
        IsRequired = false,
        Summary = false
    };

    public static readonly IntegrationProperty WhatsAppTemplateNameProperty = new IntegrationProperty("whatsAppTemplateName", PropertyType.Text)
    {
        EditorLabel = Texts.MessageBird_WhatsAppTemplateNameLabel,
        EditorDescription = Texts.MessageBird_WhatsAppTemplateNameDescription,
        IsRequired = false,
        Summary = false
    };

    public static readonly IntegrationProperty WhatsAppTemplateNamespaceProperty = new IntegrationProperty("whatsAppTemplateNamespace", PropertyType.Text)
    {
        EditorLabel = Texts.MessageBird_WhatsAppTemplateNamespaceLabel,
        EditorDescription = Texts.MessageBird_WhatsAppTemplateNamespaceDescription,
        IsRequired = false,
        Summary = false
    };

    public IntegrationDefinition Definition { get; } =
        new IntegrationDefinition(
            "MessageBird",
            Texts.MessageBird_Name,
            "<svg xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' viewBox='0 0 66 55' fill='#fff' fill-rule='evenodd' stroke='#000' stroke-linecap='round' stroke-linejoin='round'><path d='M57.425 9.57c-2.568 0-4.863 1.284-6.264 3.23l-9.454 13.228a1.9 1.9 0 0 1-1.556.817 1.91 1.91 0 0 1-1.906-1.906c0-.39.117-.778.31-1.05l7.975-11.945c.817-1.206 1.284-2.684 1.284-4.28C47.814 3.424 44.39 0 40.15 0H0v7.664h34.393c0 2.1-1.712 3.852-3.852 3.852H0c0 2.723.584 5.33 1.595 7.664H26.69c0 2.1-1.712 3.852-3.852 3.852H3.852a19.12 19.12 0 0 0 15.368 7.664h9.454a1.91 1.91 0 0 1 1.906 1.906 1.91 1.91 0 0 1-1.906 1.906H19.18L6.34 53.688h23.227c10.62 0 19.647-6.925 22.8-16.496l4.32-13.11c1.4-4.24 3.968-7.976 7.314-10.816-1.323-2.218-3.774-3.696-6.575-3.696zm0 5.252c-.778 0-1.44-.66-1.44-1.44a1.46 1.46 0 0 1 1.44-1.44 1.46 1.46 0 0 1 1.44 1.44c0 .817-.66 1.44-1.44 1.44z' stroke='none' fill='#2481d7'/></svg>",
            [
                AccessKeyProperty,
                PhoneNumberProperty,
                PhoneNumbersProperty,
                WhatsAppChannelIdProperty,
                WhatsAppTemplateNamespaceProperty,
                WhatsAppTemplateNameProperty
            ],
            [],
            new HashSet<string>
            {
                Providers.Sms
            })
        {
            Description = Texts.MessageBird_Description
        };
}
