#include "pch.h"
#include <windows.h>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <iostream>
#include <string>
#include <thread>
#include <psapi.h>

#pragma comment(lib, "ws2_32.lib")
#pragma comment(lib, "psapi.lib")

SOCKET g_socket = INVALID_SOCKET;
bool g_running = true;

std::string GetProcessInfo() {
    DWORD pid = GetCurrentProcessId();
    char processName[MAX_PATH] = "<unknown>";
    HMODULE hMod;
    DWORD cbNeeded;

    if (EnumProcessModules(GetCurrentProcess(), &hMod, sizeof(hMod), &cbNeeded)) {
        GetModuleFileNameExA(GetCurrentProcess(), hMod, processName, sizeof(processName));
    }
    MEMORYSTATUSEX memInfo{};
    memInfo.dwLength = sizeof(memInfo);
    GlobalMemoryStatusEx(&memInfo);
    std::string info = "Process ID: " + std::to_string(pid) +
        "\nProcess Name: " + processName +
        "\nTotal Virtual Memory: " + std::to_string(memInfo.ullTotalVirtual / 1024 / 1024) + " MB" +
        "\nAvailable Virtual Memory: " + std::to_string(memInfo.ullAvailVirtual / 1024 / 1024) + " MB\n";

    return info;
}

bool ConnectToServer(const char* server_ip, int server_port) {
    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) return false;

    g_socket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (g_socket == INVALID_SOCKET) { WSACleanup(); return false; }

    sockaddr_in serverAddr{};
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(server_port);
    inet_pton(AF_INET, server_ip, &serverAddr.sin_addr);
    if (connect(g_socket, (sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
        closesocket(g_socket);
        WSACleanup();
        return false;
    }
    return true;
}

void DisconnectServer() {
    if (g_socket != INVALID_SOCKET) {
        closesocket(g_socket);
        g_socket = INVALID_SOCKET;
    }
    WSACleanup();
}

void ClientThread() {
    AllocConsole();
    FILE* fOut;
    FILE* fIn;
    freopen_s(&fOut, "CONOUT$", "w", stdout);
    freopen_s(&fIn, "CONIN$", "r", stdin);
    std::cout << "Console allocated.\n";
    if (!ConnectToServer("127.0.0.1", 50000)) {
        std::cout << "Failed to connect to server.\n";
        return;
    }
    std::string processInfo = "Process Info:\n" + GetProcessInfo();
    send(g_socket, processInfo.c_str(), static_cast<int>(processInfo.size()), 0);
    std::cout << "Connected to server.\n";
    std::string message;
    while (g_running) {
        std::cout << "Enter message: ";
        std::getline(std::cin, message);
        if (!g_running) break;

        if (message.empty()) {
            std::cout << "Empty message, ignored.\n";
            continue;
        }
        std::string fullMessage = "CliCpp - " + message;
        if (send(g_socket, fullMessage.c_str(), static_cast<int>(fullMessage.size()), 0) == SOCKET_ERROR) {
            std::cout << "Failed to send message.\n";
            break;
        }
    }
    DisconnectServer();
    FreeConsole();
}

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        std::thread(ClientThread).detach();
        break;

    case DLL_PROCESS_DETACH:
        g_running = false;
        DisconnectServer();
        break;
    }
    return TRUE;
}
