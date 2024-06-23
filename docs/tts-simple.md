# 文本转语音

在完成 [文本转语音服务配置](./tts-config) 之后，现在你可以在侧边栏进入文本转语音界面。

![文本转语音界面](./assets/zh/tts-overview.png)

## 音色选择

小幻助理在输入框底部提供了基础的选择项。

第一个按钮是模型选择，比如对于 OpenAI 服务，你可以选择是 `tts` 或 `tts-hd`。对于 Azure TTS，由于没有模型的概念，这里默认是你填写的 Region。

第二个按钮是语言选择，对于 Open AI 及 Azure Open AI 服务，该选项可以忽略，因为服务会自行判断输入语言。但对于 Azure TTS 来说，不同的语言会有不同的声音可供选择。

第三个按钮调整具体的音色，应用会标注声音的性别，请按需取用。

## 语音合成与管理

简单来说，选好音色，你就可以在输入框中输入内容，点击 `发送`，你选择的服务就会帮助你把输入的文本合成为音频。

小幻助理统一使用 **wav** 格式保存音频数据。

生成的音频存放在工作目录的 `Speech` 文件夹中，文件名即音频会话 ID。

你可以在应用中查看生成历史，可以点击历史然后在应用内进行试听。

同时也可以用其它 APP 打开音频，或者保存到设备的其它地方。