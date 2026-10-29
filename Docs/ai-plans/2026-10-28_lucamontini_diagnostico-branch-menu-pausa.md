# Registro de Execução: Diagnóstico do Menu de Pausa e Aviso de Vídeo WMF

- **Autor:** @lucamontini (Luca Dalla Lana)
- **Data/Hora:** 2026-10-28 20:38 BRT
- **Status:** Concluído (Diagnóstico e Orientação de Sincronização)
- **Ambiente:** Windows 11 / PowerShell / Unity 6000.6.0f1 (URP 2D) / Git LFS 3.8.1

---

## 1. Solicitação do Usuário
O usuário abriu o jogo na Unity e observou:
1. Aviso no console: `"Color primaries 0 is unknown or unsupported by WindowsMediaFoundation. Falling back to default may result in color shift. ...\Prologo_teste.mp4"`.
2. Ao pressionar `ESC` dentro do jogo, o menu de pausa exibido era o antigo (da branch `main`), embora o usuário tivesse desenvolvido a versão final do menu de pausa na branch `unity6-startup-warnings-errors`.
3. Questionamento: as alterações não subiram para o GitHub ou a Unity não reconheceu os arquivos alterados?

---

## 2. Diagnóstico Realizado
1. **Aviso de Vídeo (`WindowsMediaFoundation`):**
   - É um aviso informativo do decodificador nativo do Windows. O arquivo MP4 não traz a tag de perfil de primárias de cor no cabeçalho; o decodificador usa o fallback padrão Rec.709/sRGB. O vídeo é decodificado e exibido normalmente.
2. **Investigação do Menu de Pausa via Git:**
   - No GitHub (`origin`), o commit com o redesign completo do menu de pausa foi enviado com sucesso:
     - Commit `fe8c098`: `feat(ui): redesign do menu de pausa com placa oxidada e fatiamento modular`
     - Arquivos adicionados: `PainelPausa_Fundo.png`, `PainelPausa_Linha.png`, `PainelPausa_Selecao.png` e novo `MenuPausa.cs`.
   - **Causa Raiz da Divergência:**
     - No repositório remoto, o nome da branch é **`fix/unity6-startup-warnings-errors`** (com o prefixo `fix/`).
     - No ambiente Windows local, foi feito checkout de uma branch chamada **`unity6-startup-warnings-errors`** (sem o prefixo `fix/`), criada a partir da `main`.
     - A branch local estava no commit `a403ed4` (`main`), 3 commits atrás da branch remota (`baae3e4`, `d1b3a4d` e `fe8c098`).
     - Os arquivos do novo menu literalmente não existiam no disco local (`Test-Path` retornou `False`), por isso a Unity continuava carregando o menu antigo.

---

## 3. Próximos Passos Recomendados
1. Sincronizar os commits remotos de `origin/fix/unity6-startup-warnings-errors` para a branch local de trabalho via `git merge origin/fix/unity6-startup-warnings-errors`.
2. Garantir o download dos binários LFS dos novos sprites (`git lfs pull`).
3. Voltar ao Unity Editor e verificar o menu com a placa oxidada à esquerda ao pressionar `ESC`.
