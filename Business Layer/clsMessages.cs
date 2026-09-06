using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Models.DTO;

namespace Business_Layer;

public class clsMessages : BaseService
{
    public clsMessages(IConfiguration config) : base(config)
    {
        
    }


    public async Task<MessagesDTO> GetMessagesAsync(int currentUserId, int pageNumber)
    {
        return await new Data_Access_Layer.MessagesData(_config).GetMessagesAsync(currentUserId, pageNumber);
    }
    public async Task<ConversationDTO> GetMessagesAsync(
    int currentUserId,
    int userId,
    int pageNumber)
    {
        return await new Data_Access_Layer.MessagesData(_config).GetMessagesAsync(currentUserId, userId, pageNumber);
    }

    public async Task<bool> ReadUserMessages(int currentUserId, int userId)
    {
        return await new Data_Access_Layer.MessagesData(_config).ReadUserMessages(currentUserId, userId);
    }

    public async Task<bool> CeckIfTheUserIsTheSender(int currentUserId, int messageId)
    {
        return await new Data_Access_Layer.MessagesData(_config).CeckIfTheUserIsTheSender(currentUserId, messageId);
    }

    public async Task<bool?> RemoveUserMessageAsync(RemoveMessageDTO request)
    {
        return await new Data_Access_Layer.MessagesData(_config).RemoveUserMessageAsync(request);
    }

    public async Task<bool> SendMessageAsync(int currentUserId, SendMessageDTO request)
    {
        return await new Data_Access_Layer.MessagesData(_config).SendMessageAsync(currentUserId, request);
    }
}
