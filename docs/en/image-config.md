# Text-to-Image Service Configuration

Rodel Agent supports a variety of text-to-image services. Please refer to the section of the service you want to configure by using the index below.

## Open AI

|||  
|-|-|  
|Documentation|https://platform.openai.com/docs/api-reference/images/create|  
|API Token|https://platform.openai.com/account/api-keys|  

Configuring Open AI is relatively simple. You only need to enter the API Key into the `Access Key` field in the settings.

### Proxy and API Compatibility

Currently, Open AI’s interface data structure has almost become a common standard. Many AI services choose to release their interfaces with addresses and data structures similar to Open AI to be more user-friendly. These are known as `Open AI Compatible Interfaces`.

If the AI service you are using is not on the supported list of Rodel Agent but uses an Open AI compatible interface, you can enter the address of this service into the `Endpoint (API)` text box in the Open AI settings block.

There is also another use case.

Due to the unavailability of Open AI in certain countries or regions, users might attempt to use Open AI services via proxy servers, such as [Open AI API Proxy](https://www.openai-proxy.com/).

You still need to use your own API key but do not directly access the OpenAI server; instead, you let the proxy service access it, thus avoiding blocks or bans.

You can enter the corresponding service address into the `Endpoint (API)` field to implement API proxy functionality. For the proxy service mentioned above, the address to be entered is: `https://api.openai-proxy.com/v1`

> [!WARNING]  
> The Open AI API Proxy mentioned above is for illustrating the use case. The developer is not responsible for its security. Users need to judge the reliability of the service themselves and bear the responsibility for its use.

> [!TIP]  
> For proxies, the version number is usually necessary, such as `v1` in `https://api.openai-proxy.com/v1`.  
> Since the API input box also needs to be compatible with other API services, some services may not use a version number in the request path.

## Azure Open AI

|||  
|-|-|  
|Documentation|https://learn.microsoft.com/azure/ai-services/openai/overview|  
|Azure Service|https://azure.microsoft.com/en-us/products/ai-services/openai-service|  

Microsoft’s relationship with Open AI is quite unique, resembling an Open AI authorized reseller. Thus, you can create Open AI resources on the Azure cloud service platform and deploy Open AI models like `Dall·E 3` as needed.

Although the models are the same, the network request formats differ, and the required configuration items are also different.

After deploying an Open AI resource, you can find the required keys (any of the two keys will work) and endpoint under `Resource Management` -> `Keys and Endpoints` on the resource page.

<div style="max-width: 500px">  

![Keys and Endpoints](../assets/en/azure-oai-secret.png)  

</div>  

Enter the corresponding values into the application settings.

You might be curious about the API version, here is the [documentation](https://learn.microsoft.com/azure/ai-services/openai/reference#completions), usually, there is no need to change it.

### Adding Custom Models

Compared to Open AI, Azure Open AI’s biggest difference is that you need to manually deploy the required models.

In the application, Azure Open AI does not provide preset models; simply filling in the keys and endpoint will not activate the configuration. We also need to create custom models.

In [Azure OpenAI Studio](https://), you can deploy specific models from the model library.

Each model has its own ID, for example, the ID of Dall·E 3 is dall-e-3.

<div style="max-width: 420px">  

![Azure Model Deployment](../assets/en/azure-model-deploy-image.png)  

</div>  

When deploying a model, Azure will require you to provide a **deployment name**, which is crucial. When we interact with the service via API, the model’s identifier is not the model ID but the deployment name you provided when deploying the model.

I recommend using a deployment name that matches the model ID as much as possible to reduce ambiguity.

When creating a custom model, you need to provide the image dimensions supported by the text-to-image model. For Dall·E 3, it supports the following dimensions:

- 1024x1024  
- 1792x1024  
- 1024x1792  

## ERNIE

|||  
|-|-|  
|Documentation|https://ai.baidu.com/ai-doc/NLP/clxim3bfq|  
|API Token|https://console.bce.baidu.com/ai-engine/old/#/ai/ocr/app/list|  

> Damn, I don't know what's going on with Baidu, but their documentation is all over the place, and they've come up with 3 different authorization methods.

Unlike configuring conversational models, here you need to [create an AI application](https://ai.baidu.com/ai-doc/REFERENCE/Ck3dwjhhu#%E4%B8%80%E3%80%81access_token%E9%89%B4%E6%9D%83%E6%9C%BA%E5%88%B6).

After creating an application on the Qianfan Large Model Platform, fill in the `API Key` as the `Access Key`, and the `Secret Key` as the `Secret Key`.

## Tencent Hunyuan

|||  
|-|-|  
|Documentation|https://cloud.tencent.com/document/product/1729/106530|  
|API Token|https://console.cloud.tencent.com/cam/capi|  

The Hunyuan text-to-image model is hosted on Tencent Cloud. You need to create a key in the [API Key Management](https://console.cloud.tencent.com/cam/capi) section.

Please note that due to security restrictions, the `Secret Key` is only visible when the key is created.

## iFlytek Spark

|||  
|-|-|  
|Documentation|https://www.xfyun.cn/doc/spark/ImageGeneration.html|  
|API Token|https://console.xfyun.cn/services/tti|  

Before using the image generation service launched by iFlytek Spark, you need to register an application on the [iFlytek Open Platform](https://www.xfyun.cn/) and enable the corresponding model service.

Models that are not enabled cannot be used.

After registration, you can find the `Service Interface Authentication Information` on the model page of `Image Generation`. These are the settings you need to fill in the application, with the specific corresponding relationships as follows:

- `APPID` -> `Application ID`  
- `APISecret` -> `Secret`  
- `APIKey` -> `Access Key`

## Zhipu

|||
|-|-|
|Document|https://open.bigmodel.cn/dev/howuse/cogview|
|API Token|https://open.bigmodel.cn/usercenter/proj-mgmt/apikeys|

Fill in the API key in Rodel Agent after creating it.

## xAI

|Document|https://docs.x.ai/docs/overview|
|-|-|
|API Token|https://console.x.ai/team/default/api-keys|

Similar to [Open AI](#open-ai), get the access key and fill it in the `access key` field.