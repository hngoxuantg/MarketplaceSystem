using MarketplaceSystem.Web.UI.Interfaces;
using MarketplaceSystem.Web.UI.Models.ApiResponses;
using MarketplaceSystem.Web.UI.Models.ViewModels.Chat;
using MarketplaceSystem.Web.UI.Services.BaseServices;
using System.Text.Json;

namespace MarketplaceSystem.Web.UI.Services
{
    public class ChatService : BaseApiService, IChatService
    {
        private readonly JsonSerializerOptions _jsonOptions;

        public ChatService(IHttpClientFactory httpClient) : base(httpClient)
        {
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<PaginatedResponse<ConversationViewModel>> GetConversationsAsync(int pageNumber = 1, int pageSize = 12)
        {
            try
            {
                var request = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var response = await GetAsync("Chat/conversations", request);

                if (response?.IsSuccessStatusCode == true)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<PaginatedResponse<ConversationViewModel>>(content, _jsonOptions);
                    
                    return result ?? new PaginatedResponse<ConversationViewModel>
                    {
                        Data = new List<ConversationViewModel>(),
                        TotalCount = 0,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                }

                return new PaginatedResponse<ConversationViewModel>
                {
                    Data = new List<ConversationViewModel>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception)
            {
                return new PaginatedResponse<ConversationViewModel>
                {
                    Data = new List<ConversationViewModel>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }

        public async Task<List<MessageViewModel>> GetConversationMessagesAsync(int conversationId, int? lastMessageId = null, bool loadOlder = true)
        {
            try
            {
                var request = new
                {
                    ConversationId = conversationId,
                    LastMessageId = lastMessageId,
                    LoadOlder = loadOlder
                };

                var response = await GetAsync($"Chat/conversations/{conversationId}/messages", request);

                if (response?.IsSuccessStatusCode == true)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var messages = JsonSerializer.Deserialize<List<MessageViewModel>>(content, _jsonOptions);
                    
                    return messages ?? new List<MessageViewModel>();
                }

                return new List<MessageViewModel>();
            }
            catch (Exception)
            {
                return new List<MessageViewModel>();
            }
        }

        public async Task<MessageViewModel> SendMessageAsync(SendMessageViewModel model)
        {
            try
            {
                HttpResponseMessage? response;

                if (model.Image != null)
                {
                    // Upload image with message
                    using var stream = model.Image.OpenReadStream();
                    var files = new List<(Stream Stream, string FileName)>
                    {
                        (stream, model.Image.FileName)
                    };

                    var additionalFields = new Dictionary<string, string>
                    {
                        { "ReceiverId", model.ReceiverId.ToString() }
                    };

                    if (!string.IsNullOrEmpty(model.Content))
                    {
                        additionalFields.Add("Content", model.Content);
                    }

                    response = await PostFilesAsync("Chat/messages", files, "Image", additionalFields);
                }
                else
                {
                    // Send text message only
                    response = await PostAsync("Chat/messages", new
                    {
                        ReceiverId = model.ReceiverId,
                        Content = model.Content
                    });
                }

                if (response?.IsSuccessStatusCode == true)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var message = JsonSerializer.Deserialize<MessageViewModel>(content, _jsonOptions);
                    
                    return message ?? new MessageViewModel();
                }

                return new MessageViewModel();
            }
            catch (Exception)
            {
                return new MessageViewModel();
            }
        }

        public async Task<bool> MarkMessagesAsReadAsync(int conversationId)
        {
            try
            {
                var response = await PutAsync($"Chat/conversations/{conversationId}/read", new { });

                return response?.IsSuccessStatusCode == true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
