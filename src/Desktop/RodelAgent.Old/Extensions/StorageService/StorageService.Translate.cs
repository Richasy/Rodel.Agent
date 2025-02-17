// Copyright (c) Richasy. All rights reserved.

using RodelTranslate.Models.Client;
using RodelTranslate.Models.Constants;
using System.Text.Json;

namespace RodelAgent.UI.Extensions;

/// <summary>
/// 存储服务.
/// </summary>
public sealed partial class StorageService
{
    /// <inheritdoc/>
    public async Task<List<TranslateSession>?> GetTranslateSessionsAsync(ProviderType type)
    {
        await InitializeTranslateSessionsAsync();
        var sessions = _translateSessions.Where(s => s.Provider == type).OrderByDescending(p => p.Time ?? DateTimeOffset.MinValue).ToList();
        return sessions;
    }

    /// <inheritdoc/>
    public async Task AddOrUpdateTranslateSessionAsync(TranslateSession session)
    {
        await InitializeTranslateSessionsAsync();
        if (_translateSessions.Any(s => s.Id == session.Id))
        {
            _translateSessions.Remove(_translateSessions.First(s => s.Id == session.Id));
        }

        // 翻译会话数量限制最多100个.
        var currentProviderSessions = await GetTranslateSessionsAsync(session.Provider);
        if (currentProviderSessions.Count >= 100)
        {
            for (var i = currentProviderSessions.Count - 1; i >= 100; i--)
            {
                await RemoveTranslateSessionAsync(currentProviderSessions[i].Id);
            }
        }

        _translateSessions.Add(session);
        var id = session.Id;
        var v = JsonSerializer.Serialize(session);
        await _dbService.AddOrUpdateTranslateDataAsync(id, v);
    }

    /// <inheritdoc/>
    public async Task RemoveTranslateSessionAsync(string sessionId)
    {
        _translateSessions.RemoveAll(p => p.Id == sessionId);
        await _dbService.RemoveTranslateDataAsync(sessionId);
    }

    private async Task InitializeTranslateSessionsAsync()
    {
        if (_translateSessions == null)
        {
            var translateSessions = new List<TranslateSession>();
            var allSessions = await _dbService.GetAllTranslateSessionAsync();
            foreach (var session in allSessions)
            {
                try
                {
                    var sessionObj = JsonSerializer.Deserialize<TranslateSession>(session);
                    var parameters = _translateParametersFactory.CreateTranslateParameters(sessionObj.Provider);
                    parameters.SetDictionary(sessionObj.Parameters.ToDictionary());
                    sessionObj.Parameters = parameters;
                    translateSessions.Add(sessionObj);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            _translateSessions = translateSessions.OrderByDescending(p => p.Time ?? DateTimeOffset.MinValue).ToList();
        }
    }
}
