# Text-to-Speech Service Configuration

Rodel Agent supports multiple text-to-speech services. Please refer to the sections indexed by subheadings to configure the desired service.

## Open AI

|||  
|-|-|  
|Documentation|https://platform.openai.com/docs/api-reference/chat/create|  
|API Token|https://platform.openai.com/account/api-keys|  

Configuring Open AI is relatively simple. Just fill in the API Key in the `Access Key` setting.

## Azure Open AI

|||  
|-|-|  
|Documentation|https://learn.microsoft.com/azure/ai-services/openai/overview|  
|Azure Service|https://azure.microsoft.com/en-us/products/ai-services/openai-service|  

The relationship between Microsoft and Open AI is quite special, akin to Open AI’s exclusive distributor. Thus, you can create Open AI resources on the Azure cloud service platform and deploy Open AI models like `tts` and `tts-hd` as needed.

Although the models are the same, the network request formats differ, as do the required configuration items.

Once you have deployed an Open AI resource, you can find the required keys (either one of the two keys will suffice) and endpoint on the resource page under `Resource Management` -> `Keys and Endpoints`.

<div style="max-width: 500px">  
![Keys and Endpoints](../assets/en/azure-oai-secret.png)  
</div>

### Adding Custom Models

The biggest difference between Azure Open AI and Open AI is that you need to manually deploy the required models.

In the application, Azure Open AI does not provide pre-configured models. Simply filling in the key and endpoint will not make the configuration effective; you also need to create custom models.

In [Azure OpenAI Studio](https://), you can deploy specific models from the model library.

Each model has its own Id, for example, the Id for Dall·E 3 is dall-e-3.

<div style="max-width: 420px">  
![Azure Model Deployment](../assets/en/azure-model-deploy-image.png)  
</div>

When deploying a model, Azure will ask you to provide a **Deployment Name**, which is crucial. We interact with the service via API using the deployment name, not the model ID, as the model identifier.

I recommend using the same deployment name as the model ID to reduce ambiguity when deploying models.

## Azure TTS

|||  
|-|-|  
|Documentation|https://learn.microsoft.com/azure/ai-services/speech-service/text-to-speech|  
|Azure Service|https://azure.microsoft.com/zh-cn/products/ai-services/text-to-speech|  

Azure's speech service inherently includes text-to-speech capabilities, which we can deploy ourselves according to the documentation.

Then, simply fill in the `Access Key` and `Region` of the corresponding resource into the application settings.

## Volcano Engine Speech Technology

|||
|-|-|
|Documentation|https://www.volcengine.com/docs/6561/1257543|
|Console|https://console.volcengine.com/speech/service/10007|

Please refer to the official documentation for specific service billing rules. New registrations have a certain amount of free quota.

Rodel Agent has selected a few representative tones from the `Large Speech Synthesis Model`. If the tone you need is not in the list of tones provided by Rodel Agent, please raise an issue.

## Tencent Cloud Speech Synthesis

|||
|-|-|
|Documentation|https://cloud.tencent.com/document/product/1073/37995|
|API Key|https://console.cloud.tencent.com/cam/capi|

Please refer to the official documentation for specific service billing rules. New registrations have a certain amount of free quota.

Rodel Agent has selected a few representative tones from the `Large Model Tones` and `Premium Tones`. If the tone you need is not in the list of tones provided by Rodel Agent, please raise an issue.

## Edge TTS

Modified from [edge-tts](https://github.com/rany2/edge-tts), free to use, but may be blocked by some service providers in certain regions.

## Windows Speech

Uses the Windows native speech synthesis functionality called via the [Windows.Media.SpeechSynthesis](https://learn.microsoft.com/uwp/api/windows.media.speechsynthesis?view=winrt-26100) API. Completely free, very fast, but the quality is not up to par.