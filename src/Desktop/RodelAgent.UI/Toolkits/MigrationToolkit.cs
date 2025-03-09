// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using Richasy.AgentKernel;
using RodelAgent.Context;
using RodelAgent.Models.Common;
using RodelAgent.Models.Feature;
using RodelAgent.UI.Models.Constants;
using SqlSugar;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Windows.ApplicationModel;

namespace RodelAgent.UI.Toolkits;

/// <summary>
/// 迁移工具箱.
/// </summary>
public static class MigrationToolkit
{
    internal static bool ShouldMigrate()
    {
        var libPath = SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);
        var dbFiles = Directory.EnumerateFiles(libPath, "*.db", SearchOption.TopDirectoryOnly).Where(p => !p.Contains(".old", StringComparison.Ordinal));
        return dbFiles.Any();
    }

    internal static async Task TryMigrateAsync()
    {
        var libPath = SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);
        var dbFiles = Directory.EnumerateFiles(libPath, "*.db", SearchOption.TopDirectoryOnly).Where(p => !p.Contains(".old", StringComparison.Ordinal));

        SettingsToolkit.WriteLocalSetting(SettingNames.MigrationFailed, false);

        try
        {
            var shouldMove = File.Exists(Path.Combine(libPath, "move"));
            foreach (var dbFile in dbFiles)
            {
                if (!shouldMove)
                {
                    var dbFileName = Path.GetFileName(dbFile);
                    if (dbFileName == "secret.db")
                    {
                        await MigrateSecretDbAsync(dbFile);
                    }
                    else if (dbFileName == "draw.db")
                    {
                        await MigrateDrawDbAsync(dbFile);
                    }
                    else if (dbFileName == "audio.db")
                    {
                        await MigrateAudioDbAsync(dbFile);
                    }
                    else if (dbFileName == "chat.db")
                    {
                        await MigrateChatDbAsync(dbFile);
                    }
                }
                else
                {
                    await MoveOldDbFileAsync(dbFile);
                }
            }

            if (!shouldMove)
            {
                await File.Create(Path.Combine(libPath, "move")).DisposeAsync();
            }
            else
            {
                File.Delete(Path.Combine(libPath, "move"));
            }
        }
        catch (Exception ex)
        {
            GlobalDependencies.Kernel.Get<ILogger<App>>().LogError(ex, $"Failed to migrate database.");
            SettingsToolkit.WriteLocalSetting(SettingNames.MigrationFailed, true);
            throw;
        }
    }

    private static async Task MigrateSecretDbAsync(string dbPath)
    {
        var data = await GetOldDataFromDatabaseAsync<SecretMeta>(dbPath, "Metadata");
        data = MigrateSecretData(data);
        var libPath = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.WorkingDirectory, string.Empty);
        var service = new SecretDataService(libPath, Package.Current.InstalledPath);
        await service.InitializeAsync();
        await service.BatchAddSecretsAsync(data);
    }

    private static async Task MigrateDrawDbAsync(string dbPath)
    {
        var data = await GetOldDataFromDatabaseAsync<DrawMeta>(dbPath, "Sessions");
        data = MigrateDrawData(data);
        var libPath = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.WorkingDirectory, string.Empty);
        var service = new DrawDataService(libPath, Package.Current.InstalledPath);
        await service.InitializeAsync();
        await service.BatchAddSessionsAsync(data);
    }

    private static async Task MigrateAudioDbAsync(string dbPath)
    {
        var data = await GetOldDataFromDatabaseAsync<AudioMeta>(dbPath, "Sessions");
        data = MigrateAudioData(data);
        var libPath = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.WorkingDirectory, string.Empty);
        var service = new AudioDataService(libPath, Package.Current.InstalledPath);
        await service.InitializeAsync();
        await service.BatchAddSessionsAsync(data);
    }

    private static async Task MigrateChatDbAsync(string dbPath)
    {
        var data = await GetOldDataFromDatabaseAsync<ChatMeta>(dbPath, "Metadata");
        data = MigrateChatData(data);
        var libPath = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.WorkingDirectory, string.Empty);
        var service = new ChatDataService(libPath, Package.Current.InstalledPath);
        await service.InitializeAsync();
        await service.BatchAddConversationsAsync(data);
    }

    private static async Task<List<T>> GetOldDataFromDatabaseAsync<T>(string dbPath, string tableName)
    {
        return await Task.Run(async () =>
        {
            using var sql = new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = $"Data Source={dbPath}",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    EntityService = (c, p) =>
                    {
                        if (!p.IsPrimarykey && new NullabilityInfoContext().Create(c).WriteState is NullabilityState.Nullable)
                        {
                            p.IsNullable = true;
                        }
                    }
                }
            });

            sql.CodeFirst.InitTables<T>();
            return await sql.Queryable<T>().AS(tableName).ToListAsync();
        });
    }

    private static async Task MoveOldDbFileAsync(string dbPath)
    {
        try
        {
            await Task.Delay(1000);
            var libFolder = SettingsToolkit.ReadLocalSetting(SettingNames.WorkingDirectory, string.Empty);
            var relativePath = Path.GetRelativePath(libFolder, Path.GetDirectoryName(dbPath)!);
            relativePath = relativePath.Trim('\\');
            var sqliteFolder = Path.Combine(libFolder, ".old");
            var oldFolder = Path.Combine(sqliteFolder, relativePath);
            if (!Directory.Exists(oldFolder))
            {
                Directory.CreateDirectory(oldFolder);
            }

            var oldFileName = Path.Combine(oldFolder!, Path.GetFileName(dbPath));
            File.Move(dbPath, oldFileName, true);
        }
        catch (Exception ex)
        {
            GlobalDependencies.Kernel.Get<ILogger<App>>().LogError(ex, $"Failed to move old database file. {Path.GetFileName(dbPath)}");
            throw;
        }
    }

    private static List<ChatMeta> MigrateChatData(List<ChatMeta> data)
    {
        var newList = new List<ChatMeta>();
        foreach (var meta in data)
        {
            var jsonNode = JsonNode.Parse(meta.Value)!;

            var id = jsonNode["id"]?.ToString();
            var maxRounds = jsonNode["max_rounds"]?.GetValue<int>() ?? 0;
            var model = jsonNode["model"]?.ToString();
            var presetId = jsonNode["preset_id"]?.ToString();
            var provider = jsonNode["provider"]?.ToString();
            var stream = jsonNode["stream"]?.GetValue<bool>();
            var system = jsonNode["system"]?.ToString();
            var title = jsonNode["title"]?.ToString();

            if (provider == "qianfan")
            {
                provider = "ernie";
            }
            else if (provider == "spark_desk")
            {
                provider = "spark";
            }
            else if (provider == "dash_scope")
            {
                provider = "qwen";
            }

            var isGroup = jsonNode["agents"] is not null;

            var conversation = new ChatConversation
            {
                Id = id!,
                Title = title,
                MaxRounds = maxRounds,
                UseStreamOutput = stream,
                Model = model,
                SystemInstruction = system,
            };

            if (isGroup)
            {
                conversation.GroupId = presetId;
            }
            else
            {
                conversation.AgentId = presetId;
            }

            if (provider is not null)
            {
                conversation.Provider = ToChatProviderType(provider);
            }

            var messages = new List<ChatInteropMessage>();
            var messageArray = jsonNode["messages"]?.AsArray();
            if (messageArray is not null)
            {
                foreach (var messageNode in messageArray)
                {
                    var author = messageNode!["author"]?.ToString();
                    var authorId = messageNode["author_id"]?.ToString();
                    var content = messageNode["content"]?.ToString();
                    var role = messageNode["role"]?.ToString();
                    var time = messageNode["time"]?.GetValue<long>() ?? 0L;

                    var msg = new ChatInteropMessage
                    {
                        AgentId = authorId,
                        Message = content ?? string.Empty,
                        Role = role ?? "user",
                        Time = time,
                    };

                    messages.Add(msg);
                }

                conversation.History = messages;
            }

            var parameterNode = jsonNode["parameters"];
            if (parameterNode is not null)
            {
                var options = new ChatOptions();
                var parameters = parameterNode.AsObject();
                foreach (var parameter in parameters)
                {
                    var key = parameter.Key;
                    if (key == "frequency_penalty")
                    {
                        options.FrequencyPenalty = parameter.Value!.GetValue<float>();
                    }
                    else if (key == "max_tokens")
                    {
                        options.MaxOutputTokens = parameter.Value!.GetValue<int>();
                    }
                    else if (key == "temperature")
                    {
                        options.Temperature = parameter.Value!.GetValue<float>();
                    }
                    else if (key == "top_p")
                    {
                        options.TopP = parameter.Value!.GetValue<float>();
                    }
                    else if (key == "top_k")
                    {
                        options.TopK = parameter.Value!.GetValue<int>();
                    }
                    else if (key == "presence_penalty")
                    {
                        options.PresencePenalty = parameter.Value!.GetValue<float>();
                    }
                }

                conversation.Options = options;
            }

            var tools = new List<string>();
            var toolsArray = jsonNode["tools"]?.AsArray();
            if (toolsArray is not null)
            {
                foreach (var toolNode in toolsArray)
                {
                    var tool = toolNode!.ToString();
                    tools.Add(tool);
                }

                conversation.Tools = tools;
            }

            if (isGroup)
            {
                var agents = new List<string>();
                var agentsArray = jsonNode["agents"]?.AsArray();
                if (agentsArray is not null)
                {
                    foreach (var agentNode in agentsArray)
                    {
                        var agent = agentNode!.ToString();
                        agents.Add(agent);
                    }
                    conversation.Agents = agents;
                }

                var terminateSequence = new List<string>();
                var terminateSequenceArray = jsonNode["terminate_text"]?.AsArray();
                if (terminateSequenceArray is not null)
                {
                    foreach (var sequenceNode in terminateSequenceArray)
                    {
                        var sequence = sequenceNode!.ToString();
                        terminateSequence.Add(sequence);
                    }

                    conversation.TerminateSequence = terminateSequence;
                }
            }

            var newMeta = new ChatMeta { Id = meta.Id, Value = JsonSerializer.Serialize(conversation, JsonGenContext.Default.ChatConversation) };
            newList.Add(newMeta);
        }

        return [.. newList.Distinct()];
    }

    private static List<AudioMeta> MigrateAudioData(List<AudioMeta> data)
    {
        var newList = new List<AudioMeta>();
        foreach (var item in data)
        {
            var jsonNode = JsonNode.Parse(item.Value);
            var providerValue = jsonNode!["provider"]?.GetValue<int>();
            if (providerValue.HasValue)
            {
                var providerStr = JsonSerializer.Serialize((AudioProviderType)providerValue.Value, JsonGenContext.Default.AudioProviderType);
                jsonNode["provider"] = providerStr.Replace("\"", string.Empty, StringComparison.Ordinal);
            }

            newList.Add(new AudioMeta { Id = item.Id, Value = jsonNode!.ToString() });
        }

        return [.. newList.Distinct()];
    }

    private static List<DrawMeta> MigrateDrawData(List<DrawMeta> data)
    {
        var newList = new List<DrawMeta>();
        foreach (var item in data)
        {
            var jsonNode = JsonNode.Parse(item.Value);
            if (jsonNode!["request"] is JsonObject jobj)
            {
                jobj.TryGetPropertyValue("negative_prompt", out var negativePrompt);
                jobj.TryGetPropertyValue("prompt", out var prompt);
                jobj.TryGetPropertyValue("size", out var size);
                var negativePromptStr = negativePrompt?.ToString();
                var promptStr = prompt?.ToString();
                var sizeStr = size?.ToString();
                if (!string.IsNullOrEmpty(negativePromptStr))
                {
                    jsonNode["negative_prompt"] = negativePromptStr.Replace("\n", "\\n", StringComparison.OrdinalIgnoreCase);
                }

                if (!string.IsNullOrEmpty(promptStr))
                {
                    jsonNode["prompt"] = promptStr.Replace("\n", "\\n", StringComparison.OrdinalIgnoreCase);
                }

                if (!string.IsNullOrEmpty(sizeStr))
                {
                    var parts = sizeStr.Split('x');
                    if (parts.Length == 2 && int.TryParse(parts[0], out var width) && int.TryParse(parts[1], out var height))
                    {
                        jsonNode["size"] = JsonNode.Parse($"{{\"width\":{width},\"height\":{height}}}");
                    }
                }
            }

            var providerValue = jsonNode["provider"]?.GetValue<int>();
            if (providerValue.HasValue)
            {
                var providerStr = JsonSerializer.Serialize((DrawProviderType)providerValue.Value, JsonGenContext.Default.DrawProviderType);
                jsonNode["provider"] = providerStr.Replace("\"", string.Empty, StringComparison.Ordinal);
            }

            newList.Add(new DrawMeta { Id = item.Id, Value = jsonNode!.ToString() });
        }

        return [.. newList.Distinct()];
    }

    private static List<SecretMeta> MigrateSecretData(List<SecretMeta> data)
    {
        var newList = new List<SecretMeta>();
        foreach (var item in data)
        {
            if (item.Id.StartsWith("Draw", StringComparison.Ordinal) && !string.IsNullOrEmpty(item.Value))
            {
                // 解析 JSON 字符串为 JsonNode
                var jsonNode = JsonNode.Parse(item.Value);
                if (jsonNode is null)
                {
                    continue;
                }

                // 获取 models 数组
                if (jsonNode!["models"] is JsonArray modelsArray)
                {
                    foreach (var model in modelsArray)
                    {
                        // 获取 sizes 数组
                        if (model!["sizes"] is JsonArray sizesArray)
                        {
                            var newSizesArray = new JsonArray();
                            foreach (var size in sizesArray)
                            {
                                if (size != null)
                                {
                                    // 分割字符串获取宽度和高度
                                    var parts = size.ToString().Split('x');
                                    if (parts.Length == 2 && int.TryParse(parts[0], out int width) && int.TryParse(parts[1], out int height))
                                    {
                                        var newSizeNode = JsonNode.Parse($"{{\"width\":{width},\"height\":{height}}}");
                                        newSizesArray.Add(newSizeNode);
                                    }
                                }
                            }
                            // 替换原有的 sizes 数组
                            model["sizes"] = newSizesArray;
                        }
                    }
                }

                newList.Add(new SecretMeta { Id = item.Id, Value = jsonNode!.ToString() });
            }
            else
            {
                newList.Add(item);
            }
        }

        return newList;
    }

    private static ChatProviderType ToChatProviderType(string oldProvider)
    {
        return oldProvider.ToLowerInvariant() switch
        {
            "openai" => ChatProviderType.OpenAI,
            "azure_openai" or "azureopenai" => ChatProviderType.AzureOpenAI,
            "azure_ai" or "azureai" => ChatProviderType.AzureAI,
            "gemini" => ChatProviderType.Gemini,
            "anthropic" => ChatProviderType.Anthropic,
            "open_router" => ChatProviderType.OpenRouter,
            "together_ai" or "togetherai" => ChatProviderType.TogetherAI,
            "groq" => ChatProviderType.Groq,
            "perplexity" => ChatProviderType.Perplexity,
            "mistral" => ChatProviderType.Mistral,
            "moonshot" => ChatProviderType.Moonshot,
            "zhipu" => ChatProviderType.ZhiPu,
            "lingyi" => ChatProviderType.LingYi,
            "qwen" => ChatProviderType.Qwen,
            "ernie" => ChatProviderType.Ernie,
            "spark" => ChatProviderType.Spark,
            "deep_seek" or "deepseek" => ChatProviderType.DeepSeek,
            "hunyuan" => ChatProviderType.Hunyuan,
            "ollama" => ChatProviderType.Ollama,
            "silicon_flow" or "siliconflow" => ChatProviderType.SiliconFlow,
            "doubao" => ChatProviderType.Doubao,
            "xai" => ChatProviderType.XAI,
            "onnx" => ChatProviderType.Onnx,
            "windows" => ChatProviderType.Windows,
            _ => throw new JsonException(),
        };
    }
}
