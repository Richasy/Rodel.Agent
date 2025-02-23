import React, { useState, useEffect } from "react";
import { Bubble } from "@ant-design/x";
import { Space, Spin, Flex, theme, Typography } from "antd";
import { ThemeProvider } from "antd-style";
import markdownit from "markdown-it";
import highlight from "highlight.js";
import EditableBubble from "./MessageItem";

function Render() {
  const { token } = theme.useToken();
  const [currentTheme, setCurrentTheme] = useState("light"); // 默认主题为 auto
  const [history, setHistory] = useState([]);
  const [temporaryOutput, setTemporaryOutput] = useState(null); // 存储临时输出
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
  });
  const renderMarkdown = (content) => (
    <Typography>
      {/* biome-ignore lint/security/noDangerouslySetInnerHtml: used in demo */}
      <div
        dangerouslySetInnerHTML={{
          __html: md.render(content),
        }}
      />
    </Typography>
  );

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
    window.changeTheme = (themeId) => {
      setCurrentTheme(themeId);
      loadThemeCSS(themeId); // 加载对应的 CSS 文件
    };
    window.setHistory = (newHistory) => {
      setHistory(newHistory);
    };
    window.addMessage = (message) => {
      setHistory((prevHistory) => [...prevHistory, message]);
      setTemporaryOutput(null); // 清除临时输出
      // 延迟 500ms 滚动到底部
      setTimeout(() => {
        window.scrollTo({
          top: document.body.scrollHeight,
          behavior: 'smooth'
        });
      }, 500);
    };
    // 设置临时输出（用于 SSE 流式返回）
    window.setOutput = (output) => {
      setTemporaryOutput(output);
    };
    // 取消临时输出
    window.setCancel = () => {
      setTemporaryOutput(null);
    };

    window.setResources = (resources) => {
      window.resources = resources;
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
          <EditableBubble
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
      </Flex>
    </ThemeProvider>
  );
}

export default Render;
