﻿// ==========================================================================
//  Notifo.io
// ==========================================================================
//  Copyright (c) Sebastian Stehle
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Notifo.Domain.Apps;
using Notifo.Domain.Integrations;
using Notifo.Domain.Resources;
using Notifo.Domain.Users;
using Notifo.Domain.Utils;
using Notifo.Infrastructure;
using Notifo.Infrastructure.Reflection.Internal;

namespace Notifo.Domain.Channels.Email.Formatting;

public sealed class EmailFormatterLiquid : IEmailFormatter
{
    private readonly string defaultBodyHtml;
    private readonly string defaultBodyText;
    private readonly string defaultSubject;
    private readonly IImageFormatter imageFormatter;
    private readonly IEmailUrl emailUrl;

    public EmailFormatterLiquid(IImageFormatter imageFormatter, IEmailUrl emailUrl)
    {
        this.emailUrl = emailUrl;

        string ReadResource(string name)
        {
            return GetType().Assembly.GetManifestResourceString($"Notifo.Domain.Channels.Email.Formatting.{name}")!;
        }

        defaultBodyHtml = ReadResource("DefaultHtml.liquid.mjml");
        defaultBodyText = ReadResource("DefaultText.liquid.text");
        defaultSubject = ReadResource("DefaultSubject.text");

        this.imageFormatter = imageFormatter;
    }

    public ValueTask<EmailTemplate> CreateInitialAsync(
        CancellationToken ct = default)
    {
        var template = new EmailTemplate
        {
            BodyHtml = defaultBodyHtml,
            BodyText = defaultBodyText,
            Subject = defaultSubject
        };

        return new ValueTask<EmailTemplate>(template);
    }

    public async ValueTask<EmailTemplate> ParseAsync(EmailTemplate input, bool strict,
        CancellationToken ct = default)
    {
        var context = EmailContext.Create(PreviewData.Jobs, PreviewData.App, PreviewData.User, imageFormatter, emailUrl);

        await FormatAsync(input, context, true, strict);

        if (context.Errors?.Count > 0)
        {
            throw new EmailFormattingException(context.Errors);
        }

        return input;
    }

    public async ValueTask<FormattedEmail> FormatAsync(EmailTemplate input, IReadOnlyList<EmailJob> jobs, App app, User user, bool noCache = false,
        CancellationToken ct = default)
    {
        var context = EmailContext.Create(jobs, app, user, imageFormatter, emailUrl);

        var message = await FormatAsync(input, context, noCache, false);

        return new FormattedEmail(message, context.Errors);
    }

    private static async Task<EmailMessage> FormatAsync(EmailTemplate template, EmailContext context, bool noCache, bool strict)
    {
        var subject = string.Empty;

        if (!string.IsNullOrWhiteSpace(template.Subject))
        {
            subject = FormatSubject(template.Subject, context)!;
        }

        string? bodyText = null;

        if (!string.IsNullOrWhiteSpace(template.BodyText))
        {
            bodyText = FormatText(template.BodyText, context);
        }

        string? bodyHtml = null;

        if (!string.IsNullOrWhiteSpace(template.BodyHtml))
        {
            bodyHtml = await FormatBodyHtmlAsync(template.BodyHtml, context, noCache, strict)!;
        }

        var firstJob = context.Jobs[0];

        var message = new EmailMessage
        {
            BodyHtml = bodyHtml,
            BodyText = bodyText,
            FromEmail = template.FromEmail.OrDefault(firstJob.FromEmail!),
            FromName = template.FromEmail.OrDefault(firstJob.FromName!),
            Subject = subject,
            ToEmail = context.User.EmailAddress!,
            ToName = context.User.FullName
        };

        if (string.IsNullOrWhiteSpace(bodyHtml) && string.IsNullOrWhiteSpace(bodyText))
        {
            context.AddError(EmailTemplateType.General, Texts.Email_TemplateUndefined);
        }

        return message;
    }

    private static string? FormatSubject(string template, EmailContext context)
    {
        return RenderTemplate(template, context, EmailTemplateType.Subject, false);
    }

    private static string? FormatText(string template, EmailContext context)
    {
        var result = RenderTemplate(template, context, EmailTemplateType.BodyText, false);

        context.ValidateTemplate(result, EmailTemplateType.BodyText);

        return result;
    }

    private static async Task<string?> FormatBodyHtmlAsync(string template, EmailContext context, bool noCache, bool strict)
    {
        var result = RenderTemplate(template, context, EmailTemplateType.BodyHtml, noCache);

        context.ValidateTemplate(result, EmailTemplateType.BodyHtml);

        var (rendered, errors) = await MjmlRenderer.RenderAsync(result, strict);

        foreach (var error in errors.OrEmpty())
        {
            context.AddError(EmailTemplateType.BodyHtml, error);
        }

        return AddTrackingLinks(rendered, context);
    }

    private static string? RenderTemplate(string template, EmailContext context, EmailTemplateType type, bool noCache)
    {
        var (rendered, errors) = context.Liquid.Render(template, noCache);

        foreach (var error in errors.OrEmpty())
        {
            context.AddError(type, error);
        }

        return rendered;
    }

    private static string? AddTrackingLinks(string? html, EmailContext context)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return html!;
        }

        foreach (var job in context.Jobs)
        {
            if (!string.IsNullOrEmpty(job.Notification.TrackSeenUrl))
            {
                var trackingLink = job.Notification.HtmlTrackingLink(job.ConfigurationId);

                html = html.Replace("</body>", $"{trackingLink}</body>", StringComparison.OrdinalIgnoreCase);
            }
        }

        return html;
    }
}
