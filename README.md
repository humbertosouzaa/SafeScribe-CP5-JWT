# CP5 – Autenticação e Autorização com JWT

**Aluno:** Humberto Souza - rm558482

---

## 📘 Descrição

API desenvolvida em **.NET 8** para demonstrar autenticação e autorização usando **JWT (JSON Web Token)**.  
Inclui controle de acesso por **roles** (Leitor, Editor e Admin), **hash de senha com BCrypt** e **logout com blacklist**.

---

## 🔑 Configuração JWT

Arquivo: `appsettings.json`

```json
"Jwt": {
  "Key": "H7xM9tQ2pA4dL1rW5sC8eV0yZ3nF6bT9uJ2kP7mR4vN1gY5aX8cE0qD3sL6fB9t",
  "Issuer": "SafeScribe",
  "Audience": "SafeScribeClients",
  "ExpiresMinutes": 60
}
