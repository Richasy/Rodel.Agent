/// <reference types="vite/client" />

declare interface Window{
    chrome: {
        webview: {
            addEventListener(type: string, listener: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void;
            removeEventListener(type: string, listener: EventListenerOrEventListenerObject, options?: boolean | EventListenerOptions): void;
            postMessage(message: any) : void;
        }
    }
}

declare enum AuthorRole {
    System,
    User,
    Assistant,
    Tool,
    Client,
}

declare enum EditorTheme {
    Light = 0,
    Dark = 1
}

declare class ChatMessage {
    message: string;
    role: number;
    time: number;
    avatar: string;
}