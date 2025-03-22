import React, { useState, useEffect } from "react";
import { Bubble } from "@ant-design/x";
import { Space, Spin, Flex, theme, Typography, Collapse, Button } from "antd";
import { ThemeProvider } from "antd-style";
import markdownit from "markdown-it";
import highlight from "highlight.js";
import katex from "markdown-it-katex";
import "katex/dist/katex.min.css";
import MessageItem from "./MessageItem";

function Render() {
  const { token } = theme.useToken();
  const [currentTheme, setCurrentTheme] = useState("light"); // 默认主题为 auto
  const [history, setHistory] = useState([]);
  const [temporaryLoading, setTemporaryLoading] = useState(false); // 临时加载状态
  const [temporaryOutput, setTemporaryOutput] = useState(null); // 存储临时输出
  let codeIdx = 0;
  const md = markdownit({
    html: true,
    breaks: true,
    highlight: (str, lang) => {
      if (lang && highlight.getLanguage(lang)) {
        try {
          return highlight.highlight(str, { language: lang }).value;
        } catch (__) {}
      }
      return ""; // use external default escaping
    },
  }).use(katex);

  const extractLang = (info) => {
    return info
      .trim()
      .replace(/=(\d*)/, "")
      .replace(/:(no-)?line-numbers({| |$|=\d*).*/, "")
      .replace(/(-vue|{| ).*$/, "")
      .replace(/^vue-html$/, "template")
      .replace(/^ansi$/, "");
  };

  const handleCopyClick = (id) => {
    const button = document.getElementById(id);
    const codeContainer = button.closest(".code-container");
    const codeElement = codeContainer?.querySelector("pre code");
    if (codeElement) {
      sendMessage(
        {
          type: "copyText",
          content: codeElement.innerText,
        },
        false,
        false
      );
    }
  };

  const renderMarkdown = (content) => {
    // 如果content有<think>，但是没有</think>，则在结尾添加一个 </think>并追加一个p标签
    if (content.includes("<think>") && !content.includes("</think>")) {
      const thinking = window.resources?.thinking
        ? window.resources.thinking
        : "Thinking...";
      content += `</think><p>${thinking}</p>`;
    }

    // 判断文本是否以 <!> 开头.
    const shouldShowHeader = !content.startsWith("<!>");
    if(!shouldShowHeader){
      content = content.replace("<!>", "");
    }

    content = content.replace("<think>", '<div class="think">');
    content = content.replace("</think>", "</div>");
    md.renderer.rules.fence = (...args) => {
      const [tokens, idx] = args;
      const token = tokens[idx];

      // remove title from info
      token.info = token.info.replace(/\[.*\]/, "");
      const lang = extractLang(token.info);

      // Generate a unique ID for the code block
      const codeId = `code-block-${codeIdx++}`;
      const highlightedCode = md.options.highlight(
        token.content,
        lang || "plaintext",
        ""
      );
      // Create the header with language and copy button
      const header = `
        <div class="code-header">
          <span class="code-lang">${lang || "plaintext"}</span>
          <button onclick="window.handleCopyClick('copy${codeIdx}')" class="copy-button" id="copy${codeIdx}">
            <svg class="svgIcon" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor" viewBox="0 0 24 24">
              <path fill-rule="evenodd" d="M18 3a2 2 0 0 1 2 2v10a2 2 0 0 1-2 2h-1V9a4 4 0 0 0-4-4h-3a1.99 1.99 0 0 0-1 .267V5a2 2 0 0 1 2-2h7Z" clip-rule="evenodd"/>
              <path fill-rule="evenodd" d="M8 7.054V11H4.2a2 2 0 0 1 .281-.432l2.46-2.87A2 2 0 0 1 8 7.054ZM10 7v4a2 2 0 0 1-2 2H4v6a2 2 0 0 0 2 2h7a2 2 0 0 0 2-2V9a2 2 0 0 0-2-2h-3Z" clip-rule="evenodd"/>
            </svg>
          </button>
        </div>
      `;

      // Wrap the code block in a container with the header
      const codeBlock = shouldShowHeader
        ? `
        <div class="code-container">
          ${header}
          <pre><code id="${codeId}" class="hljs ${lang}">${highlightedCode}</code></pre>
        </div>
      `
        : `
        <div class="code-container">
          <pre><code id="${codeId}" class="hljs ${lang}">${highlightedCode}</code></pre>
        </div>
      `;

      return codeBlock;
    };

    // 使用 markdown-it 渲染 Markdown 内容
    const htmlContent = md.render(content);
    // 创建一个 DOMParser 实例
    const parser = new DOMParser();
    const doc = parser.parseFromString(htmlContent, "text/html");

    // 查找所有的 <think> 标签
    const thinkElements = doc.querySelectorAll(".think");

    // 如果没有 <think> 标签，直接返回原始 HTML
    if (thinkElements.length === 0) {
      return (
        <Typography>
          {/* biome-ignore lint/security/noDangerouslySetInnerHtml: used in demo */}
          <div
            dangerouslySetInnerHTML={{
              __html: htmlContent,
            }}
          />
        </Typography>
      );
    }

    const thinkHtmlList = Array.from(thinkElements).map((thinkElement) => {
      thinkElement.style = "display:none";
      return thinkElement.innerHTML;
    });

    const thinkingHeader = window.resources?.thoughtProcess
      ? window.resources.thoughtProcess
      : "Thought process";
    return (
      <Typography>
        {/* biome-ignore lint/security/noDangerouslySetInnerHtml: used in demo */}
        {Array.from(thinkHtmlList).map((thinkHtml, index) => {
          const inner = (
            <div
              dangerouslySetInnerHTML={{
                __html: thinkHtml,
              }}
            />
          );

          const items = [
            {
              key: index,
              label: thinkingHeader,
              children: inner,
            },
          ];
          return <Collapse key={index} size="small" items={items} />;
        })}
        {/* biome-ignore lint/security/noDangerouslySetInnerHtml: used in demo */}
        <div
          dangerouslySetInnerHTML={{
            __html: doc.body.innerHTML,
          }}
        />
      </Typography>
    );
  };

  const originalConsoleError = console.error;
  console.error = function (msg, ...optionalArguments) {
    originalConsoleError.apply(console, arguments);
    sendMessage(
      "TEMP:" + JSON.stringify({ msg, optionalArguments }),
      false,
      true
    );
  };

  window.onerror = function (msg, url, lineNo, columnNo, error) {
    sendMessage(
      "TEMP:" + JSON.stringify({ msg, url, lineNo, columnNo, error }),
      false,
      true
    );
  };

  const loadThemeCSS = (themeName) => {
    // 移除现有的主题 CSS 文件
    const existingLink1 = document.getElementById("theme-style");
    const existingLink2 = document.getElementById("code-style");
    if (existingLink1) {
      document.head.removeChild(existingLink1);
    }
    if (existingLink2) {
      document.head.removeChild(existingLink2);
    }

    // 创建新的 <link> 标签
    const link1 = document.createElement("link");
    link1.id = "theme-style";
    link1.rel = "stylesheet";
    link1.href =
      themeName === "dark" ? "./github-dark.css" : "./github-light.css";
    document.head.appendChild(link1);

    const link2 = document.createElement("link");
    link2.id = "code-style";
    link2.rel = "stylesheet";
    link2.href =
      themeName === "dark" ? "./atom-one-dark.css" : "./atom-one-light.css";
    document.head.appendChild(link2);
  };

  const sendMessage = (data, isMsg = false, isErr = false) => {
    var prefix = isErr ? "error:" : isMsg ? "msg:" : "data:";
    var content = isMsg ? data : JSON.stringify(data);
    var msg = prefix + content;
    if (window.chrome.webview) {
      window.chrome.webview.postMessage(msg);
    } else {
      console.log(msg);
    }
  };

  const removeMessage = (id) => {
    setHistory((prevHistory) =>
      prevHistory.filter((item, idx) => item.id != id)
    );
  };

  // 在组件挂载时，将 setHistory 方法挂载到 window 对象上
  useEffect(() => {
    window.delayToBottom = () => {
      // 延迟 500ms 滚动到底部
      setTimeout(() => {
        window.scrollTo({
          top: document.body.scrollHeight,
          behavior: "smooth",
        });
      }, 500);
    };
    window.handleCopyClick = handleCopyClick;
    window.changeTheme = (themeId) => {
      setCurrentTheme(themeId);
      loadThemeCSS(themeId); // 加载对应的 CSS 文件
    };
    window.setHistory = (newHistory) => {
      setHistory(newHistory);
      window.delayToBottom();
    };
    window.addMessage = (message) => {
      setHistory((prevHistory) => [...prevHistory, message]);
      if (message.role != "tool") {
        setTemporaryOutput(null); // 清除临时输出
        setTemporaryLoading(false); // 清除临时加载状态
      }
      window.delayToBottom();
    };
    window.deleteMessage = (id) => {
      setHistory((prevHistory) =>
        prevHistory.filter((item, idx) => item.id != id)
      );
    };
    // 设置临时输出（用于 SSE 流式返回）
    window.setOutput = (output) => {
      setTemporaryLoading(false);
      setTemporaryOutput(output);
    };
    // 设置临时加载状态
    window.setLoading = (loading) => {
      setTemporaryLoading(loading);
    };
    // 取消临时输出
    window.setCancel = () => {
      setTemporaryLoading(false);
      setTemporaryOutput(null);
    };

    window.setResources = (resources) => {
      window.resources = resources;
    };
    // 清空历史消息
    window.clearMessages = () => {
      setHistory([]);
    };

    sendMessage("loaded", true);
  }, []);

  useEffect(() => {
    loadThemeCSS(currentTheme);
  }, [currentTheme]);

  return (
    <ThemeProvider appearance={currentTheme}>
      <Flex vertical gap={0} style={{ marginBottom: "12px" }}>
        {history.map((item) => (
          <MessageItem
            key={item.id}
            item={item}
            sendMessage={sendMessage}
            renderMarkdown={renderMarkdown}
            removeMessage={removeMessage}
          />
        ))}

        {/* 渲染临时输出 */}
        {temporaryOutput && (
          <Bubble
            key="temporaryOutput"
            shape="corner"
            content={temporaryOutput.message}
            header={temporaryOutput.author}
            typing={{
              step: 2,
              interval: 10,
            }}
            className="markdown-body"
            placement="start"
            messageRender={renderMarkdown}
            footer={
              <Space size={token.paddingXXS}>
                <Spin size="small" />
              </Space>
            }
          />
        )}

        {/* 渲染临时加载状态 */}
        {temporaryLoading && (
          <Bubble
            key="temporaryLoading"
            shape="corner"
            loading={true}
            className="markdown-body"
            placement="start"
          />
        )}
      </Flex>
    </ThemeProvider>
  );
}

export default Render;
