# Begin the Session

After completing the [Chat Service Configuration](./chat-config), you can now enter the chat interface from the sidebar.

![Chat Interface](../assets/en/chat-overview-clean.png)

## Quick Start

***Select an available AI service on the left, then you can start typing in the chat box to begin a conversation with the AI.***

Next, we will briefly introduce the chat interface.

## "Contacts" Panel

On the left side of the chat interface, there are two main functional areas.

### Services and Agents

During the chat configuration, you might configure multiple AI services to meet different needs.

The application will display the configured AI chat services on the left side, allowing you to select a service and start chatting.

This panel also shows `Agents`, which are specific handlers based on AI services.

For example, the `Translation Expert` shown in the screenshot is an AI assistant specifically used for English-Chinese translation.

If you want to learn more about agents, please read [Agents and Presets](./agent-preset).

### Plugin Management

As AI has developed, mainstream AI services and models generally support tool calling (Tool Call).

Unlike AI services deployed in the cloud, Rodel Agent, as a native Windows application, can easily run local code.

Therefore, Rodel Agent supports users in creating local plugins to access AI tool calling based on [semantic-kernel](https://github.com/Microsoft/semantic-kernel). For more information, refer to [Tool Call](./chat-tools).

If you have a C# programming background, writing a plugin will be very easy. If you are interested, please refer to [Plugin Development](./tool-dev) to create your own plugin.

## Chat Panel

After selecting a `Service` / `Agent` / `Session Preset` from the left panel, the application will load the chat panel.

This interface should be familiar to you as it resembles most chat tools we usually use.

You can enter text in the bottom input box, press the `Enter` key to send it, and wait for the AI to respond.

<div style="max-width:400px">

![Generating Response](../assets/en/chat-generating.png)

</div>

### Switching Models

AI services usually provide multiple models. If you want to switch between different models, click the brain-like icon on the left side of the chat toolbar, then you can switch between different models.

<div style="max-width:300px">

![Switching Models](../assets/en/chat-switch-model.png)

</div>

### Message Tools

Rodel Agent supports individual operations on each message in the chat history.

Move your cursor over the message to see the message toolbar, where you can `Copy` / `Edit` / `Delete`.

<div style="max-width:240px">

![Message Tools](../assets/en/chat-message-tool.png)

</div>

This helps you fine-tune the AI's output.

> [!DANGER]
> Note that some AI services have strict requirements for message history, requiring alternating submission of user messages and AI messages (e.g., [Wenxin Yiyan](https://cloud.baidu.com/doc/WENXINWORKSHOP/s/clntwmv7t#body%E5%8F%82%E6%95%B0)). Deleting a message may cause the AI service to fail in generating a response.

## Management Panel

The right side of the interface is the management panel.

It is divided into two parts.

### Control Panel

The upper part is the control panel for the current session.

Here, you can enter system prompts or switch to `Session Options` to adjust the AI's behavior from a numerical perspective.

<div style="max-width:300px">

![Session Options](../assets/en/chat-session-options.png)

</div>

> [!TIP]
> Session options may vary depending on the current AI service. For "Open AI-like services," Open AI's parameter options will be uniformly applied, but this does not mean all parameters will be correctly processed. This depends on the capabilities of the specific service.

### Session List

The lower part of the management panel is for managing the session list.

Note that the session list here is not the entire session list but dynamically displays grouped lists based on the currently selected `Service` / `Assistant` / `Session Preset`.

This means that your chat sessions with Open AI will not be displayed in the Azure Open AI session list.

This helps better categorize sessions and avoid confusion. However, it may also introduce a problem where indexing session records can be more difficult.

Therefore, a global session content search feature will be introduced later to solve this problem. (TBD)

#### Session Renaming

For a new session, its name is `Casual Chat`, but sometimes we do not want the list full of casual chats.

To better manage historical sessions, we need to rename the session title.

You can right-click on a session, and there are two renaming options in the menu:

- Rename
- Smart Rename

Rename allows you to manually change the session title.

Smart Rename lets the AI write the title based on the first message in the session.

> [!TIP]
> When smart renaming, the AI service used is the current chat's AI service, so whether the name is appropriate depends on the model's capability.
>
> Also, since session renaming itself is a call to the AI, consuming Tokens, it is manually triggered rather than automatic renaming.