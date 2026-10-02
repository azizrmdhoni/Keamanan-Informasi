import socket
import struct
import sys
import threading
from des import des_encrypt, des_decrypt, check_mode

DEFAULT_HOST = "127.0.0.1"
DEFAULT_PORT = 5555
DEFAULT_KEY = "KunciRahasiaDES12345"
DEFAULT_MODE = "CBC"

def recv_exact(sock, n):
    data = b""
    while len(data) < n:
        chunk = sock.recv(n - len(data))
        if not chunk:
            return None
        data += chunk
    return data

def send_msg(sock, data):
    sock.sendall(struct.pack("!I", len(data)) + data)

def recv_msg(sock):
    header = recv_exact(sock, 4)
    if header is None:
        return None
    return recv_exact(sock, struct.unpack("!I", header)[0])

def receive_loop(sock, key, mode):
    while True:
        try:
            ciphertext = recv_msg(sock)
        except OSError:
            break
        if ciphertext is None:
            print("\n[*] Terputus. Tekan Enter untuk keluar.")
            break
        try:
            plaintext = des_decrypt(ciphertext, key, mode)
        except ValueError as e:
            plaintext = f"(gagal dekripsi: {e})"
        print(f"\n[TERIMA] Ciphertext: {ciphertext.hex()}")
        print(f"[TERIMA] Plaintext : {plaintext}")
        print("> ", end="", flush=True)

def chat(sock, key, mode):
    threading.Thread(target=receive_loop, args=(sock, key, mode), daemon=True).start()
    print(f"Mode {mode}. Ketik pesan lalu Enter.")
    try:
        while True:
            msg = input("> ").strip()
            if msg.lower() == "exit":
                break
            if not msg:
                continue
            ciphertext = des_encrypt(msg, key, mode)
            print(f"[KIRIM ] Plaintext : {msg}")
            print(f"[KIRIM ] Ciphertext: {ciphertext.hex()}")
            send_msg(sock, ciphertext)
    except (KeyboardInterrupt, EOFError, OSError):
        pass
    finally:
        sock.close()

def run_receiver(port, key, mode):
    srv = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    srv.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    srv.bind(("0.0.0.0", port))
    srv.listen(1)
    print(f"[*] Receiver menunggu di port {port} ...")
    conn, addr = srv.accept()
    print(f"[+] Terhubung dengan {addr[0]}:{addr[1]}")
    srv.close()
    chat(conn, key, mode)

def run_sender(host, port, key, mode):
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.connect((host, port))
    print(f"[+] Terhubung ke {host}:{port}")
    chat(sock, key, mode)

if __name__ == "__main__":
    a = sys.argv[1:]
    mode = DEFAULT_MODE
    for x in a[1:]:
        if x.upper() in ("CBC", "ECB"):
            mode = check_mode(x)
            a.remove(x)
            break

    if a and a[0] == "receiver":
        run_receiver(int(a[1]) if len(a) > 1 else DEFAULT_PORT,
                     a[2] if len(a) > 2 else DEFAULT_KEY,
                     mode)
    elif a and a[0] == "sender":
        run_sender(a[1] if len(a) > 1 else DEFAULT_HOST,
                   int(a[2]) if len(a) > 2 else DEFAULT_PORT,
                   a[3] if len(a) > 3 else DEFAULT_KEY,
                   mode)
    else:
        print(__doc__)