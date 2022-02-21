using System;
using System.Runtime.InteropServices;

namespace FrostyEditor
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DiscordRichPresence
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string state;   /* max 128 bytes */
        [MarshalAs(UnmanagedType.LPStr)]
        public string details; /* max 128 bytes */
        public long startTimestamp;
        public long endTimestamp;
        [MarshalAs(UnmanagedType.LPStr)]
        public string largeImageKey;  /* max 32 bytes */
        [MarshalAs(UnmanagedType.LPStr)]
        public string largeImageText; /* max 128 bytes */
        [MarshalAs(UnmanagedType.LPStr)]
        public string smallImageKey;  /* max 32 bytes */
        [MarshalAs(UnmanagedType.LPStr)]
        public string smallImageText; /* max 128 bytes */
        [MarshalAs(UnmanagedType.LPStr)]
        public string partyId;        /* max 128 bytes */
        public int partySize;
        public int partyMax;
        [MarshalAs(UnmanagedType.LPStr)]
        public string matchSecret;    /* max 128 bytes */
        [MarshalAs(UnmanagedType.LPStr)]
        public string joinSecret;     /* max 128 bytes */
        [MarshalAs(UnmanagedType.LPStr)]
        public string spectateSecret; /* max 128 bytes */
        public byte instance;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DiscordUser
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string userId;
        [MarshalAs(UnmanagedType.LPStr)]
        public string username;
        [MarshalAs(UnmanagedType.LPStr)]
        public string discriminator;
        [MarshalAs(UnmanagedType.LPStr)]
        public string avatar;
    }

    public delegate void ReadyCallback(DiscordUser user);
    public delegate void DisconnectedCallback(int errorCode, string message);
    public delegate void ErroredCallback(int errorCode, string message);
    public delegate void JoinGameCallback(string joinSecret);
    public delegate void SpectateGameCallback(string spectateSecret);
    public delegate void JoinRequestCallback(DiscordUser request);

    [StructLayout(LayoutKind.Sequential)]
    public struct DiscordEventHandlers
    {
        public IntPtr ready;
        public IntPtr disconnected;
        public IntPtr errored;
        public IntPtr joinGame;
        public IntPtr spectateGame;
        public IntPtr joinRequest;
    }

    public class DiscordRPC
    {
        [DllImport("thirdparty/discord-rpc.dll", EntryPoint = "Discord_Initialize")]
        public static extern void Discord_Initialize(
            [MarshalAs(UnmanagedType.LPStr)]
            string applicationId, 
            ref DiscordEventHandlers handlers, 
            int autoRegister,
            [MarshalAs(UnmanagedType.LPStr)]
            string optionalSteamId = "");

        [DllImport("thirdparty/discord-rpc.dll", EntryPoint = "Discord_UpdatePresence")]
        public static extern void Discord_UpdatePresence(ref DiscordRichPresence presence);

        [DllImport("thirdparty/discord-rpc.dll", EntryPoint = "Discord_RunCallbacks")]
        public static extern void Discord_RunCallbacks();

        [DllImport("thirdparty/discord-rpc.dll", EntryPoint = "Discord_Shutdown")]
        public static extern void Discord_Shutdown();
    }
}
