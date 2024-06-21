// Copyright (c) Rodel. All rights reserved.

using System.Text.Json;
using RodelAgent.UI.Toolkits;
using RodelDraw.Models.Client;

namespace RodelAgent.UI.Extensions;

/// <summary>
/// 存储服务.
/// </summary>
public sealed partial class StorageService
{
    /// <inheritdoc/>
    public async Task<List<DrawSession>> GetDrawSessionsAsync()
    {
        await InitializeDrawSessionsAsync();
        var sessions = _drawSessions.OrderByDescending(p => p.Time ?? DateTimeOffset.MinValue).ToList();
        return sessions;
    }

    /// <inheritdoc/>
    public async Task AddOrUpdateDrawSessionAsync(DrawSession session, byte[] imageData)
    {
        await InitializeDrawSessionsAsync();
        if (_drawSessions.Any(s => s.Id == session.Id))
        {
            _drawSessions.Remove(_drawSessions.First(s => s.Id == session.Id));
        }

        _drawSessions.Add(session);
        var id = session.Id;
        var v = JsonSerializer.Serialize(session);
        await _dbService.AddOrUpdateDrawDataAsync(id, v);

        if (imageData == null || imageData.Length == 0)
        {
            return;
        }

        var imagePath = AppToolkit.GetDrawPicturePath(session.Id);
        if (!Directory.Exists(Path.GetDirectoryName(imagePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(imagePath));
        }

        await File.WriteAllBytesAsync(imagePath, imageData);
    }

    /// <inheritdoc/>
    public async Task RemoveDrawSessionAsync(string sessionId)
    {
        _drawSessions.RemoveAll(p => p.Id == sessionId);
        await _dbService.RemoveDrawDataAsync(sessionId);
        var imagePath = AppToolkit.GetDrawPicturePath(sessionId);
        if (File.Exists(imagePath))
        {
            await Task.Run(() => File.Delete(imagePath));
        }
    }

    private async Task InitializeDrawSessionsAsync()
    {
        if (_drawSessions == null)
        {
            var drawSessions = new List<DrawSession>();
            var allSessions = await _dbService.GetAllDrawSessionAsync();
            foreach (var session in allSessions)
            {
                try
                {
                    var sessionObj = JsonSerializer.Deserialize<DrawSession>(session);
                    if (sessionObj.Parameters != null)
                    {
                        var parameters = _drawParametersFactory.CreateDrawParameters(sessionObj.Provider);
                        parameters.SetDictionary(sessionObj.Parameters.ToDictionary());
                        sessionObj.Parameters = parameters;
                    }

                    drawSessions.Add(sessionObj);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            _drawSessions = drawSessions.OrderByDescending(p => p.Time ?? DateTimeOffset.MinValue).ToList();
        }
    }
}
