# 文生图服务配置

小幻助理支持多种文生图服务，请根据子标题索引，跳转到你想要配置的服务段落查看。

## Open AI

|||
|-|-|
|文档|https://platform.openai.com/docs/api-reference/images/create|
|API 令牌|https://platform.openai.com/account/api-keys|

Open AI 的配置相对来说很简单，只要你将 API Key 填入设置中的 `访问密钥` 即可。

### 代理与 API 兼容

现在 Open AI 的接口数据结构几乎成为了一种通用标准，有很多的 AI 服务在暴露接口时为了更易用，都会选择以 Open AI 相似的接口地址和数据结构发布。这就是所谓的 `Open AI 兼容接口`。

如果你正在使用的 AI 服务不在小幻助理的支持列表中，但又恰好使用了 Open AI 兼容接口，那么你就可以把该服务的地址写入 Open AI 设置区块的 `终结点（API）` 文本框中。

当然，还有另一种使用场景。

由于 Open AI 在部分国家或地区不可用，用户会尝试以代理服务器的方式使用 Open AI 服务，比如 [Open AI API Proxy](https://www.openai-proxy.com/)。

你依然需要使用自己的 API 密钥，但不直接访问 OpenAI 服务器，而是交由代理服务进行访问，从而避免被墙或者封号。

你可以将对应的服务地址填入 `终结点（API）` 中，以实现 API 代理功能。比如上面的代理服务，需要填入的地址就是：`https://api.openai-proxy.com/v1`

> [!WARNING]
> 上文提到的 Open AI API Proxy 仅用作说明该使用场景，开发者不对其安全性负责，使用者需要自行辨别服务的可靠性并为之负责。

> [!TIP]
> 对于代理来说，版本号通常是有必要的，比如 `https://api.openai-proxy.com/v1` 中的 `v1`。
> 因为 API 输入框还需要兼容其它 API 服务，有些服务并不见得会在请求路径中使用版本号。

## Azure Open AI

|||
|-|-|
|文档|https://learn.microsoft.com/azure/ai-services/openai/overview|
|Azure 服务|https://azure.microsoft.com/en-us/products/ai-services/openai-service|

Microsoft 与 Open AI 的关系较为特殊，有种 Open AI 特许经销商的感觉，所以你能够在 Azure 这个云服务平台创建 Open AI 资源，并按需部署 `Dall·E 3` 等 Open AI 模型。

虽然它们的模型一致，但是网络请求格式却不尽相同，所需要的配置项也不一样。

当你部署了 Open AI 资源后，你能在资源页面的 `资源管理` -> `密钥和终结点` 中找到所需的密钥（两个密钥任选其一即可）与终结点。

<div style="max-width: 500px">

![密钥与终结点](./assets/zh/azure-oai-secret.png)

</div>

将对应的值填入应用设置中。

你可能会对 API 版本感到好奇，这里是 [文档](https://learn.microsoft.com/azure/ai-services/openai/reference#completions)，通常情况下不需要改动。

### 添加自定义模型

Azure Open AI 相较于 Open AI，最大的不同在于你需要手动部署需要的模型。

在应用中，Azure Open AI 不提供预置模型，仅仅填写密钥和终结点并不能让配置生效，我们还需要创建自定义模型。

在 [Azure OpenAI Studio](https://) 中，你可以从模型库里部署特定的模型。

每个模型有自己的 Id，比如 Dall·E 3 的 Id 就是 dall-e-3。

<div style="max-width: 420px">

![Azure 模型部署](./assets/zh/azure-model-deploy-image.png)

</div>

在你部署模型时，Azure 会要求你提供 **部署名**，这个很重要，我们通过 API 与服务进行交互式，模型的标识符不是模型 ID，而是你部署模型时的部署名。

我推荐在部署模型时尽量使用和模型 ID 相同的部署名，以减少歧义。

在创建自定义模型时，你需要提供文生图模型支持的图片尺寸，对于 Dall·E 3 来说，它支持以下尺寸：

- 1024x1024
- 1792x1024
- 1024x1792

## 文心生图

|||
|-|-|
|文档|https://ai.baidu.com/ai-doc/NLP/clxim3bfq|
|API 令牌|https://console.bce.baidu.com/ai-engine/old/#/ai/ocr/app/list|

> 他妈的也不知道百度抽什么风，文档这里放一坨那里放一坨，一个授权方式都整出来3种。

和配置对话模型不同，这里你需要 [创建一个 AI 应用](https://ai.baidu.com/ai-doc/REFERENCE/Ck3dwjhhu#%E4%B8%80%E3%80%81access_token%E9%89%B4%E6%9D%83%E6%9C%BA%E5%88%B6)

在千帆大模型平台创建应用后，将 `API Key` 填入 `访问密钥`，将 `Secret Key` 填入 `密文（Secret）` 即可。

## 腾讯混元

|||
|-|-|
|文档|https://cloud.tencent.com/document/product/1729/106530|
|API 令牌|https://console.cloud.tencent.com/cam/capi|

混元生图模型托管在腾讯云上，你需要在 [API 密钥管理](https://console.cloud.tencent.com/cam/capi) 中创建一个密钥。

需要注意的是，由于安全限制，密钥的 `Secret Key` 仅在创建密钥时可见。

## 讯飞星火

|||
|-|-|
|文档|https://www.xfyun.cn/doc/spark/ImageGeneration.html|
|API 令牌|https://console.xfyun.cn/services/tti|

在使用讯飞星火推出的图片生成服务前，你需要在 [讯飞开放平台](https://www.xfyun.cn/) 先注册一个应用，然后开通对应的模型服务。

未开通的模型将无法使用。

注册后，你能在 `图片生成` 的模型页中找到 `服务接口认证信息`，这些是你需要填入应用中的设置项，具体对应关系如下：

- `APPID` -> `应用 ID`
- `APISecret` -> `密文（Secret）`
- `APIKey` -> `访问密钥`

## 智谱

|||
|-|-|
|文档|https://open.bigmodel.cn/dev/howuse/cogview|
|API 令牌|https://open.bigmodel.cn/usercenter/proj-mgmt/apikeys|

创建 API 密钥后填入小幻助理即可。

## xAI

|文档|https://docs.x.ai/docs/overview|
|-|-| 
|API 令牌|https://console.x.ai/team/default/api-keys|

和 [Open AI](#open-ai) 类似，获取访问密钥后，将其填入 `访问密钥` 中即可。