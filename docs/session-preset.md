# 会话预设

会话预设的概念类似 Ollama 的 [Modelfile](https://github.com/ollama/ollama/blob/main/docs/modelfile.md)。它用于创建一个会话模板，你可以在这个预设的基础上快速创建会话。

## 格式

会话预设（Session Preset）采用 JSON 格式定义。

```json
{
    "name": "OpenAI Gpt4 示例",
    "provider": "openai",
    "model": "gpt-4-turbo",
    "parameters": {
        "temperature": 0
    },
    "system": "你是一个专业的 C# 工程师"
}
```

|属性|类型|必填|描述|
|-|-|-|-|
|`name`|string|是|预设的名称，它也将作为标识符存在，所以不能创建同名预设|
|`model`|string|是|指定的对话模型 ID，在线模型需要填写其模型 ID，本地模型则是转义后的绝对路径（该本地模型必须预先导入应用）|
|`provider`|string|是|模型所属的服务，具体的服务 ID 参考 [支持的服务](./services) |
|`system`|string|否|系统提示词，可以用于指定 AI 扮演的角色等|
|`stream`|boolean|否|是否启用流式传输，默认为 `True`|
|`max_rounds`|int|否|最大对话轮次。<br/>*一问一答称之为一轮*，默认为 `0`，表示不限轮次。<br/>_假如该值为 `1`，则每次只会发送用户的问题而不引入前文；为 `2` 则只会引入前一轮对话，依此类推_|
|`messages`|array\<[ChatMessage](#chatmessage)>|否|预定义的会话记录|
|`parameters`|object|否|会话参数是一个动态对象，它具体的值由 `provider` 决定，所以关于它的属性及定义，请查阅 [支持的服务](./services)，对于本地模型特殊的指令模板，请查看 [模板指令](#模板指令) |
|`stop_sequences`|array\<string>|否|终止令牌序列|
|`filter_chars`|array\<string>|否|在模型生成内容后要过滤掉的文本|

## 消息类型

### ChatMessage

```json
{
    "role": "user",
    "content": "从现在开始，你叫小明。"
}
```

这是一种最为基础的写法，确定消息的发送方与内容。

该写法等同于：

```json{5-6}
{
    "role": "user",
    "content": [
        {
            "type": "text",
            "text": "从现在开始，你叫小明。"
        }
    ]
}
```

为什么需要有这两种定义方式？因为后一种可以支持图片以及未来可能的多媒体输入。

比如包含图片的消息定义如下：

```json
{
    "role": "user",
    "content": [
        {
            "type": "image",
            "text": "https://www.example.com/example.png",
            "detail": "auto"
        },
        {
            "type": "text",
            "text": "解析这张图片"
        }
    ]
}
```

完整的消息结构体定义如下：

|属性|类型|必填|描述|
|-|-|-|-|
|`role`|string|是|表示消息发送者的身份，可选值为 `user` 和 `assistant` |
|`content`|string 或 array\<[ChatMessageContent](#chatmessagecontent)>|是|表示消息的具体内容|
|`time`|long/int64|否|发送消息的 Unix 时间戳（秒），可以不填|

> [!WARNING]
> **role** 支持 `system` 角色，但系统提示词应尽量写在预设的专用属性 `system` 而不是混入消息列表中。

#### ChatMessageContent

|属性|类型|必填|描述|
|-|-|-|-|
|`type`|string|是|表示消息内容的类型，可选值为 `text` 和 `image` |
|`text`|string|是|- 在 type 为 text 时，该属性表示消息的文本内容<br/>- 在 type 为 image 时，该属性表示图片的链接或者是 base64 字符串（如果模型支持）|
|`detail`|string|否|图片解析细节，可选值为 `auto`, `high` 和 `low`，需要模型服务支持该参数|

## 模板指令

在创建本地模型的预设时，会话参数中存在以下属性：

|属性|类型|必填|描述|
|-|-|-|-|
|`system_template`|string|否|系统指令的模板字符串|
|`user_template`|string|否|用户指令的模板字符串|
|`assistant_template`|string|否|助理/机器人/模型回复指令的模板字符串|
|`end_template`|string|否|表示当聊天记录被转换成上下文发送给模型时追加的内容|

这些是特殊的 **`模板指令`**。

大模型在进行微调时通常都会以特定的指令模板进行训练，比如用特定的令牌（Token）表示特定的角色，或者表示一段文本结束。

对于没有特别指定模板指令的本地模型，应用会使用默认的格式来组织 Prompt，样例如下：

```
System: 你是一个专业的助手。

User: 你好。

Assistant: 你好，有什么我可以帮助你的？

User: 写一首关于鸡，黑子的诗。

Assistant:
```

在这个示例中，`System:`, `User:`, `Assistant:` 就是简单的指令，如果把这套指令写在预设中，那就是：

```json
{
    ...
    "parameters": {
        ...
        "system_template": "System: {{system}}",
        "user_template": "User: {{user}}",
        "assistant_template": "Assistant: {{assistant}}",
        "end_template": "Assistant: "
    }
}
```

> [!TIP]
> **为什么要有 `end_template`？**  
> 我们知道，聊天模型本质上是文本生成模型针对聊天场景（问答）特调的模型。  
> 所以在组织内容时，通常我们会留一个后缀，以便模型在此基础上续写文本，这也就是上面示例中结尾的 `Assistant:`，即 `end_template`  


### 使用不同的指令

默认指令能覆盖基本场景，但不同模型之间并不会共用一套标准的聊天指令

让我们以 [microsoft/Phi-3-mini-4k-instruct-gguf](https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-gguf#chat-format) 举例。

在文档中，微软提到我们调用模型时提供的 Prompt 应具备下面的结构

```
<|user|>
{{user_question}}<|end|>
<|assistant|>
{{assistant_response}}<|end|>
<|user|>
{{user_question}}<|end|>
<|assistant|>
```

首先，根据 [讨论](https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-gguf/discussions/11)，它不支持 `System` 角色。

其次，观察这个 Prompt 结构，我们会发现它具备更结构化的特征。

而基于此结构训练出的模型对于默认指令的支持并不好，故而我们需要自定义指令模板，其定义如下：

```json
{
    ...
    "parameters": {
        ...
        "system_template": "<|user|>\n{{system}}<|end|>",
        "user_template": "<|user|>\n{{user}}<|end|>",
        "assistant_template": "<|assistant|>\n{{assistant}}<|end|>",
        "end_template": "<|assistant|>"
    }
}
```

其中，<span v-pre>`{{system}}`</span>, <span v-pre>`{{user}}`</span>, <span v-pre>`{{assistant}}`</span> 这些属于各自属性下的占位符，它们最终会被替换成对应的文本内容。

## 从 Modelfile 迁移

会话预设这个功能是受了 Ollama 的 [Modelfile](https://github.com/ollama/ollama/blob/main/docs/modelfile.md) 启发，所以它们的原理基本一致。

但是在处理 `TEMPLATE` 时，Ollama 使用了基于 Go 语言模板的语法，这并不够通用（至少我无法在 C# 中直接使用），所以我选择了对我来说更方便的 JSON。

如果你打算将 Modelfile 中的 TEMPLATE 迁移到会话预设中，以下是一个示例。

### 示例

还是以 [microsoft/Phi-3-mini-4k-instruct-gguf](https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-gguf#chat-format) 举例。

微软提供了一个 [Modelfile 样例](https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-gguf/blob/main/Modelfile_q4)：

```go
FROM ./Phi-3-mini-4k-instruct-q4.gguf
TEMPLATE """<s>{{ if .Prompt }}<|user|>
{{ .Prompt }}<|end|>
{{ end }}<|assistant|>
{{ .Response }}<|end|>"""
PARAMETER stop <|endoftext|>
PARAMETER stop <|end|>
PARAMETER num_ctx 4096
```

转换成会话预设就是：

```json
{
    "model": "E:\\path\\to\\model\\Phi-3-mini-4k-instruct-q4.gguf",
    "name": "Phi3 Q4 预设",
    "provider": "local",
    "stop_sequences": [
        "<|endoftext|>",
        "<|end|>"
    ],
    "parameters": {
        "max_tokens": 4096,
        "user_template": "<|user|>\n{{user}}<|end|>",
        "assistant_template": "<|assistant|>\n{{assistant}}<|end|>",
    }
}
```