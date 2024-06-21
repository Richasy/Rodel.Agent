# Agent and Preset

Since the launch of ChatGPT, people have been exploring how to make the best use of AI.

The most widely spread and easiest method is `role-playing`.

The basic principle is to guide the AI model with `system prompts`, telling it the background of the session, its identity/duties/role, how it should respond to users, and so on, thus making the general-purpose AI perform specific tasks by playing a "role."

Rodel Agent provides two ways to help you create roles, which are `Agent` and `Preset`.

## Agent

**Agent = Role**

Click on the robot button at the top of the left panel to add a new Agent.

<div style="max-width:500px">

![Create Agent](../assets/en/new-agent.png)

</div>

First, you need to select an AI service, which the Agent will be bound to.

You can give it a name, then write system prompts to guide its behavior, and finally give it an avatar (optional).

Next, let's create an Agent responsible for English-Chinese translation to help you understand this process.

> [!TIP]
> The prompt content is taken from [lobe-chat-agents](https://github.com/lobehub/lobe-chat-agents/blob/main/src/en-cn-translator.zh-CN.json)

### Basic Information

|||
|-|-|
|Preset Name|English-Chinese Translator Agent|
|System Instructions|- Expertise: 双向翻译<br>- Language Pairs: 中文 <-> 英文<br>- Description: You are an expert in English-Chinese translation. You will translate Chinese input into English, or English input into Chinese. For non-Chinese content, you will provide the translation result in Chinese. Users can send you content that needs to be translated, and you will respond with the corresponding translation results, ensuring compliance with Chinese language habits. You can adjust the tone and style, considering the cultural connotations and regional differences of certain terms. As a translator, you need to ensure that the translated text meets the standards of "faithfulness, expressiveness, and elegance". "Faithfulness" means being faithful to the content and intent of the original text; "Expressiveness" means that the translation should be smooth and easy to understand, with clear expression; "Elegance" aims for cultural aesthetics and linguistic beauty in the translation. The goal is to create a translation that is both true to the original spirit and in line with the cultural and aesthetic preferences of the target language audience.|

Copy the above content into the corresponding input boxes in the creation panel.

For the avatar, you can choose any image you like or skip it.

### Parameter Settings

Click `Next`, and you will see a parameter settings panel.

![Session Parameters Configuration](../assets/en/preset-parameters.png)

It looks quite complex, but it's actually not too complicated.

Alright, you don't need to worry. You can hover over each parameter title to see the specific parameter description.

To create a translator, we mainly need to modify the following two parameters:

1. Temperature: We do not want the AI model to add any unnecessary flair in the translation, so set the temperature to `0` to make it as compliant as possible with our instructions.
2. Session Rounds: Suppose the content we want to translate is independent, then we don't want each translation to include previous chat records. We can set the session rounds to `1`, so each request will only send the current user's input without including the previous context.

Click `Finish`, and you will find the `English-Chinese Translator Agent` you just created in the chat list.

Now, start translating content!

<div style="max-width:450px">

![Translator Agent](../assets/en/agent-translator.png)

</div>

## Session Presets

Agents are essentially session presets.

However, they have different applications.

- **Agent**: Creates a role, describing and guiding its behavior, with an emphasis on system prompts.
- **Session Preset**: Saves a snapshot of a session, starting a new session based on this snapshot each time, with an emphasis on chat records and session parameters.

Sometimes, we test or interrogate a model to uncover some "special" content (such as jailbreaking).

In such cases, we frequently adjust parameters or modify chat records to fine-tune the model's output.

Once these tasks have reached a certain stage, we may want to save the current session so that next time we can continue interacting with the AI model based on the current chat records and parameters.

Therefore, the entry point for creating session presets is at the top of the chat interface, where there is a save button.

<div style="max-width:420px">

![Save Session](../assets/en/save-session.png)

</div>

Click save, confirm the current session parameters, and the session will be saved as a snapshot, appearing in the `Session Presets` section of the left panel.