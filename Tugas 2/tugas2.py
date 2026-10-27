import threading
import socket
import random

SBOX = [
    0x63,0x7c,0x77,0x7b,0xf2,0x6b,0x6f,0xc5,0x30,0x01,0x67,0x2b,0xfe,0xd7,0xab,0x76,
    0xca,0x82,0xc9,0x7d,0xfa,0x59,0x47,0xf0,0xad,0xd4,0xa2,0xaf,0x9c,0xa4,0x72,0xc0,
    0xb7,0xfd,0x93,0x26,0x36,0x3f,0xf7,0xcc,0x34,0xa5,0xe5,0xf1,0x71,0xd8,0x31,0x15,
    0x04,0xc7,0x23,0xc3,0x18,0x96,0x05,0x9a,0x07,0x12,0x80,0xe2,0xeb,0x27,0xb2,0x75,
    0x09,0x83,0x2c,0x1a,0x1b,0x6e,0x5a,0xa0,0x52,0x3b,0xd6,0xb3,0x29,0xe3,0x2f,0x84,
    0x53,0xd1,0x00,0xed,0x20,0xfc,0xb1,0x5b,0x6a,0xcb,0xbe,0x39,0x4a,0x4c,0x58,0xcf,
    0xd0,0xef,0xaa,0xfb,0x43,0x4d,0x33,0x85,0x45,0xf9,0x02,0x7f,0x50,0x3c,0x9f,0xa8,
    0x51,0xa3,0x40,0x8f,0x92,0x9d,0x38,0xf5,0xbc,0xb6,0xda,0x21,0x10,0xff,0xf3,0xd2,
    0xcd,0x0c,0x13,0xec,0x5f,0x97,0x44,0x17,0xc4,0xa7,0x7e,0x3d,0x64,0x5d,0x19,0x73,
    0x60,0x81,0x4f,0xdc,0x22,0x2a,0x90,0x88,0x46,0xee,0xb8,0x14,0xde,0x5e,0x0b,0xdb,
    0xe0,0x32,0x3a,0x0a,0x49,0x06,0x24,0x5c,0xc2,0xd3,0xac,0x62,0x91,0x95,0xe4,0x79,
    0xe7,0xc8,0x37,0x6d,0x8d,0xd5,0x4e,0xa9,0x6c,0x56,0xf4,0xea,0x65,0x7a,0xae,0x08,
    0xba,0x78,0x25,0x2e,0x1c,0xa6,0xb4,0xc6,0xe8,0xdd,0x74,0x1f,0x4b,0xbd,0x8b,0x8a,
    0x70,0x3e,0xb5,0x66,0x48,0x03,0xf6,0x0e,0x61,0x35,0x57,0xb9,0x86,0xc1,0x1d,0x9e,
    0xe1,0xf8,0x98,0x11,0x69,0xd9,0x8e,0x94,0x9b,0x1e,0x87,0xe9,0xce,0x55,0x28,0xdf,
    0x8c,0xa1,0x89,0x0d,0xbf,0xe6,0x42,0x68,0x41,0x99,0x2d,0x0f,0xb0,0x54,0xbb,0x16,
]
 
# inverse S-box dibuat dari S-box (untuk dekrip)
INV_SBOX = [0] * 256
for i in range(256):
    INV_SBOX[SBOX[i]] = i
 
RCON = [0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36]
 
# perkalian di GF(2^8)
def kali(a, b):
    hasil = 0
    for i in range(8):
        if b & 1 == 1:
            hasil = hasil ^ a
        a = a << 1
        if a > 255:
            a = a ^ 0x11b
        b = b >> 1
    return hasil

# ubah 16 byte jadi matriks 4x4
def blok_ke_matrix(blok):
    state = [[0] * 4 for i in range(4)]
    for r in range(4):
        for c in range(4):
            state[r][c] = blok[r + 4 * c]
    return state
 
 
# ubah matriks 4x4 jadi 16 byte lagi
def matrix_ke_blok(state):
    blok = []
    for c in range(4):
        for r in range(4):
            blok.append(state[r][c])
    return blok

# 1. Byte Substitution
def sub_bytes(state):
    for r in range(4):
        for c in range(4):
            state[r][c] = SBOX[state[r][c]]
  
def inv_sub_bytes(state):
    for r in range(4):
        for c in range(4):
            state[r][c] = INV_SBOX[state[r][c]]
 
# 2. Shift Rows (baris ke-r geser kiri r kali)
def shift_rows(state):
    for r in range(1, 4):
        state[r] = state[r][r:] + state[r][:r]
 
def inv_shift_rows(state):
    for r in range(1, 4):
        state[r] = state[r][-r:] + state[r][:-r]
 
# 3. Mix Columns
def mix_columns(state):
    for c in range(4):
        a0 = state[0][c]
        a1 = state[1][c]
        a2 = state[2][c]
        a3 = state[3][c]
        state[0][c] = kali(2, a0) ^ kali(3, a1) ^ a2 ^ a3
        state[1][c] = a0 ^ kali(2, a1) ^ kali(3, a2) ^ a3
        state[2][c] = a0 ^ a1 ^ kali(2, a2) ^ kali(3, a3)
        state[3][c] = kali(3, a0) ^ a1 ^ a2 ^ kali(2, a3)
 
def inv_mix_columns(state):
    for c in range(4):
        a0 = state[0][c]
        a1 = state[1][c]
        a2 = state[2][c]
        a3 = state[3][c]
        state[0][c] = kali(14, a0) ^ kali(11, a1) ^ kali(13, a2) ^ kali(9, a3)
        state[1][c] = kali(9, a0) ^ kali(14, a1) ^ kali(11, a2) ^ kali(13, a3)
        state[2][c] = kali(13, a0) ^ kali(9, a1) ^ kali(14, a2) ^ kali(11, a3)
        state[3][c] = kali(11, a0) ^ kali(13, a1) ^ kali(9, a2) ^ kali(14, a3)
 
# 4. Add Round Key (XOR state dengan round key)
def add_round_key(state, round_key):
    for r in range(4):
        for c in range(4):
            state[r][c] = state[r][c] ^ round_key[r][c]

# Key Expansion: 16 byte key jadi 11 round key
def key_expansion(key):
    w = []
    for i in range(4):
        w.append([key[4 * i], key[4 * i + 1], key[4 * i + 2], key[4 * i + 3]])
 
    for i in range(4, 44):
        temp = w[i - 1][:]
        if i % 4 == 0:
            temp = [temp[1], temp[2], temp[3], temp[0]]   # rotate
            for j in range(4):
                temp[j] = SBOX[temp[j]]                   # S-box
            temp[0] = temp[0] ^ RCON[i // 4 - 1]          # rcon
        baru = []
        for j in range(4):
            baru.append(w[i - 4][j] ^ temp[j])
        w.append(baru)
      
    round_keys = []
    for ronde in range(11):
        rk = [[0] * 4 for i in range(4)]
        for c in range(4):
            for r in range(4):
                rk[r][c] = w[4 * ronde + c][r]
        round_keys.append(rk)
    return round_keys
 
# enkripsi 1 blok (16 byte)
def enkripsi_blok(blok, round_keys):
    state = blok_ke_matrix(blok)
    add_round_key(state, round_keys[0])
    for ronde in range(1, 10):
        sub_bytes(state)
        shift_rows(state)
        mix_columns(state)
        add_round_key(state, round_keys[ronde])
    sub_bytes(state)
    shift_rows(state)
    add_round_key(state, round_keys[10])
    return matrix_ke_blok(state)
 
# dekripsi 1 blok (16 byte)
def dekripsi_blok(blok, round_keys):
    state = blok_ke_matrix(blok)
    add_round_key(state, round_keys[10])
    for ronde in range(9, 0, -1):
        inv_shift_rows(state)
        inv_sub_bytes(state)
        add_round_key(state, round_keys[ronde])
        inv_mix_columns(state)
    inv_shift_rows(state)
    inv_sub_bytes(state)
    add_round_key(state, round_keys[0])
    return matrix_ke_blok(state)
 
# key teks dibuat 16 karakter (jika kurang akan ditambah '0', sedangkan lebih akan dipotong)
def siapkan_key(key_teks):
    while len(key_teks) < 16:
        key_teks = key_teks + "0"
    key_teks = key_teks[:16]
    key = []
    for ch in key_teks:
        key.append(ord(ch))
    return key
 
# plaintext (ASCII) menajadi ciphertext (HEX)
def aes_encrypt(plaintext, key_teks):
    round_keys = key_expansion(siapkan_key(key_teks))
    data = []
    for ch in plaintext:
        data.append(ord(ch))
    while len(data) % 16 != 0:      # padding 0 sampai kelipatan 16
        data.append(0)
      
    hasil = []
    for i in range(0, len(data), 16):
        hasil = hasil + enkripsi_blok(data[i:i + 16], round_keys)
 
    hex_teks = ""
    for b in hasil:
        hex_teks = hex_teks + "%02x" % b
    return hex_teks
 
# ciphertext (HEX) menajadi plaintext (ASCII)
def aes_decrypt(hex_teks, key_teks):
    round_keys = key_expansion(siapkan_key(key_teks))
    data = []
    for i in range(0, len(hex_teks), 2):
        data.append(int(hex_teks[i:i + 2], 16))
 
    hasil = []
    for i in range(0, len(data), 16):
        hasil = hasil + dekripsi_blok(data[i:i + 16], round_keys)
 
    while len(hasil) > 0 and hasil[-1] == 0:    # buang padding 0
        hasil.pop()

    teks = ""
    for b in hasil:
        teks = teks + chr(b)
    return teks

tutup = False

def terima_pesan(f, ks, nama_lawan):
    global tutup
    for baris in f:
        cipher = baris.strip()
        try:
            plain = aes_decrypt(cipher, ks)
        except:
            plain = "(gagal dekrip)"
        print("\n[" + nama_lawan + "] ciphertext (hex):", cipher)
        print("[" + nama_lawan + "] plaintext        :", plain)
    tutup = True
    print("\nKoneksi ditutup, tekan Enter")

def mulai_chat(sock, f, ks, nama_lawan):
    global tutup
    tutup = False
    t = threading.Thread(target=terima_pesan, args=(f, ks, nama_lawan), daemon=True)
    t.start()

    print("Chat dimulai ketik pesan atau /exit untuk ganti key")
    while True:
        pesan = input()
        if pesan == "/exit" or tutup == True:
            break
        if pesan != "":
            cipher = aes_encrypt(pesan, ks)
            print("ciphertext (hex):", cipher)
            sock.sendall((cipher + "\n").encode())
    sock.close()
    
def jalankan_kdc():
  server = socket.socket()
  server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
  server.bind(("0.0.0.0", 5000))
  server.listen()
  server.settimeout(1)

  huruf = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

  while True:
    ka = input("\nMaster key Alice (Ka): ")
    kb = input("Master key Bob (Kb): ")
    print("KDC menunggu Alice...")
    while True:
      try:
        conn, addr = server.accept()
        break

      except socket.timeout:
        pass
    conn.settimeout(None)

    # langkah 1: terima IDA || IDB || N1
    data = conn.makefile().readline().strip()
    bagian = data.split("|")
    ida = bagian[0]
    idb = bagian[1]
    n1 = bagian[2]
    print("(1) dari", addr[0], ":", data)

    # buat session key Ks (16 karakter acak)
    ks = ""
    for i in range(16):
        ks = ks + random.choice(huruf)

    # langkah 2: kirim E(Ka,[Ks||IDA||IDB||N1]) || E(Kb,[Ks||IDA])
    paket_a = aes_encrypt(ks + "|" + ida + "|" + idb + "|" + n1, ka)
    paket_b = aes_encrypt(ks + "|" + ida, kb)
    print("(2) Ks =", ks)
    print("    E(Ka) =", paket_a)
    print("    E(Kb) =", paket_b)
    conn.sendall((paket_a + "|" + paket_b + "\n").encode())
    conn.close()

print("1. KDC")
pilihan = input("Jalankan sebagai (1/2/3): ")

if pilihan == "1":
    jalankan_kdc()
