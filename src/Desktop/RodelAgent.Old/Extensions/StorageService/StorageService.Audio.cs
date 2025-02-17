// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.Toolkits;
using RodelAudio.Models.Client;
using System.Text.Json;

namespace RodelAgent.UI.Extensions;

/// <summary>
/// 存储服务.
/// </summary>
public sealed partial class StorageService
{
    /// <inheritdoc/>
    public async Task<List<AudioSession>?> GetAudioSessionsAsync()
    {
        await InitializeAudioSessionsAsync();
        var sessions = _audioSessions.OrderByDescending(p => p.Time ?? DateTimeOffset.MinValue).ToList();
        return sessions;
    }

    /// <inheritdoc/>
    public async Task AddOrUpdateAudioSessionAsync(AudioSession session, byte[]? imageData)
    {
        await InitializeAudioSessionsAsync();
        if (_audioSessions.Any(s => s.Id == session.Id))
        {
            _audioSessions.Remove(_audioSessions.First(s => s.Id == session.Id));
        }

        _audioSessions.Add(session);
        var id = session.Id;
        var v = JsonSerializer.Serialize(session);
        await _dbService.AddOrUpdateAudioDataAsync(id, v);

        if (imageData == null || imageData.Length == 0)
        {
            return;
        }

        var imagePath = AppToolkit.GetSpeechPath(session.Id);
        if (!Directory.Exists(Path.GetDirectoryName(imagePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(imagePath));
        }

        await File.WriteAllBytesAsync(imagePath, imageData);
    }

    /// <inheritdoc/>
    public async Task RemoveAudioSessionAsync(string sessionId)
    {
        _audioSessions.RemoveAll(p => p.Id == sessionId);
        await _dbService.RemoveAudioDataAsync(sessionId);
        var imagePath = AppToolkit.GetSpeechPath(sessionId);
        if (File.Exists(imagePath))
        {
            await Task.Run(() => File.Delete(imagePath));
        }
    }

    private async Task InitializeAudioSessionsAsync()
    {
        if (_audioSessions == null)
        {
            var audioSessions = new List<AudioSession>();
            var allSessions = await _dbService.GetAllAudioSessionAsync();
            foreach (var session in allSessions)
            {
                try
                {
                    var sessionObj = JsonSerializer.Deserialize<AudioSession>(session);
                    if (sessionObj.Parameters != null)
                    {
                        var parameters = _audioParametersFactory.CreateAudioParameters(sessionObj.Provider);
                        parameters.SetDictionary(sessionObj.Parameters.ToDictionary());
                        sessionObj.Parameters = parameters;
                    }

                    audioSessions.Add(sessionObj);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            _audioSessions = audioSessions.OrderByDescending(p => p.Time ?? DateTimeOffset.MinValue).ToList();
        }
    }
}
