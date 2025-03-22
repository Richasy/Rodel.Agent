import React, { useState, useEffect } from "react";
import { Bubble } from "@ant-design/x";
import { Button, Space, Input, Flex, Tooltip, Avatar, Collapse } from "antd";
import {
  CopyOutlined,
  EditOutlined,
  DeleteOutlined,
  CloseOutlined,
  CheckOutlined,
} from "@ant-design/icons";

const MessageItem = ({ item, sendMessage, renderMarkdown, removeMessage }) => {
  const [isEditing, setIsEditing] = useState(false);
  const [editedContent, setEditedContent] = useState(item.message);
  const [isHovered, setIsHovered] = useState(false); // 鼠标悬停状态
  const { TextArea } = Input;

  // 当 item 更新时，重置编辑状态
  useEffect(() => {
    setEditedContent(item.message);
    setIsEditing(false);
  }, [item]);

  const handleEdit = () => {
    setIsEditing(true);
  };

  const handleSave = () => {
    setIsEditing(false);
    item.message = editedContent;
    sendMessage(
      {
        type: "edit",
        content: JSON.stringify({ message: editedContent, id: item.id }),
      },
      false,
      false
    );
  };

  const timestampToDate = (timestamp) => {
    let timestampInMilliseconds = timestamp * 1000;
    let date = new Date(timestampInMilliseconds);
    return date.toLocaleString();
  };

  const handleCancel = () => {
    setIsEditing(false);
    setEditedContent(item.message); // 恢复初始内容
  };

  const handleKeyUp = (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSave();
    }
  };

  if (item.role === "tool") {
    let toolData = item.tool_data;
    let isJson = false;
    try {
      toolData = JSON.parse(item.tool_data);
      if (
        toolData.content &&
        toolData.content.length > 0 &&
        toolData.content[0].text
      ) {
        toolData = JSON.parse(toolData.content[0].text);
      }
      isJson = true;
    } catch {
      // do nothing
    }
    return (
      <Collapse
        style={{ margin: "12px" }}
        items={[
          {
            key: item.id,
            label: `${item.tool_client_id}: ${item.tool_method}`,
            children: isJson
              ? renderMarkdown(
                  "<!>```json\n" + JSON.stringify(toolData, null, 2) + "\n```"
                )
              : JSON.stringify(toolData, null, 2),
          },
        ]}
      />
    );
  }

  return (
    <div>
      {isEditing ? (
        <div style={{ position: "relative" }}>
          <TextArea
            autoFocus
            style={{ marginTop: "16px" }}
            value={editedContent}
            autoSize={{ minRows: 4, maxRows: 18 }}
            onChange={(e) => setEditedContent(e.target.value)}
            onKeyUp={handleKeyUp}
            onBlur={handleSave}
          />
          {/* 按钮容器 */}
          <Flex
            gap="small"
            style={{
              marginTop: "8px",
            }}
          >
            <Button
              type="primary"
              onClick={handleSave}
              icon={<CheckOutlined />}
            >
              {window.resources?.save ? window.resources.save : "Save"}
            </Button>
            <Button onClick={handleCancel} icon={<CloseOutlined />}>
              {window.resources?.discard ? window.resources.discard : "Discard"}
            </Button>
          </Flex>
        </div>
      ) : (
        <Bubble
          key={item.id}
          shape="corner"
          onMouseEnter={() => setIsHovered(true)}
          onMouseLeave={() => setIsHovered(false)}
          content={item.message}
          avatar={
            item.emoji && item.showLogo ? (
              <Avatar
                size="large"
                gap={0}
                style={{
                  backgroundColor: "transparent",
                  fontFamily: "Segoe UI Emoji",
                  fontSize: "48px",
                  marginTop: "0.4em",
                }}
              >
                {item.emoji}
              </Avatar>
            ) : item.avatar && item.showLogo ? (
              <Avatar
                size={36}
                gap={0}
                src={<img src={item.avatar} alt={item.author} />}
                style={{ marginTop: "1.2em" }}
              />
            ) : null
          }
          header={
            <div
              className={
                (isHovered ? "c-visible" : "c-hidden") + " message-time"
              }
            >
              {timestampToDate(item.time)}
            </div>
          }
          className={
            item.role == "user" ? "user-bubble markdown-body" : "markdown-body"
          }
          placement={item.role == "user" ? "end" : "start"}
          messageRender={renderMarkdown}
          footer={
            <Space size={8} className={isHovered ? "c-visible" : "c-hidden"}>
              <Tooltip
                title={window.resources?.copy ? window.resources.copy : "Copy"}
              >
                <Button
                  color="default"
                  variant="text"
                  size="small"
                  onClick={() =>
                    sendMessage(
                      { type: "copy", content: item.message },
                      false,
                      false
                    )
                  }
                  icon={<CopyOutlined />}
                />
              </Tooltip>
              <Tooltip
                title={window.resources?.edit ? window.resources.edit : "Edit"}
              >
                <Button
                  color="default"
                  variant="text"
                  size="small"
                  onClick={handleEdit}
                  icon={<EditOutlined />}
                />
              </Tooltip>
              <Tooltip
                title={
                  window.resources?.delete ? window.resources.delete : "Delete"
                }
              >
                <Button
                  color="danger"
                  variant="text"
                  size="small"
                  onClick={() => {
                    removeMessage(item.id);
                    sendMessage(
                      { type: "delete", content: item.id },
                      false,
                      false
                    );
                  }}
                  icon={<DeleteOutlined />}
                />
              </Tooltip>
            </Space>
          }
        />
      )}
    </div>
  );
};

export default MessageItem;
