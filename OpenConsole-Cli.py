import socket

def main():
    server_ip = "127.0.0.1"
    server_port = 5000

    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        try:
            s.connect((server_ip, server_port))
            print(f"Connected to server {server_ip}:{server_port}")
            while True:
                message = input("Enter message: ")
                if not message.strip():
                    print("Empty message, ignored.")
                    continue
                full_message = f"CliPython - {message}"

                try:
                    s.sendall(full_message.encode("utf-8"))
                except Exception as e:
                    print(f"Failed to send message: {e}")
                    break
        except ConnectionRefusedError:
            print("Could not connect to server.")
        except KeyboardInterrupt:
            print("\nDisconnected from server.")

if __name__ == "__main__":
    main()
