# Plano Executado — Resolução de Erro de Espelho APT, Instalação do Git LFS e Checkout dos Assets

- **Autor (GitHub):** @lucamontini (Luca Dalla Lana)
- **Data e Hora:** 2026-10-27 14:26 (BRT)
- **Status:** Concluído com Sucesso
- **Ambiente/Plataforma:** Linux Mint 22.3 (Zena / Ubuntu Noble), Unity 6000.6.0f1 target, Git LFS 3.4.1, Graphify 0.9.96

---

## 1. Solicitação do Usuário

> Usuário reportou falha ao instalar o Git LFS via APT:
> ```
> Erro:1 https://mirror.uepg.br/ubuntu noble-updates/universe amd64 git-lfs amd64 3.4.1-1ubuntu0.4
>   403  Forbidden [IP: 143.208.160.8 443]
> E: Falhou ao buscar https://mirror.uepg.br/ubuntu/pool/universe/g/git-lfs/git-lfs_3.4.1-1ubuntu0.4_amd64.deb  403  Forbidden [IP: 143.208.160.8 443]
> E: Impossível buscar alguns arquivos, talvez executar apt-get update ou tentar com --fix-missing?
> git: 'lfs' não é um comando git.
> ```

---

## 2. Diagnóstico e Contexto Consultado

1. **Causa Raiz do Erro:**
   - O espelho local configurado em `/etc/apt/sources.list.d/` (`mirror.uepg.br`) retornou HTTP `403 Forbidden` ao tentar baixar o pacote deb do `git-lfs`.
   - O repositório oficial da Canonical (`archive.ubuntu.com`) estava online e respondendo HTTP `200 OK` para o mesmo pacote `git-lfs_3.4.1-1ubuntu0.4_amd64.deb`.
2. **Contexto Arquitetural e Governança:**
   - Memória persistente: [`docs/ai-memory/INDEX.md`](../ai-memory/INDEX.md) e [`docs/ai-memory/CURRENT_STATE.md`](../ai-memory/CURRENT_STATE.md).
   - Cofre Obsidian: [`docs/architecture-vault/manual/Visao_Geral_Arquitetura.md`](../architecture-vault/manual/Visao_Geral_Arquitetura.md).
   - Graphify CLI: `graphify query "git lfs"` executado.

---

## 3. Ações Realizadas

1. **Instalação do Git LFS sem Depender do Espelho Quebrado:**
   - Download direto do pacote oficial `git-lfs_3.4.1-1ubuntu0.4_amd64.deb` a partir de `archive.ubuntu.com`.
   - Extração do binário `git-lfs` (v3.4.1) via `dpkg-deb -x`.
   - Instalação no diretório de usuário `~/.local/bin/git-lfs` (já incluído no `$PATH` do usuário).
   - Configuração de permissão de execução (`chmod +x`).
2. **Inicialização e Download dos Assets:**
   - Execução de `git lfs install` (atualizou Git hooks).
   - Execução de `git lfs pull`: download de 1.438 objetos LFS (167 MB).
   - Execução de `git lfs checkout`: checkout concluído com sucesso para 1.769 objetos (172 MB).
3. **Validação Técnica dos Assets:**
   - Verificação com utilitário `file`:
     - `Projeto Jogo 2D/Assets/Resources/Audio/Foley/bocha.ogg`: validado como `Ogg data, Vorbis audio, stereo, 44100 Hz`.
     - Sprites em `Projeto Jogo 2D/Assets/Art/Free_pixel_tiles_pack/*.png`: validados como `PNG image data, 16 x 16, 8-bit/color RGBA`.
4. **Ciclo de Saída de Governança:**
   - Execução de `./scripts/graphify-refresh.sh`.
   - Execução de `./scripts/graphify-validate.sh` (93 scripts C# validados).
   - Atualização do [`docs/ai-memory/CHANGELOG_AI.md`](../ai-memory/CHANGELOG_AI.md).

---

## 4. Arquivos Modificados / Criados

- `~/.local/bin/git-lfs` (binário instalado).
- `docs/ai-plans/2026-10-27_lucamontini_instalacao-git-lfs-e-pull-assets.md` (este documento).
- `docs/ai-memory/CHANGELOG_AI.md` (registro da intervenção).

---

## 5. Validação e Testes

- `git lfs version`: `git-lfs/3.4.1 (GitHub; linux amd64; go 1.22.2)`.
- `git lfs checkout`: 100% (1769/1769 objetos) desembalados na árvore de trabalho.
- `./scripts/graphify-refresh.sh`: PASS.
- `./scripts/graphify-validate.sh`: PASS (93 scripts C# cobertos, integridade confirmada).

---

## 6. Próximos Passos

O usuário pode abrir normalmente o Unity Hub, adicionar a pasta `Projeto Jogo 2D/` e abrir com o Unity Editor `6000.6.0f1`.
