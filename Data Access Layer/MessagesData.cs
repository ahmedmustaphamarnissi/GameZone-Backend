using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.DTO;
using Models.Data.enums;
using GameZoneBack.Models;

namespace Data_Access_Layer; 

public class MessagesData : BaseData
{
    public MessagesData(IConfiguration config) : base(config)
    {
        
    }

    public async Task<MessagesDTO> GetMessagesAsync(int currentUserId, int pageNumber)
    {
        using var context = CreateDbContext();

        const int pageSize = 8;

        var query = context.Messages
            .Where(m =>
                m.SenderUserId == currentUserId ||
                m.ReceiverUserId == currentUserId);

        // Total number of different people the current user has conversations with
        var conversationsCount = await query
            .Select(m =>
                m.SenderUserId == currentUserId
                    ? m.ReceiverUserId
                    : m.SenderUserId)
            .Distinct()
            .CountAsync();

        // Get conversations ordered by the latest message
        var conversations = await query
            .GroupBy(m =>
                m.SenderUserId == currentUserId
                    ? m.ReceiverUserId
                    : m.SenderUserId)
            .OrderByDescending(g => g.Max(m => m.SentAt))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(g => new ConversationItemDTO
            {
                userId = g.Key,

                userName = g.Select(m =>
                    m.SenderUserId == currentUserId
                        ? m.ReceiverUser.UserName
                        : m.SenderUser.UserName
                ).First(),

                picture = g.Select(m =>
                    m.SenderUserId == currentUserId
                        ? m.ReceiverUser.Picture.Path
                        : m.SenderUser.Picture.Path
                ).First(),

                nonReadMessagesCount = g.Count(m =>
                    m.SenderUserId != currentUserId &&
                    !m.IsRead)
            })
            .ToListAsync();

        return new MessagesDTO
        {
            conversationsCount = conversationsCount,
            conversations = conversations
        };
    }


    public async Task<ConversationDTO> GetMessagesAsync(
    int currentUserId,
    int userId,
    int pageNumber)
    {
        using var context = CreateDbContext();

        const int pageSize = 6;

        var query = context.Messages
            .AsNoTracking()
            .Where(m =>
                (m.SenderUserId == currentUserId &&
                 m.ReceiverUserId == userId ) ||
                (m.ReceiverUserId == currentUserId &&
                 m.SenderUserId == userId ));

        var messagesCount = await query.CountAsync();

        var messages = await query
    .OrderByDescending(m => m.SentAt)
    .ThenByDescending(m => m.MessageId)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .Select(res => new MessageDTO
    {
        messageId = res.MessageId,
        isMine = res.SenderUserId == currentUserId,
        message = res.Message1,
        sentAt = res.SentAt,
        readAt = res.ReadAt,
        isRemovedBySender = res.IsDeletedBySender,
        isRemovedByReceiver = res.IsDeletedByReceiver
    })
    .ToListAsync();

        messages.Reverse();

        return new ConversationDTO
        {
            messages = messages,
            messagesCount = messagesCount
        };
    }
    public async Task<bool> ReadUserMessages(int currentUserId, int userId)
    {
        using var context = CreateDbContext();

        var messages = await context.Messages
            .Where(m =>
                m.SenderUserId == userId &&
                m.ReceiverUserId == currentUserId &&
                !m.IsRead)
            .ToListAsync();

        if (messages.Count == 0)
            return true;

        var readAt = DateTime.UtcNow;

        foreach (var message in messages)
        {
            message.IsRead = true;
            message.ReadAt = readAt;
        }

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> CeckIfTheUserIsTheSender(int currentUserId, int messageId)
    {
        using var context = CreateDbContext();
        return await context.Messages.AnyAsync(m => m.MessageId == messageId && m.SenderUserId == currentUserId);
    }

    public async Task<bool?> RemoveUserMessageAsync(RemoveMessageDTO request)
    {
        using var context = CreateDbContext();
        var message = await context.Messages.FindAsync(request.messageId);
        if(message == null)
            return null;
        switch(request.removeType)
        {
            case MessageRemoveType.ForMe:
                message.IsDeletedBySender = true;
                break;
            case MessageRemoveType.ForEveryone:
                message.IsDeletedBySender = true;
                message.IsDeletedByReceiver = true;
                break;
        }
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> SendMessageAsync(int currentUserId,SendMessageDTO request)
    {
        using var context = CreateDbContext();
        var message = new Message
        {
            SenderUserId = currentUserId,
            ReceiverUserId = request.receiverId,
            Message1 = request.message,
            SentAt = DateTime.UtcNow,
            IsRead = false,
            IsDeletedBySender = false,
            IsDeletedByReceiver = false
        };
        context.Messages.Add(message);
        return await context.SaveChangesAsync() > 0;
    }
}
