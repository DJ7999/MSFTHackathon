import React, { useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";

const colorPalette = [
    "#D32F2F", // Red
    "#1976D2", // Blue
    "#388E3C", // Green
    "#F57C00", // Orange
    "#7B1FA2", // Purple
    "#00796B", // Teal
    "#C2185B", // Deep Pink
    "#303F9F", // Indigo
    "#455A64", // Blue Grey
    "#5D4037", // Brown
    "#0097A7", // Cyan
    "#AFB42B", // Lime
    "#FBC02D", // Yellow (bold enough)
    "#E64A19", // Deep Orange
    "#512DA8", // Deep Purple
    "#0288D1", // Light Blue
    "#388E3C", // Forest Green
    "#C62828", // Crimson
    "#6A1B9A", // Dark Violet
    "#1976D2"  // Royal Blue
];


function ChatApp() {
    const [connection, setConnection] = useState(null);
    const [messages, setMessages] = useState([]);
    const [user, setUser] = useState("User" + Math.floor(Math.random() * 100));
    const [newMessage, setNewMessage] = useState("");
    const [userColors, setUserColors] = useState({});
    const bottomRef = useRef(null);

    const userColorMapRef = useRef({});

const getUserColor = (username) => {
    if (!userColorMapRef.current[username]) {
        const existingColors = Object.values(userColorMapRef.current);
        const availableColors = colorPalette.filter(c => !existingColors.includes(c));
        const assignedColor = availableColors.length > 0 ? availableColors[0] : getRandomColor();
        userColorMapRef.current[username] = assignedColor;
    }
    return userColorMapRef.current[username];
};

    const getRandomColor = () => {
        return "#" + Math.floor(Math.random() * 16777215).toString(16);
    };

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5053/chat")
            .withAutomaticReconnect()
            .build();

        setConnection(newConnection);
    }, []);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => {
                    console.log("Connected!");
                    connection.on("ReceiveMessage", (user, message) => {
                        console.log("📥 Received from hub:", user, message);
                        getUserColor(user);
                        setMessages(prev => [...prev, { user, message }]);
                         // Ensure color is assigned
                    });
                })
                .catch(e => console.log("Connection failed: ", e));
        }
    }, [connection]);

    useEffect(() => {
        // Auto-scroll to the bottom when messages change
        if (bottomRef.current) {
            bottomRef.current.scrollIntoView({ behavior: "smooth" });
        }
    }, [messages]);

    const sendMessage = async () => {
        if (connection && connection.state === "Connected") {
            if (newMessage.trim() === "") return;
            try {
                setMessages(prev => [...prev, { user: "input", message: newMessage }]);
                await connection.invoke("SendMessage", { User: user, Message: newMessage });
                console.log("✅ Message sent successfully");
                setNewMessage("");
            } catch (err) {
                console.error("❌ Error while sending:", err);
            }
        } else {
            console.warn("⚠️ SignalR connection not established");
        }
    };

    return (
        <div style={{
            fontFamily: "sans-serif",
            padding: "20px",
            height: "100vh",
            width: "100vw",
            boxSizing: "border-box",
            display: "flex",
            flexDirection: "column"
        }}>
            <h2 style={{ margin: 0 }}>
                Chat as <span style={{ color: getUserColor(user) }}>{user}</span>
            </h2>

            <div style={{
                flexGrow: 1,
                margin: "20px 0",
                overflowY: "auto",
                border: "1px solid #ccc",
                borderRadius: "8px",
                padding: "10px",
                background: "#f9f9f9"
            }}>
                {messages.map((msg, index) => (
                    <div key={index} style={{ color: getUserColor(msg.user), marginBottom: "10px" }}>
                        <strong>{msg.user}</strong>:
                        <ReactMarkdown remarkPlugins={[remarkGfm]}>
                {msg.message}
            </ReactMarkdown>
                    </div>
                ))}
                <div ref={bottomRef} />
            </div>

            <div style={{ display: "flex", gap: "10px" }}>
                <input
                    value={newMessage}
                    onChange={(e) => setNewMessage(e.target.value)}
                    onKeyDown={(e) => {
                        if (e.key === 'Enter') {
                            sendMessage();
                        }
                    }}
                    placeholder="Type message..."
                    style={{
                        flexGrow: 1,
                        padding: "10px",
                        fontSize: "16px",
                        borderRadius: "5px",
                        border: "1px solid #ccc"
                    }}
                />
                <button
                    onClick={sendMessage}
                    style={{
                        padding: "10px 20px",
                        fontSize: "16px",
                        borderRadius: "5px",
                        cursor: "pointer",
                        background: "#4CAF50",
                        color: "white",
                        border: "none"
                    }}
                >
                    Send
                </button>
            </div>
        </div>
    );
}

export default ChatApp;
